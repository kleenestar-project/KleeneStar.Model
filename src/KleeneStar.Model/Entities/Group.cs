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
        IEnumerable<IIdentityPolicy> IIdentityGroup.Policies => GroupPolicies
            .Select(x => x.Policy)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(ResolvePolicyType)
            .Where(x => x is not null)
            .Select(x => Activator.CreateInstance(x!) as IIdentityPolicy)
            .Where(x => x is not null)!;

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
