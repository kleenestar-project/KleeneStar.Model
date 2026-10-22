using KleeneStar.Model.Entities;
using KleeneStar.Model.Interceptors;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebExpress.WebIndex.Queries;
using WebExpress.WebIndex.Wql;

namespace KleeneStar.Model.Test
{
    /// <summary>
    /// Contains unit tests for <see cref="StringComparisonQueryInterceptor"/>, which rewrites the
    /// string comparisons WQL produces into a shape EF Core can translate.
    /// </summary>
    public class UnitTestStringComparisonQueryInterceptor
    {
        /// <summary>
        /// The rows the rewritten predicates are evaluated against.
        /// </summary>
        private static readonly string[] Rows = ["Incident on server", "INCIDENT report", "Change request", null];

        /// <summary>
        /// Verifies that an ignore-case comparison loses its comparison argument, gains a lowered
        /// target and argument, and still matches ignoring case.
        /// </summary>
        [Theory]
        [InlineData(StringComparison.OrdinalIgnoreCase)]
        [InlineData(StringComparison.CurrentCultureIgnoreCase)]
        [InlineData(StringComparison.InvariantCultureIgnoreCase)]
        public void IgnoreCaseContainsIsLowered(StringComparison comparison)
        {
            Expression<Func<string, bool>> predicate = x => x != null && x.Contains("incident", comparison);

            var rewritten = Rewrite(Bind(predicate, comparison));

            Assert.DoesNotContain(typeof(StringComparison).Name, rewritten.ToString());
            Assert.Contains("ToLower()", rewritten.ToString());
            Assert.Equal(["Incident on server", "INCIDENT report"], Rows.Where(rewritten.Compile()));
        }

        /// <summary>
        /// Verifies that a case-sensitive comparison only drops its comparison argument.
        /// </summary>
        [Fact]
        public void OrdinalStartsWithDropsTheComparison()
        {
            Expression<Func<string, bool>> predicate = x => x != null && x.StartsWith("Incident", StringComparison.Ordinal);

            var rewritten = Rewrite(predicate);

            Assert.DoesNotContain(typeof(StringComparison).Name, rewritten.ToString());
            Assert.DoesNotContain("ToLower()", rewritten.ToString());
            Assert.Equal(["Incident on server"], Rows.Where(rewritten.Compile()));
        }

        /// <summary>
        /// Verifies that calls the providers already translate are left as they are.
        /// </summary>
        [Fact]
        public void PlainContainsIsUntouched()
        {
            Expression<Func<string, bool>> predicate = x => x != null && x.Contains("request");

            var rewritten = Rewrite(predicate);

            Assert.Equal(predicate.ToString(), rewritten.ToString());
        }

        /// <summary>
        /// Verifies that the invariant case mappings the WebIndex query filters lower with
        /// become the plain ones and keep matching ignoring case.
        /// </summary>
        [Fact]
        public void InvariantCaseMappingBecomesThePlainOne()
        {
            Expression<Func<string, bool>> predicate = x => x != null && x.ToLowerInvariant() == "incident report";

            var rewritten = Rewrite(predicate);

            Assert.DoesNotContain("Invariant", rewritten.ToString());
            Assert.Contains("ToLower()", rewritten.ToString());
            Assert.Equal(["INCIDENT report"], Rows.Where(rewritten.Compile()));
        }

        /// <summary>
        /// Verifies that the static equality with a comparison, which WQL's <c>=</c> builds on
        /// text, becomes an equality of the lowered sides.
        /// </summary>
        [Fact]
        public void StaticIgnoreCaseEqualsBecomesAnEquality()
        {
            Expression<Func<string, bool>> predicate = x => x != null && string.Equals(x, "change REQUEST", StringComparison.OrdinalIgnoreCase);

            var rewritten = Rewrite(predicate);

            Assert.DoesNotContain(typeof(StringComparison).Name, rewritten.ToString());
            Assert.Contains("ToLower()", rewritten.ToString());
            Assert.Equal(["Change request"], Rows.Where(rewritten.Compile()));
        }

        /// <summary>
        /// Verifies that the static ordering with a comparison, which WQL's <c>&lt;</c> and its
        /// kin build on text, becomes the two-argument ordering of the lowered sides.
        /// </summary>
        [Fact]
        public void StaticIgnoreCaseCompareBecomesThePlainOrdering()
        {
            Expression<Func<string, bool>> predicate = x => x != null && string.Compare(x, "D", StringComparison.OrdinalIgnoreCase) < 0;

            var rewritten = Rewrite(predicate);

            Assert.DoesNotContain(typeof(StringComparison).Name, rewritten.ToString());
            Assert.Equal(["Change request"], Rows.Where(rewritten.Compile()));
        }

