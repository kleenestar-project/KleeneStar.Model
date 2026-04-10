using KleeneStar.Model;
using KleeneStar.Model.Entities;
using WebExpress.WebIndex.Queries;

namespace Kleenestar.Model.Test.Hub
{
    /// <summary>
    /// Provides unit tests for the ModelHub form.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestModelHubForm
    {
        /// <summary>
        /// Verifies that all forms can be retrieved from the database and that the
        /// expected number of forms is returned.
        /// </summary>
        [Fact]
        public void AllForms()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "AllForms",
                Assembly = "KleeneStar.Model.Test"
            };

            using (var db = ModelHub.CreateDbContext())
            {
                db.Workspaces.Add(new Workspace
                {
                    Id = Guid.Parse("3946B811-DFBB-4575-A83B-5C1C0240DF22"),
                    Key = "ws-1",
                    Name = "workspace"
                });
                db.SaveChanges();
            }

            using (var db = ModelHub.CreateDbContext())
            {
                db.Classes.Add(new Class
                {
                    Id = Guid.Parse("B54AA5B2-01D5-490A-90A3-4D57FE50320B"),
                    Name = "class",
                    Key = "CLASS",
                    WorkspaceId = Guid.Parse("3946B811-DFBB-4575-A83B-5C1C0240DF22")
                });
                db.SaveChanges();
            }

            using (var db = ModelHub.CreateDbContext())
            {
                db.Forms.Add(new Form
                {
                    Id = Guid.NewGuid(),
                    Name = "Alpha",
                    ClassId = Guid.Parse("B54AA5B2-01D5-490A-90A3-4D57FE50320B")
                });
                db.Forms.Add(new Form
                {
                    Id = Guid.NewGuid(),
                    Name = "Beta",
                    ClassId = Guid.Parse("B54AA5B2-01D5-490A-90A3-4D57FE50320B")
                });
                db.SaveChanges();
            }

            // act
            var result = ModelHub.GetForms(new Query<Form>()).ToList();

