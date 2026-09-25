using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Linq.Expressions;
using System.Reflection;

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
            return new Visitor().Visit(expression);
        }

        /// <summary>
        /// The visitor replacing the comparison calls.
        /// </summary>
        private sealed class Visitor : ExpressionVisitor
        {
            private static readonly MethodInfo ToLower = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes);

            /// <summary>
            /// Replaces <c>Contains</c>, <c>StartsWith</c>, <c>EndsWith</c> and <c>Equals</c>
            /// called with a string and a comparison by the overload without the comparison.
            /// </summary>
            /// <param name="node">The call.</param>
            /// <returns>The replacement, or the call itself.</returns>
            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                var visited = (MethodCallExpression)base.VisitMethodCall(node);
                var method = visited.Method;

                if (method.DeclaringType != typeof(string)
                    || method.IsStatic
                    || visited.Arguments.Count != 2
                    || visited.Arguments[0].Type != typeof(string)
                    || visited.Arguments[1].Type != typeof(StringComparison)
                    || method.Name is not (nameof(string.Contains) or nameof(string.StartsWith) or nameof(string.EndsWith) or nameof(string.Equals)))
                {
                    return visited;
                }

                // a comparison that is not a constant is decided per row, which no provider can
                // translate either way; left alone, it fails as before rather than silently
                // changing its meaning
                if (visited.Arguments[1] is not ConstantExpression { Value: StringComparison comparison })
                {
                    return visited;
                }

                var plain = typeof(string).GetMethod(method.Name, [typeof(string)]);
                var target = visited.Object;
                var argument = visited.Arguments[0];

                if (comparison is StringComparison.OrdinalIgnoreCase
                    or StringComparison.CurrentCultureIgnoreCase
                    or StringComparison.InvariantCultureIgnoreCase)
                {
                    target = Expression.Call(target, ToLower);
                    argument = Expression.Call(argument, ToLower);
                }

                return Expression.Call(target, plain, argument);
            }
        }
    }
}
