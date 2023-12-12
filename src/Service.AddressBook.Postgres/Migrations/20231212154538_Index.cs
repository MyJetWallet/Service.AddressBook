using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.AddressBook.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Index : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_addressbook_OwnerClientId_Iban",
                schema: "addressbook",
                table: "addressbook");

            migrationBuilder.CreateIndex(
                name: "IX_addressbook_OwnerClientId_Iban_IbanType",
                schema: "addressbook",
                table: "addressbook",
                columns: new[] { "OwnerClientId", "Iban", "IbanType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_addressbook_OwnerClientId_Iban_IbanType",
                schema: "addressbook",
                table: "addressbook");

            migrationBuilder.CreateIndex(
                name: "IX_addressbook_OwnerClientId_Iban",
                schema: "addressbook",
                table: "addressbook",
                columns: new[] { "OwnerClientId", "Iban" },
                unique: true);
        }
    }
}
