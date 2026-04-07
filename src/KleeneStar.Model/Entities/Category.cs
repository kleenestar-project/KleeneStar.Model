using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a category used to organize workspaces.
    /// </summary>
    public class Category : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the workspace.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Returns the name of the workspace category.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Returns the description of the category.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the collection of workspaces associated with the current category.
        /// </summary>
        [JsonIgnore]
        public List<Workspace> Workspaces { get; set; } = [];

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Category()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the 
        /// specified unique identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier to assign to the category.
        /// </param>
        public Category(Guid id)
        {
            Id = id;
        }

        /// <summary>
        /// Returns the name of the object as a string.
        /// </summary>
        /// <returns>
        /// A string that represents the value of the Name property.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
