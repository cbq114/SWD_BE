using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tutor.Infratructures.Migrations
{
    /// <inheritdoc />
    public partial class updateeee99 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteInstructorDetails_FavoriteInstructors_FavoriteInstructorId",
                table: "FavoriteInstructorDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteInstructorDetails_Users_tutor",
                table: "FavoriteInstructorDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteInstructors_Users_UserName",
                table: "FavoriteInstructors");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteInstructorDetails_FavoriteInstructors_FavoriteInstructorId",
                table: "FavoriteInstructorDetails",
                column: "FavoriteInstructorId",
                principalTable: "FavoriteInstructors",
                principalColumn: "FavoriteInstructorId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteInstructorDetails_Users_tutor",
                table: "FavoriteInstructorDetails",
                column: "tutor",
                principalTable: "Users",
                principalColumn: "UserName",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteInstructors_Users_UserName",
                table: "FavoriteInstructors",
                column: "UserName",
                principalTable: "Users",
                principalColumn: "UserName",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteInstructorDetails_FavoriteInstructors_FavoriteInstructorId",
                table: "FavoriteInstructorDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteInstructorDetails_Users_tutor",
                table: "FavoriteInstructorDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteInstructors_Users_UserName",
                table: "FavoriteInstructors");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteInstructorDetails_FavoriteInstructors_FavoriteInstructorId",
                table: "FavoriteInstructorDetails",
                column: "FavoriteInstructorId",
                principalTable: "FavoriteInstructors",
                principalColumn: "FavoriteInstructorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteInstructorDetails_Users_tutor",
                table: "FavoriteInstructorDetails",
                column: "tutor",
                principalTable: "Users",
                principalColumn: "UserName",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteInstructors_Users_UserName",
                table: "FavoriteInstructors",
                column: "UserName",
                principalTable: "Users",
                principalColumn: "UserName");
        }
    }
}
