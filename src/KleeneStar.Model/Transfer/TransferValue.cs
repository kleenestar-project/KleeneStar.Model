using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;

namespace KleeneStar.Model.Transfer
{
    /// <summary>
    /// Writes the values of the model into the export file and reads them back, as tolerantly as
    /// a file written by another version of the application needs.
    /// </summary>
    /// <remarks>
    /// A value is written in the form the database stores it, after the value converter of its
    /// property (an icon as its address, a list as its JSON text), except that an enumeration is
    /// written by the name of its member: a name survives a reordered enumeration, a number does
    /// not. Reading accepts every reasonable spelling (a number as text, a name for a number, a
    /// member name in any case, a JSON array where a JSON text was written); what cannot be read
    /// is reported as missing and the property stays empty.
    /// </remarks>
    internal static class TransferValue
    {
        /// <summary>
        /// The options of the JSON values that are no scalars.
        /// </summary>
        private static readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
        };

        /// <summary>
        /// Writes the value of a property.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value of the entity (before conversion).</param>
        public static void Write(Utf8JsonWriter writer, IProperty property, object value)
        {
            var converter = property.GetValueConverter();

            if (value is not null && converter is not null)
            {
                try
                {
                    value = converter.ConvertToProvider(value);
                }
                catch (Exception)
                {
                    value = null;
                }
            }

            WriteScalar(writer, value);
        }

