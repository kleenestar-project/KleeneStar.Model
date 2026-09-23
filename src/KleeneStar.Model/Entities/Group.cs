using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using WebExpress.WebCore.WebIdentity;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a global user group (e.g., "Marketing", "Admin", "Engineering").
    /// </summary>
    public class Group : IEntity, IIdentityGroup
    {
        /// <summary>
        /// The id of the group whose members administer the installation - the accounts, and
        /// the one-time links that set their passwords.
        /// </summary>
        /// <remarks>
        /// The permission model administers workspaces and classes; nothing on its resource
        /// chain stands above a workspace, so "who may administer the accounts" has no grant to
        /// be read from. Until it has, the answer is the membership of this one group, which the
        /// seeder creates as <c>Admin</c>. The check fails closed: an installation without the
        /// group has no account administrator.
        /// </remarks>
        public static readonly Guid AdministratorsId = Guid.Parse("7F57823B-8B94-4284-8DA1-39C49E152C8C");

        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the group.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the optional description of the group.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the group state (active, disabled, etc.).
        /// </summary>
        public GroupState State { get; set; }

        /// <summary>
        /// Navigation property for persisted policy assignments.
        /// </summary>
        [JsonIgnore]
        public List<GroupPolicy> GroupPolicies { get; set; } = [];

        /// <summary>
        /// Navigation property for identity memberships (m:n).
        /// </summary>
        [JsonIgnore]
        public List<IdentityGroupMembership> GroupMemberships { get; set; } = [];

        /// <summary>
        /// Gets the collection of policies associated with the identity group.
        /// </summary>
        /// <remarks>
        /// The administrators group (<see cref="AdministratorsId"/>) always holds WebExpress's
        /// <c>SystemAccessPolicy</c>, whatever is stored: it is the installation's administration,
        /// and the framework shows its own system pages (plugins, sitemap, monitor, ...) only to
        /// an identity whose groups carry that policy. Stored assignments come on top.
        /// </remarks>
        IEnumerable<IIdentityPolicy> IIdentityGroup.Policies => GroupPolicies
            .Select(x => x.Policy)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(ResolvePolicyType)
            .Where(x => x is not null)
            .Select(x => Activator.CreateInstance(x!) as IIdentityPolicy)
            .Where(x => x is not null)
            .Concat(Id == AdministratorsId && State == GroupState.Active
                ? [new WebExpress.WebCore.WebPolicies.SystemAccessPolicy()]
                : [])
            .GroupBy(x => x.GetType())
            .Select(x => x.First())!;

        private static readonly Lazy<Dictionary<string, Type>> PolicyTypes = new(() =>
        {
            var result = new Dictionary<string, Type>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in GetLoadableTypes(assembly))
                {
                    if (type is null ||
                        !typeof(IIdentityPolicy).IsAssignableFrom(type) ||
                        !type.IsClass ||
                        type.IsAbstract ||
                        type.FullName is null)
                    {
                        continue;
                    }

                    result[type.FullName.ToLowerInvariant()] = type;
                }
            }

            return result;
        });

        private static Type ResolvePolicyType(string policy)
        {
            var normalizedPolicy = policy.ToLowerInvariant();

            return PolicyTypes.Value.TryGetValue(normalizedPolicy, out var type) ? type : null;
        }

        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(x => x is not null)!;
            }
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Group()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the
        /// specified unique identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier to assign to the group.
        /// </param>
        public Group(Guid id)
        {
            Id = id;
        }
    }
}
