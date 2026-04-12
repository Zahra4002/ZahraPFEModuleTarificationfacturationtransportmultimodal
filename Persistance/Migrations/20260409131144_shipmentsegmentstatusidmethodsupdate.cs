using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class shipmentsegmentstatusidmethodsupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-1111-1111-1111-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 9, 13, 11, 43, 683, DateTimeKind.Utc).AddTicks(5477));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-2222-2222-2222-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 9, 13, 11, 43, 683, DateTimeKind.Utc).AddTicks(5488));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-3333-3333-3333-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 9, 13, 11, 43, 683, DateTimeKind.Utc).AddTicks(5492));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-4444-4444-4444-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 9, 13, 11, 43, 683, DateTimeKind.Utc).AddTicks(5498));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-5555-5555-5555-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 9, 13, 11, 43, 683, DateTimeKind.Utc).AddTicks(5503));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-6666-6666-6666-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 9, 13, 11, 43, 683, DateTimeKind.Utc).AddTicks(5509));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-7777-7777-7777-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 9, 13, 11, 43, 683, DateTimeKind.Utc).AddTicks(5512));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
