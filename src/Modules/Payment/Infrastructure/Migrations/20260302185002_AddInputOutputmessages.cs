using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwiftScale.Modules.Payment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInputOutputmessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Transactions",
                schema: "payment",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "IsSuccess",
                schema: "payment",
                table: "Transactions");

            migrationBuilder.RenameTable(
                name: "Transactions",
                schema: "payment",
                newName: "Payments",
                newSchema: "payment");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "payment",
                table: "Payments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                schema: "payment",
                table: "Payments",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "InboxMessages",
                schema: "payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    OccurredOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                schema: "payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    OccurredOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InboxMessages",
                schema: "payment");

            migrationBuilder.DropTable(
                name: "OutboxMessages",
                schema: "payment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                schema: "payment",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "payment",
                table: "Payments");

            migrationBuilder.RenameTable(
                name: "Payments",
                schema: "payment",
                newName: "Transactions",
                newSchema: "payment");

            migrationBuilder.AddColumn<bool>(
                name: "IsSuccess",
                schema: "payment",
                table: "Transactions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transactions",
                schema: "payment",
                table: "Transactions",
                column: "Id");
        }
    }
}
