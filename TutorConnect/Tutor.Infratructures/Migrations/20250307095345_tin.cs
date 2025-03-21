using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tutor.Infratructures.Migrations
{
    /// <inheritdoc />
    public partial class tin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TransactionId",
                table: "Refunds",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "LessonId",
                table: "Promotions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_LessonId",
                table: "Promotions",
                column: "LessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Promotions_Lessons_LessonId",
                table: "Promotions",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "LessonId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Promotions_Lessons_LessonId",
                table: "Promotions");

            migrationBuilder.DropIndex(
                name: "IX_Promotions_LessonId",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "LessonId",
                table: "Promotions");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionId",
                table: "Refunds",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
