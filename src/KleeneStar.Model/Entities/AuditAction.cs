using System;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Names what was done. The action is the verb of an <see cref="AuditEvent"/>: it says what
    /// happened, while the <see cref="AuditDelta"/> entries beneath it say what that did to the
    /// state of the target.
    /// </summary>
    /// <remarks>
    /// Every event carries one of these rather than a sentence. A log line reading "user admin
    /// deleted class Bug" cannot be filtered, counted or compared across languages; the triple
    /// (<see cref="AuditOrigin"/>, <see cref="AuditCategory"/>, <see cref="AuditAction"/>) can,
    /// and the sentence is reconstructed for display from those three plus the target. That is
    /// why the enumeration is long: an action that has to be spelled out in prose is an action
    /// that is missing from it.
    /// <para>
    /// The values are persisted as their ordinal, so new entries are appended rather than
    /// inserted. <see cref="Created"/> is first because it is the most common.
    /// </para>
    /// </remarks>
    public enum AuditAction
    {
        /// <summary>
        /// A record was brought into existence. Its deltas carry every populated attribute, so
        /// the log is replayable from this point without consulting the record itself.
        /// </summary>
        Created,

        /// <summary>
        /// An existing record was modified. Its deltas carry only the attributes that moved.
        /// </summary>
        Updated,

        /// <summary>
        /// A record was removed. Its deltas carry the attributes it held at the moment it went,
        /// so the log preserves what was lost.
        /// </summary>
        Deleted,

        /// <summary>
        /// A record was taken out of active use without being removed.
        /// </summary>
        Archived,

        /// <summary>
        /// A previous state of a record was reapplied, or an archived record was brought back.
        /// </summary>
        Restored,

        /// <summary>
        /// An object travelled a workflow transition into another status.
        /// </summary>
        Transitioned,

        /// <summary>
        /// An identity authenticated successfully.
        /// </summary>
        SignedIn,

        /// <summary>
        /// An identity ended its session deliberately.
        /// </summary>
        SignedOut,

        /// <summary>
        /// An authentication attempt was rejected. Recorded with
        /// <see cref="AuditOutcome.Failed"/> and without an actor, because the credential did
        /// not identify one.
        /// </summary>
        SignInFailed,

        /// <summary>
        /// A session was ended by the installation rather than by its owner: revoked from the
        /// device list, or invalidated by an administrator.
        /// </summary>
        SessionRevoked,

        /// <summary>
        /// A group was granted a policy on a scope.
        /// </summary>
        PermissionGranted,

        /// <summary>
        /// A group's policy on a scope was withdrawn.
        /// </summary>
        PermissionRevoked,

        /// <summary>
        /// A request was refused because the caller held no policy that allowed it. The
        /// attempted operation is recorded as deltas.
        /// </summary>
        AccessDenied,

        /// <summary>
        /// A personal access token was created. The secret itself never enters the log; the
        /// prefix and the scopes do.
        /// </summary>
        TokenIssued,

        /// <summary>
        /// A personal access token was withdrawn before its expiry.
        /// </summary>
        TokenRevoked,

        /// <summary>
        /// The installation began serving requests.
        /// </summary>
        Started,

        /// <summary>
        /// The installation stopped serving requests.
        /// </summary>
        Stopped,

        /// <summary>
        /// The database schema was brought to a new version.
        /// </summary>
        Migrated,

        /// <summary>
        /// The installation populated itself with its initial data set.
        /// </summary>
        Seeded,

        /// <summary>
        /// Audit events older than the retention horizon were removed. The event names the
        /// range that went and the hash it ended on, so the surviving chain stays verifiable
        /// and the gap is itself part of the record.
        /// </summary>
        Pruned,

        /// <summary>
        /// An SLA target was escalated to its next level.
        /// </summary>
        Escalated,

        /// <summary>
        /// An SLA target elapsed without being met.
        /// </summary>
        Breached,

        /// <summary>
        /// Records were taken in from outside the installation.
        /// </summary>
        Imported,

        /// <summary>
        /// Records were handed out of the installation.
        /// </summary>
        Exported,

        /// <summary>
        /// An endpoint outside the installation boundary was called. Used for API traffic that
        /// is worth recording as access even when it changed nothing.
        /// </summary>
        Invoked
    }

    /// <summary>
    /// Provides extension methods for the <see cref="AuditAction"/> enumeration.
    /// </summary>
    public static class AuditActionExtensions
    {
        /// <summary>
        /// Returns the wire token the REST API and the quickfilters exchange the action as.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>The lower-case wire token.</returns>
        public static string Token(this AuditAction action)
        {
            return action switch
            {
                AuditAction.Created => "created",
                AuditAction.Updated => "updated",
                AuditAction.Deleted => "deleted",
                AuditAction.Archived => "archived",
                AuditAction.Restored => "restored",
                AuditAction.Transitioned => "transitioned",
                AuditAction.SignedIn => "signedin",
                AuditAction.SignedOut => "signedout",
                AuditAction.SignInFailed => "signinfailed",
                AuditAction.SessionRevoked => "sessionrevoked",
                AuditAction.PermissionGranted => "permissiongranted",
                AuditAction.PermissionRevoked => "permissionrevoked",
                AuditAction.AccessDenied => "accessdenied",
                AuditAction.TokenIssued => "tokenissued",
                AuditAction.TokenRevoked => "tokenrevoked",
                AuditAction.Started => "started",
                AuditAction.Stopped => "stopped",
                AuditAction.Migrated => "migrated",
                AuditAction.Seeded => "seeded",
                AuditAction.Pruned => "pruned",
                AuditAction.Escalated => "escalated",
                AuditAction.Breached => "breached",
                AuditAction.Imported => "imported",
                AuditAction.Exported => "exported",
                AuditAction.Invoked => "invoked",
                _ => "updated"
            };
        }

        /// <summary>
        /// Returns the localized text key for the action, suitable for passing to
        /// <c>I18N.Translate</c>.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>The translation key.</returns>
        public static string Text(this AuditAction action)
        {
            return string.Concat("kleenestar.core:audit.action.", action.Token());
        }

        /// <summary>
        /// Parses a wire token into the matching action. An unknown, empty or <c>null</c> token
        /// reads as <see cref="AuditAction.Updated"/>.
        /// </summary>
        /// <param name="token">The wire token.</param>
        /// <returns>The parsed action.</returns>
        public static AuditAction Parse(string token)
        {
            var normalized = (token?.Trim() ?? string.Empty).ToLowerInvariant();

            foreach (var candidate in Enum.GetValues<AuditAction>())
            {
                if (candidate.Token() == normalized)
                {
                    return candidate;
                }
            }

            return AuditAction.Updated;
        }
    }
}
