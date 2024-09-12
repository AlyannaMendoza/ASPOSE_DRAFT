using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMS_DRAFT.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FilesData_FileMetadataId",
                table: "FilesData");

            migrationBuilder.CreateIndex(
                name: "IX_FilesData_FileMetadataId",
                table: "FilesData",
                column: "FileMetadataId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FilesData_FileMetadataId",
                table: "FilesData");

            migrationBuilder.CreateIndex(
                name: "IX_FilesData_FileMetadataId",
                table: "FilesData",
                column: "FileMetadataId");
        }
    }
}
