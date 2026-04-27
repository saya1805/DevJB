using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevJBackend.Migrations
{
    /// <inheritdoc />
    public partial class addmodulenewlist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModuleDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModuleDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CrsTopicModelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleDetail_CrsInfo_CrsTopicModelId",
                        column: x => x.CrsTopicModelId,
                        principalTable: "CrsInfo",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModuleDetail_CrsTopicModelId",
                table: "ModuleDetail",
                column: "CrsTopicModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModuleDetail");
        }
    }
}
