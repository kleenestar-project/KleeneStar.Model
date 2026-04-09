using KleeneStar.Model;
using KleeneStar.Model.Entities;
using KleeneStar.Model.Test;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the Field class.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestField
    {
        /// <summary>
        /// Verifies that a new Field instance is assigned a non-empty unique identifier upon initialization.
        /// </summary>
        [Fact]
        public void InitializeId()
        {
            // act
            var field = new Field();

            // validation
            Assert.NotEqual(Guid.Empty, field.Id);
        }

        /// <summary>
        /// Verifies that two Field instances receive distinct unique identifiers.
        /// </summary>
        [Fact]
        public void InitializeUniqueIds()
        {
            // act
            var field1 = new Field();
            var field2 = new Field();

            // validation
            Assert.NotEqual(field1.Id, field2.Id);
        }

        /// <summary>
        /// Verifies that a Field created with a specific Guid retains that identifier.
        /// </summary>
        [Fact]
        public void InitializeWithSpecificGuid()
        {
            // arrange
            var id = Guid.NewGuid();

            // act
            var field = new Field(id);

            // validation
            Assert.Equal(id, field.Id);
        }

        /// <summary>
        /// Verifies that default values are correctly set for a new Field instance.
        /// </summary>
        [Fact]
        public void DefaultValues()
        {
            // act
            var field = new Field();

            // validation
            Assert.Null(field.Name);
            Assert.Null(field.Description);
            Assert.Null(field.HelpText);
            Assert.Null(field.Placeholder);
            Assert.Null(field.FieldType);
            Assert.Null(field.DefaultSpec);
            Assert.Equal(FieldState.Active, field.State);
            Assert.Equal(FieldCardinality.Single, field.Cardinality);
            Assert.Equal(AccessModifier.Private, field.AccessModifier);
            Assert.False(field.Required);
            Assert.False(field.Unique);
            Assert.False(field.Deprecated);
            Assert.NotNull(field.ValidationRules);
            Assert.Empty(field.ValidationRules);
        }

        /// <summary>
        /// Sets the properties of a Field instance and verifies that the values are assigned correctly.
        /// </summary>
        [Theory]
        [InlineData("Field A", "Description A", FieldState.Active)]
        [InlineData("Field B", null, FieldState.Archived)]
        public void SetProperties(string name, string description, FieldState state)
        {
            // arrange
            var field = new Field();

            // act
            field.Name = name;
            field.Description = description;
            field.State = state;

            // validation
            Assert.Equal(name, field.Name);
            Assert.Equal(description, field.Description);
            Assert.Equal(state, field.State);
        }

        /// <summary>
        /// Sets the metadata properties (HelpText, Placeholder) and verifies assignment.
        /// </summary>
        [Fact]
        public void SetMetadataProperties()
        {
            // arrange
            var field = new Field();

            // act
            field.HelpText = "Enter a value";
            field.Placeholder = "e.g. 42";

            // validation
            Assert.Equal("Enter a value", field.HelpText);
            Assert.Equal("e.g. 42", field.Placeholder);
        }

        /// <summary>
        /// Verifies that FieldState enum values are correctly assigned and compared.
        /// </summary>
        [Theory]
        [InlineData(FieldState.Active)]
        [InlineData(FieldState.Archived)]
        public void FieldStateEnum(FieldState state)
        {
            // arrange
            var field = new Field();

            // act
            field.State = state;

            // validation
            Assert.Equal(state, field.State);
        }

        /// <summary>
        /// Verifies that FieldCardinality enum values are correctly assigned.
        /// </summary>
        [Theory]
        [InlineData(FieldCardinality.Single)]
        [InlineData(FieldCardinality.Multiple)]
        public void FieldCardinalityEnum(FieldCardinality cardinality)
        {
            // arrange
            var field = new Field();

            // act
            field.Cardinality = cardinality;

            // validation
            Assert.Equal(cardinality, field.Cardinality);
        }

        /// <summary>
        /// Verifies that AccessModifier enum values are correctly assigned.
        /// </summary>
        [Theory]
        [InlineData(AccessModifier.Private)]
        [InlineData(AccessModifier.Protected)]
        [InlineData(AccessModifier.Public)]
        [InlineData(AccessModifier.Internal)]
        public void AccessModifierEnum(AccessModifier modifier)
        {
            // arrange
            var field = new Field();

            // act
            field.AccessModifier = modifier;

            // validation
            Assert.Equal(modifier, field.AccessModifier);
        }

        /// <summary>
        /// Verifies that the FieldType string property can be set and retrieved.
        /// </summary>
        [Fact]
        public void SetFieldType()
        {
            // arrange
            var field = new Field();

            // act
            field.FieldType = "Number";

            // validation
            Assert.Equal("Number", field.FieldType);
        }

        /// <summary>
        /// Verifies that the boolean flags (Required, Unique, Deprecated) can be set.
        /// </summary>
        [Theory]
        [InlineData(true, false, false)]
        [InlineData(false, true, false)]
        [InlineData(false, false, true)]
        [InlineData(true, true, true)]
        public void BooleanFlags(bool required, bool unique, bool deprecated)
        {
            // arrange
            var field = new Field();

            // act
            field.Required = required;
            field.Unique = unique;
            field.Deprecated = deprecated;

            // validation
            Assert.Equal(required, field.Required);
            Assert.Equal(unique, field.Unique);
            Assert.Equal(deprecated, field.Deprecated);
        }

        /// <summary>
        /// Verifies that validation rules can be added and retrieved.
        /// </summary>
        [Fact]
        public void ValidationRulesCollection()
        {
            // arrange
            var field = new Field();

            // act
            field.ValidationRules.Add("required");
            field.ValidationRules.Add("max-length:100");

            // validation
            Assert.Equal(2, field.ValidationRules.Count);
            Assert.Contains("required", field.ValidationRules);
            Assert.Contains("max-length:100", field.ValidationRules);
        }

        /// <summary>
        /// Verifies that the DefaultSpec can be set and cleared.
        /// </summary>
        [Fact]
        public void SetDefaultSpec()
        {
            // arrange
            var field = new Field();

            // act
            field.DefaultSpec = "default-value";

            // validation
            Assert.Equal("default-value", field.DefaultSpec);

            // act - clear
            field.DefaultSpec = null;

            // validation
            Assert.Null(field.DefaultSpec);
        }

        /// <summary>
        /// Verifies that the ClassId relationship property can be set.
        /// </summary>
        [Fact]
        public void SetClassRelationship()
        {
            // arrange
            var field = new Field();
            var classId = Guid.NewGuid();

            // act
            field.ClassId = classId;

            // validation
            Assert.Equal(classId, field.ClassId);
        }

        /// <summary>
        /// Verifies that lifecycle timestamps (Created, Updated) can be set.
        /// </summary>
        [Fact]
        public void SetLifecycleTimestamps()
        {
            // arrange
            var field = new Field();
            var now = DateTime.UtcNow;

            // act
            field.Created = now;
            field.Updated = now;

            // validation
            Assert.Equal(now, field.Created);
            Assert.Equal(now, field.Updated);
        }

        /// <summary>
        /// Verifies that a Field can be persisted and retrieved through the DbContext
        /// with all new properties preserved.
        /// </summary>
        [Fact]
        public void PersistFieldWithNewProperties()
        {
            // arrange
            using var db = InMemoryDbContextFactory.Create("PersistFieldWithNewProperties");
            var now = DateTime.UtcNow;

            var field = new Field
            {
                Id = Guid.NewGuid(),
                Name = "TestField",
                Description = "A test field",
                HelpText = "Help for test field",
                Placeholder = "Enter value",
                State = FieldState.Active,
                FieldType = "Text",
                Cardinality = FieldCardinality.Multiple,
                ValidationRules = new System.Collections.Generic.List<string> { "required", "min:1" },
                DefaultSpec = "default-val",
                Required = true,
                Unique = false,
                Deprecated = false,
                AccessModifier = AccessModifier.Public,
                Created = now,
                Updated = now
            };

            // act
            db.Fields.Add(field);
            db.SaveChanges();

            // validation
            var retrieved = db.Fields.Single();
            Assert.Equal("TestField", retrieved.Name);
            Assert.Equal("A test field", retrieved.Description);
            Assert.Equal("Help for test field", retrieved.HelpText);
            Assert.Equal("Enter value", retrieved.Placeholder);
            Assert.Equal(FieldState.Active, retrieved.State);
            Assert.Equal("Text", retrieved.FieldType);
            Assert.Equal(FieldCardinality.Multiple, retrieved.Cardinality);
            Assert.Equal("default-val", retrieved.DefaultSpec);
            Assert.True(retrieved.Required);
            Assert.False(retrieved.Unique);
            Assert.False(retrieved.Deprecated);
            Assert.Equal(AccessModifier.Public, retrieved.AccessModifier);
        }

        /// <summary>
        /// Verifies that a Field with null optional properties persists correctly.
        /// </summary>
        [Fact]
        public void PersistFieldWithNullOptionalProperties()
        {
            // arrange
            using var db = InMemoryDbContextFactory.Create("PersistFieldNullOptional");

            var field = new Field
            {
                Id = Guid.NewGuid(),
                Name = "MinimalField",
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };

            // act
            db.Fields.Add(field);
            db.SaveChanges();

            // validation
            var retrieved = db.Fields.Single();
            Assert.Equal("MinimalField", retrieved.Name);
            Assert.Null(retrieved.Description);
            Assert.Null(retrieved.HelpText);
            Assert.Null(retrieved.Placeholder);
            Assert.Null(retrieved.FieldType);
            Assert.Null(retrieved.DefaultSpec);
            Assert.False(retrieved.Required);
            Assert.False(retrieved.Unique);
            Assert.False(retrieved.Deprecated);
        }

        /// <summary>
        /// Verifies that enum values are correctly persisted through the DbContext.
        /// </summary>
        [Fact]
        public void PersistEnumValues()
        {
            // arrange
            using var db = InMemoryDbContextFactory.Create("PersistEnumValues");

            var field = new Field
            {
                Id = Guid.NewGuid(),
                Name = "EnumField",
                State = FieldState.Archived,
                Cardinality = FieldCardinality.Multiple,
                AccessModifier = AccessModifier.Internal,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };

            // act
            db.Fields.Add(field);
            db.SaveChanges();

            // validation
            var retrieved = db.Fields.Single();
            Assert.Equal(FieldState.Archived, retrieved.State);
            Assert.Equal(FieldCardinality.Multiple, retrieved.Cardinality);
            Assert.Equal(AccessModifier.Internal, retrieved.AccessModifier);
        }
    }
}
