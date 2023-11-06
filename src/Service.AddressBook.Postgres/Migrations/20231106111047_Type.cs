using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.AddressBook.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Type : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankCountry",
                schema: "addressbook",
                table: "addressbook",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                schema: "addressbook",
                table: "addressbook",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IbanType",
                schema: "addressbook",
                table: "addressbook",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankCountry",
                schema: "addressbook",
                table: "addressbook");

            migrationBuilder.DropColumn(
                name: "FullName",
                schema: "addressbook",
                table: "addressbook");

            migrationBuilder.DropColumn(
                name: "IbanType",
                schema: "addressbook",
                table: "addressbook");
        }
    }
}
