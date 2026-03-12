using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddMerchandiseType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "MerchandiseTypes",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedById", "CreatedDate", "DeletedDate", "Description", "HazardousLevel", "IsActive", "IsDeleted", "ModifiedBy", "ModifiedById", "ModifiedDate", "Name", "PriceMultiplier", "RequiresSpecialHandling" },
                values: new object[,]
                {
                    { new Guid("88888888-8888-8888-8888-888888888888"), "GEN001", null, null, null, null, "Standard general cargo with no special requirements", 0, true, false, null, null, null, "General Cargo", 1.0m, false },
                    { new Guid("99999999-9999-9999-9999-999999999999"), "PER002", null, null, null, null, "Temperature-sensitive goods requiring refrigeration or special handling", 0, true, false, null, null, null, "Perishable Goods", 1.5m, true },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "HAZ003", null, null, null, null, "Dangerous goods requiring special permits and handling procedures", 3, true, false, null, null, null, "Hazardous Materials", 2.0m, true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MerchandiseTypes",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"));

            migrationBuilder.DeleteData(
                table: "MerchandiseTypes",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"));

            migrationBuilder.DeleteData(
                table: "MerchandiseTypes",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));
        }
    }
}
