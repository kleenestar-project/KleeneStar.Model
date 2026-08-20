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
                name: "Branding",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branding", x => x.Id);
                });

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
                name: "Commit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Object = table.Column<Guid>(type: "TEXT", nullable: false),
                    ObjectKey = table.Column<string>(type: "TEXT", nullable: true),
                    Parent = table.Column<Guid>(type: "TEXT", nullable: true),
                    Number = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedByName = table.Column<string>(type: "TEXT", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commit", x => x.Id);
                    table.UniqueConstraint("AK_Commit_Guid", x => x.Guid);
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
                name: "Group",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group", x => x.Id);
                    table.UniqueConstraint("AK_Group_Guid", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "KanbanBoard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Workspace = table.Column<Guid>(type: "TEXT", nullable: false),
                    Kind = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Filter = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KanbanBoard", x => x.Id);
                    table.UniqueConstraint("AK_KanbanBoard_Guid", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "KindDashboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Workspace = table.Column<Guid>(type: "TEXT", nullable: false),
                    Kind = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KindDashboard", x => x.Id);
                    table.UniqueConstraint("AK_KindDashboard_Guid", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "Maintenance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maintenance", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NavigatorLink",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Uri = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: false),
                    Ordinal = table.Column<int>(type: "INTEGER", nullable: false),
                    State = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NavigatorLink", x => x.Id);
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
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusCategory", x => x.Id);
                    table.UniqueConstraint("AK_StatusCategory_Guid", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "Tenant",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenant", x => x.Id);
                    table.UniqueConstraint("AK_Tenant_Guid", x => x.Guid);
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
                    Inherited = table.Column<Guid>(type: "TEXT", nullable: true),
                    Sealed = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessModifier = table.Column<int>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workspace", x => x.Id);
                    table.UniqueConstraint("AK_Workspace_Guid", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Workspace_Workspace_Inherited",
                        column: x => x.Inherited,
                        principalTable: "Workspace",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Change",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Commit = table.Column<Guid>(type: "TEXT", nullable: false),
                    Field = table.Column<Guid>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    OldValue = table.Column<string>(type: "TEXT", nullable: true),
                    NewValue = table.Column<string>(type: "TEXT", nullable: true),
                    Ordinal = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Change", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Change_Commit_Commit",
                        column: x => x.Commit,
                        principalTable: "Commit",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
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
                    Color = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    Position = table.Column<int>(type: "INTEGER", nullable: false),
                    Key = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
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
                name: "GroupPolicy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    GroupId = table.Column<int>(type: "INTEGER", nullable: false),
                    Policy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupPolicy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupPolicy_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PermissionAssignment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Group = table.Column<Guid>(type: "TEXT", nullable: false),
                    Policy = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Scope = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    ScopeId = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionAssignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PermissionAssignment_Group_Group",
                        column: x => x.Group,
                        principalTable: "Group",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KanbanBoardColumn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Color = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    Position = table.Column<int>(type: "INTEGER", nullable: false),
                    Category = table.Column<Guid>(type: "TEXT", nullable: true),
                    Key = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    Board = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KanbanBoardColumn", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KanbanBoardColumn_KanbanBoard_Board",
                        column: x => x.Board,
                        principalTable: "KanbanBoard",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KanbanBoardSwimlane",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Color = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    Filter = table.Column<string>(type: "TEXT", nullable: true),
                    Position = table.Column<int>(type: "INTEGER", nullable: false),
                    Class = table.Column<Guid>(type: "TEXT", nullable: true),
                    Key = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    Board = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KanbanBoardSwimlane", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KanbanBoardSwimlane_KanbanBoard_Board",
                        column: x => x.Board,
                        principalTable: "KanbanBoard",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KindDashboardColumn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Size = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    Color = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    Position = table.Column<int>(type: "INTEGER", nullable: false),
                    Key = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    Board = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KindDashboardColumn", x => x.Id);
                    table.UniqueConstraint("AK_KindDashboardColumn_Guid", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_KindDashboardColumn_KindDashboard_Board",
                        column: x => x.Board,
                        principalTable: "KindDashboard",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    EmailVerified = table.Column<bool>(type: "INTEGER", nullable: false),
                    Bio = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true),
                    PhoneCountry = table.Column<string>(type: "TEXT", maxLength: 8, nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    Website = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Location = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    Position = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    Language = table.Column<string>(type: "TEXT", maxLength: 16, nullable: true),
                    TimeZone = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    DateFormat = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    WeekStart = table.Column<int>(type: "INTEGER", nullable: false),
                    Role = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    RoleSince = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Department = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    CostCenter = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    PersonnelNumber = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    Deputy = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: true),
                    Tenant = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: true),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identity", x => x.Id);
                    table.UniqueConstraint("AK_Identity_Guid", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Identity_Identity_Deputy",
                        column: x => x.Deputy,
                        principalTable: "Identity",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Identity_Tenant_Tenant",
                        column: x => x.Tenant,
                        principalTable: "Tenant",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
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
                    PortalVisible = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    Kind = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false, defaultValue: "issue"),
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
                name: "ObjectView",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Kind = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false, defaultValue: "issue"),
                    ViewType = table.Column<int>(type: "INTEGER", nullable: false),
                    Configuration = table.Column<string>(type: "TEXT", nullable: true),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    Workspace = table.Column<Guid>(type: "TEXT", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectView", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjectView_Workspace_Workspace",
                        column: x => x.Workspace,
                        principalTable: "Workspace",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sprint",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Goal = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    Start = table.Column<DateTime>(type: "TEXT", nullable: true),
                    End = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Capacity = table.Column<int>(type: "INTEGER", nullable: false),
                    Workspace = table.Column<Guid>(type: "TEXT", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sprint", x => x.Id);
                    table.UniqueConstraint("AK_Sprint_Guid", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Sprint_Workspace_Workspace",
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
                name: "WorkspaceTenant",
                columns: table => new
                {
                    TenantId = table.Column<int>(type: "INTEGER", nullable: false),
                    WorkspaceId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceTenant", x => new { x.TenantId, x.WorkspaceId });
                    table.ForeignKey(
                        name: "FK_WorkspaceTenant_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkspaceTenant_Workspace_WorkspaceId",
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
                    Type = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    Color = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    Params = table.Column<string>(type: "TEXT", nullable: true),
                    Position = table.Column<int>(type: "INTEGER", nullable: false),
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
                name: "KindDashboardWidget",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    Type = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    Color = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    Params = table.Column<string>(type: "TEXT", nullable: true),
                    Position = table.Column<int>(type: "INTEGER", nullable: false),
                    Column = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KindDashboardWidget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KindDashboardWidget_KindDashboardColumn_Column",
                        column: x => x.Column,
                        principalTable: "KindDashboardColumn",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccessToken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Owner = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Prefix = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    TokenHash = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true),
                    Scopes = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUsed = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Expires = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Revoked = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessToken_Identity_Owner",
                        column: x => x.Owner,
                        principalTable: "Identity",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomQuickfilter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    ViewKey = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    ContextKey = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Query = table.Column<string>(type: "TEXT", nullable: false),
                    Owner = table.Column<Guid>(type: "TEXT", nullable: false),
                    Shared = table.Column<bool>(type: "INTEGER", nullable: false),
                    Ordinal = table.Column<int>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomQuickfilter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomQuickfilter_Identity_Owner",
                        column: x => x.Owner,
                        principalTable: "Identity",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IdentityGroupMembership",
                columns: table => new
                {
                    IdentityId = table.Column<int>(type: "INTEGER", nullable: false),
                    GroupId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityGroupMembership", x => new { x.IdentityId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_IdentityGroupMembership_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IdentityGroupMembership_Identity_IdentityId",
                        column: x => x.IdentityId,
                        principalTable: "Identity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentitySession",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Owner = table.Column<Guid>(type: "TEXT", nullable: false),
                    Device = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Client = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    Mobile = table.Column<bool>(type: "INTEGER", nullable: false),
                    Location = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastActive = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Current = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentitySession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentitySession_Identity_Owner",
                        column: x => x.Owner,
                        principalTable: "Identity",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SavedSearch",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Query = table.Column<string>(type: "TEXT", nullable: true),
                    Owner = table.Column<Guid>(type: "TEXT", nullable: false),
                    Starred = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastUsed = table.Column<DateTime>(type: "TEXT", nullable: false),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedSearch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedSearch_Identity_Owner",
                        column: x => x.Owner,
                        principalTable: "Identity",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserNotification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Owner = table.Column<Guid>(type: "TEXT", nullable: false),
                    Actor = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: true),
                    TitleKey = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    MessageKey = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    Subject = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    TargetUri = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true),
                    SubjectIcon = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true),
                    Read = table.Column<bool>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserNotification_Identity_Actor",
                        column: x => x.Actor,
                        principalTable: "Identity",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_UserNotification_Identity_Owner",
                        column: x => x.Owner,
                        principalTable: "Identity",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSession",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Owner = table.Column<Guid>(type: "TEXT", nullable: false),
                    Scope = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Key = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSession_Identity_Owner",
                        column: x => x.Owner,
                        principalTable: "Identity",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkspaceBookmark",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Owner = table.Column<Guid>(type: "TEXT", nullable: false),
                    Workspace = table.Column<Guid>(type: "TEXT", nullable: false),
                    Favorite = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastVisited = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceBookmark", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkspaceBookmark_Identity_Owner",
                        column: x => x.Owner,
                        principalTable: "Identity",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkspaceBookmark_Workspace_Workspace",
                        column: x => x.Workspace,
                        principalTable: "Workspace",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Calendar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    TimeZone = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    Region = table.Column<string>(type: "TEXT", maxLength: 16, nullable: true),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Class = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calendar", x => x.Id);
                    table.UniqueConstraint("AK_Calendar_Guid", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Calendar_Class_Class",
                        column: x => x.Class,
                        principalTable: "Class",
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
                    FieldType = table.Column<int>(type: "INTEGER", maxLength: 128, nullable: false),
                    Cardinality = table.Column<int>(type: "INTEGER", nullable: false),
                    CardinalityMin = table.Column<int>(type: "INTEGER", nullable: false),
                    CardinalityMax = table.Column<int>(type: "INTEGER", nullable: false),
                    CardinalityUnlimited = table.Column<bool>(type: "INTEGER", nullable: false),
                    RegexPattern = table.Column<string>(type: "TEXT", nullable: true),
                    Options = table.Column<string>(type: "TEXT", nullable: true),
                    WorkflowId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DefaultPriorityId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SelectedPriorityIds = table.Column<string>(type: "TEXT", nullable: true),
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
                    table.UniqueConstraint("AK_Field_Guid", x => x.Guid);
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
                    Class = table.Column<Guid>(type: "TEXT", nullable: false),
                    Version = table.Column<int>(type: "INTEGER", nullable: false),
                    PortalTemplate = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Form", x => x.Id);
                    table.UniqueConstraint("AK_Form_Guid", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Form_Class_Class",
                        column: x => x.Class,
                        principalTable: "Class",
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
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
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
                    Class = table.Column<Guid>(type: "TEXT", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "Template",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Category = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    Presets = table.Column<string>(type: "TEXT", nullable: true),
                    Parent = table.Column<Guid>(type: "TEXT", nullable: true),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    Class = table.Column<Guid>(type: "TEXT", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Template", x => x.Id);
                    table.UniqueConstraint("AK_Template_Guid", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Template_Class_Class",
                        column: x => x.Class,
                        principalTable: "Class",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Template_Template_Parent",
                        column: x => x.Parent,
                        principalTable: "Template",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
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
                    Kind = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false, defaultValue: "issue"),
                    Workspace = table.Column<Guid>(type: "TEXT", nullable: false),
                    Class = table.Column<Guid>(type: "TEXT", nullable: false),
                    Parent = table.Column<Guid>(type: "TEXT", nullable: true),
                    Creator = table.Column<Guid>(type: "TEXT", nullable: true),
                    Assignee = table.Column<Guid>(type: "TEXT", nullable: true),
                    Sprint = table.Column<Guid>(type: "TEXT", nullable: true),
                    SprintRank = table.Column<int>(type: "INTEGER", nullable: false),
                    StoryPoints = table.Column<int>(type: "INTEGER", nullable: true),
                    Updater = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Object", x => x.Id);
                    table.UniqueConstraint("AK_Object_Guid", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Object_Class_Class",
                        column: x => x.Class,
                        principalTable: "Class",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Object_Identity_Assignee",
                        column: x => x.Assignee,
                        principalTable: "Identity",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Object_Identity_Creator",
                        column: x => x.Creator,
                        principalTable: "Identity",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Object_Identity_Updater",
                        column: x => x.Updater,
                        principalTable: "Identity",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Object_Object_Parent",
                        column: x => x.Parent,
                        principalTable: "Object",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Object_Sprint_Sprint",
                        column: x => x.Sprint,
                        principalTable: "Sprint",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Object_Workspace_Workspace",
                        column: x => x.Workspace,
                        principalTable: "Workspace",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusinessHourSlot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    DayOfWeek = table.Column<int>(type: "INTEGER", nullable: false),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    Calendar = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessHourSlot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessHourSlot_Calendar_Calendar",
                        column: x => x.Calendar,
                        principalTable: "Calendar",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Holiday",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Region = table.Column<string>(type: "TEXT", maxLength: 16, nullable: true),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    Calendar = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holiday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Holiday_Calendar_Calendar",
                        column: x => x.Calendar,
                        principalTable: "Calendar",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SlaPolicy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    Calendar = table.Column<Guid>(type: "TEXT", nullable: true),
                    Notifications = table.Column<int>(type: "INTEGER", nullable: false),
                    PauseOn = table.Column<string>(type: "TEXT", nullable: true),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Class = table.Column<Guid>(type: "TEXT", nullable: false),
                    Owner = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlaPolicy", x => x.Id);
                    table.UniqueConstraint("AK_SlaPolicy_Guid", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_SlaPolicy_Calendar_Calendar",
                        column: x => x.Calendar,
                        principalTable: "Calendar",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SlaPolicy_Class_Class",
                        column: x => x.Class,
                        principalTable: "Class",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SlaPolicy_Identity_Owner",
                        column: x => x.Owner,
                        principalTable: "Identity",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "FormTab",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Form = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Position = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormTab", x => x.Id);
                    table.UniqueConstraint("AK_FormTab_Guid", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_FormTab_Form_Form",
                        column: x => x.Form,
                        principalTable: "Form",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
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
                    Color = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    DashArray = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    Waypoints = table.Column<string>(type: "TEXT", nullable: true),
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

            migrationBuilder.CreateTable(
                name: "WorkflowStatus",
                columns: table => new
                {
                    Workflow = table.Column<Guid>(type: "TEXT", nullable: false),
                    Status = table.Column<Guid>(type: "TEXT", nullable: false),
                    X = table.Column<int>(type: "INTEGER", nullable: false),
                    Y = table.Column<int>(type: "INTEGER", nullable: false),
                    IsStart = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsEnd = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowStatus", x => new { x.Workflow, x.Status });
                    table.ForeignKey(
                        name: "FK_WorkflowStatus_Status_Status",
                        column: x => x.Status,
                        principalTable: "Status",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkflowStatus_Workflow_Workflow",
                        column: x => x.Workflow,
                        principalTable: "Workflow",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attachment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", nullable: true),
                    Size = table.Column<long>(type: "INTEGER", nullable: false),
                    StoragePath = table.Column<string>(type: "TEXT", nullable: true),
                    Content = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Object = table.Column<Guid>(type: "TEXT", nullable: false),
                    Uploader = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attachment_Identity_Uploader",
                        column: x => x.Uploader,
                        principalTable: "Identity",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Attachment_Object_Object",
                        column: x => x.Object,
                        principalTable: "Object",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    Visibility = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsPinned = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    Object = table.Column<Guid>(type: "TEXT", nullable: false),
                    Author = table.Column<Guid>(type: "TEXT", nullable: false),
                    ParentComment = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.Id);
                    table.UniqueConstraint("AK_Comment_Guid", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Comment_Comment_ParentComment",
                        column: x => x.ParentComment,
                        principalTable: "Comment",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comment_Identity_Author",
                        column: x => x.Author,
                        principalTable: "Identity",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comment_Object_Object",
                        column: x => x.Object,
                        principalTable: "Object",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjectLink",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Source = table.Column<Guid>(type: "TEXT", nullable: false),
                    Target = table.Column<Guid>(type: "TEXT", nullable: false),
                    RelationType = table.Column<int>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjectLink_Object_Source",
                        column: x => x.Source,
                        principalTable: "Object",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObjectLink_Object_Target",
                        column: x => x.Target,
                        principalTable: "Object",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ObjectShare",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Object = table.Column<Guid>(type: "TEXT", nullable: false),
                    Identity = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectShare", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjectShare_Identity_Identity",
                        column: x => x.Identity,
                        principalTable: "Identity",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ObjectShare_Object_Object",
                        column: x => x.Object,
                        principalTable: "Object",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjectTag",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Object = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Color = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectTag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjectTag_Object_Object",
                        column: x => x.Object,
                        principalTable: "Object",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjectVisit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Owner = table.Column<Guid>(type: "TEXT", nullable: false),
                    Object = table.Column<Guid>(type: "TEXT", nullable: false),
                    LastVisited = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Favorite = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectVisit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjectVisit_Identity_Owner",
                        column: x => x.Owner,
                        principalTable: "Identity",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ObjectVisit_Object_Object",
                        column: x => x.Object,
                        principalTable: "Object",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjectWatcher",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Object = table.Column<Guid>(type: "TEXT", nullable: false),
                    Identity = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectWatcher", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjectWatcher_Identity_Identity",
                        column: x => x.Identity,
                        principalTable: "Identity",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ObjectWatcher_Object_Object",
                        column: x => x.Object,
                        principalTable: "Object",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Value",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Object = table.Column<Guid>(type: "TEXT", nullable: false),
                    Field = table.Column<Guid>(type: "TEXT", nullable: false),
                    Data = table.Column<string>(type: "TEXT", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Value", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Value_Field_Field",
                        column: x => x.Field,
                        principalTable: "Field",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Value_Object_Object",
                        column: x => x.Object,
                        principalTable: "Object",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SlaEscalationLevel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    AfterValue = table.Column<int>(type: "INTEGER", nullable: false),
                    Unit = table.Column<int>(type: "INTEGER", nullable: false),
                    Notify = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true),
                    Policy = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlaEscalationLevel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SlaEscalationLevel_SlaPolicy_Policy",
                        column: x => x.Policy,
                        principalTable: "SlaPolicy",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SlaScopeRule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    RuleType = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Policy = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlaScopeRule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SlaScopeRule_SlaPolicy_Policy",
                        column: x => x.Policy,
                        principalTable: "SlaPolicy",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SlaTarget",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Kind = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetValue = table.Column<int>(type: "INTEGER", nullable: false),
                    Unit = table.Column<int>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Policy = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlaTarget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SlaTarget_SlaPolicy_Policy",
                        column: x => x.Policy,
                        principalTable: "SlaPolicy",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormElement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Tab = table.Column<Guid>(type: "TEXT", nullable: false),
                    Parent = table.Column<Guid>(type: "TEXT", nullable: true),
                    Position = table.Column<int>(type: "INTEGER", nullable: false),
                    Kind = table.Column<int>(type: "INTEGER", nullable: false),
                    Field = table.Column<Guid>(type: "TEXT", nullable: true),
                    Label = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Layout = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormElement", x => x.Id);
                    table.UniqueConstraint("AK_FormElement_Guid", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_FormElement_Field_Field",
                        column: x => x.Field,
                        principalTable: "Field",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormElement_FormElement_Parent",
                        column: x => x.Parent,
                        principalTable: "FormElement",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormElement_FormTab_Tab",
                        column: x => x.Tab,
                        principalTable: "FormTab",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommentLike",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Comment = table.Column<Guid>(type: "TEXT", nullable: false),
                    Author = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentLike", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentLike_Comment_Comment",
                        column: x => x.Comment,
                        principalTable: "Comment",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentLike_Identity_Author",
                        column: x => x.Author,
                        principalTable: "Identity",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommentReaction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", maxLength: 36, nullable: false),
                    Emoji = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Comment = table.Column<Guid>(type: "TEXT", nullable: false),
                    Author = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentReaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentReaction_Comment_Comment",
                        column: x => x.Comment,
                        principalTable: "Comment",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentReaction_Identity_Author",
                        column: x => x.Author,
                        principalTable: "Identity",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessToken_Owner",
                table: "AccessToken",
                column: "Owner");

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_Object_Created",
                table: "Attachment",
                columns: new[] { "Object", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_Uploader",
                table: "Attachment",
                column: "Uploader");

            migrationBuilder.CreateIndex(
                name: "IX_Branding_Guid",
                table: "Branding",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessHourSlot_Calendar_DayOfWeek",
                table: "BusinessHourSlot",
                columns: new[] { "Calendar", "DayOfWeek" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Calendar_Class_Name",
                table: "Calendar",
                columns: new[] { "Class", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Category_Name",
                table: "Category",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Change_Commit_Ordinal",
                table: "Change",
                columns: new[] { "Commit", "Ordinal" });

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
                name: "IX_Comment_Author",
                table: "Comment",
                column: "Author");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_Object_Created",
                table: "Comment",
                columns: new[] { "Object", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_ParentComment",
                table: "Comment",
                column: "ParentComment");

            migrationBuilder.CreateIndex(
                name: "IX_CommentLike_Author",
                table: "CommentLike",
                column: "Author");

            migrationBuilder.CreateIndex(
                name: "IX_CommentLike_Comment_Author",
                table: "CommentLike",
                columns: new[] { "Comment", "Author" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommentReaction_Author",
                table: "CommentReaction",
                column: "Author");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReaction_Comment_Author_Emoji",
                table: "CommentReaction",
                columns: new[] { "Comment", "Author", "Emoji" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Commit_Object_Created",
                table: "Commit",
                columns: new[] { "Object", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_Commit_Object_Number",
                table: "Commit",
                columns: new[] { "Object", "Number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomQuickfilter_Guid",
                table: "CustomQuickfilter",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomQuickfilter_Owner",
                table: "CustomQuickfilter",
                column: "Owner");

            migrationBuilder.CreateIndex(
                name: "IX_CustomQuickfilter_ViewKey_ContextKey",
                table: "CustomQuickfilter",
                columns: new[] { "ViewKey", "ContextKey" });

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
                name: "IX_DashboardColumn_Dashboard",
                table: "DashboardColumn",
                column: "Dashboard");

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
                name: "IX_FormElement_Field",
                table: "FormElement",
                column: "Field");

            migrationBuilder.CreateIndex(
                name: "IX_FormElement_Parent",
                table: "FormElement",
                column: "Parent");

            migrationBuilder.CreateIndex(
                name: "IX_FormElement_Tab_Position",
                table: "FormElement",
                columns: new[] { "Tab", "Position" });

            migrationBuilder.CreateIndex(
                name: "IX_FormTab_Form_Position",
                table: "FormTab",
                columns: new[] { "Form", "Position" });

            migrationBuilder.CreateIndex(
                name: "IX_Group_Name",
                table: "Group",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupPolicy_GroupId",
                table: "GroupPolicy",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Holiday_Calendar_Date",
                table: "Holiday",
                columns: new[] { "Calendar", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Identity_Deputy",
                table: "Identity",
                column: "Deputy");

            migrationBuilder.CreateIndex(
                name: "IX_Identity_Tenant",
                table: "Identity",
                column: "Tenant");

            migrationBuilder.CreateIndex(
                name: "IX_Identity_UserName",
                table: "Identity",
                column: "UserName");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityGroupMembership_GroupId",
                table: "IdentityGroupMembership",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentitySession_Owner",
                table: "IdentitySession",
                column: "Owner");

            migrationBuilder.CreateIndex(
                name: "IX_KanbanBoard_Workspace_Kind",
                table: "KanbanBoard",
                columns: new[] { "Workspace", "Kind" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KanbanBoardColumn_Board",
                table: "KanbanBoardColumn",
                column: "Board");

            migrationBuilder.CreateIndex(
                name: "IX_KanbanBoardSwimlane_Board",
                table: "KanbanBoardSwimlane",
                column: "Board");

            migrationBuilder.CreateIndex(
                name: "IX_KindDashboard_Workspace_Kind",
                table: "KindDashboard",
                columns: new[] { "Workspace", "Kind" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KindDashboardColumn_Board",
                table: "KindDashboardColumn",
                column: "Board");

            migrationBuilder.CreateIndex(
                name: "IX_KindDashboardWidget_Column",
                table: "KindDashboardWidget",
                column: "Column");

            migrationBuilder.CreateIndex(
                name: "IX_Maintenance_Guid",
                table: "Maintenance",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NavigatorLink_Name",
                table: "NavigatorLink",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Object_Assignee",
                table: "Object",
                column: "Assignee");

            migrationBuilder.CreateIndex(
                name: "IX_Object_Class",
                table: "Object",
                column: "Class");

            migrationBuilder.CreateIndex(
                name: "IX_Object_Creator",
                table: "Object",
                column: "Creator");

            migrationBuilder.CreateIndex(
                name: "IX_Object_Key",
                table: "Object",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Object_Parent",
                table: "Object",
                column: "Parent");

            migrationBuilder.CreateIndex(
                name: "IX_Object_Sprint",
                table: "Object",
                column: "Sprint");

            migrationBuilder.CreateIndex(
                name: "IX_Object_Updater",
                table: "Object",
                column: "Updater");

            migrationBuilder.CreateIndex(
                name: "IX_Object_Workspace_Kind",
                table: "Object",
                columns: new[] { "Workspace", "Kind" });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectLink_Source",
                table: "ObjectLink",
                column: "Source");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectLink_Source_Target_RelationType",
                table: "ObjectLink",
                columns: new[] { "Source", "Target", "RelationType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjectLink_Target",
                table: "ObjectLink",
                column: "Target");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectShare_Identity",
                table: "ObjectShare",
                column: "Identity");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectShare_Object_Identity",
                table: "ObjectShare",
                columns: new[] { "Object", "Identity" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjectTag_Object_Name",
                table: "ObjectTag",
                columns: new[] { "Object", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjectView_Workspace_Kind_Name",
                table: "ObjectView",
                columns: new[] { "Workspace", "Kind", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjectVisit_Object",
                table: "ObjectVisit",
                column: "Object");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectVisit_Owner_Object",
                table: "ObjectVisit",
                columns: new[] { "Owner", "Object" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjectWatcher_Identity",
                table: "ObjectWatcher",
                column: "Identity");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectWatcher_Object_Identity",
                table: "ObjectWatcher",
                columns: new[] { "Object", "Identity" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PermissionAssignment_Group",
                table: "PermissionAssignment",
                column: "Group");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionAssignment_Guid",
                table: "PermissionAssignment",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PermissionAssignment_Scope_ScopeId",
                table: "PermissionAssignment",
                columns: new[] { "Scope", "ScopeId" });

            migrationBuilder.CreateIndex(
                name: "IX_PermissionAssignment_Scope_ScopeId_Group_Policy",
                table: "PermissionAssignment",
                columns: new[] { "Scope", "ScopeId", "Group", "Policy" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Priority_Class_Name",
                table: "Priority",
                columns: new[] { "Class", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SavedSearch_Owner",
                table: "SavedSearch",
                column: "Owner");

            migrationBuilder.CreateIndex(
                name: "IX_SlaEscalationLevel_Policy_Level",
                table: "SlaEscalationLevel",
                columns: new[] { "Policy", "Level" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SlaPolicy_Calendar",
                table: "SlaPolicy",
                column: "Calendar");

            migrationBuilder.CreateIndex(
                name: "IX_SlaPolicy_Class_Name",
                table: "SlaPolicy",
                columns: new[] { "Class", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SlaPolicy_Owner",
                table: "SlaPolicy",
                column: "Owner");

            migrationBuilder.CreateIndex(
                name: "IX_SlaScopeRule_Policy",
                table: "SlaScopeRule",
                column: "Policy");

            migrationBuilder.CreateIndex(
                name: "IX_SlaTarget_Policy_Kind",
                table: "SlaTarget",
                columns: new[] { "Policy", "Kind" });

            migrationBuilder.CreateIndex(
                name: "IX_Sprint_Workspace_Name",
                table: "Sprint",
                columns: new[] { "Workspace", "Name" },
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
                name: "IX_Template_Class_Name",
                table: "Template",
                columns: new[] { "Class", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Template_Parent",
                table: "Template",
                column: "Parent");

            migrationBuilder.CreateIndex(
                name: "IX_Tenant_Name",
                table: "Tenant",
                column: "Name",
                unique: true);

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
                name: "IX_UserNotification_Actor",
                table: "UserNotification",
                column: "Actor");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotification_Owner_Read",
                table: "UserNotification",
                columns: new[] { "Owner", "Read" });

            migrationBuilder.CreateIndex(
                name: "IX_UserSession_Owner_Scope_Key",
                table: "UserSession",
                columns: new[] { "Owner", "Scope", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Value_Field",
                table: "Value",
                column: "Field");

            migrationBuilder.CreateIndex(
                name: "IX_Value_Object_Field",
                table: "Value",
                columns: new[] { "Object", "Field" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Widget_Column",
                table: "Widget",
                column: "Column");

            migrationBuilder.CreateIndex(
                name: "IX_Workflow_Class_Name",
                table: "Workflow",
                columns: new[] { "Class", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStatus_Status",
                table: "WorkflowStatus",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Workspace_Inherited",
                table: "Workspace",
                column: "Inherited");

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
                name: "IX_WorkspaceBookmark_Owner_Workspace",
                table: "WorkspaceBookmark",
                columns: new[] { "Owner", "Workspace" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceBookmark_Workspace",
                table: "WorkspaceBookmark",
                column: "Workspace");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceCategory_WorkspaceId",
                table: "WorkspaceCategory",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceTenant_WorkspaceId",
                table: "WorkspaceTenant",
                column: "WorkspaceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessToken");

            migrationBuilder.DropTable(
                name: "Attachment");

            migrationBuilder.DropTable(
                name: "Branding");

            migrationBuilder.DropTable(
                name: "BusinessHourSlot");

            migrationBuilder.DropTable(
                name: "Change");

            migrationBuilder.DropTable(
                name: "ClassAllowedChild");

            migrationBuilder.DropTable(
                name: "CommentLike");

            migrationBuilder.DropTable(
                name: "CommentReaction");

            migrationBuilder.DropTable(
                name: "CustomQuickfilter");

            migrationBuilder.DropTable(
                name: "DashboardCategory");

            migrationBuilder.DropTable(
                name: "FormElement");

            migrationBuilder.DropTable(
                name: "GroupPolicy");

            migrationBuilder.DropTable(
                name: "Holiday");

            migrationBuilder.DropTable(
                name: "IdentityGroupMembership");

            migrationBuilder.DropTable(
                name: "IdentitySession");

            migrationBuilder.DropTable(
                name: "KanbanBoardColumn");

            migrationBuilder.DropTable(
                name: "KanbanBoardSwimlane");

            migrationBuilder.DropTable(
                name: "KindDashboardWidget");

            migrationBuilder.DropTable(
                name: "Maintenance");

            migrationBuilder.DropTable(
                name: "NavigatorLink");

            migrationBuilder.DropTable(
                name: "ObjectLink");

            migrationBuilder.DropTable(
                name: "ObjectShare");

            migrationBuilder.DropTable(
                name: "ObjectTag");

            migrationBuilder.DropTable(
                name: "ObjectView");

            migrationBuilder.DropTable(
                name: "ObjectVisit");

            migrationBuilder.DropTable(
                name: "ObjectWatcher");

            migrationBuilder.DropTable(
                name: "PermissionAssignment");

            migrationBuilder.DropTable(
                name: "Priority");

            migrationBuilder.DropTable(
                name: "SavedSearch");

            migrationBuilder.DropTable(
                name: "SlaEscalationLevel");

            migrationBuilder.DropTable(
                name: "SlaScopeRule");

            migrationBuilder.DropTable(
                name: "SlaTarget");

            migrationBuilder.DropTable(
                name: "Template");

            migrationBuilder.DropTable(
                name: "Transition");

            migrationBuilder.DropTable(
                name: "UserNotification");

            migrationBuilder.DropTable(
                name: "UserSession");

            migrationBuilder.DropTable(
                name: "Value");

            migrationBuilder.DropTable(
                name: "Widget");

            migrationBuilder.DropTable(
                name: "WorkflowStatus");

            migrationBuilder.DropTable(
                name: "WorkspaceBookmark");

            migrationBuilder.DropTable(
                name: "WorkspaceCategory");

            migrationBuilder.DropTable(
                name: "WorkspaceTenant");

            migrationBuilder.DropTable(
                name: "Commit");

            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "FormTab");

            migrationBuilder.DropTable(
                name: "KanbanBoard");

            migrationBuilder.DropTable(
                name: "KindDashboardColumn");

            migrationBuilder.DropTable(
                name: "Group");

            migrationBuilder.DropTable(
                name: "SlaPolicy");

            migrationBuilder.DropTable(
                name: "Field");

            migrationBuilder.DropTable(
                name: "DashboardColumn");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropTable(
                name: "Workflow");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Object");

            migrationBuilder.DropTable(
                name: "Form");

            migrationBuilder.DropTable(
                name: "KindDashboard");

            migrationBuilder.DropTable(
                name: "Calendar");

            migrationBuilder.DropTable(
                name: "Dashboard");

            migrationBuilder.DropTable(
                name: "StatusCategory");

            migrationBuilder.DropTable(
                name: "Identity");

            migrationBuilder.DropTable(
                name: "Sprint");

            migrationBuilder.DropTable(
                name: "Class");

            migrationBuilder.DropTable(
                name: "Tenant");

            migrationBuilder.DropTable(
                name: "Workspace");
        }
    }
}
