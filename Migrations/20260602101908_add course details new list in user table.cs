using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevJBackend.Migrations
{
    /// <inheritdoc />
    public partial class addcoursedetailsnewlistinusertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserCourseDetails",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    crsId = table.Column<int>(type: "int", nullable: false),
                    Coursename = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Course_status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Course_fee = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    course_days = table.Column<int>(type: "int", nullable: false),
                    payment_mode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    payment_status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCourseDetails", x => x.id);
                    table.ForeignKey(
                        name: "FK_UserCourseDetails_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCourseDetails_UserId",
                table: "UserCourseDetails",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCourseDetails");
        }
    }
}
