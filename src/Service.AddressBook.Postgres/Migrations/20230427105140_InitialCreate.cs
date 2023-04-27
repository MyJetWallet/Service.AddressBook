using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.AddressBook.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "addressbook");

            migrationBuilder.CreateTable(
                name: "addressbook",
                schema: "addressbook",
                columns: table => new
                {
                    ContactId = table.Column<string>(type: "text", nullable: false),
                    OwnerClientId = table.Column<string>(type: "text", nullable: true),
                    Nickname = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    LastTs = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)),
                    ReceiveApprovalGranted = table.Column<bool>(type: "boolean", nullable: false),
                    TransfersCount = table.Column<int>(type: "integer", nullable: false),
                    Iban = table.Column<string>(type: "text", nullable: true),
                    Bic = table.Column<string>(type: "text", nullable: true),
                    BankName = table.Column<string>(type: "text", nullable: true),
                    Order = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_addressbook", x => x.ContactId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_addressbook_OwnerClientId_Iban",
                schema: "addressbook",
                table: "addressbook",
                columns: new[] { "OwnerClientId", "Iban" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_addressbook_OwnerClientId_Name",
                schema: "addressbook",
                table: "addressbook",
                columns: new[] { "OwnerClientId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_addressbook_OwnerClientId_Nickname",
                schema: "addressbook",
                table: "addressbook",
                columns: new[] { "OwnerClientId", "Nickname" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "addressbook",
                schema: "addressbook");
        }
    }
}