            // validation
            Assert.Equal(2, result.Count);
        }

        /// <summary>
        /// Verifies that the form filtering functionality returns only forms matching the specified
        /// predicate.
        /// </summary>
        [Fact]
        public void FilteredForms()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "FilteredForms",
                Assembly = "KleeneStar.Model.Test"
            };

            using (var db = ModelHub.CreateDbContext())
            {
                db.Workspaces.Add(new Workspace
                {
                    Id = Guid.Parse("3946B811-DFBB-4575-A83B-5C1C0240DF22"),
                    Key = "ws-1",
                    Name = "workspace"
                });
                db.SaveChanges();
            }

            using (var db = ModelHub.CreateDbContext())
            {
                db.Classes.Add(new Class
                {
                    Id = Guid.Parse("B54AA5B2-01D5-490A-90A3-4D57FE50320B"),
                    Name = "class",
                    Key = "CLASS",
                    WorkspaceId = Guid.Parse("3946B811-DFBB-4575-A83B-5C1C0240DF22")
                });
                db.SaveChanges();
            }

            using (var db = ModelHub.CreateDbContext())
            {
                db.Forms.Add(new Form
                {
                    Id = Guid.NewGuid(),
                    Name = "Alpha",
                    ClassId = Guid.Parse("B54AA5B2-01D5-490A-90A3-4D57FE50320B")
                });
                db.Forms.Add(new Form
                {
                    Id = Guid.NewGuid(),
                    Name = "Beta",
                    ClassId = Guid.Parse("B54AA5B2-01D5-490A-90A3-4D57FE50320B")
                });
                db.SaveChanges();
            }

            // act
            var result = ModelHub.GetForms(new Query<Form>().Where(x => x.Name.StartsWith("A")))
                .ToList();

            // validation
            Assert.Single(result);
            Assert.Equal("Alpha", result[0].Name);
        }

        /// <summary>
        /// Verifies that a form is added only if it does not already exist in the database.
        /// </summary>
        [Fact]
        public void AddFormWhenNotExists()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "AddFormWhenNotExists",
                Assembly = "KleeneStar.Model.Test"
            };

            var form = new Form { Id = Guid.NewGuid(), Name = "A" };

            // act
            ModelHub.Add(form);

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Single(db.Forms);
        }

        /// <summary>
        /// Verifies that adding a form with an identifier that already exists in the
        /// database does not result in duplicate entries.
        /// </summary>
        [Fact]
        public void AddFormWhenIdExists()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "AddFormWhenIdExists",
                Assembly = "KleeneStar.Model.Test"
            };

            var id = Guid.NewGuid();
            var form1 = new Form { Id = id, Name = "Alpha" };
            var form2 = new Form { Id = id, Name = "Beta" }; // same ID

            // act
            ModelHub.Add(form1);
            ModelHub.Add(form2);

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Single(db.Forms);
        }

        /// <summary>
        /// Removes an existing form from the database and verifies that it has been deleted.
        /// </summary>
        [Fact]
        public void RemoveExistingForm()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "RemoveExistingForm",
                Assembly = "KleeneStar.Model.Test"
            };

            var id = Guid.NewGuid();

            using var db = ModelHub.CreateDbContext();
            var form = new Form { Id = id, Name = "A", FormType = FormType.Additional };
            db.Forms.Add(form);
            db.SaveChanges();

            // act
            ModelHub.Remove(form);

            // validation
            using var db2 = ModelHub.CreateDbContext();
            Assert.Empty(db2.Forms);
        }

        /// <summary>
        /// Verifies that removing a form that does not exist in the database does not
        /// result in an error and leaves the form collection empty.
        /// </summary>
        [Fact]
        public void RemoveWhenFormNotExists()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "RemoveWhenFormNotExists",
                Assembly = "KleeneStar.Model.Test"
            };

            var id = Guid.NewGuid();

            // act
            ModelHub.Remove(new Form { RawId = 1, Id = id, FormType = FormType.Additional });

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Empty(db.Forms);
        }

        /// <summary>
        /// Updates an existing form in the database and verifies that the changes are persisted.
        /// </summary>
        [Fact]
        public void UpdateExistingForm()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "UpdateExistingForm",
                Assembly = "KleeneStar.Model.Test"
            };

            using var db = ModelHub.CreateDbContext();
            var form = new Form { Id = Guid.NewGuid(), Name = "Original" };
            db.Forms.Add(form);
            db.SaveChanges();

            // act
            form.Name = "Updated";
            ModelHub.Update(form);

            // validation
            using var db2 = ModelHub.CreateDbContext();
            Assert.Equal("Updated", db2.Forms.Single().Name);
        }

        /// <summary>
        /// Verifies that attempting to remove a standard form throws an InvalidOperationException.
        /// </summary>
        [Fact]
        public void RemoveStandardFormThrows()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "RemoveStandardFormThrows",
                Assembly = "KleeneStar.Model.Test"
            };

            var id = Guid.NewGuid();

            using var db = ModelHub.CreateDbContext();
            var form = new Form { Id = id, Name = "Standard", FormType = FormType.Standard };
            db.Forms.Add(form);
            db.SaveChanges();

            // act & validation
            Assert.Throws<InvalidOperationException>(() => ModelHub.Remove(form));

            // verify the form still exists
            using var db2 = ModelHub.CreateDbContext();
            Assert.Single(db2.Forms);
        }

        /// <summary>
        /// Verifies that an additional form can be removed successfully.
        /// </summary>
        [Fact]
        public void RemoveAdditionalForm()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "RemoveAdditionalForm",
                Assembly = "KleeneStar.Model.Test"
            };

            var id = Guid.NewGuid();

            using var db = ModelHub.CreateDbContext();
            var form = new Form { Id = id, Name = "Custom", FormType = FormType.Additional };
            db.Forms.Add(form);
            db.SaveChanges();

            // act
            ModelHub.Remove(form);

            // validation
            using var db2 = ModelHub.CreateDbContext();
            Assert.Empty(db2.Forms);
        }
    }
}
