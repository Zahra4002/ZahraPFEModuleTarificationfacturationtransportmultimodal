using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddDataMerchandise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedById", "CreatedDate", "DecimalPlaces", "DeletedDate", "IsActive", "IsDefault", "IsDeleted", "ModifiedBy", "ModifiedById", "ModifiedDate", "Name", "Symbol" },
                values: new object[,]
                {
                    { new Guid("55555555-5555-5555-5555-555555555555"), "EUR", null, null, null, 2, null, true, false, false, null, null, null, "Euro", "€" },
                    { new Guid("66666666-6666-6666-6666-666666666666"), "USD", null, null, null, 2, null, true, false, false, null, null, null, "US Dollar", "$" },
                    { new Guid("77777777-7777-7777-7777-777777777777"), "MAD", null, null, null, 2, null, true, false, false, null, null, null, "Dirham Marocain", "DH" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"));

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"));
        }
    }
}