        /// <summary>
        /// Writes a value that has no property (a translated reference).
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value">The value.</param>
        public static void WriteScalar(Utf8JsonWriter writer, object value)
        {
            switch (value)
            {
                case null:
                    writer.WriteNullValue();
                    break;
                case string s:
                    writer.WriteStringValue(s);
                    break;
                case bool b:
                    writer.WriteBooleanValue(b);
                    break;
                case Guid g:
                    writer.WriteStringValue(g.ToString("D"));
                    break;
                case DateTime dt:
                    writer.WriteStringValue(dt.ToString("O", CultureInfo.InvariantCulture));
                    break;
                case DateTimeOffset dto:
                    writer.WriteStringValue(dto.ToString("O", CultureInfo.InvariantCulture));
                    break;
                case DateOnly d:
                    writer.WriteStringValue(d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                    break;
                case TimeOnly t:
                    writer.WriteStringValue(t.ToString("HH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture));
                    break;
                case TimeSpan ts:
                    writer.WriteStringValue(ts.ToString("c", CultureInfo.InvariantCulture));
                    break;
                case Enum e:
                    writer.WriteStringValue(e.ToString());
                    break;
                case byte[] bytes:
                    writer.WriteBase64StringValue(bytes);
                    break;
                case char c:
                    writer.WriteStringValue(c.ToString());
                    break;
                case byte or sbyte or short or ushort or int or uint or long:
                    writer.WriteNumberValue(Convert.ToInt64(value, CultureInfo.InvariantCulture));
                    break;
                case ulong ul:
                    writer.WriteNumberValue(ul);
                    break;
                case float f:
                    writer.WriteNumberValue(f);
                    break;
                case double d:
                    writer.WriteNumberValue(d);
                    break;
                case decimal m:
                    writer.WriteNumberValue(m);
                    break;
                default:
                    JsonSerializer.Serialize(writer, value, value.GetType(), _options);
                    break;
            }
        }

        /// <summary>
        /// Reads the value of a property.
        /// </summary>
        /// <param name="element">The value in the file.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value for the entity.</param>
        /// <returns><see langword="true"/> when the value could be read.</returns>
        public static bool TryRead(JsonElement element, IProperty property, out object value)
        {
            var converter = property.GetValueConverter();
            var type = converter?.ProviderClrType ?? property.ClrType;

            if (!TryReadScalar(element, type, out var provider))
            {
                value = null;

                return false;
            }

            if (provider is null)
            {
                value = null;

                // a null for a value that cannot be null is no value
                return property.IsNullable || !property.ClrType.IsValueType || Nullable.GetUnderlyingType(property.ClrType) is not null;
            }

            if (converter is null)
            {
                value = provider;

                return true;
            }

            try
            {
                value = converter.ConvertFromProvider(provider);

                return true;
            }
            catch (Exception)
            {
                value = null;

                return false;
            }
        }

        /// <summary>
        /// Reads a value of a type.
        /// </summary>
        /// <param name="element">The value in the file.</param>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        /// <returns><see langword="true"/> when the value could be read.</returns>
        public static bool TryReadScalar(JsonElement element, Type type, out object value)
        {
            value = null;

            if (element.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
            {
                return true;
            }

            var target = Nullable.GetUnderlyingType(type) ?? type;

            try
            {
                if (target == typeof(string))
                {
                    value = element.ValueKind switch
                    {
                        JsonValueKind.String => element.GetString(),
                        // a structured value where text was written before: its JSON text
                        _ => element.GetRawText()
                    };

                    return true;
                }

                if (target == typeof(Guid))
                {
                    if (element.ValueKind == JsonValueKind.String && Guid.TryParse(element.GetString(), out var guid))
                    {
                        value = guid;

                        return true;
                    }

                    return false;
                }

                if (target == typeof(bool))
                {
                    switch (element.ValueKind)
                    {
                        case JsonValueKind.True:
                            value = true;
                            return true;
                        case JsonValueKind.False:
                            value = false;
                            return true;
                        case JsonValueKind.Number:
                            value = element.GetDouble() != 0;
                            return true;
                        case JsonValueKind.String:
                            var text = element.GetString()?.Trim();

                            if (bool.TryParse(text, out var parsed))
                            {
                                value = parsed;
                                return true;
                            }

                            if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
                            {
                                value = number != 0;
                                return true;
                            }

                            return false;
                        default:
                            return false;
                    }
                }

                if (target.IsEnum)
                {
                    return TryReadEnum(element, target, out value);
                }

                if (IsNumber(target))
                {
                    return TryReadNumber(element, target, out value);
                }

                if (target == typeof(DateTime))
                {
                    if (element.ValueKind == JsonValueKind.String
                        && DateTime.TryParse(element.GetString(), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dt))
                    {
                        value = dt;
                        return true;
                    }

                    if (element.ValueKind == JsonValueKind.Number && element.TryGetInt64(out var unix))
                    {
                        value = DateTimeOffset.FromUnixTimeMilliseconds(unix).UtcDateTime;
                        return true;
                    }

                    return false;
                }

                if (target == typeof(DateTimeOffset))
                {
                    if (element.ValueKind == JsonValueKind.String
                        && DateTimeOffset.TryParse(element.GetString(), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dto))
                    {
                        value = dto;
                        return true;
                    }

                    return false;
                }

                if (target == typeof(DateOnly))
                {
                    var text = element.ValueKind == JsonValueKind.String ? element.GetString() : null;

                    if (DateOnly.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
                    {
                        value = d;
                        return true;
                    }

                    if (DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dt))
                    {
                        value = DateOnly.FromDateTime(dt);
                        return true;
                    }

                    return false;
                }

                if (target == typeof(TimeOnly))
                {
                    var text = element.ValueKind == JsonValueKind.String ? element.GetString() : null;

                    if (TimeOnly.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out var t))
                    {
                        value = t;
                        return true;
                    }

                    if (TimeSpan.TryParse(text, CultureInfo.InvariantCulture, out var span) && span >= TimeSpan.Zero && span < TimeSpan.FromDays(1))
                    {
                        value = TimeOnly.FromTimeSpan(span);
                        return true;
                    }

                    return false;
                }

                if (target == typeof(TimeSpan))
                {
                    if (element.ValueKind == JsonValueKind.String && TimeSpan.TryParse(element.GetString(), CultureInfo.InvariantCulture, out var span))
                    {
                        value = span;
                        return true;
                    }

                    if (element.ValueKind == JsonValueKind.Number)
                    {
                        value = TimeSpan.FromTicks(element.GetInt64());
                        return true;
                    }

                    return false;
                }

                if (target == typeof(byte[]))
                {
                    if (element.ValueKind == JsonValueKind.String && element.TryGetBytesFromBase64(out var bytes))
                    {
                        value = bytes;
                        return true;
                    }

                    if (element.ValueKind == JsonValueKind.Array)
                    {
                        value = element.EnumerateArray().Select(x => (byte)x.GetInt32()).ToArray();
                        return true;
                    }

                    return false;
                }

                if (target == typeof(char))
                {
                    var text = element.ValueKind == JsonValueKind.String ? element.GetString() : element.GetRawText();

                    if (!string.IsNullOrEmpty(text))
                    {
                        value = text[0];
                        return true;
                    }

                    return false;
                }

                // a structured value (a list, a dictionary): its JSON, or JSON written as text
                var json = element.ValueKind == JsonValueKind.String ? element.GetString() : element.GetRawText();

                value = JsonSerializer.Deserialize(json, target, _options);

                return true;
            }
            catch (Exception ex) when (ex is FormatException or JsonException or InvalidOperationException or OverflowException or ArgumentException or NotSupportedException)
            {
                value = null;

                return false;
            }
        }

        /// <summary>
        /// Returns the values of a property that are references (one, or each of a list).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The references.</returns>
        public static IEnumerable<object> References(object value)
        {
            return Values(value).Select(Key);
        }

        /// <summary>
        /// Returns the values of a property as they are (one, or each of a list).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The values.</returns>
        public static IEnumerable<object> Values(object value)
        {
            if (value is null)
            {
                yield break;
            }

            if (value is string || value.GetType().IsValueType)
            {
                yield return value;
                yield break;
            }

            if (value is IEnumerable list)
            {
                foreach (var item in list)
                {
                    if (item is not null)
                    {
                        yield return item;
                    }
                }
            }
        }

        /// <summary>
        /// Returns the comparable form of a key value: a text naming an id is the id, so a scope
        /// id written as text matches the id of a workspace.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The comparable value.</returns>
        public static object Key(object value)
        {
            return value switch
            {
                string s when Guid.TryParse(s, out var guid) => guid,
                string s => s.ToUpperInvariant(),
                _ => value
            };
        }

        /// <summary>
        /// Converts a key value into the type of a property, so it can be compared in a query.
        /// </summary>
        /// <param name="value">The key value.</param>
        /// <param name="type">The type of the property.</param>
        /// <returns>The values to compare with (a text has more than one spelling).</returns>
        public static IEnumerable<object> ToPropertyType(object value, Type type)
        {
            var target = Nullable.GetUnderlyingType(type) ?? type;

            if (value is null)
            {
                yield break;
            }

            if (target == typeof(string))
            {
                if (value is Guid guid)
                {
                    yield return guid.ToString("D");
                    yield return guid.ToString("D").ToUpperInvariant();
                }
                else
                {
                    yield return value.ToString();
                }

                yield break;
            }

            if (target.IsInstanceOfType(value))
            {
                yield return value;
                yield break;
            }

            if (target == typeof(Guid) && value is string s && Guid.TryParse(s, out var parsed))
            {
                yield return parsed;
                yield break;
            }

            if (IsNumber(target) && value is IConvertible)
            {
                object converted;

                try
                {
                    converted = Convert.ChangeType(value, target, CultureInfo.InvariantCulture);
                }
                catch (Exception ex) when (ex is FormatException or InvalidCastException or OverflowException)
                {
                    converted = null;
                }

                if (converted is not null)
                {
                    yield return converted;
                }
            }
        }

        /// <summary>
        /// Reads an enumeration member by name, number or numeric text; flags combine.
        /// </summary>
        private static bool TryReadEnum(JsonElement element, Type type, out object value)
        {
            value = null;

            if (element.ValueKind == JsonValueKind.Number && element.TryGetInt64(out var number))
            {
                value = Enum.ToObject(type, number);
                return true;
            }

            if (element.ValueKind != JsonValueKind.String)
            {
                return false;
            }

            var text = element.GetString()?.Trim();

            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            if (Enum.TryParse(type, text, ignoreCase: true, out var parsed))
            {
                value = parsed;
                return true;
            }

            // a combination of flags of which one is unknown to this version: the known ones
            if (type.IsDefined(typeof(FlagsAttribute), false))
            {
                long combined = 0;
                var any = false;

                foreach (var part in text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    if (Enum.TryParse(type, part, ignoreCase: true, out var member))
                    {
                        combined |= Convert.ToInt64(member, CultureInfo.InvariantCulture);
                        any = true;
                    }
                }

                if (any)
                {
                    value = Enum.ToObject(type, combined);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Reads a number of a numeric type.
        /// </summary>
        private static bool TryReadNumber(JsonElement element, Type type, out object value)
        {
            value = null;

            object raw = element.ValueKind switch
            {
                JsonValueKind.Number when element.TryGetDecimal(out var m) => m,
                JsonValueKind.Number => element.GetDouble(),
                JsonValueKind.String when decimal.TryParse(element.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var m) => m,
                JsonValueKind.String when double.TryParse(element.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var d) => d,
                JsonValueKind.True => 1m,
                JsonValueKind.False => 0m,
                _ => null
            };

            if (raw is null)
            {
                return false;
            }

            try
            {
                // integers take the integral part of a fraction
                if (type != typeof(double) && type != typeof(float) && type != typeof(decimal))
                {
                    raw = raw is decimal dm ? decimal.Truncate(dm) : Math.Truncate((double)raw);
                }

                value = Convert.ChangeType(raw, type, CultureInfo.InvariantCulture);

                return true;
            }
            catch (Exception ex) when (ex is OverflowException or InvalidCastException or FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns whether a type is numeric.
        /// </summary>
        private static bool IsNumber(Type type)
        {
            return type == typeof(byte) || type == typeof(sbyte) || type == typeof(short) || type == typeof(ushort)
                || type == typeof(int) || type == typeof(uint) || type == typeof(long) || type == typeof(ulong)
                || type == typeof(float) || type == typeof(double) || type == typeof(decimal);
        }
    }
}
