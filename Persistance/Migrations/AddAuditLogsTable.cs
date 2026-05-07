// Persistance/Migrations/AddAuditLogsTable.cs (corrected)
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations  // Fixed spelling
{
    public partial class AddAuditLogsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    ActionDescription = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    EntityType = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    PerformedBy = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    PerformedByRole = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    Result = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Changes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            // Create indexes for common queries - removed the unsupported 'descending' parameter
            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Timestamp",
                table: "AuditLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityType_EntityId",
                table: "AuditLogs",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Action",
                table: "AuditLogs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_PerformedBy",
                table: "AuditLogs",
                column: "PerformedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Result",
                table: "AuditLogs",
                column: "Result");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "AuditLogs");
        }
    }
}