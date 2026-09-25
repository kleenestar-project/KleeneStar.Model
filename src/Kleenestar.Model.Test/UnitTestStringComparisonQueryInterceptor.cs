using KleeneStar.Model.Interceptors;
using System.Linq.Expressions;

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
