using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class InitCreatetionn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                table: "Zones",
                type: "numeric",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-1111-1111-1111-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 5, 23, 38, 38, 714, DateTimeKind.Utc).AddTicks(8045));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-2222-2222-2222-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 5, 23, 38, 38, 714, DateTimeKind.Utc).AddTicks(8062));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-3333-3333-3333-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 5, 23, 38, 38, 714, DateTimeKind.Utc).AddTicks(8066));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-4444-4444-4444-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 5, 23, 38, 38, 714, DateTimeKind.Utc).AddTicks(8070));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-5555-5555-5555-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 5, 23, 38, 38, 714, DateTimeKind.Utc).AddTicks(8078));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-6666-6666-6666-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 5, 23, 38, 38, 714, DateTimeKind.Utc).AddTicks(8083));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-7777-7777-7777-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 5, 23, 38, 38, 714, DateTimeKind.Utc).AddTicks(8116));

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "Id",
                keyValue: new Guid("10111111-1111-1111-1111-111111111111"),
                column: "TaxRate",
                value: null);

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "Id",
                keyValue: new Guid("12111111-1111-1111-1111-111111111111"),
                column: "TaxRate",
                value: null);

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "Id",
                keyValue: new Guid("13111111-1111-1111-1111-111111111111"),
                column: "TaxRate",
                value: null);

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "Id",
                keyValue: new Guid("14111111-1111-1111-1111-111111111111"),
                column: "TaxRate",
                value: null);

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "Id",
                keyValue: new Guid("51111111-1111-1111-1111-111111111111"),
                column: "TaxRate",
                value: null);

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "Id",
                keyValue: new Guid("61111111-1111-1111-1111-111111111111"),
                column: "TaxRate",
                value: null);

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "Id",
                keyValue: new Guid("71111111-1111-1111-1111-111111111111"),
                column: "TaxRate",
                value: null);

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "Id",
                keyValue: new Guid("81111111-1111-1111-1111-111111111111"),
                column: "TaxRate",
                value: null);

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "Id",
                keyValue: new Guid("91111111-1111-1111-1111-111111111111"),
                column: "TaxRate",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxRate",
                table: "Zones");

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-1111-1111-1111-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 3, 23, 15, 58, 30, 313, DateTimeKind.Utc).AddTicks(3130));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-2222-2222-2222-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 3, 23, 15, 58, 30, 313, DateTimeKind.Utc).AddTicks(3150));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-3333-3333-3333-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 3, 23, 15, 58, 30, 313, DateTimeKind.Utc).AddTicks(3155));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-4444-4444-4444-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 3, 23, 15, 58, 30, 313, DateTimeKind.Utc).AddTicks(3161));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-5555-5555-5555-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 3, 23, 15, 58, 30, 313, DateTimeKind.Utc).AddTicks(3167));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-6666-6666-6666-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 3, 23, 15, 58, 30, 313, DateTimeKind.Utc).AddTicks(3174));

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-7777-7777-7777-aaaaaaaaaaaa"),
                column: "CreatedDate",
                value: new DateTime(2026, 3, 23, 15, 58, 30, 313, DateTimeKind.Utc).AddTicks(3179));
        }
    }
}
