using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class addcountrycodetotaxerule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "TaxRules",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-1111-1111-1111-aaaaaaaaaaaa"),
                columns: new[] { "CountryCode", "CreatedDate" },
                values: new object[] { "", new DateTime(2026, 4, 8, 21, 20, 24, 736, DateTimeKind.Utc).AddTicks(7423) });

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-2222-2222-2222-aaaaaaaaaaaa"),
                columns: new[] { "CountryCode", "CreatedDate" },
                values: new object[] { "", new DateTime(2026, 4, 8, 21, 20, 24, 736, DateTimeKind.Utc).AddTicks(7431) });

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-3333-3333-3333-aaaaaaaaaaaa"),
                columns: new[] { "CountryCode", "CreatedDate" },
                values: new object[] { "", new DateTime(2026, 4, 8, 21, 20, 24, 736, DateTimeKind.Utc).AddTicks(7437) });

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-4444-4444-4444-aaaaaaaaaaaa"),
                columns: new[] { "CountryCode", "CreatedDate" },
                values: new object[] { "", new DateTime(2026, 4, 8, 21, 20, 24, 736, DateTimeKind.Utc).AddTicks(7441) });

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-5555-5555-5555-aaaaaaaaaaaa"),
                columns: new[] { "CountryCode", "CreatedDate" },
                values: new object[] { "", new DateTime(2026, 4, 8, 21, 20, 24, 736, DateTimeKind.Utc).AddTicks(7445) });

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-6666-6666-6666-aaaaaaaaaaaa"),
                columns: new[] { "CountryCode", "CreatedDate" },
                values: new object[] { "", new DateTime(2026, 4, 8, 21, 20, 24, 736, DateTimeKind.Utc).AddTicks(7451) });

            migrationBuilder.UpdateData(
                table: "TaxRules",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-7777-7777-7777-aaaaaaaaaaaa"),
                columns: new[] { "CountryCode", "CreatedDate" },
                values: new object[] { "", new DateTime(2026, 4, 8, 21, 20, 24, 736, DateTimeKind.Utc).AddTicks(7455) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "TaxRules");

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
        }
    }
}
