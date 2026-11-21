using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Linq.Expressions;
using System.Reflection;
using WebExpress.WebIndex.Wql;

namespace KleeneStar.Model.Interceptors
{
    /// <summary>
    /// Rewrites string comparisons that name a <see cref="StringComparison"/> into a shape the
    /// database providers can translate, before a query is compiled.
    /// </summary>
    /// <remarks>
    /// WQL's text operators (<c>~</c> above all) compile to
    /// <c>x.Summary.Contains(value, StringComparison.OrdinalIgnoreCase)</c>. That is right for a
    /// list in memory, but EF Core translates none of the <see cref="StringComparison"/>
    /// overloads, so every WQL table backed by the database answered such a query with an
    /// "could not be translated" error - the global search among them, and with it every saved
    /// search written with <c>~</c>. The rewrite keeps the meaning: an ignore-case comparison
    /// becomes the same call on both sides lowered (<c>a.ToLower().Contains(b.ToLower())</c>),
    /// a case-sensitive one simply drops the argument. Both translate on SQLite and SQL Server
    /// and behave the same in memory.
    /// <para>
    /// Since the WQL rework of WebIndex (2026-10-17) the framework builds three more shapes,
    /// none of which EF Core translates either, and the first of which reaches every page: the
    /// <c>IQuery</c> string filters lowered both sides with <c>ToLowerInvariant()</c> until
    /// 2026-10-22 (a tag lookup in the header's help menu used it, so every page answered 500;
    /// they use <c>ToLower()</c> now, the mapping stays for any other caller), and WQL's <c>=</c>,
    /// <c>!=</c> and orderings on text call the static <c>string.Equals(a, b, comparison)</c> and
    /// <c>string.Compare(a, b, comparison)</c>. The invariant case mappings become the plain ones
    /// (the database lowers by its own rules either way), the static equality becomes <c>==</c>
    /// and the static ordering the two-argument <c>string.Compare</c>, both sides lowered where
    /// the comparison ignores case. WQL compares that ordering against zero, which is the form
    /// the relational providers translate.
    /// </para>
    /// <para>
    /// The same rework evaluates WQL's text contract - <c>=</c> as a phrase, <c>~</c> as words,
    /// wildcards, distance, similarity - through a <see cref="WqlTextMatcher"/> embedded in the
    /// expression as a constant, which only an in-memory list can run. WebIndex answers that
    /// itself since 2026-10-22: <see cref="WqlExpressionBuilder.ToTranslatable"/> turns such a
    /// call into a lower-cased substring test per word (one <c>LIKE</c> each) and the
    /// <see cref="StringComparison.OrdinalIgnoreCase"/> comparisons into lowered ones. The
    /// database-backed endpoints ask for that mode themselves
    /// (<see cref="WqlQueryMode.Translatable"/>); the rewrite runs here first anyway, over the
    /// whole query, for an endpoint that does not and for the framework bases that offer no
    /// choice. On a tree already rewritten it changes nothing. Its text search pre-selects rather
    /// than reproducing the contract: order, distance and word boundaries are not checked, the
    /// database folds no diacritics, a fuzzy search only excludes null, and a negated one
    /// selects a subset. What it leaves - other comparisons, the invariant case mappings - the
    /// visitor below handles.
    /// </para>
    /// <para>
    /// It sits here rather than in the query that builds the expression because the expression
    /// is the framework's and every provider needs the same answer; registered in
    /// <see cref="KleeneStarDbContext.OnConfiguring"/>, it reaches every context however it is
    /// created.
    /// </para>
    /// </remarks>
    public sealed class StringComparisonQueryInterceptor : IQueryExpressionInterceptor
    {
        /// <summary>
        /// Gets the shared instance; the interceptor holds no state.
        /// </summary>
        public static StringComparisonQueryInterceptor Instance { get; } = new();

        /// <summary>
        /// Rewrites the query expression before EF Core compiles it.
        /// </summary>
        /// <param name="queryExpression">The query expression.</param>
        /// <param name="eventData">The event data.</param>
        /// <returns>The rewritten expression.</returns>
        public Expression QueryCompilationStarting(Expression queryExpression, QueryExpressionEventData eventData)
        {
            return Rewrite(queryExpression);
        }

        /// <summary>
        /// Rewrites every string comparison naming a <see cref="StringComparison"/> in an
        /// expression. Exposed for the tests, which check the rewritten shape.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>The rewritten expression.</returns>
        public static Expression Rewrite(Expression expression)
        {
            return new Visitor().Visit(WqlExpressionBuilder.ToTranslatable(expression));
        }

