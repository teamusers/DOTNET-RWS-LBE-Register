using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RWS_LBE_Register.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingChanges2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RLPUserNumberings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    year = table.Column<long>(type: "bigint", nullable: false),
                    month = table.Column<long>(type: "bigint", nullable: false),
                    day = table.Column<long>(type: "bigint", nullable: false),
                    rlp_id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    rlp_no = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    rlp_id_ending_no = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RLPUserNumberings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RLPUserNumberings");
        }
    }
}
