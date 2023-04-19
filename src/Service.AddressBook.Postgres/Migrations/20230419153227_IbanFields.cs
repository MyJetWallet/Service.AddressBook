using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.AddressBook.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class IbanFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankName",
                schema: "addressbook",
                table: "addressbok",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bic",
                schema: "addressbook",
                table: "addressbok",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Iban",
                schema: "addressbook",
                table: "addressbok",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Order",
                schema: "addressbook",
                table: "addressbok",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_addressbok_OwnerClientId_Iban",
                schema: "addressbook",
                table: "addressbok",
                columns: new[] { "OwnerClientId", "Iban" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_addressbok_OwnerClientId_Iban",
                schema: "addressbook",
                table: "addressbok");

            migrationBuilder.DropColumn(
                name: "BankName",
                schema: "addressbook",
                table: "addressbok");

            migrationBuilder.DropColumn(
                name: "Bic",
                schema: "addressbook",
                table: "addressbok");

            migrationBuilder.DropColumn(
                name: "Iban",
                schema: "addressbook",
                table: "addressbok");

            migrationBuilder.DropColumn(
                name: "Order",
                schema: "addressbook",
                table: "addressbok");
        }
    }
}