        /// <summary>
        /// The visitor replacing the comparison calls.
        /// </summary>
        private sealed class Visitor : ExpressionVisitor
        {
            private static readonly MethodInfo ToLower = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes);
            private static readonly MethodInfo ToUpper = typeof(string).GetMethod(nameof(string.ToUpper), Type.EmptyTypes);
            private static readonly MethodInfo ToLowerInvariant = typeof(string).GetMethod(nameof(string.ToLowerInvariant), Type.EmptyTypes);
            private static readonly MethodInfo ToUpperInvariant = typeof(string).GetMethod(nameof(string.ToUpperInvariant), Type.EmptyTypes);
            private static readonly MethodInfo Compare = typeof(string).GetMethod(nameof(string.Compare), [typeof(string), typeof(string)]);

            /// <summary>
            /// Replaces <c>Contains</c>, <c>StartsWith</c>, <c>EndsWith</c> and <c>Equals</c>
            /// called with a string and a comparison by the overload without the comparison,
            /// the static <c>Equals</c> and <c>Compare</c> with a comparison by an equality and
            /// the two-argument <c>Compare</c>, and the invariant case mappings by the plain ones.
            /// </summary>
            /// <param name="node">The call.</param>
            /// <returns>The replacement, or the call itself.</returns>
            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                var visited = (MethodCallExpression)base.VisitMethodCall(node);
                var method = visited.Method;

                if (method.DeclaringType != typeof(string))
                {
                    return visited;
                }

                if (method == ToLowerInvariant)
                {
                    return Expression.Call(visited.Object, ToLower);
                }

                if (method == ToUpperInvariant)
                {
                    return Expression.Call(visited.Object, ToUpper);
                }

                if (method.IsStatic)
                {
                    return RewriteStatic(visited);
                }

                if (visited.Arguments.Count != 2
                    || visited.Arguments[0].Type != typeof(string)
                    || method.Name is not (nameof(string.Contains) or nameof(string.StartsWith) or nameof(string.EndsWith) or nameof(string.Equals))
                    || !TryReadComparison(visited.Arguments[1], out var ignoreCase))
                {
                    return visited;
                }

                var plain = typeof(string).GetMethod(method.Name, [typeof(string)]);

                return Expression.Call(Fold(visited.Object, ignoreCase), plain, Fold(visited.Arguments[0], ignoreCase));
            }

            /// <summary>
            /// Replaces the static <c>string.Equals(a, b, comparison)</c> by <c>a == b</c> and
            /// <c>string.Compare(a, b, comparison)</c> by <c>string.Compare(a, b)</c>, both sides
            /// lowered when the comparison ignores case.
            /// </summary>
            /// <param name="call">The static call.</param>
            /// <returns>The replacement, or the call itself.</returns>
            private static Expression RewriteStatic(MethodCallExpression call)
            {
                if (call.Arguments.Count != 3
                    || call.Arguments[0].Type != typeof(string)
                    || call.Arguments[1].Type != typeof(string)
                    || call.Method.Name is not (nameof(string.Equals) or nameof(string.Compare))
                    || !TryReadComparison(call.Arguments[2], out var ignoreCase))
                {
                    return call;
                }

                var left = Fold(call.Arguments[0], ignoreCase);
                var right = Fold(call.Arguments[1], ignoreCase);

                return call.Method.Name == nameof(string.Equals)
                    ? Expression.Equal(left, right)
                    : Expression.Call(Compare, left, right);
            }

            /// <summary>
            /// Reads a comparison argument.
            /// </summary>
            /// <param name="argument">The argument.</param>
            /// <param name="ignoreCase">Whether the comparison ignores case.</param>
            /// <returns>True for a constant comparison; false otherwise.</returns>
            private static bool TryReadComparison(Expression argument, out bool ignoreCase)
            {
                ignoreCase = false;

                // a comparison that is not a constant is decided per row, which no provider can
                // translate either way; left alone, it fails as before rather than silently
                // changing its meaning
                if (argument.Type != typeof(StringComparison)
                    || argument is not ConstantExpression { Value: StringComparison comparison })
                {
                    return false;
                }

                ignoreCase = comparison is StringComparison.OrdinalIgnoreCase
                    or StringComparison.CurrentCultureIgnoreCase
                    or StringComparison.InvariantCultureIgnoreCase;

                return true;
            }

            /// <summary>
            /// Lowers an operand when the comparison ignores case.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="ignoreCase">Whether the comparison ignores case.</param>
            /// <returns>The operand, lowered or as it was.</returns>
            private static Expression Fold(Expression operand, bool ignoreCase)
            {
                return ignoreCase ? Expression.Call(operand, ToLower) : operand;
            }
        }
    }
}
