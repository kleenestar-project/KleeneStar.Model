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
            migrationBuilder.AddColumn<int>(
                name: "AccessModifier",
                table: "Field",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Cardinality",
                table: "Field",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DefaultSpec",
                table: "Field",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deprecated",
                table: "Field",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "FieldType",
                table: "Field",
                type: "TEXT",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HelpText",
                table: "Field",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Placeholder",
                table: "Field",
                type: "TEXT",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Required",
                table: "Field",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Unique",
                table: "Field",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ValidationRules",
                table: "Field",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessModifier",
                table: "Field");

            migrationBuilder.DropColumn(
                name: "Cardinality",
                table: "Field");

            migrationBuilder.DropColumn(
                name: "DefaultSpec",
                table: "Field");

            migrationBuilder.DropColumn(
                name: "Deprecated",
                table: "Field");

            migrationBuilder.DropColumn(
                name: "FieldType",
                table: "Field");

            migrationBuilder.DropColumn(
                name: "HelpText",
                table: "Field");

            migrationBuilder.DropColumn(
                name: "Placeholder",
                table: "Field");

            migrationBuilder.DropColumn(
                name: "Required",
                table: "Field");

            migrationBuilder.DropColumn(
                name: "Unique",
                table: "Field");

            migrationBuilder.DropColumn(
                name: "ValidationRules",
                table: "Field");
        }
    }
}
