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
                name: "addressbok",
                schema: "addressbook",
                columns: table => new
                {
                    OwnerClientId = table.Column<string>(type: "text", nullable: false),
                    ContactClientId = table.Column<string>(type: "text", nullable: false),
                    Nickname = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    LastTs = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)),
                    ReceiveApprovalGranted = table.Column<bool>(type: "boolean", nullable: false),
                    TransfersCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_addressbok", x => new { x.OwnerClientId, x.ContactClientId });
                });

            migrationBuilder.CreateIndex(
                name: "IX_addressbok_OwnerClientId_Name",
                schema: "addressbook",
                table: "addressbok",
                columns: new[] { "OwnerClientId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_addressbok_OwnerClientId_Nickname",
                schema: "addressbook",
                table: "addressbok",
                columns: new[] { "OwnerClientId", "Nickname" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "addressbok",
                schema: "addressbook");
        }
    }
}
