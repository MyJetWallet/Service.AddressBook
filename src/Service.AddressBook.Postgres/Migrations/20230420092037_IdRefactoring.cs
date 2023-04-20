using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.AddressBook.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class IdRefactoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_addressbok",
                schema: "addressbook",
                table: "addressbok");

            migrationBuilder.RenameColumn(
                name: "ContactClientId",
                schema: "addressbook",
                table: "addressbok",
                newName: "ContactId");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerClientId",
                schema: "addressbook",
                table: "addressbok",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_addressbok",
                schema: "addressbook",
                table: "addressbok",
                column: "ContactId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_addressbok",
                schema: "addressbook",
                table: "addressbok");

            migrationBuilder.RenameColumn(
                name: "ContactId",
                schema: "addressbook",
                table: "addressbok",
                newName: "ContactClientId");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerClientId",
                schema: "addressbook",
                table: "addressbok",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_addressbok",
                schema: "addressbook",
                table: "addressbok",
                columns: new[] { "OwnerClientId", "ContactClientId" });
        }
    }
}
