using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddCasePartyDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PartyType",
                table: "CaseParties",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "CaseParties",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "CaseParties",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "CaseParties",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LawyerName",
                table: "CaseParties",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LawyerPhone",
                table: "CaseParties",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PartyRole",
                table: "CaseParties",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PartyType",
                table: "CaseParties");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "CaseParties");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "CaseParties");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "CaseParties");

            migrationBuilder.DropColumn(
                name: "LawyerName",
                table: "CaseParties");

            migrationBuilder.DropColumn(
                name: "LawyerPhone",
                table: "CaseParties");

            migrationBuilder.AlterColumn<string>(
                name: "PartyRole",
                table: "CaseParties",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);
        }
    }
}
