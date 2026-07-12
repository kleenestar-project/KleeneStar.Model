namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the lifecycle state of an <see cref="Attachment"/>.
    /// </summary>
    public enum AttachmentState
    {
        /// <summary>
        /// The attachment is available and can be downloaded.
        /// </summary>
        Active,

        /// <summary>
        /// The attachment has been uploaded but is held back from download pending a
        /// virus / content scan. The row is shown in the file list but the download link
        /// is suppressed.
        /// </summary>
        Quarantined,

        /// <summary>
        /// The attachment has been soft-deleted by a user or moderator. The row is kept
        /// for audit purposes but is no longer shown in the file list.
        /// </summary>
        Deleted
    }

    /// <summary>
    /// Provides extension methods for the <see cref="AttachmentState"/> enumeration.
    /// </summary>
    public static class AttachmentStateExtensions
    {
        /// <summary>
        /// Determines whether the supplied state corresponds to an attachment that is
        /// currently available for download.
        /// </summary>
        /// <param name="state">The attachment state.</param>
        /// <returns><c>true</c> when the attachment can be downloaded; <c>false</c> otherwise.</returns>
        public static bool IsAvailable(this AttachmentState state)
        {
            return state == AttachmentState.Active;
        }

        /// <summary>
        /// Determines whether the supplied state corresponds to an attachment that should
        /// be listed in the object's file list (everything except soft-deleted rows).
        /// </summary>
        /// <param name="state">The attachment state.</param>
        /// <returns><c>true</c> when the attachment is visible; <c>false</c> otherwise.</returns>
        public static bool IsVisible(this AttachmentState state)
        {
            return state != AttachmentState.Deleted;
        }
    }
}
