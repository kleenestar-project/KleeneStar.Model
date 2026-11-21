using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Normalizes the key persisted in <see cref="Identity.AuthenticationSource"/>.
    /// </summary>
    /// <remarks>
    /// The column holds <see langword="null"/> for an internal account, so "internal" has one
    /// spelling in the data however a caller wrote it. <see cref="Local"/> is the key the
    /// internal source is registered under, and <see cref="Normalize"/> folds it back into the
    /// unset state before anything persists it.
    /// </remarks>
    public static class IdentitySource
    {
        /// <summary>
        /// The key of the internal source: the account's password is kept by this
        /// installation and checked against <see cref="Identity.PasswordHash"/>.
        /// </summary>
        public const string Local = "local";

        /// <summary>
        /// The keys whose selection ids have been handed out, by id - so the id a picker submits
        /// can be read back as the key it stands for.
        /// </summary>
        private static readonly ConcurrentDictionary<Guid, string> _known = new();

        /// <summary>
        /// Initializes the class with the internal source known.
        /// </summary>
        static IdentitySource()
        {
            Remember(Local);
        }

        /// <summary>
        /// Answers the selection id a key is offered under.
        /// </summary>
        /// <remarks>
        /// A picker identifies its entries by guid, a source by a key, so the id is derived from
        /// the key - the same on every start and every node - and remembered, so it can be read
        /// back by <see cref="FromId"/>.
        /// </remarks>
        /// <param name="key">The key. May be null, which is the internal source.</param>
        /// <returns>The selection id.</returns>
        public static Guid IdOf(string key)
        {
            var normalized = Normalize(key) ?? Local;
            var id = new Guid(SHA256.HashData(Encoding.UTF8.GetBytes("kleenestar.authenticationsource:" + normalized)).AsSpan(0, 16));

            _known.TryAdd(id, normalized);

            return id;
        }

        /// <summary>
        /// Makes a key's selection id readable by <see cref="FromId"/>. The catalog of sources
        /// calls it for every source it registers.
        /// </summary>
        /// <param name="key">The key.</param>
        public static void Remember(string key)
        {
            IdOf(key);
        }

        /// <summary>
        /// Reads what a picker or a client submitted as a source key: the key a known selection
        /// id stands for, or the value itself - so a hand-written client may send the key.
        /// </summary>
        /// <param name="value">The submitted value. May be null.</param>
        /// <returns>The key, or the value unchanged when it is no known selection id.</returns>
        public static string FromId(string value)
        {
            return Guid.TryParse(value?.Trim(), out var id) && _known.TryGetValue(id, out var key)
                ? key
                : value;
        }

        /// <summary>
        /// Normalizes a source key: trimmed and lower-cased, with a blank key and
        /// <see cref="Local"/> both answered as <see langword="null"/> - the internal source.
        /// </summary>
        /// <param name="key">The key. May be null.</param>
        /// <returns>The normalized key, or <see langword="null"/> for the internal source.</returns>
        public static string Normalize(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            var normalized = key.Trim().ToLowerInvariant();

            return normalized == Local ? null : normalized;
        }

        /// <summary>
        /// Determines whether the supplied key names the internal source.
        /// </summary>
        /// <param name="key">The key. May be null.</param>
        /// <returns><see langword="true"/> for the internal source.</returns>
        public static bool IsLocal(string key)
        {
            return Normalize(key) is null;
        }

        /// <summary>
        /// Determines whether the supplied identity is an internal account, whose password
        /// this installation keeps.
        /// </summary>
        /// <param name="identity">The identity. May be null, which answers false.</param>
        /// <returns><see langword="true"/> for an internal account.</returns>
        public static bool IsLocal(Identity identity)
        {
            return identity is not null && IsLocal(identity.AuthenticationSource);
        }

        /// <summary>
        /// Answers the key the supplied identity is authenticated by, spelled the way the
        /// catalog registers it - <see cref="Local"/> for an internal account.
        /// </summary>
        /// <param name="identity">The identity. May be null.</param>
        /// <returns>The source key. Never null.</returns>
        public static string KeyOf(Identity identity)
        {
            return Normalize(identity?.AuthenticationSource) ?? Local;
        }

        /// <summary>
        /// Compares two source keys after normalization.
        /// </summary>
        /// <param name="left">The first key.</param>
        /// <param name="right">The second key.</param>
        /// <returns><see langword="true"/> when both name the same source.</returns>
        public static bool AreEqual(string left, string right)
        {
            return string.Equals(Normalize(left), Normalize(right), StringComparison.Ordinal);
        }
    }
}
