using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tutor.Infratructures.Migrations
{
    /// <inheritdoc />
    public partial class addetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LessonAttendanceDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LessonAttendanceId = table.Column<int>(type: "int", nullable: false),
                    ParticipantUsername = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    JoinTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LeaveTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonAttendanceDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonAttendanceDetails_LessonAttendances_LessonAttendanceId",
                        column: x => x.LessonAttendanceId,
                        principalTable: "LessonAttendances",
                        principalColumn: "LessonAttendanceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LessonAttendanceDetails_Users_ParticipantUsername",
                        column: x => x.ParticipantUsername,
                        principalTable: "Users",
                        principalColumn: "UserName",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LessonAttendanceDetails_LessonAttendanceId",
                table: "LessonAttendanceDetails",
                column: "LessonAttendanceId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonAttendanceDetails_ParticipantUsername",
                table: "LessonAttendanceDetails",
                column: "ParticipantUsername");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LessonAttendanceDetails");
        }
    }
}
