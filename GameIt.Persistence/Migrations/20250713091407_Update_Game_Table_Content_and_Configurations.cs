using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameIt.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Update_Game_Table_Content_and_Configurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "Comments_Games",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "Comments_Users",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_Publishers",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_Users_ApplicationUserId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "Purchases_Games",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "Purchases_Users",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Games",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Wishlists_Games",
                table: "Wishlists");

            migrationBuilder.DropForeignKey(
                name: "FK_Wishlists_Users",
                table: "Wishlists");

            migrationBuilder.DropTable(
                name: "Publishers");

            migrationBuilder.DropIndex(
                name: "IX_Wishlists_User_Game",
                table: "Wishlists");

            migrationBuilder.DropIndex(
                name: "IX_Games_PublisherId",
                table: "Games");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Discounts_DateRange",
                table: "Discounts");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Games");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_User_Game",
                table: "Reviews",
                newName: "IX_Reviews_UserId_GameId");

            migrationBuilder.RenameColumn(
                name: "GameName",
                table: "Games",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "Games",
                newName: "PublisherId1");

            migrationBuilder.RenameIndex(
                name: "IX_Games_ApplicationUserId",
                table: "Games",
                newName: "IX_Games_PublisherId1");

            migrationBuilder.RenameColumn(
                name: "CommentText",
                table: "Comments",
                newName: "Content");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Wishlists",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Last modification timestamp");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Wishlists",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()",
                oldComment: "When the item was added to wishlist");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Reviews",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Last modification timestamp");

            migrationBuilder.AlterColumn<int>(
                name: "Rating",
                table: "Reviews",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Rating value (1-5 stars)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Reviews",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()",
                oldComment: "When the item was added to wishlist");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Reviews",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true,
                oldComment: "Optional review text content");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reviews",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWID()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Purchases",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Last modification timestamp");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionId",
                table: "Purchases",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "Payment gateway reference");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PurchaseDate",
                table: "Purchases",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()",
                oldComment: "UTC timestamp of purchase");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentStatus",
                table: "Purchases",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldUnicode: false,
                oldMaxLength: 20,
                oldComment: "Status of the payment (Completed, Pending, Failed, Refunded)");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "Purchases",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldUnicode: false,
                oldMaxLength: 20,
                oldComment: "Payment method used (CreditCard, PayPal, Crypto, GiftCard)");

            migrationBuilder.AlterColumn<decimal>(
                name: "OriginalPrice",
                table: "Purchases",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true,
                oldComment: "List price at time of purchase");

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
                oldNullable: true,
                oldDefaultValue: "USD",
                oldComment: "ISO 4217 currency code");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Purchases",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()",
                oldComment: "When the item was added to wishlist");

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountPaid",
                table: "Purchases",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "Final paid amount after discounts");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Purchases",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWID()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Games",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Last modification timestamp");

            migrationBuilder.AlterColumn<string>(
                name: "SystemRequirements",
                table: "Games",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "JSON formatted system requirements for the game");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleaseDate",
                table: "Games",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()",
                oldComment: "Release date of the game");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Games",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldDefaultValue: 0m,
                oldComment: "Base price of the game");

            migrationBuilder.AlterColumn<bool>(
                name: "IsFree",
                table: "Games",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false,
                oldComment: "Indicates if the game is free to play");

            migrationBuilder.AlterColumn<bool>(
                name: "IsFeatured",
                table: "Games",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false,
                oldComment: "Indicates if the game is featured on the platform");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Games",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldComment: "URL to game cover image");

            migrationBuilder.AlterColumn<string>(
                name: "DownloadLink",
                table: "Games",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldComment: "Direct download link for the game");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Games",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldComment: "Detailed game description");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Games",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()",
                oldComment: "When the item was added to wishlist");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Games",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "Official game title");

            migrationBuilder.AddColumn<double>(
                name: "AverageRating",
                table: "Games",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<long>(
                name: "FileSizeInBytes",
                table: "Games",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalReviews",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Discounts",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Last modification timestamp");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Discounts",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Start date of the discount period");

            migrationBuilder.AlterColumn<decimal>(
                name: "Percentage",
                table: "Discounts",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldComment: "Discount percentage (0-100)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Discounts",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true,
                oldComment: "Whether discount is currently active");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Discounts",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "End date of the discount period");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Discounts",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "Optional description of the discount");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Discounts",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()",
                oldComment: "When the item was added to wishlist");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Discounts",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWID()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Comments",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Last modification timestamp");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Comments",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()",
                oldComment: "When the item was added to wishlist");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Comments",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWID()");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Comments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldDefaultValue: "",
                oldComment: "Text content of the comment");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Categories",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Last modification timestamp");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Categories",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()",
                oldComment: "When the item was added to wishlist");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryName",
                table: "Categories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "Official category name for grouping games");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Discounts_DateRange",
                table: "Discounts",
                sql: "[StartDate] < [EndDate]");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Games_GameId",
                table: "Comments",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Users_UserId",
                table: "Comments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Users_PublisherId1",
                table: "Games",
                column: "PublisherId1",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Games_GameId",
                table: "Purchases",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Users_UserId",
                table: "Purchases",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Games_GameId",
                table: "Reviews",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users_UserId",
                table: "Reviews",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Wishlists_Games_GameId",
                table: "Wishlists",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Wishlists_Users_UserId",
                table: "Wishlists",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Games_GameId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Users_UserId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_Users_PublisherId1",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Games_GameId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Users_UserId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Games_GameId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users_UserId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Wishlists_Games_GameId",
                table: "Wishlists");

            migrationBuilder.DropForeignKey(
                name: "FK_Wishlists_Users_UserId",
                table: "Wishlists");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Discounts_DateRange",
                table: "Discounts");

            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "FileSizeInBytes",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "TotalReviews",
                table: "Games");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_UserId_GameId",
                table: "Reviews",
                newName: "IX_Reviews_User_Game");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Games",
                newName: "GameName");

            migrationBuilder.RenameColumn(
                name: "PublisherId1",
                table: "Games",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Games_PublisherId1",
                table: "Games",
                newName: "IX_Games_ApplicationUserId");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Comments",
                newName: "CommentText");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Wishlists",
                type: "datetime2",
                nullable: true,
                comment: "Last modification timestamp",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Wishlists",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                comment: "When the item was added to wishlist",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Reviews",
                type: "datetime2",
                nullable: true,
                comment: "Last modification timestamp",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Rating",
                table: "Reviews",
                type: "int",
                nullable: false,
                comment: "Rating value (1-5 stars)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Reviews",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                comment: "When the item was added to wishlist",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Reviews",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                comment: "Optional review text content",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reviews",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Purchases",
                type: "datetime2",
                nullable: true,
                comment: "Last modification timestamp",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TransactionId",
                table: "Purchases",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "Payment gateway reference",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PurchaseDate",
                table: "Purchases",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                comment: "UTC timestamp of purchase",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentStatus",
                table: "Purchases",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: false,
                comment: "Status of the payment (Completed, Pending, Failed, Refunded)",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "Purchases",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: false,
                comment: "Payment method used (CreditCard, PayPal, Crypto, GiftCard)",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<decimal>(
                name: "OriginalPrice",
                table: "Purchases",
                type: "decimal(18,2)",
                nullable: true,
                comment: "List price at time of purchase",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "Purchases",
                type: "char(3)",
                fixedLength: true,
                maxLength: 3,
                nullable: true,
                defaultValue: "USD",
                comment: "ISO 4217 currency code",
                oldClrType: typeof(string),
                oldType: "char(3)",
                oldFixedLength: true,
                oldMaxLength: 3,
                oldNullable: true,
                oldDefaultValue: "USD");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Purchases",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                comment: "When the item was added to wishlist",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountPaid",
                table: "Purchases",
                type: "decimal(18,2)",
                nullable: false,
                comment: "Final paid amount after discounts",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Purchases",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Games",
                type: "datetime2",
                nullable: true,
                comment: "Last modification timestamp",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SystemRequirements",
                table: "Games",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                comment: "JSON formatted system requirements for the game",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleaseDate",
                table: "Games",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                comment: "Release date of the game",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Games",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m,
                comment: "Base price of the game",
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsFree",
                table: "Games",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Indicates if the game is free to play",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsFeatured",
                table: "Games",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Indicates if the game is featured on the platform",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Games",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "",
                comment: "URL to game cover image",
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DownloadLink",
                table: "Games",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                comment: "Direct download link for the game",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Games",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                comment: "Detailed game description",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Games",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                comment: "When the item was added to wishlist",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "GameName",
                table: "Games",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "Official game title",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "Games",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                comment: "Game file size (e.g., '15GB')");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Discounts",
                type: "datetime2",
                nullable: true,
                comment: "Last modification timestamp",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Discounts",
                type: "datetime2",
                nullable: false,
                comment: "Start date of the discount period",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<decimal>(
                name: "Percentage",
                table: "Discounts",
                type: "decimal(5,2)",
                nullable: false,
                comment: "Discount percentage (0-100)",
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Discounts",
                type: "bit",
                nullable: false,
                defaultValue: true,
                comment: "Whether discount is currently active",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Discounts",
                type: "datetime2",
                nullable: false,
                comment: "End date of the discount period",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Discounts",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "Optional description of the discount",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Discounts",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                comment: "When the item was added to wishlist",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Discounts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Comments",
                type: "datetime2",
                nullable: true,
                comment: "Last modification timestamp",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Comments",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                comment: "When the item was added to wishlist",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Comments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "CommentText",
                table: "Comments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                comment: "Text content of the comment",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Categories",
                type: "datetime2",
                nullable: true,
                comment: "Last modification timestamp",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Categories",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                comment: "When the item was added to wishlist",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryName",
                table: "Categories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "Official category name for grouping games",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateTable(
                name: "Publishers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()", comment: "When the item was added to wishlist"),
                    PublisherLogoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true, comment: "URL to publisher's logo image"),
                    PublisherName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Official publisher/developer name"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Last modification timestamp")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publishers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_User_Game",
                table: "Wishlists",
                columns: new[] { "UserId", "GameId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Games_PublisherId",
                table: "Games",
                column: "PublisherId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Discounts_DateRange",
                table: "Discounts",
                sql: "[StartDate] < [EndDate] AND [EndDate] > GETDATE()");

            migrationBuilder.AddForeignKey(
                name: "Comments_Games",
                table: "Comments",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "Comments_Users",
                table: "Comments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Publishers",
                table: "Games",
                column: "PublisherId",
                principalTable: "Publishers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Users_ApplicationUserId",
                table: "Games",
                column: "ApplicationUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "Purchases_Games",
                table: "Purchases",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "Purchases_Users",
                table: "Purchases",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Games",
                table: "Reviews",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users",
                table: "Reviews",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Wishlists_Games",
                table: "Wishlists",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Wishlists_Users",
                table: "Wishlists",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
