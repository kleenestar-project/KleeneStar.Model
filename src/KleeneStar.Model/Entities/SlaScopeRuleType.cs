namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies how an <see cref="SlaScopeRule"/> selects matching tickets.
    /// </summary>
    public enum SlaScopeRuleType
    {
        /// <summary>
        /// Restricts the policy to tickets of a specific priority.
        /// </summary>
        Priority,

        /// <summary>
        /// Restricts the policy to tickets covered by a contract tier.
        /// </summary>
        Contract,

        /// <summary>
        /// Restricts the policy to tickets raised by a specific customer.
        /// </summary>
        Customer,

        /// <summary>
        /// Restricts the policy to tickets created from a service catalog entry.
        /// </summary>
        Catalog,

        /// <summary>
        /// Restricts the policy to tickets carrying a specific tag.
        /// </summary>
        Tag,

        /// <summary>
        /// Restricts the policy to tickets affecting a specific system.
        /// </summary>
        System,

        /// <summary>
        /// Restricts the policy to tickets originating from a specific site.
        /// </summary>
        Site,

        /// <summary>
        /// Restricts the policy to tickets of a specific category.
        /// </summary>
        Category,

        /// <summary>
        /// Restricts the policy to tickets from a specific source (e.g. migration).
        /// </summary>
        Source,

        /// <summary>
        /// Restricts the policy to tickets of a specific ticket type.
        /// </summary>
        Type
    }
}
