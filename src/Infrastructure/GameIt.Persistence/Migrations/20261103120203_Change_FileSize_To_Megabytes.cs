using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameIt.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Change_FileSize_To_Megabytes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileSizeInBytes",
                table: "Games",
                newName: "FileSizeInMB");

            migrationBuilder.Sql("UPDATE [Games] SET [FileSizeInMB] = [FileSizeInMB] / (1024 * 1024) WHERE [FileSizeInMB] IS NOT NULL AND [FileSizeInMB] > 1000000;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileSizeInMB",
                table: "Games",
                newName: "FileSizeInBytes");
        }
    }
}
