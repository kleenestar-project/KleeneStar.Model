using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace KleeneStar.Model
{
    /// <summary>
    /// Counts the writes per entity type within this process.
    /// </summary>
    /// <remarks>
    /// A version says nothing about the content, only whether rows of the type were written
    /// since it was read. It covers every write that goes through <c>SaveChanges</c>; a bulk
    /// statement (<c>ExecuteUpdate</c>, <c>ExecuteDelete</c>, raw SQL) has to call
    /// <see cref="Advance(IEnumerable{Type})"/> itself. Writes of another process are not
    /// counted, so a cache keyed by these versions has to be short lived.
    /// </remarks>
    public static class EntityVersions
    {
        /// <summary>
        /// The version per entity type.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, long> _versions = new();

        /// <summary>
        /// Returns the current version of an entity type.
        /// </summary>
        /// <param name="type">The entity type.</param>
        /// <returns>The version; zero while nothing of the type was written.</returns>
        public static long Of(Type type)
        {
            return type is not null && _versions.TryGetValue(type, out var version) ? version : 0;
        }

        /// <summary>
        /// Returns a combined version of several entity types, which changes whenever one of
        /// them is written.
        /// </summary>
        /// <param name="types">The entity types.</param>
        /// <returns>The combined version.</returns>
        public static long Of(params Type[] types)
        {
            var sum = 0L;

            foreach (var type in types ?? [])
            {
                sum += Of(type);
            }

            return sum;
        }

        /// <summary>
        /// Advances the versions of the supplied entity types.
        /// </summary>
        /// <param name="types">The entity types that were written.</param>
        public static void Advance(IEnumerable<Type> types)
        {
            foreach (var type in types ?? [])
            {
                _versions.AddOrUpdate(type, 1, (_, version) => version + 1);
            }
        }
    }
}
