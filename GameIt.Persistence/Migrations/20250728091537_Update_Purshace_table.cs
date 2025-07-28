using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameIt.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Update_Purshace_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "IsRefunded",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "RefundDate",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "RefundId",
                table: "Purchases");

            migrationBuilder.AlterColumn<string>(
                name: "StripePaymentIntentId",
                table: "Purchases",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "StripePaymentIntentId",
                table: "Purchases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Purchases",
                type: "char(3)",
                fixedLength: true,
                maxLength: 3,
                nullable: false,
                defaultValue: "USD");

            migrationBuilder.AddColumn<bool>(
                name: "IsRefunded",
                table: "Purchases",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefundDate",
                table: "Purchases",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefundId",
                table: "Purchases",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
