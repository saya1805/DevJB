using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevJBackend.Migrations
{
    /// <inheritdoc />
    public partial class add2columnsincrsinfotable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "crsDurationInDays",
                table: "CrsInfo",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "crsPrice",
                table: "CrsInfo",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "crsDurationInDays",
                table: "CrsInfo");

            migrationBuilder.DropColumn(
                name: "crsPrice",
                table: "CrsInfo");
        }
    }
}
