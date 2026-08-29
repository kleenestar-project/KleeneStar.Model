using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KleeneStar.Model.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class AttachmentVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Attachment",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_Object_FileName_Version",
                table: "Attachment",
                columns: new[] { "Object", "FileName", "Version" });

            // Rows written before the column existed all carry version 0, which would read as
            // "predates versioning" forever and leave an installation's whole history unnumbered.
            // They are numbered here from what the store already knows: within one object and one
            // file name, the upload date orders the chain, and the row id breaks a tie between two
            // uploads that share a timestamp. The result is 1..n per name, which is exactly what
            // ModelHub.Add assigns from now on.
            migrationBuilder.Sql(
                """
                UPDATE "Attachment"
                SET "Version" =
                (
                    SELECT COUNT(*)
                    FROM "Attachment" AS peer
                    WHERE peer."Object" = "Attachment"."Object"
                      AND peer."FileName" = "Attachment"."FileName"
                      AND (peer."Created" < "Attachment"."Created"
                        OR (peer."Created" = "Attachment"."Created" AND peer."Id" <= "Attachment"."Id"))
                )
                WHERE "Version" = 0;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Attachment_Object_FileName_Version",
                table: "Attachment");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Attachment");
        }
    }
}
