using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace KleeneStar.Model
{
    internal static partial class ModelHub
    {
        /// <summary>
        /// Returns the creation record of an idempotency key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The record, or <see langword="null"/>.</returns>
        public static ObjectCreation GetObjectCreation(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            using var db = CreateDbContext();

            return db.ObjectCreations
                .AsNoTracking()
                .FirstOrDefault(x => x.Key == key);
        }

        /// <summary>
        /// Returns the creation records in a state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>The records, oldest first.</returns>
        public static System.Collections.Generic.IReadOnlyList<ObjectCreation> GetObjectCreations(ObjectCreationState state)
        {
            using var db = CreateDbContext();

            return [.. db.ObjectCreations
                .AsNoTracking()
                .Where(x => x.State == state)
                .OrderBy(x => x.RawId)];
        }

        /// <summary>
        /// Removes the completed creation records finished before a point in time.
        /// </summary>
        /// <remarks>
        /// A key only has to outlive the retries of its client; a record kept for longer only
        /// grows the table. Rolled-back records are kept, they carry the error somebody may
        /// still investigate.
        /// </remarks>
        /// <param name="before">The point in time.</param>
        /// <returns>The number of records removed.</returns>
        public static int PruneObjectCreations(DateTime before)
        {
            using var db = CreateDbContext();

            var stale = db.ObjectCreations
                .Where(x => x.State == ObjectCreationState.Completed && x.Finished != null && x.Finished < before)
                .ToList();

            db.ObjectCreations.RemoveRange(stale);
            db.SaveChanges();

            return stale.Count;
        }

        /// <summary>
        /// Claims an idempotency key for a creation that is about to run.
        /// </summary>
        /// <remarks>
        /// A key nobody used is inserted as a running record. A key whose creation was rolled
        /// back is taken over and run again. A key that is running or completed is not claimed:
        /// the caller gets the existing record back and answers from it. The insert relies on the
        /// unique index, so of two requests racing for one key exactly one claims it.
        /// </remarks>
        /// <param name="creation">The running record to insert.</param>
        /// <param name="existing">When the key was not claimed, the record that holds it.</param>
        /// <returns><see langword="true"/> when the key was claimed for this creation.</returns>
        public static bool TryClaimObjectCreation(ObjectCreation creation, out ObjectCreation existing)
        {
            ArgumentNullException.ThrowIfNull(creation);

            using var db = CreateDbContext();

            var stored = db.ObjectCreations.FirstOrDefault(x => x.Key == creation.Key);

            if (stored is not null)
            {
                if (stored.State != ObjectCreationState.RolledBack)
                {
                    existing = stored;
                    return false;
                }

                // a rolled-back attempt left nothing behind, so its key may run again
                stored.State = ObjectCreationState.Running;
                stored.TemplateId = creation.TemplateId;
                stored.IdentityId = creation.IdentityId;
                stored.RootObjectId = null;
                stored.ObjectIds = null;
                stored.RelationIds = null;
                stored.Error = null;
                stored.Created = creation.Created;
                stored.Finished = null;
                db.SaveChanges();

                creation.Id = stored.Id;
                existing = null;
                return true;
            }

            db.ObjectCreations.Add(creation);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                // a parallel request inserted the key between the read and the write
                using var reread = CreateDbContext();

                existing = reread.ObjectCreations.AsNoTracking().FirstOrDefault(x => x.Key == creation.Key);

                if (existing is null)
                {
                    throw;
                }

                return false;
            }

            existing = null;
            return true;
        }

        /// <summary>
        /// Writes the progress of a creation: what it has written so far, its root, its state.
        /// </summary>
        /// <param name="creation">The record carrying the new values.</param>
        public static void UpdateObjectCreation(ObjectCreation creation)
        {
            ArgumentNullException.ThrowIfNull(creation);

            using var db = CreateDbContext();

            var stored = db.ObjectCreations.FirstOrDefault(x => x.Id == creation.Id);

            if (stored is null)
            {
                return;
            }

            stored.RootObjectId = creation.RootObjectId;
            stored.State = creation.State;
            stored.ObjectIds = creation.ObjectIds;
            stored.RelationIds = creation.RelationIds;
            stored.Error = creation.Error;
            stored.Finished = creation.Finished;

            db.SaveChanges();
        }
    }
}
