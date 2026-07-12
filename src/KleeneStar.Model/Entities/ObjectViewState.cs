using System;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the state of an <see cref="ObjectView"/>.
    /// </summary>
    public enum ObjectViewState
    {
        /// <summary>
        /// The view is active and rendered as a tab inside the workspace.
        /// </summary>
        Active,

        /// <summary>
        /// The view is archived and hidden from the tab control.
        /// </summary>
        Archived
    }

    /// <summary>
    /// Provides extension methods for <see cref="ObjectViewState"/>.
    /// </summary>
    public static class ObjectViewStateExtensions
    {
        /// <summary>
        /// Returns the well-known unique identifier for the specified state.
        /// </summary>
        public static Guid Id(this ObjectViewState state)
        {
            return state switch
            {
                ObjectViewState.Active => Guid.Parse("1AA2BB33-4455-6677-8899-AABBCCDDEEFF"),
                ObjectViewState.Archived => Guid.Parse("99887766-5544-3322-11AA-BBCCDDEEFF00"),
                _ => Guid.Empty
            };
        }

        /// <summary>
        /// Returns the resource key label associated with the specified state.
        /// </summary>
        public static string Text(this ObjectViewState state)
        {
            return state switch
            {
                ObjectViewState.Active => "kleenestar.core:state.active.label",
                ObjectViewState.Archived => "kleenestar.core:state.archived.label",
                _ => null
            };
        }

        /// <summary>
        /// Returns the CSS color class associated with the specified state.
        /// </summary>
        public static string Color(this ObjectViewState state)
        {
            return state switch
            {
                ObjectViewState.Active => TypeColorSelection.Success.ToClass(),
                ObjectViewState.Archived => TypeColorSelection.Danger.ToClass(),
                _ => TypeColorSelection.Default.ToClass()
            };
        }
    }
}
