using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAWA.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatedonationtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripeSessionId",
                table: "Donations");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Donations",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Donations",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeSessionId",
                table: "Donations",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
