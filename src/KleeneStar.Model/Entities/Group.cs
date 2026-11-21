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
        /// The id of the built-in group every signed-in, active account belongs to.
        /// </summary>
        /// <remarks>
        /// Its membership is implicit - no row names it, and the permission evaluation adds it
        /// to the groups of every caller it can resolve to an account. It is what a workspace
        /// template grants its everyday access to, because "everybody who signed in" has no
        /// other group to be named by, and a grant to it deliberately leaves out a caller who
        /// is not signed in. See <see cref="IsImplicit"/>.
        /// </remarks>
        public static readonly Guid AuthenticatedId = Guid.Parse("0A741FBC-F2BB-4E40-975A-B00B215EE1DF");

        /// <summary>
        /// The id of the built-in group every caller belongs to, signed in or not.
        /// </summary>
        /// <remarks>
        /// A grant to it opens a resource to anonymous visitors, which is why nothing grants to it
        /// by default: an administrator who wants a public workspace says so explicitly. Its
        /// membership is implicit like <see cref="AuthenticatedId"/>'s.
        /// </remarks>
        public static readonly Guid AnonymousId = Guid.Parse("7D45AE88-F1C1-40C1-825A-57FA8712A952");

        /// <summary>
        /// Determines whether a group's membership is implicit - decided by whether and how the
        /// caller signed in, not by rows - so it can neither be joined, left nor deleted.
        /// </summary>
        /// <param name="groupId">The id of the group.</param>
        /// <returns><see langword="true"/> for the built-in signed-in and anonymous groups.</returns>
        public static bool IsImplicit(Guid groupId)
        {
            return groupId == AuthenticatedId || groupId == AnonymousId;
        }

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
