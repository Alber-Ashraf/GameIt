using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameIt.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_AdditionalDownloadLink_To_Games : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalDownloadLink",
                table: "Games",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalDownloadLink",
                table: "Games");
        }
    }
}
