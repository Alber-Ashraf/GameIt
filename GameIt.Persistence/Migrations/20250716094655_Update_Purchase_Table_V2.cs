using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameIt.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Update_Purchase_Table_V2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Purchases_TransactionId",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "RefundReason",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Purchases");

            migrationBuilder.RenameColumn(
                name: "PaymentStatus",
                table: "Purchases",
                newName: "Status");

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "Purchases",
                type: "char(3)",
                fixedLength: true,
                maxLength: 3,
                nullable: false,
                defaultValue: "USD",
                oldClrType: typeof(string),
                oldType: "char(3)",
                oldFixedLength: true,
                oldMaxLength: 3,
                oldNullable: true,
                oldDefaultValue: "USD");

            migrationBuilder.AddColumn<string>(
                name: "StripePaymentIntentId",
                table: "Purchases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripePaymentIntentId",
                table: "Purchases");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Purchases",
                newName: "PaymentStatus");

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "Purchases",
                type: "char(3)",
                fixedLength: true,
                maxLength: 3,
                nullable: true,
                defaultValue: "USD",
                oldClrType: typeof(string),
                oldType: "char(3)",
                oldFixedLength: true,
                oldMaxLength: 3,
                oldDefaultValue: "USD");

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Purchases",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RefundReason",
                table: "Purchases",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "Purchases",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_TransactionId",
                table: "Purchases",
                column: "TransactionId",
                unique: true);
        }
    }
}
