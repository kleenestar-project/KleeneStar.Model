using KleeneStar.Model;
using KleeneStar.Model.Entities;
using KleeneStar.Model.Forms;
using Microsoft.EntityFrameworkCore;

namespace Kleenestar.Model.Test.Hub
{
    /// <summary>
    /// Provides unit tests for the ModelHub form structure operations
    /// (<c>GetFormWithStructure</c> and <c>SaveFormStructure</c>).
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestModelHubFormStructure
    {
        private static readonly Guid WorkspaceId = Guid.Parse("3946B811-DFBB-4575-A83B-5C1C0240DF22");
        private static readonly Guid ClassId = Guid.Parse("B54AA5B2-01D5-490A-90A3-4D57FE50320B");

        private static Guid Seed(string connectionString, params Field[] fields)
        {
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig
            {
                ConnectionString = connectionString,
                Assembly = "KleeneStar.Model.Test"
            };

            var formId = Guid.NewGuid();

            using var db = ModelHub.CreateDbContext();

            db.Workspaces.Add(new Workspace
            {
                Id = WorkspaceId,
                Key = "ws-1",
                Name = "workspace"
            });

            db.Classes.Add(new Class
            {
                Id = ClassId,
                Name = "class",
                WorkspaceId = WorkspaceId
            });

            db.Forms.Add(new Form
            {
                Id = formId,
                Name = "Custom",
                FormType = FormType.Default,
                ClassId = ClassId
            });

            foreach (var f in fields)
            {
                db.Fields.Add(f);
            }

            db.SaveChanges();
            return formId;
        }

        [Fact]
        public void SaveAndLoad_Empty()
        {
            // arrange
            var formId = Seed(nameof(SaveAndLoad_Empty));

            // act
            var newVersion = ModelHub.SaveFormStructure(formId, new FormStructureSnapshot
            {
                FormName = "Renamed",
                FormDescription = "Saved via test"
            }, expectedVersion: 0);

            var loaded = ModelHub.GetFormWithStructure(formId);

            // validation
            Assert.Equal(1, newVersion);
            Assert.NotNull(loaded);
            Assert.Equal(1, loaded.Version);
            Assert.Equal("Renamed", loaded.Name);
            Assert.Equal("Saved via test", loaded.Description);
            Assert.Empty(loaded.Tabs);
        }

        [Fact]
        public void SaveAndLoad_Nested()
        {
            // arrange
            var fieldA = new Field { Id = Guid.NewGuid(), Name = "A", FieldType = FieldType.Text, ClassId = ClassId };
            var fieldB = new Field { Id = Guid.NewGuid(), Name = "B", FieldType = FieldType.Number, ClassId = ClassId };
            var formId = Seed(nameof(SaveAndLoad_Nested), fieldA, fieldB);

            var snapshot = new FormStructureSnapshot
            {
                FormName = "Custom",
                Tabs =
                [
                    new TabSnapshot
                    {
                        Name = "General",
                        Children =
                        [
                            new GroupSnapshot
                            {
                                Label = "Outer",
                                Layout = FormGroupLayout.Vertical,
                                Children =
                                [
                                    new GroupSnapshot
                                    {
                                        Label = "Inner",
                                        Layout = FormGroupLayout.Horizontal,
                                        Children =
                                        [
                                            new FieldRefSnapshot { FieldId = fieldA.Id },
                                            new FieldRefSnapshot { FieldId = fieldB.Id }
                                        ]
                                    }
                                ]
                            }
                        ]
                    }
                ]
            };

            // act
            ModelHub.SaveFormStructure(formId, snapshot, expectedVersion: 0);
            var loaded = ModelHub.GetFormWithStructure(formId);

            // validation
            Assert.NotNull(loaded);
            var tab = Assert.Single(loaded.Tabs);
            Assert.Equal("General", tab.Name);

            var outer = Assert.Single(tab.Elements);
            var outerGroup = Assert.IsType<FormGroupElement>(outer);
            Assert.Equal("Outer", outerGroup.Label);
            Assert.Equal(FormGroupLayout.Vertical, outerGroup.Layout);

            var inner = Assert.Single(outerGroup.Children);
            var innerGroup = Assert.IsType<FormGroupElement>(inner);
            Assert.Equal(FormGroupLayout.Horizontal, innerGroup.Layout);

            Assert.Equal(2, innerGroup.Children.Count);
            var first = Assert.IsType<FormFieldRefElement>(innerGroup.Children[0]);
            var second = Assert.IsType<FormFieldRefElement>(innerGroup.Children[1]);
            Assert.Equal(fieldA.Id, first.FieldId);
            Assert.Equal(fieldB.Id, second.FieldId);
            Assert.Equal(0, first.Position);
            Assert.Equal(1, second.Position);
        }

        [Fact]
        public void Save_OverwritesExisting()
        {
            // arrange
            var fieldA = new Field { Id = Guid.NewGuid(), Name = "A", FieldType = FieldType.Text, ClassId = ClassId };
            var formId = Seed(nameof(Save_OverwritesExisting), fieldA);

            ModelHub.SaveFormStructure(formId, new FormStructureSnapshot
            {
                FormName = "v1",
                Tabs = [new TabSnapshot { Name = "T1", Children = [new FieldRefSnapshot { FieldId = fieldA.Id }] }]
            }, expectedVersion: 0);

            // act
            ModelHub.SaveFormStructure(formId, new FormStructureSnapshot
            {
                FormName = "v2",
                Tabs = [new TabSnapshot { Name = "T2", Children = [] }]
            }, expectedVersion: 1);

            var loaded = ModelHub.GetFormWithStructure(formId);

            // validation
            Assert.Equal(2, loaded.Version);
            Assert.Equal("v2", loaded.Name);
            var tab = Assert.Single(loaded.Tabs);
            Assert.Equal("T2", tab.Name);
            Assert.Empty(tab.Elements);
        }

        [Fact]
        public void Save_StaleVersion_Throws()
        {
            // arrange
            var formId = Seed(nameof(Save_StaleVersion_Throws));

            ModelHub.SaveFormStructure(formId, new FormStructureSnapshot { FormName = "v1" }, expectedVersion: 0);

            // act / validation
            Assert.Throws<DbUpdateConcurrencyException>(() =>
                ModelHub.SaveFormStructure(formId, new FormStructureSnapshot { FormName = "v2" }, expectedVersion: 0));
        }

        [Fact]
        public void Save_BumpsVersion()
        {
            // arrange
            var formId = Seed(nameof(Save_BumpsVersion));

            // act
            var v1 = ModelHub.SaveFormStructure(formId, new FormStructureSnapshot { FormName = "x" }, expectedVersion: 0);
            var v2 = ModelHub.SaveFormStructure(formId, new FormStructureSnapshot { FormName = "y" }, expectedVersion: 1);

            // validation
            Assert.Equal(1, v1);
            Assert.Equal(2, v2);
        }

        [Fact]
        public void GetFormWithStructure_ReturnsNullWhenMissing()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig
            {
                ConnectionString = nameof(GetFormWithStructure_ReturnsNullWhenMissing),
                Assembly = "KleeneStar.Model.Test"
            };

            // act
            var loaded = ModelHub.GetFormWithStructure(Guid.NewGuid());

            // validation
            Assert.Null(loaded);
        }
    }
}
