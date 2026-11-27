using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;

namespace KleeneStar.Model.Transfer
{
    /// <summary>
    /// Reads and writes the properties of an entity whose type is only known at run time.
    /// </summary>
    internal static class TransferEntity
    {
        /// <summary>
        /// Returns the entity type of a row: the most derived type of its table.
        /// </summary>
        /// <param name="root">The root entity type of the table.</param>
        /// <param name="entity">The row.</param>
        /// <returns>The entity type.</returns>
        public static IEntityType TypeOf(IEntityType root, object entity)
        {
            if (root.HasSharedClrType)
            {
                return root;
            }

            return root.Model.FindEntityType(entity.GetType()) ?? root;
        }

        /// <summary>
        /// Returns the value of a property.
        /// </summary>
        /// <param name="context">The context tracking the row (needed for shadow values).</param>
        /// <param name="type">The entity type of the row.</param>
        /// <param name="entity">The row.</param>
        /// <param name="property">The property.</param>
        /// <returns>The value.</returns>
        public static object Get(DbContext context, IEntityType type, object entity, IProperty property)
        {
            if (entity is IDictionary<string, object> bag)
            {
                return bag.TryGetValue(property.Name, out var value) ? value : null;
            }

            if (property.IsShadowProperty())
            {
                if (property == type.FindDiscriminatorProperty())
                {
                    return type.GetDiscriminatorValue();
                }

                return context.Entry(entity).Property(property.Name).CurrentValue;
            }

            if (property.PropertyInfo is { } info && info.CanRead)
            {
                return info.GetValue(entity);
            }

            return property.FieldInfo?.GetValue(entity);
        }

        /// <summary>
        /// Creates a row of an entity type.
        /// </summary>
        /// <param name="type">The entity type.</param>
        /// <returns>The row.</returns>
        public static object Create(IEntityType type)
        {
            return type.HasSharedClrType && type.ClrType == typeof(Dictionary<string, object>)
                ? new Dictionary<string, object>()
                : Activator.CreateInstance(type.ClrType, nonPublic: true);
        }

        /// <summary>
        /// Sets the value of a property of a row that is not tracked yet. Shadow properties are
        /// left to <see cref="SetShadow"/>.
        /// </summary>
        /// <param name="entity">The row.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        /// <returns><see langword="true"/> when the value was set.</returns>
        public static bool Set(object entity, IProperty property, object value)
        {
            if (entity is IDictionary<string, object> bag)
            {
                bag[property.Name] = value;

                return true;
            }

            if (property.IsShadowProperty())
            {
                return false;
            }

            try
            {
                if (property.PropertyInfo is { } info && info.CanWrite)
                {
                    info.SetValue(entity, value);

                    return true;
                }

                if (property.FieldInfo is { } field)
                {
                    field.SetValue(entity, value);

                    return true;
                }
            }
            catch (ArgumentException)
            {
                // a value of a type the property does not take stays out
            }

            return false;
        }

        /// <summary>
        /// Sets a shadow value of a tracked row (never the discriminator, which the type sets).
        /// </summary>
        /// <param name="context">The context tracking the row.</param>
        /// <param name="type">The entity type.</param>
        /// <param name="entity">The row.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        public static void SetShadow(DbContext context, IEntityType type, object entity, IProperty property, object value)
        {
            if (!property.IsShadowProperty() || property == type.FindDiscriminatorProperty() || entity is IDictionary<string, object>)
            {
                return;
            }

            context.Entry(entity).Property(property.Name).CurrentValue = value;
        }
    }
}
