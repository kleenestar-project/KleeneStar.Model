using KleeneStar.Model.Entities;
using Microsoft.AspNetCore.Identity;
using System;

namespace KleeneStar.Model
{
    /// <summary>
    /// Hashes and verifies the passwords of internal accounts.
    /// </summary>
    /// <remarks>
    /// The scheme is ASP.NET's <see cref="PasswordHasher{TUser}"/> (PBKDF2, salted, with the
    /// format marker and iteration count inside the hash), which is also what WebExpress's
    /// <c>LocalIdentityProvider</c> checks - so a hash written here is one the framework's own
    /// <c>/api/auth/login</c> accepts, and a later raise of the work factor re-hashes on the
    /// next sign-in instead of invalidating every stored password. It lives in the model
    /// rather than in the core because the seeder writes hashes too.
    /// </remarks>
    public static class IdentityPassword
    {
        private static readonly PasswordHasher<Identity> _hasher = new();

        /// <summary>
        /// The hash verified when there is no account to verify against, so an unknown name
        /// costs the same work as a known one and the response time does not tell them apart.
        /// </summary>
        private static readonly string _dummyHash = _hasher.HashPassword(null, Guid.NewGuid().ToString());

        /// <summary>
        /// The prefix of the placeholders the seeder wrote before passwords were checked.
        /// Nothing produces such a hash; <see cref="KleeneStarDbSeeder"/> replaces them.
        /// </summary>
        public const string SeedPlaceholderPrefix = "$seed$";

        /// <summary>
        /// Hashes a password for the supplied account.
        /// </summary>
        /// <param name="identity">The account. May be null; the scheme does not read it.</param>
        /// <param name="password">The password. Must not be null.</param>
        /// <returns>The hash to store in <see cref="Identity.PasswordHash"/>.</returns>
        public static string Hash(Identity identity, string password)
        {
            ArgumentNullException.ThrowIfNull(password);

            return _hasher.HashPassword(identity, password);
        }

        /// <summary>
        /// Verifies a password against the hash an account carries.
        /// </summary>
        /// <param name="identity">The account, or <see langword="null"/> when the name named
        /// none - which is still verified, against a dummy, to cost the same time.</param>
        /// <param name="password">The offered password.</param>
        /// <returns>The outcome: <see cref="PasswordVerificationResult.SuccessRehashNeeded"/>
        /// asks the caller to store a fresh hash of the password it just verified.</returns>
        public static PasswordVerificationResult Verify(Identity identity, string password)
        {
            var hash = identity?.PasswordHash;
            var usable = IsUsable(hash);

            try
            {
                var result = _hasher.VerifyHashedPassword(identity, usable ? hash : _dummyHash, password ?? string.Empty);

                return usable ? result : PasswordVerificationResult.Failed;
            }
            catch (FormatException)
            {
                return PasswordVerificationResult.Failed;
            }
        }

        /// <summary>
        /// Determines whether a stored hash is one a password can match: present, and not one
        /// of the seeder's old placeholders.
        /// </summary>
        /// <param name="hash">The stored hash. May be null.</param>
        /// <returns><see langword="true"/> when a password can be verified against it.</returns>
        public static bool IsUsable(string hash)
        {
            return !string.IsNullOrEmpty(hash)
                && !hash.StartsWith(SeedPlaceholderPrefix, StringComparison.Ordinal);
        }
    }
}
