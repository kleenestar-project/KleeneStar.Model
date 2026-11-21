using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace KleeneStar.Model.Interceptors
{
    /// <summary>
    /// Advances the version of every entity type a save writes (<see cref="EntityVersions"/>),
    /// so a cache keyed by those versions stops answering from rows that changed.
    /// </summary>
    /// <remarks>
    /// The types are collected before the save, because the change tracker forgets the states
    /// once it succeeded, and advanced after it, whether it succeeded or not: a failed save
    /// may have written part of its rows, and a version advanced once too often costs one read.
    /// </remarks>
    public sealed class EntityVersionInterceptor : SaveChangesInterceptor
    {
        /// <summary>
        /// The instance added to every context.
        /// </summary>
        public static EntityVersionInterceptor Instance { get; } = new();

        /// <summary>
        /// The entity types a save in progress writes, per context.
        /// </summary>
        private static readonly ConditionalWeakTable<DbContext, HashSet<Type>> _pending = [];

        /// <inheritdoc/>
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            Collect(eventData.Context);

            return result;
        }

        /// <inheritdoc/>
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            Collect(eventData.Context);

            return ValueTask.FromResult(result);
        }

        /// <inheritdoc/>
        public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
        {
            Advance(eventData.Context);

            return result;
        }

        /// <inheritdoc/>
        public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            Advance(eventData.Context);

            return ValueTask.FromResult(result);
        }

        /// <inheritdoc/>
        public override void SaveChangesFailed(DbContextErrorEventData eventData)
        {
            Advance(eventData.Context);
        }

        /// <inheritdoc/>
        public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
        {
            Advance(eventData.Context);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Records the entity types the pending save writes.
        /// </summary>
        /// <param name="context">The saving context.</param>
        private static void Collect(DbContext context)
        {
            if (context is null)
            {
                return;
            }

            var types = context.ChangeTracker
                .Entries()
                .Where(x => x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
                .Select(x => x.Metadata.ClrType)
                .ToHashSet();

            if (types.Count > 0)
            {
                _pending.AddOrUpdate(context, types);
            }
        }

        /// <summary>
        /// Advances the versions of the entity types the finished save wrote.
        /// </summary>
        /// <param name="context">The context that saved.</param>
        private static void Advance(DbContext context)
        {
            if (context is null || !_pending.TryGetValue(context, out var types))
            {
                return;
            }

            _pending.Remove(context);

            EntityVersions.Advance(types);
        }
    }
}
