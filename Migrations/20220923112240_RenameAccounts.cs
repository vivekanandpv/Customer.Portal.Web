using Microsoft.EntityFrameworkCore.Migrations;

namespace Customer.Portal.Web.Migrations
{
    public partial class RenameAccounts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Type_Customers_CustomerId",
                table: "Type");

            migrationBuilder.DropForeignKey(
                name: "FK_Type_Customers_CustomerId1",
                table: "Type");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Type",
                table: "Type");

            migrationBuilder.RenameTable(
                name: "Type",
                newName: "Accounts");

            migrationBuilder.RenameIndex(
                name: "IX_Type_CustomerId1",
                table: "Accounts",
                newName: "IX_Accounts_CustomerId1");

            migrationBuilder.RenameIndex(
                name: "IX_Type_CustomerId",
                table: "Accounts",
                newName: "IX_Accounts_CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Customers_CustomerId",
                table: "Accounts",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Customers_CustomerId1",
                table: "Accounts",
                column: "CustomerId1",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Customers_CustomerId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Customers_CustomerId1",
                table: "Accounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts");

            migrationBuilder.RenameTable(
                name: "Accounts",
                newName: "Type");

            migrationBuilder.RenameIndex(
                name: "IX_Accounts_CustomerId1",
                table: "Type",
                newName: "IX_Type_CustomerId1");

            migrationBuilder.RenameIndex(
                name: "IX_Accounts_CustomerId",
                table: "Type",
                newName: "IX_Type_CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Type",
                table: "Type",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Type_Customers_CustomerId",
                table: "Type",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Type_Customers_CustomerId1",
                table: "Type",
                column: "CustomerId1",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
