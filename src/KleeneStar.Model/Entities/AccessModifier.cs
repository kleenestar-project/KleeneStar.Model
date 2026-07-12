using System;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the access level of a field, controlling its visibility and usage scope.
    /// </summary>
    public enum AccessModifier
    {
        /// <summary>
        /// Indicates that the field is accessible only within its own class.
        /// </summary>
        Private,

        /// <summary>
        /// Indicates that the field is accessible within its own class and derived classes.
        /// </summary>
        Protected,

        /// <summary>
        /// Indicates that the field is accessible from any context.
        /// </summary>
        Public,

        /// <summary>
        /// Indicates that the field is accessible within the same module or assembly.
        /// </summary>
        Internal
    }

    /// <summary>
    /// Provides extension methods for working with the AccessModifier enumeration, enabling retrieval 
    /// of associated identifiers, text labels, and color selections.
    /// </summary>
    public static class AccessModifierExtensions
    {
        /// <summary>
        /// Returns the unique identifier (GUID) associated with the specified access modifier.
        /// </summary>
        /// <param name="state">
        /// The access modifier for which the corresponding GUID should be retrieved.
        /// </param>
        /// <returns>
        /// The GUID associated with the specified access modifier. Returns <see cref="Guid.Empty"/>
        /// if the value is not recognized.
        /// </returns>
        public static Guid Id(this AccessModifier state)
        {
            return state switch
            {
                AccessModifier.Public => Guid.Parse("D6090833-9E7B-46DB-AB83-1D81634D7E06"),
                AccessModifier.Protected => Guid.Parse("77E333DA-ED29-4455-8A43-BCDB336C62E5"),
                AccessModifier.Private => Guid.Parse("EBFA17B8-11BD-49B8-88C7-A9C431082829"),
                AccessModifier.Internal => Guid.Parse("219B1D3D-961D-46EE-8F14-8CC2AFBC77BA"),
                _ => Guid.Empty
            };
        }

        /// <summary>
        /// Returns the resource key label associated with the specified access modifier.
        /// </summary>
        /// <remarks>The returned string is intended to be used as a resource key for localization
        /// purposes. If the provided access modifier is not one of the defined values, the method returns
        /// null.</remarks>
        /// <param name="state">The access modifier for which to retrieve the resource key label.</param>
        /// <returns>
        /// A string containing the resource key label for the specified access modifier, or null if the 
        /// access modifier is not recognized.
        /// </returns>
        public static string Text(this AccessModifier state)
        {
            return state switch
            {
                AccessModifier.Public => "kleenestar.core:accessmodifier.public.label",
                AccessModifier.Protected => "kleenestar.core:accessmodifier.protected.label",
                AccessModifier.Private => "kleenestar.core:accessmodifier.private.label",
                AccessModifier.Internal => "kleenestar.core:accessmodifier.internal.label",
                _ => null
            };
        }

        /// <summary>
        /// Returns the CSS class name associated with the specified access modifier for styling purposes.
        /// </summary>
        /// <remarks>This method is typically used to map access modifiers to consistent color styles in
        /// UI components. The returned class name can be applied to HTML elements to visually distinguish different
        /// access levels.</remarks>
        /// <param name="state">
        /// The access modifier whose corresponding CSS class name is to be retrieved.
        /// </param>
        /// <returns>
        /// A string containing the CSS class name that represents the specified access modifier.
        /// </returns>
        public static string Color(this AccessModifier state)
        {
            return state switch
            {
                _ => TypeColorSelection.Primary.ToClass()
            };
        }
    }
}
