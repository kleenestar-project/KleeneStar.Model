using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KleeneStar.Model.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class PortalSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PortalTemplate",
                table: "Form",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PortalVisible",
                table: "Class",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

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

            migrationBuilder.CreateIndex(
                name: "IX_ObjectShare_Identity",
                table: "ObjectShare",
                column: "Identity");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectShare_Object_Identity",
                table: "ObjectShare",
                columns: new[] { "Object", "Identity" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObjectShare");

            migrationBuilder.DropColumn(
                name: "PortalTemplate",
                table: "Form");

            migrationBuilder.DropColumn(
                name: "PortalVisible",
                table: "Class");
        }
    }
}
