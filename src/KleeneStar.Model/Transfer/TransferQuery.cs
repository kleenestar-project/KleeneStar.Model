using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace KleeneStar.Model.Transfer
{
    /// <summary>
    /// Queries a table of the model whose type is only known at run time.
    /// </summary>
    internal static class TransferQuery
    {
        /// <summary>
        /// The number of values a single query compares with at most.
        /// </summary>
        private const int ChunkSize = 2000;

        private static readonly MethodInfo _set = typeof(DbContext).GetMethods()
            .Single(x => x.Name == nameof(DbContext.Set) && x.IsGenericMethodDefinition && x.GetParameters().Length == 0);

        private static readonly MethodInfo _sharedSet = typeof(DbContext).GetMethods()
            .Single(x => x.Name == nameof(DbContext.Set) && x.IsGenericMethodDefinition && x.GetParameters().Length == 1);

        private static readonly MethodInfo _property = typeof(EF).GetMethod(nameof(EF.Property));

        /// <summary>
        /// Returns the rows of a table whose property has one of the given values, for each
        /// condition; a row that meets several is returned once.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="type">The root entity type of the table.</param>
        /// <param name="conditions">The conditions: a property and the values it may have.</param>
        /// <param name="tracking">Whether the rows are tracked (shadow values are only read from tracked rows).</param>
        /// <returns>The rows.</returns>
        public static List<object> Load(DbContext context, IEntityType type, IEnumerable<(IProperty Property, IEnumerable<object> Values)> conditions, bool tracking)
        {
            var rows = new List<object>();
            var seen = new HashSet<object>(ReferenceEqualityComparer.Instance);

            foreach (var (property, values) in conditions)
            {
                var typed = values
                    .SelectMany(x => TransferValue.ToPropertyType(x, property.ClrType))
                    .Distinct()
                    .ToList();

                foreach (var chunk in typed.Chunk(ChunkSize))
                {
                    foreach (var row in Where(context, type, property, chunk, tracking))
                    {
                        if (seen.Add(row))
                        {
                            rows.Add(row);
                        }
                    }
                }
            }

            return rows;
        }

        /// <summary>
        /// Returns every row of a table.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="type">The root entity type of the table.</param>
        /// <returns>The rows.</returns>
        public static IEnumerable<object> All(DbContext context, IEntityType type)
        {
            return Set(context, type, tracking: false).Cast<object>();
        }

        /// <summary>
        /// Returns the rows whose property has one of the values.
        /// </summary>
        private static IEnumerable<object> Where(DbContext context, IEntityType type, IProperty property, object[] values, bool tracking)
        {
            var clr = type.ClrType;
            var source = Set(context, type, tracking);
            var parameter = Expression.Parameter(clr, "x");
            var access = Expression.Call(_property.MakeGenericMethod(property.ClrType), parameter, Expression.Constant(property.Name));

            var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(property.ClrType));

            foreach (var value in values)
            {
                list.Add(value);
            }

            var contains = Expression.Call
            (
                typeof(Enumerable),
                nameof(Enumerable.Contains),
                [property.ClrType],
                Expression.Constant(list),
                access
            );

            var lambda = Expression.Lambda(contains, parameter);
            var where = Expression.Call(typeof(Queryable), nameof(Queryable.Where), [clr], source.Expression, Expression.Quote(lambda));

            return source.Provider.CreateQuery(where).Cast<object>().ToList();
        }

        /// <summary>
        /// Returns the set of a table.
        /// </summary>
        private static IQueryable Set(DbContext context, IEntityType type, bool tracking)
        {
            var set = type.HasSharedClrType
                ? (IQueryable)_sharedSet.MakeGenericMethod(type.ClrType).Invoke(context, [type.Name])
                : (IQueryable)_set.MakeGenericMethod(type.ClrType).Invoke(context, null);

            var method = tracking ? nameof(EntityFrameworkQueryableExtensions.AsTracking) : nameof(EntityFrameworkQueryableExtensions.AsNoTracking);
            var call = Expression.Call(typeof(EntityFrameworkQueryableExtensions), method, [type.ClrType], set.Expression);

            return set.Provider.CreateQuery(call);
        }
    }
}
