using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class shipmentsegmentstatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "TransportSegments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-1111-1111-1111-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 9, 11, 17, 6, 626, DateTimeKind.Utc).AddTicks(9033));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-2222-2222-2222-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 9, 11, 17, 6, 626, DateTimeKind.Utc).AddTicks(9047));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-3333-3333-3333-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 9, 11, 17, 6, 626, DateTimeKind.Utc).AddTicks(9058));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-4444-4444-4444-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 9, 11, 17, 6, 626, DateTimeKind.Utc).AddTicks(9066));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-5555-5555-5555-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 9, 11, 17, 6, 626, DateTimeKind.Utc).AddTicks(9072));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-6666-6666-6666-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 9, 11, 17, 6, 626, DateTimeKind.Utc).AddTicks(9078));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-7777-7777-7777-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 9, 11, 17, 6, 626, DateTimeKind.Utc).AddTicks(9082));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "TransportSegments");

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-1111-1111-1111-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 8, 21, 20, 24, 736, DateTimeKind.Utc).AddTicks(7423));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-2222-2222-2222-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 8, 21, 20, 24, 736, DateTimeKind.Utc).AddTicks(7431));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-3333-3333-3333-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 8, 21, 20, 24, 736, DateTimeKind.Utc).AddTicks(7437));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-4444-4444-4444-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 8, 21, 20, 24, 736, DateTimeKind.Utc).AddTicks(7441));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-5555-5555-5555-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 8, 21, 20, 24, 736, DateTimeKind.Utc).AddTicks(7445));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-6666-6666-6666-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 8, 21, 20, 24, 736, DateTimeKind.Utc).AddTicks(7451));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-7777-7777-7777-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 8, 21, 20, 24, 736, DateTimeKind.Utc).AddTicks(7455));
        }
    }
}
