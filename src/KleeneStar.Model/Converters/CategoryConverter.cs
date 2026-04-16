using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;

namespace KleeneStar.Model.Converters
{
    public class CategoryConverter : IRestValueConverter
    {
        /// <summary>
        /// Converts a raw value to the specified target type.
        /// </summary>
        /// <param name="rawValue">
        /// The value to convert. Can be any object representing the raw data to be transformed.
        /// </param>
        /// <param name="targetType">
        /// The type to which the raw value should be converted. Cannot be null.
        /// </param>
        /// <returns>
        /// An object of the specified target type representing the converted value.
        /// </returns>
        public object FromRaw(object rawValue, Type targetType)
        {
            if (rawValue is null)
            {
                return null;
            }

            if (rawValue is string s)
            {
                var names = s.Split(";", StringSplitOptions.RemoveEmptyEntries)
                             .Select(x => x.Trim())
                             .Where(x => x.Length > 0)
                             .ToList();

                using var db = ModelHub.CreateDbContext();

                var existing = db.Categories
                    .AsNoTracking()
                    .Where(c => names.Contains(c.Name))
                    .ToList();

                var result = new List<Category>(existing);

                foreach (var name in names)
                {
                    if (!existing.Any(c => c.Name == name))
                    {
                        var newCat = new Category
                        {
                            Name = name,
                            Description = "",
                            Id = Guid.NewGuid()
                        };

                        result.Add(newCat);
                    }
                }

                return result;
            }

            return rawValue;
        }

        /// <summary>
        /// Converts the specified value to its raw representation based on the provided source type.
        /// </summary>
        /// <param name="value">
        /// The value to convert to a raw representation. Can be null if the conversion supports 
        /// null values.</param>
        /// <param name="sourceType">
        /// The type that describes how the value should be interpreted and converted. Cannot be null.
        /// </param>
        /// <returns>
        /// An object representing the raw form of the input value, as determined by the source type.
        /// </returns>
        public object ToRaw(object value, Type sourceType)
        {
            return value;
        }
    }
}
