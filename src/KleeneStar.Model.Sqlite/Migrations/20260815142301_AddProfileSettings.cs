using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KleeneStar.Model.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "Identity",
                type: "TEXT",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CostCenter",
                table: "Identity",
                type: "TEXT",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DateFormat",
                table: "Identity",
                type: "TEXT",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "Identity",
                type: "TEXT",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Deputy",
                table: "Identity",
                type: "TEXT",
                maxLength: 36,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailVerified",
                table: "Identity",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Identity",
                type: "TEXT",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Identity",
                type: "TEXT",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PersonnelNumber",
                table: "Identity",
                type: "TEXT",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Identity",
                type: "TEXT",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneCountry",
                table: "Identity",
                type: "TEXT",
                maxLength: 8,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "Identity",
                type: "TEXT",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Identity",
                type: "TEXT",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RoleSince",
                table: "Identity",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TimeZone",
                table: "Identity",
                type: "TEXT",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Identity",
                type: "TEXT",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Identity",
                type: "TEXT",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WeekStart",
                table: "Identity",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.CreateIndex(
                name: "IX_Identity_Deputy",
                table: "Identity",
                column: "Deputy");

            migrationBuilder.CreateIndex(
                name: "IX_Identity_UserName",
                table: "Identity",
                column: "UserName");

            migrationBuilder.CreateIndex(
                name: "IX_AccessToken_Owner",
                table: "AccessToken",
                column: "Owner");

            migrationBuilder.CreateIndex(
                name: "IX_IdentitySession_Owner",
                table: "IdentitySession",
                column: "Owner");

            migrationBuilder.AddForeignKey(
                name: "FK_Identity_Identity_Deputy",
                table: "Identity",
                column: "Deputy",
                principalTable: "Identity",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Identity_Identity_Deputy",
                table: "Identity");

            migrationBuilder.DropTable(
                name: "AccessToken");

            migrationBuilder.DropTable(
                name: "IdentitySession");

            migrationBuilder.DropIndex(
                name: "IX_Identity_Deputy",
                table: "Identity");

            migrationBuilder.DropIndex(
                name: "IX_Identity_UserName",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "Bio",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "CostCenter",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "DateFormat",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "Deputy",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "EmailVerified",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "PersonnelNumber",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "PhoneCountry",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "RoleSince",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "TimeZone",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "WeekStart",
                table: "Identity");
        }
    }
}
