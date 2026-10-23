using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KleeneStar.Model
{
    internal static partial class ModelHub
    {
        /// <summary>
        /// Returns the catalogs a workspace sees - its own and the installation-wide ones - or,
        /// without a workspace, every catalog.
        /// </summary>
        /// <param name="workspaceId">The workspace, or <see langword="null"/>.</param>
        /// <returns>The catalogs, ordered by name.</returns>
        public static IReadOnlyList<ValueCatalog> GetValueCatalogs(Guid? workspaceId)
        {
            using var db = CreateDbContext();

            var catalogs = db.ValueCatalogs.AsNoTracking();

            if (workspaceId is Guid id)
            {
                catalogs = catalogs.Where(x => x.WorkspaceId == null || x.WorkspaceId == id);
            }

            return [.. catalogs.OrderBy(x => x.Name)];
        }

        /// <summary>
        /// Returns a catalog.
        /// </summary>
        /// <param name="catalogId">The catalog.</param>
        /// <returns>The catalog, or <see langword="null"/>.</returns>
        public static ValueCatalog GetValueCatalog(Guid catalogId)
        {
            using var db = CreateDbContext();

            return db.ValueCatalogs.AsNoTracking().FirstOrDefault(x => x.Id == catalogId);
        }

        /// <summary>
        /// Adds or changes a catalog's own data; its versions are written apart.
        /// </summary>
        /// <param name="catalog">The catalog.</param>
        public static void SaveValueCatalog(ValueCatalog catalog)
        {
            ArgumentNullException.ThrowIfNull(catalog);

            using var db = CreateDbContext();

            var stored = db.ValueCatalogs.FirstOrDefault(x => x.Id == catalog.Id);

            if (stored is null)
            {
                db.ValueCatalogs.Add(catalog);
            }
            else
            {
                stored.Name = catalog.Name;
                stored.Description = catalog.Description;
                stored.Source = catalog.Source;
                stored.Query = catalog.Query;
                stored.ValueAttribute = catalog.ValueAttribute;
                stored.Url = catalog.Url;
                stored.RefreshMinutes = catalog.RefreshMinutes;
                stored.State = catalog.State;
                stored.CurrentVersion = catalog.CurrentVersion;
                stored.LastRefresh = catalog.LastRefresh;
                stored.LastError = catalog.LastError;
                stored.Updated = DateTime.UtcNow;
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Removes a catalog with its versions and bindings; the fields keep their options.
        /// </summary>
        /// <param name="catalogId">The catalog.</param>
        public static void RemoveValueCatalog(Guid catalogId)
        {
            using var db = CreateDbContext();

            db.FieldCatalogBindings.RemoveRange(db.FieldCatalogBindings.Where(x => x.CatalogId == catalogId));
            db.ValueCatalogVersions.RemoveRange(db.ValueCatalogVersions.Where(x => x.CatalogId == catalogId));
            db.ValueCatalogs.RemoveRange(db.ValueCatalogs.Where(x => x.Id == catalogId));
            db.SaveChanges();
        }

        /// <summary>
        /// Returns the versions of a catalog, the newest first.
        /// </summary>
        /// <param name="catalogId">The catalog.</param>
        /// <returns>The versions.</returns>
        public static IReadOnlyList<ValueCatalogVersion> GetValueCatalogVersions(Guid catalogId)
        {
            using var db = CreateDbContext();

            return [.. db.ValueCatalogVersions.AsNoTracking().Where(x => x.CatalogId == catalogId).OrderByDescending(x => x.Number)];
        }

        /// <summary>
        /// Returns a version of a catalog.
        /// </summary>
        /// <param name="catalogId">The catalog.</param>
        /// <param name="number">The number.</param>
        /// <returns>The version, or <see langword="null"/>.</returns>
        public static ValueCatalogVersion GetValueCatalogVersion(Guid catalogId, int number)
        {
            using var db = CreateDbContext();

            return db.ValueCatalogVersions.AsNoTracking().FirstOrDefault(x => x.CatalogId == catalogId && x.Number == number);
        }

        /// <summary>
        /// Adds a version and makes it the catalog's current one, provided nobody wrote that
        /// number first.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns><see langword="true"/> when written.</returns>
        public static bool AddValueCatalogVersion(ValueCatalogVersion version)
        {
            ArgumentNullException.ThrowIfNull(version);

            using var db = CreateDbContext();

            var catalog = db.ValueCatalogs.FirstOrDefault(x => x.Id == version.CatalogId);

            if (catalog is null || catalog.CurrentVersion != version.Number - 1)
            {
                return false;
            }

            db.ValueCatalogVersions.Add(version);
            catalog.CurrentVersion = version.Number;
            catalog.Updated = DateTime.UtcNow;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the bindings of a catalog, or of every catalog.
        /// </summary>
        /// <param name="catalogId">The catalog, or <see langword="null"/>.</param>
        /// <returns>The bindings.</returns>
        public static IReadOnlyList<FieldCatalogBinding> GetFieldCatalogBindings(Guid? catalogId)
        {
            using var db = CreateDbContext();

            return [.. db.FieldCatalogBindings.AsNoTracking().Where(x => catalogId == null || x.CatalogId == catalogId)];
        }

        /// <summary>
        /// Binds a field to a catalog, replacing a binding it had.
        /// </summary>
        /// <param name="binding">The binding.</param>
        public static void SaveFieldCatalogBinding(FieldCatalogBinding binding)
        {
            ArgumentNullException.ThrowIfNull(binding);

            using var db = CreateDbContext();

            db.FieldCatalogBindings.RemoveRange(db.FieldCatalogBindings.Where(x => x.FieldId == binding.FieldId));
            db.FieldCatalogBindings.Add(binding);
            db.SaveChanges();
        }

        /// <summary>
        /// Releases a field from its catalog.
        /// </summary>
        /// <param name="fieldId">The field.</param>
        public static void RemoveFieldCatalogBinding(Guid fieldId)
        {
            using var db = CreateDbContext();

            db.FieldCatalogBindings.RemoveRange(db.FieldCatalogBindings.Where(x => x.FieldId == fieldId));
            db.SaveChanges();
        }
    }
}
