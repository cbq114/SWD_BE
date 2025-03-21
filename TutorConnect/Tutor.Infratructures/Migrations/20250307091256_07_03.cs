using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tutor.Infratructures.Migrations
{
    /// <inheritdoc />
    public partial class _07_03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "reason",
                table: "upgradeRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "reason",
                table: "upgradeRequests");
        }
    }
}
