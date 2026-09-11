using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KleeneStar.Model.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class TransitionRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GuardExpression",
                table: "Transition",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostFunctions",
                table: "Transition",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ScreenForm",
                table: "Transition",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ValidatorExpression",
                table: "Transition",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transition_ScreenForm",
                table: "Transition",
                column: "ScreenForm");

            migrationBuilder.AddForeignKey(
                name: "FK_Transition_Form_ScreenForm",
                table: "Transition",
                column: "ScreenForm",
                principalTable: "Form",
                principalColumn: "Guid",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transition_Form_ScreenForm",
                table: "Transition");

            migrationBuilder.DropIndex(
                name: "IX_Transition_ScreenForm",
                table: "Transition");

            migrationBuilder.DropColumn(
                name: "GuardExpression",
                table: "Transition");

            migrationBuilder.DropColumn(
                name: "PostFunctions",
                table: "Transition");

            migrationBuilder.DropColumn(
                name: "ScreenForm",
                table: "Transition");

            migrationBuilder.DropColumn(
                name: "ValidatorExpression",
                table: "Transition");
        }
    }
}
