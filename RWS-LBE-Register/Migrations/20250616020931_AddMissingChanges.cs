using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RWS_LBE_Register.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditLogs",
                table: "AuditLogs");

            migrationBuilder.RenameTable(
                name: "AuditLogs",
                newName: "audit_logs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_audit_logs",
                table: "audit_logs",
                column: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_audit_logs",
                table: "audit_logs");

            migrationBuilder.RenameTable(
                name: "audit_logs",
                newName: "AuditLogs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditLogs",
                table: "AuditLogs",
                column: "ID");
        }
    }
}
