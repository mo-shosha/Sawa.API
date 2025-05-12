using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAWA.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class update_report_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "Reports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reason",
                table: "Reports");
        }
    }
}
