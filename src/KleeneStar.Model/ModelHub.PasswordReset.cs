using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides utility methods for the credentials of identities: their password and the
    /// one-time links that set it.
    /// </summary>
    /// <remarks>
    /// The password is written here and nowhere else. <see cref="Update(Identity)"/> writes a
    /// whole row, and every copy of an identity that has been out to a client comes back with
    /// its hash blanked - so a profile save that went through it would wipe the password.
    /// These methods touch the credential columns alone, on a freshly read row.
    /// </remarks>
    internal static partial class ModelHub
    {
        /// <summary>
        /// Stores a new password hash on an identity.
        /// </summary>
        /// <param name="identityId">The identity.</param>
        /// <param name="hash">The hash, or <see langword="null"/> to remove the password.</param>
        /// <param name="changed">The point in time the password was set.</param>
        /// <returns><see langword="true"/> when the identity exists.</returns>
        public static bool SetPasswordHash(Guid identityId, string hash, DateTime? changed)
        {
            using var db = CreateDbContext();

            var identity = db.Identities.FirstOrDefault(x => x.Id == identityId);

            if (identity is null)
            {
                return false;
            }

            identity.PasswordHash = hash;
            identity.PasswordChanged = changed;

            db.SaveChanges();

            return true;
        }

        /// <summary>
        /// Binds an external account to the subject its source knows it by.
        /// </summary>
        /// <param name="identityId">The identity.</param>
        /// <param name="subject">The subject.</param>
        /// <returns><see langword="true"/> when the identity exists.</returns>
        public static bool SetExternalSubject(Guid identityId, string subject)
        {
            using var db = CreateDbContext();

            var identity = db.Identities.FirstOrDefault(x => x.Id == identityId);

            if (identity is null)
            {
                return false;
            }

            identity.ExternalSubject = subject;

            db.SaveChanges();

            return true;
        }

        /// <summary>
        /// Returns the reset link whose secret hashes to the supplied value.
        /// </summary>
        /// <param name="tokenHash">The hash of the link's secret.</param>
        /// <returns>The link, or <see langword="null"/>.</returns>
        public static PasswordReset GetPasswordResetByHash(string tokenHash)
        {
            if (string.IsNullOrEmpty(tokenHash))
            {
                return null;
            }

            using var db = CreateDbContext();

            return db.PasswordResets
                .AsNoTracking()
                .FirstOrDefault(x => x.TokenHash == tokenHash);
        }

        /// <summary>
        /// Stores a new reset link and spends every link of the same account that is still
        /// open, in one save - so an account never has two links that work.
        /// </summary>
        /// <param name="reset">The link to store.</param>
        public static void AddPasswordReset(PasswordReset reset)
        {
            ArgumentNullException.ThrowIfNull(reset);

            using var db = CreateDbContext();

            var now = DateTime.UtcNow;

            foreach (var open in db.PasswordResets.Where(x => x.IdentityId == reset.IdentityId && x.Used == null))
            {
                open.Used = now;
            }

            db.PasswordResets.Add(reset);
            db.SaveChanges();
        }

        /// <summary>
        /// Spends a reset link and sets the password it was issued for, in one save.
        /// </summary>
        /// <remarks>
        /// The link is re-read inside the write and must still be open, so two submissions of
        /// the same link cannot both set a password.
        /// </remarks>
        /// <param name="resetId">The link.</param>
        /// <param name="hash">The hash of the new password.</param>
        /// <returns>The identity whose password was set, or <see langword="null"/> when the
        /// link was no longer open.</returns>
        public static Guid? CompletePasswordReset(Guid resetId, string hash)
        {
            using var db = CreateDbContext();

            var now = DateTime.UtcNow;
            var reset = db.PasswordResets.FirstOrDefault(x => x.Id == resetId);

            if (reset is null || !reset.IsOpen(now))
            {
                return null;
            }

            var identity = db.Identities.FirstOrDefault(x => x.Id == reset.IdentityId);

            if (identity is null)
            {
                return null;
            }

            reset.Used = now;
            identity.PasswordHash = hash;
            identity.PasswordChanged = now;

            db.SaveChanges();

            return identity.Id;
        }
    }
}
