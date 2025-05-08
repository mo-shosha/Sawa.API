using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAWA.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_phone_number_to_branches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Branches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Branches");
        }
    }
}
