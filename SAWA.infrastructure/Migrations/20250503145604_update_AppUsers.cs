using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAWA.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class update_AppUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUsers_Photos_ProfilePhotoId",
                table: "AppUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUsers_Photos_WallpaperPhotoId",
                table: "AppUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Photos_AppUsers_UserId",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Photos_UserId",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_AppUsers_ProfilePhotoId",
                table: "AppUsers");

            migrationBuilder.DropIndex(
                name: "IX_AppUsers_WallpaperPhotoId",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "ProfilePhotoId",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "WallpaperPhotoId",
                table: "AppUsers");

            migrationBuilder.AddColumn<string>(
                name: "ProfilePhotoURL",
                table: "AppUsers",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WallpaperPhotoURL",
                table: "AppUsers",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePhotoURL",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "WallpaperPhotoURL",
                table: "AppUsers");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Photos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProfilePhotoId",
                table: "AppUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WallpaperPhotoId",
                table: "AppUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Photos_UserId",
                table: "Photos",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_ProfilePhotoId",
                table: "AppUsers",
                column: "ProfilePhotoId",
                unique: true,
                filter: "[ProfilePhotoId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_WallpaperPhotoId",
                table: "AppUsers",
                column: "WallpaperPhotoId",
                unique: true,
                filter: "[WallpaperPhotoId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUsers_Photos_ProfilePhotoId",
                table: "AppUsers",
                column: "ProfilePhotoId",
                principalTable: "Photos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppUsers_Photos_WallpaperPhotoId",
                table: "AppUsers",
                column: "WallpaperPhotoId",
                principalTable: "Photos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_AppUsers_UserId",
                table: "Photos",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
