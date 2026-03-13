using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class RepaireSomeIssues : Migration
    {
        /// <inheritdoc />  
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransportSegments_Zones_ZoneFrometId",
                table: "TransportSegments");

            migrationBuilder.DropIndex(
                name: "IX_TransportSegments_ZoneFrometId",
                table: "TransportSegments");

            migrationBuilder.DropColumn(
                name: "ZoneFrometId",
                table: "TransportSegments");

            migrationBuilder.CreateIndex(
                name: "IX_TransportSegments_ZoneFromId",
                table: "TransportSegments",
                column: "ZoneFromId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransportSegments_Zones_ZoneFromId",
                table: "TransportSegments",
                column: "ZoneFromId",
                principalTable: "Zones",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransportSegments_Zones_ZoneFromId",
                table: "TransportSegments");

            migrationBuilder.DropIndex(
                name: "IX_TransportSegments_ZoneFromId",
                table: "TransportSegments");

            migrationBuilder.AddColumn<Guid>(
                name: "ZoneFrometId",
                table: "TransportSegments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TransportSegments_ZoneFrometId",
                table: "TransportSegments",
                column: "ZoneFrometId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransportSegments_Zones_ZoneFrometId",
                table: "TransportSegments",
                column: "ZoneFrometId",
                principalTable: "Zones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