        /// <summary>
        /// Verifies against the SQLite provider that every text filter the framework builds -
        /// the <c>IQuery</c> string filters and WQL's text conditions - translates once the
        /// interceptor has rewritten it, and finds the rows it should. Without the rewrite each
        /// of these throws "could not be translated", which took every page down when the WQL
        /// rework of WebIndex arrived (2026-10-17). The text conditions go through WebIndex's own
        /// <see cref="WqlExpressionBuilder.ToTranslatable"/>, a substring test per word.
        /// </summary>
        /// <param name="filter">The filter: a name of an <c>IQuery</c> filter or <c>wql:</c> and a statement.</param>
        /// <param name="expected">The names expected, separated by a bar.</param>
        [Theory]
        [InlineData("equalsignorecase", "IT Service Desk")]
        [InlineData("containsignorecase", "IT Service Desk|Help Desk Archive")]
        [InlineData("startswithignorecase", "IT Service Desk")]
        [InlineData("endswithignorecase", "IT Service Desk")]
        [InlineData("wql:Name = 'it service desk'", "IT Service Desk")]
        [InlineData("wql:Name != 'it service desk'", "Help Desk Archive|Software Development")]
        [InlineData("wql:Name < 'j'", "Help Desk Archive|IT Service Desk")]
        [InlineData("wql:Name >= 'S'", "Software Development")]
        [InlineData("wql:Name ~ 'desk'", "IT Service Desk|Help Desk Archive")]
        [InlineData("wql:Name ~ 'desk help'", "Help Desk Archive")]
        [InlineData("wql:Name ~ 'dev*'", "Software Development")]
        // the database pre-selects: a phrase demands its words, not their order
        [InlineData("wql:Name = 'desk service'", "IT Service Desk")]
        [InlineData("wql:Name !~ 'desk'", "Software Development")]
        public void FrameworkTextFiltersTranslateOnSqlite(string filter, string expected)
        {
            using var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<KleeneStarDbContext>()
                .UseSqlite(connection)
                .Options;

            using var context = new KleeneStarDbContext(options);
            context.Database.EnsureCreated();
            context.Workspaces.AddRange
            (
                new Workspace { Id = Guid.NewGuid(), Key = "SD", Name = "IT Service Desk" },
                new Workspace { Id = Guid.NewGuid(), Key = "DEV", Name = "Software Development" },
                new Workspace { Id = Guid.NewGuid(), Key = "ARC", Name = "Help Desk Archive" }
            );
            context.SaveChanges();

            IQuery<Workspace> query = filter switch
            {
                "equalsignorecase" => new Query<Workspace>().WhereEqualsIgnoreCase(x => x.Name, "it SERVICE desk"),
                "containsignorecase" => new Query<Workspace>().WhereContainsIgnoreCase(x => x.Name, "DESK"),
                "startswithignorecase" => new Query<Workspace>().WhereStartsWithIgnoreCase(x => x.Name, "it"),
                "endswithignorecase" => new Query<Workspace>().WhereEndsWithIgnoreCase(x => x.Name, "DESK"),
                _ => Parse(filter["wql:".Length..])
            };

            var names = query.Apply(context.Workspaces.AsNoTracking())
                .Select(x => x.Name)
                .ToList()
                .Order()
                .ToList();

            Assert.Equal(expected.Split('|', StringSplitOptions.RemoveEmptyEntries).Order(), names);
        }

        /// <summary>
        /// Parses a WQL expression over workspaces into a query.
        /// </summary>
        /// <param name="wql">The expression.</param>
        /// <returns>The query.</returns>
        private static IQuery<Workspace> Parse(string wql)
        {
            var statement = new WqlParser<Workspace>().Parse(wql);

            Assert.False(statement.HasErrors, wql);

            return statement.ToQuery();
        }

        /// <summary>
        /// Rewrites a predicate through the interceptor.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The rewritten predicate.</returns>
        private static Expression<Func<string, bool>> Rewrite(Expression<Func<string, bool>> predicate)
        {
            return (Expression<Func<string, bool>>)StringComparisonQueryInterceptor.Rewrite(predicate);
        }

        /// <summary>
        /// Replaces the captured comparison by a constant, the shape the WQL parser produces.
        /// </summary>
        /// <param name="predicate">The predicate capturing the comparison.</param>
        /// <param name="comparison">The comparison.</param>
        /// <returns>The predicate with a constant comparison.</returns>
        private static Expression<Func<string, bool>> Bind(Expression<Func<string, bool>> predicate, StringComparison comparison)
        {
            return (Expression<Func<string, bool>>)new ConstantBinder(comparison).Visit(predicate);
        }

        /// <summary>
        /// Turns the closure field holding the comparison into a constant.
        /// </summary>
        /// <param name="comparison">The comparison.</param>
        private sealed class ConstantBinder(StringComparison comparison) : ExpressionVisitor
        {
            protected override Expression VisitMember(MemberExpression node)
            {
                return node.Type == typeof(StringComparison)
                    ? Expression.Constant(comparison)
                    : base.VisitMember(node);
            }
        }
    }
}
