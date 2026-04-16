using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WebExpress.WebIndex;

namespace KleeneStar.Model
{
    /// <summary>
    /// Extension methods for the DbContext to support generic entity updates.
    /// </summary>
    public static class KleeneStarDbContextExtensions
    {
        /// <summary>
        /// Adds a new entity to the DbContext with optional relation handling and timestamping.
        /// </summary>
        /// <typeparam name="TEntity">The entity type to add.</typeparam>
        /// <param name="context">The DbContext to add the entity to.</param>
        /// <param name="entity">The entity instance to add. Must not be null.</param>
        /// </param>
        /// <param name="includeRelations">
        /// Optional set of navigation property names representing collection relations that should 
        /// be prepared (attach existing related entities or mark new ones for insert). If null, 
        /// relations are ignored.
        /// </param>
        /// <param name="autoSetTimestamps">
        /// If true, sets Created and Updated properties to DateTime.UtcNow when present and not 
        /// already set.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown when entity is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the entity type has no primary key or an entity with the same primary 
        /// key already exists.
        /// </exception>
        public static void AddEntity<TEntity>
        (
            this DbContext context,
            TEntity entity,
            HashSet<string> includeRelations = null,
            bool autoSetTimestamps = true

        )
            where TEntity : class
        {
            ArgumentNullException.ThrowIfNull(entity);

            includeRelations ??= [];

            // identify primary key
            var entityType = context.Model.FindEntityType(typeof(TEntity)) ?? throw new InvalidOperationException("Entity type not known to model.");
            var primaryKey = entityType.FindPrimaryKey() ?? throw new InvalidOperationException("No PK found for entity type.");
            var keyProp = primaryKey.Properties.FirstOrDefault() ?? throw new InvalidOperationException("No PK property found.");
            var keyVal = keyProp.PropertyInfo.GetValue(entity);

            // if key has a non-default value, ensure no existing entity with same key exists
            bool HasNonDefaultKey(object val)
            {
                if (val == null) return false;
                var t = val.GetType();
                if (t == typeof(Guid)) return (Guid)val != Guid.Empty;
                if (t.IsValueType) return !Equals(val, Activator.CreateInstance(t));
                if (t == typeof(string)) return !string.IsNullOrEmpty((string)val);
                return true;
            }

            if (HasNonDefaultKey(keyVal))
            {
                // check existence using Find if possible (works for single key)
                try
                {
                    var found = context.Find(typeof(TEntity), keyVal);
                    if (found is not null)
                    {
                        throw new InvalidOperationException("An entity with the same primary key already exists.");
                    }
                }
                catch
                {
                    // fallback to query if Find fails for some providers
                    var query = context.Set<TEntity>().AsQueryable();
                    var existing = query.AsEnumerable()
                                        .FirstOrDefault(e => keyProp.PropertyInfo.GetValue(e)?.Equals(keyVal) == true);
                    if (existing != null)
                    {
                        throw new InvalidOperationException("An entity with the same primary key already exists.");
                    }
                }
            }

            // auto-set timestamps if requested
            if (autoSetTimestamps)
            {
                // set Created if present and default
                var createdProp = typeof(TEntity).GetProperty("Created", BindingFlags.Public | BindingFlags.Instance);
                if (createdProp is not null && createdProp.PropertyType == typeof(DateTime) && createdProp.CanWrite)
                {
                    var cur = (DateTime)createdProp.GetValue(entity);
                    if (cur == default)
                    {
                        // set created to now
                        createdProp.SetValue(entity, DateTime.UtcNow);
                    }
                }

                // set Updated if present
                var updatedProp = typeof(TEntity).GetProperty("Updated", BindingFlags.Public | BindingFlags.Instance);
                if (updatedProp is not null && updatedProp.PropertyType == typeof(DateTime) && updatedProp.CanWrite)
                {
                    updatedProp.SetValue(entity, DateTime.UtcNow);
                }
            }

            // prepare relations: for each named collection navigation attach existing related entities or mark new ones
            foreach (var relationName in includeRelations)
            {
                // find navigation and property
                var navigation = entityType.FindNavigation(relationName);
                if (navigation is null || !navigation.IsCollection)
                {
                    continue;
                }

                var propInfo = typeof(TEntity).GetProperty(relationName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (propInfo is null)
                {
                    continue;
                }

                // get incoming collection
                var incoming = propInfo.GetValue(entity) as IEnumerable;
                if (incoming is null)
                {
                    continue;
                }

                // determine element type of the collection
                Type elementType = null;
                if (propInfo.PropertyType.IsArray)
                {
                    elementType = propInfo.PropertyType.GetElementType();
                }
                else if (propInfo.PropertyType.IsGenericType)
                {
                    elementType = propInfo.PropertyType.GetGenericArguments().FirstOrDefault();
                }
                elementType ??= typeof(object);

                // create a concrete list instance to hold prepared related items
                var listType = typeof(List<>).MakeGenericType(elementType);
                var preparedList = (IList)Activator.CreateInstance(listType);

                // helper to get primary key value for related object
                object GetRelatedKey(object obj)
                {
                    var meta = context.Model.FindEntityType(obj.GetType());
                    var pk = meta?.FindPrimaryKey()?.Properties.FirstOrDefault();
                    return pk?.PropertyInfo.GetValue(obj);
                }

                // iterate incoming items
                foreach (var rel in incoming)
                {
                    if (rel is null)
                    {
                        continue;
                    }

                    var relKey = GetRelatedKey(rel);
                    // if rel has non-default key try to find tracked instance
                    bool relHasKey = relKey != null;
                    if (relHasKey)
                    {
                        bool relKeyNonDefault;
                        var t = relKey.GetType();
                        if (t == typeof(Guid)) relKeyNonDefault = (Guid)relKey != Guid.Empty;
                        else if (t.IsValueType) relKeyNonDefault = !Equals(relKey, Activator.CreateInstance(t));
                        else if (t == typeof(string)) relKeyNonDefault = !string.IsNullOrEmpty((string)relKey);
                        else relKeyNonDefault = true;

                        if (relKeyNonDefault)
                        {
                            // try to find tracked or existing entity
                            var tracked = context.Find(rel.GetType(), relKey);
                            if (tracked is not null)
                            {
                                preparedList.Add(tracked);
                                continue;
                            }
                        }
                    }

                    // if no key or not found: assume new or provided detached entity
                    // attach if it appears to represent an existing entity (non-default key but not tracked)
                    if (relHasKey)
                    {
                        try
                        {
                            // try to attach (assume caller knows what they do)
                            context.Attach(rel);
                            preparedList.Add(rel);
                            continue;
                        }
                        catch
                        {
                            // fall back to add below
                        }
                    }

                    // new related entity: set timestamps if applicable and add to context
                    try
                    {
                        // set Created/Updated when available
                        var relType = rel.GetType();
                        var createdPropRel = relType.GetProperty("Created", BindingFlags.Public | BindingFlags.Instance);
                        if (createdPropRel is not null && createdPropRel.PropertyType == typeof(DateTime) && createdPropRel.CanWrite)
                        {
                            var cur = (DateTime)createdPropRel.GetValue(rel);
                            if (cur == default)
                            {
                                createdPropRel.SetValue(rel, DateTime.UtcNow);
                            }
                        }

                        var updatedPropRel = relType.GetProperty("Updated", BindingFlags.Public | BindingFlags.Instance);
                        if (updatedPropRel is not null && updatedPropRel.PropertyType == typeof(DateTime) && updatedPropRel.CanWrite)
                        {
                            updatedPropRel.SetValue(rel, DateTime.UtcNow);
                        }
                    }
                    catch
                    {
                        // ignore timestamp setting errors
                    }

                    // add the related entity (will be inserted with SaveChanges)
                    context.Add(rel);
                    preparedList.Add(rel);
                }

                // assign the prepared list back to the entity's property (if writable)
                if (propInfo.CanWrite)
                {
                    propInfo.SetValue(entity, preparedList);
                }
                else
                {
                    // if property is not settable but is an IList, try to clear/add items
                    var existingCollection = propInfo.GetValue(entity) as IList;
                    if (existingCollection is not null)
                    {
                        existingCollection.Clear();
                        foreach (var it in preparedList)
                        {
                            existingCollection.Add(it);
                        }
                    }
                }
            }

            // finally add the root entity to the context
            context.Add(entity);
        }

        /// <summary>
        /// Updates an existing entity and its related collections in the database context using 
        /// the values from the specified incoming entity. Supports updating navigation properties 
        /// and optionally sets a timestamp if configured.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The type of the entity to update. Must be a reference type.
        /// </typeparam>
        /// <param name="context">
        /// The database context in which the entity is tracked and updated.
        /// </param>
        /// <param name="incomingEntity">
        /// The entity instance containing updated values. Must have a valid primary key value 
        /// corresponding to an existing entity in the context.
        /// </param>
        /// <param name="includeRelations">
        /// A set of navigation property names representing related collections to include and 
        /// synchronize. If null, no relationships are updated.
        /// </param>
        /// <param name="autoSetUpdatedTimestamp">
        /// A value indicating whether to automatically set the 'Updated' property to the current 
        /// UTC time if present.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if incomingEntity is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the entity type does not have a primary key or if no existing entity with 
        /// the specified primary key is found in the context.
        /// </exception>
        public static void UpdateEntity<TEntity>
        (
            this DbContext context,
            TEntity incomingEntity,
            HashSet<string> includeRelations = null,
            bool autoSetUpdatedTimestamp = true
        )
            where TEntity : class, IIndexItem
        {
            ArgumentNullException.ThrowIfNull(incomingEntity);
            includeRelations ??= [];

            // identify primary key
            var entityType = context.Model.FindEntityType(typeof(TEntity));
            var primaryKey = entityType.FindPrimaryKey();
            var keyProperty = primaryKey.Properties.FirstOrDefault()
                ?? throw new InvalidOperationException("No PK found.");
            var keyValue = keyProperty.PropertyInfo.GetValue(incomingEntity);

            // load existing entity via server-side PK lookup
            var existingEntity = context.Find<TEntity>(keyValue)
                ?? throw new InvalidOperationException("Entity not found.");

            // load collection navigations for diffing
            foreach (var relation in includeRelations)
            {
                context.Entry(existingEntity).Collection(relation).Load();
            }

            // update scalar
            context.Entry(existingEntity).CurrentValues.SetValues(incomingEntity);

            // timestamp
            if (autoSetUpdatedTimestamp)
            {
                var updatedProp = typeof(TEntity).GetProperty("Updated");
                if (updatedProp != null && updatedProp.PropertyType == typeof(DateTime) && updatedProp.CanWrite)
                {
                    updatedProp.SetValue(existingEntity, DateTime.UtcNow);
                }
            }

            // update relationships with diff-logic (add/remove instead of clear/add)
            foreach (var relationName in includeRelations)
            {
                var navigation = entityType.FindNavigation(relationName);
                if (navigation is null || !navigation.IsCollection)
                {
                    continue;
                }

                var propInfo = typeof(TEntity).GetProperty(relationName);
                var existingCollection = propInfo.GetValue(existingEntity) as IList;

                if (existingCollection is null || propInfo.GetValue(incomingEntity) is not IEnumerable incomingCollection)
                {
                    continue;
                }

                object GetId(object obj)
                {
                    var type = obj.GetType();
                    var pk = context.Model.FindEntityType(type).FindPrimaryKey().Properties[0];
                    return pk.PropertyInfo.GetValue(obj);
                }

                // convert to list of objects for easier handling
                var incomingList = incomingCollection.Cast<object>().ToList();
                var existingList = existingCollection.Cast<object>().ToList();

                // find items to remove (exist in DB but not in incoming)
                var toRemove = existingList
                    .Where(e => !incomingList.Any(i => GetId(i).Equals(GetId(e))))
                    .ToList();

                // find items to add (exist in incoming but not in DB list)
                var toAdd = incomingList
                    .Where(i => !existingList.Any(e => GetId(e).Equals(GetId(i))))
                    .ToList();

                // process removals
                foreach (var item in toRemove)
                {
                    // remove relationship
                    existingCollection.Remove(item);
                }

                // process additions
                foreach (var item in toAdd)
                {
                    var itemId = GetId(item);
                    var trackedItem = context.Find(item.GetType(), itemId);

                    if (trackedItem is not null)
                    {
                        existingCollection.Add(trackedItem);
                    }
                    else
                    {
                        context.Attach(item);
                        existingCollection.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// Removes an entity and optionally its orphaned related items from the DbContext.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The type of the entity to remove. Must be a reference type and implement IIndexItem.
        /// </typeparam>
        /// <param name="context">The database context.</param>
        /// <param name="incomingEntity">The entity instance identifying the record to remove (must contain primary key).</param>
        /// <param name="includeRelations">
        /// A set of navigation property names representing related collections to include when 
        /// loading the entity. If null, no related collections are inspected for orphan cleanup.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown if incomingEntity is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if no primary key is defined or the entity is not found in the database.</exception>
        public static void RemoveEntity<TEntity>
        (
            this DbContext context,
            TEntity incomingEntity,
            HashSet<string> includeRelations = null
        )
            where TEntity : class, IIndexItem
        {
            ArgumentNullException.ThrowIfNull(incomingEntity);

            includeRelations ??= [];

            // identify primary key
            var entityType = context.Model.FindEntityType(typeof(TEntity));
            var primaryKey = entityType.FindPrimaryKey();
            var keyProperty = primaryKey?.Properties.FirstOrDefault() ?? throw new InvalidOperationException("No PK found.");
            var keyValue = keyProperty.PropertyInfo.GetValue(incomingEntity);

            // prepare query with includes
            var query = context.Set<TEntity>().AsQueryable();

            foreach (var relation in includeRelations)
            {
                var navigation = entityType.FindNavigation(relation);

                if (navigation != null)
                {
                    query = query.Include(relation);
                }
            }

            // load existing entity via server-side PK lookup
            var existingEntity = context.Find<TEntity>(keyValue);

            if (existingEntity is null)
            {
                return; // entity not found, nothing to remove
            }

            // load collection navigations so EF tracks the join-table entries
            foreach (var relation in includeRelations)
            {
                var navigation = entityType.FindNavigation(relation);
                if (navigation != null)
                {
                    context.Entry(existingEntity).Collection(relation).Load();
                }
            }

            // remove the entity (join table entries will be removed on SaveChanges)
            context.Remove(existingEntity);
        }
    }
}
