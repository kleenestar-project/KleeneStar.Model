using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a value entity that stores the data for a single field of an object.
    /// </summary>
    /// <remarks>
    /// A value links an <see cref="Entities.Object"/> to a specific <see cref="Entities.Field"/>
    /// defined on the object's class. There is at most one value per (object, field) pair;
    /// fields with multi-cardinality serialize their list of values into <see cref="Data"/>.
    /// The raw payload is stored as a string and interpreted according to
    /// <see cref="Field.FieldType"/> by the consuming code.
    /// </remarks>
    public class Value : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the value.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the object that owns this value.
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the object this value belongs to.
        /// </summary>
        [JsonIgnore]
        public Object Object { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the field this value is bound to.
        /// </summary>
        public Guid FieldId { get; set; }

        /// <summary>
        /// Gets or sets the field this value is bound to.
        /// </summary>
        [JsonIgnore]
        public Field Field { get; set; }

        /// <summary>
        /// Gets or sets the raw payload of the value. The serialization format depends on
        /// the referenced <see cref="Field.FieldType"/>: scalar fields store the literal
        /// representation, multi-cardinality and structured fields use a JSON-encoded
        /// representation.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was updated.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Value()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the value.</param>
        public Value(Guid id)
        {
            Id = id;
        }
    }
}
