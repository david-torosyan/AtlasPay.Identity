using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtlasPay.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRegistrationDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Registrations",
                type: "character varying(320)",
                maxLength: 320,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Registrations",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Registrations",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Registrations");
        }
    }
}
