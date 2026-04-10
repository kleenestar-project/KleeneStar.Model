using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KleeneStar.Model.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dashboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dashboard", x => x.Id);
                    table.UniqueConstraint("AK_Dashboard_Guid", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "StatusCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Color = table.Column<string>(type: "TEXT", maxLength: 9, nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusCategory", x => x.Id);
                    table.UniqueConstraint("AK_StatusCategory_Guid", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "Workspace",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Key = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workspace", x => x.Id);
                    table.UniqueConstraint("AK_Workspace_Guid", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "DashboardCategory",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    DashboardId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardCategory", x => new { x.CategoryId, x.DashboardId });
                    table.ForeignKey(
                        name: "FK_DashboardCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DashboardCategory_Dashboard_DashboardId",
                        column: x => x.DashboardId,
                        principalTable: "Dashboard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DashboardColumn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Size = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    Dashboard = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardColumn", x => x.Id);
                    table.UniqueConstraint("AK_DashboardColumn_Guid", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_DashboardColumn_Dashboard_Dashboard",
                        column: x => x.Dashboard,
                        principalTable: "Dashboard",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Class",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsAbstract = table.Column<bool>(type: "INTEGER", nullable: false),
                    Inherited = table.Column<Guid>(type: "TEXT", nullable: true),
                    Sealed = table.Column<bool>(type: "INTEGER", nullable: false),
                    Parent = table.Column<Guid>(type: "TEXT", nullable: true),
                    AccessModifier = table.Column<int>(type: "INTEGER", nullable: false),
                    Workspace = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Class", x => x.Id);
                    table.UniqueConstraint("AK_Class_Guid", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Class_Class_Inherited",
                        column: x => x.Inherited,
                        principalTable: "Class",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Class_Class_Parent",
                        column: x => x.Parent,
                        principalTable: "Class",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Class_Workspace_Workspace",
                        column: x => x.Workspace,
                        principalTable: "Workspace",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkspaceCategory",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    WorkspaceId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceCategory", x => new { x.CategoryId, x.WorkspaceId });
                    table.ForeignKey(
                        name: "FK_WorkspaceCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkspaceCategory_Workspace_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "Workspace",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Widget",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Wql = table.Column<string>(type: "TEXT", nullable: true),
                    Column = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Widget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Widget_DashboardColumn_Column",
                        column: x => x.Column,
                        principalTable: "DashboardColumn",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClassAllowedChild",
                columns: table => new
                {
                    ChildId = table.Column<int>(type: "INTEGER", nullable: false),
                    ClassId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassAllowedChild", x => new { x.ChildId, x.ClassId });
                    table.ForeignKey(
                        name: "FK_ClassAllowedChild_Class_ChildId",
                        column: x => x.ChildId,
                        principalTable: "Class",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassAllowedChild_Class_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Class",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Field",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    HelpText = table.Column<string>(type: "TEXT", nullable: true),
                    Placeholder = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Class = table.Column<Guid>(type: "TEXT", nullable: false),
                    FieldType = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    Cardinality = table.Column<int>(type: "INTEGER", nullable: false),
                    ValidationRules = table.Column<string>(type: "TEXT", nullable: true),
                    DefaultSpec = table.Column<string>(type: "TEXT", nullable: true),
                    Required = table.Column<bool>(type: "INTEGER", nullable: false),
                    Unique = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deprecated = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessModifier = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Field", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Field_Class_Class",
                        column: x => x.Class,
                        principalTable: "Class",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Form",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    FormType = table.Column<int>(type: "INTEGER", nullable: false),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Class = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Form", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Form_Class_Class",
                        column: x => x.Class,
                        principalTable: "Class",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Object",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Key = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Summary = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Workspace = table.Column<Guid>(type: "TEXT", nullable: false),
                    Class = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Object", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Object_Class_Class",
                        column: x => x.Class,
                        principalTable: "Class",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Object_Workspace_Workspace",
                        column: x => x.Workspace,
                        principalTable: "Workspace",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Priority",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Class = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Priority", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Priority_Class_Class",
                        column: x => x.Class,
                        principalTable: "Class",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Workflow",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Class = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workflow", x => x.Id);
                    table.UniqueConstraint("AK_Workflow_Guid", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Workflow_Class_Class",
                        column: x => x.Class,
                        principalTable: "Class",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Category = table.Column<Guid>(type: "TEXT", nullable: false),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Class = table.Column<Guid>(type: "TEXT", nullable: false),
                    WorkflowRawId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.Id);
                    table.UniqueConstraint("AK_Status_Guid", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Status_Class_Class",
                        column: x => x.Class,
                        principalTable: "Class",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Status_StatusCategory_Category",
                        column: x => x.Category,
                        principalTable: "StatusCategory",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Status_Workflow_WorkflowRawId",
                        column: x => x.WorkflowRawId,
                        principalTable: "Workflow",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Transition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Workflow = table.Column<Guid>(type: "TEXT", nullable: false),
                    SourceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TargetId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transition_Status_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Status",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transition_Status_TargetId",
                        column: x => x.TargetId,
                        principalTable: "Status",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transition_Workflow_Workflow",
                        column: x => x.Workflow,
                        principalTable: "Workflow",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Category_Name",
                table: "Category",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Class_Inherited",
                table: "Class",
                column: "Inherited");

            migrationBuilder.CreateIndex(
                name: "IX_Class_Parent",
                table: "Class",
                column: "Parent");

            migrationBuilder.CreateIndex(
                name: "IX_Class_Workspace_Name",
                table: "Class",
                columns: new[] { "Workspace", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassAllowedChild_ClassId",
                table: "ClassAllowedChild",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Dashboard_Name",
                table: "Dashboard",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DashboardCategory_DashboardId",
                table: "DashboardCategory",
                column: "DashboardId");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardColumn_Dashboard_Name",
                table: "DashboardColumn",
                columns: new[] { "Dashboard", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Field_Class_Name",
                table: "Field",
                columns: new[] { "Class", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Form_Class_Name",
                table: "Form",
                columns: new[] { "Class", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Object_Class",
                table: "Object",
                column: "Class");

            migrationBuilder.CreateIndex(
                name: "IX_Object_Key",
                table: "Object",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Object_Workspace",
                table: "Object",
                column: "Workspace");

            migrationBuilder.CreateIndex(
                name: "IX_Priority_Class_Name",
                table: "Priority",
                columns: new[] { "Class", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Status_Category_Name_Class",
                table: "Status",
                columns: new[] { "Category", "Name", "Class" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Status_Class_Name",
                table: "Status",
                columns: new[] { "Class", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Status_WorkflowRawId",
                table: "Status",
                column: "WorkflowRawId");

            migrationBuilder.CreateIndex(
                name: "IX_Transition_SourceId",
                table: "Transition",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Transition_TargetId",
                table: "Transition",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_Transition_Workflow_Name",
                table: "Transition",
                columns: new[] { "Workflow", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Widget_Column_Name",
                table: "Widget",
                columns: new[] { "Column", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Workflow_Class_Name",
                table: "Workflow",
                columns: new[] { "Class", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Workspace_Key",
                table: "Workspace",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Workspace_Name",
                table: "Workspace",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceCategory_WorkspaceId",
                table: "WorkspaceCategory",
                column: "WorkspaceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassAllowedChild");

            migrationBuilder.DropTable(
                name: "DashboardCategory");

            migrationBuilder.DropTable(
                name: "Field");

            migrationBuilder.DropTable(
                name: "Form");

            migrationBuilder.DropTable(
                name: "Object");

            migrationBuilder.DropTable(
                name: "Priority");

            migrationBuilder.DropTable(
                name: "Transition");

            migrationBuilder.DropTable(
                name: "Widget");

            migrationBuilder.DropTable(
                name: "WorkspaceCategory");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropTable(
                name: "DashboardColumn");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "StatusCategory");

            migrationBuilder.DropTable(
                name: "Workflow");

            migrationBuilder.DropTable(
                name: "Dashboard");

            migrationBuilder.DropTable(
                name: "Class");

            migrationBuilder.DropTable(
                name: "Workspace");
        }
    }
}
