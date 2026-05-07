using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class logaudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-1111-1111-1111-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 25, 18, 15, 15, 777, DateTimeKind.Utc).AddTicks(1780));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-2222-2222-2222-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 25, 18, 15, 15, 777, DateTimeKind.Utc).AddTicks(1790));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-3333-3333-3333-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 25, 18, 15, 15, 777, DateTimeKind.Utc).AddTicks(1794));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-4444-4444-4444-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 25, 18, 15, 15, 777, DateTimeKind.Utc).AddTicks(1801));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-5555-5555-5555-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 25, 18, 15, 15, 777, DateTimeKind.Utc).AddTicks(1805));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-6666-6666-6666-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 25, 18, 15, 15, 777, DateTimeKind.Utc).AddTicks(1810));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-7777-7777-7777-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 25, 18, 15, 15, 777, DateTimeKind.Utc).AddTicks(1813));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-1111-1111-1111-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 10, 19, 50, 29, 821, DateTimeKind.Utc).AddTicks(4902));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-2222-2222-2222-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 10, 19, 50, 29, 821, DateTimeKind.Utc).AddTicks(4913));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-3333-3333-3333-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 10, 19, 50, 29, 821, DateTimeKind.Utc).AddTicks(4918));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-4444-4444-4444-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 10, 19, 50, 29, 821, DateTimeKind.Utc).AddTicks(4923));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-5555-5555-5555-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 10, 19, 50, 29, 821, DateTimeKind.Utc).AddTicks(4927));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-6666-6666-6666-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 10, 19, 50, 29, 821, DateTimeKind.Utc).AddTicks(4932));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-7777-7777-7777-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 10, 19, 50, 29, 821, DateTimeKind.Utc).AddTicks(4937));
        }
    }
}
