using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SmartPOS.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleEntityWithSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Permissions = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Permissions", "RoleName" },
                values: new object[,]
                {
                    { 1, "{\"Modules\":{\"UserManagement\":{\"CanCreate\":true,\"CanRead\":true,\"CanUpdate\":true,\"CanDelete\":true},\"Inventory\":{\"CanCreate\":true,\"CanRead\":true,\"CanUpdate\":true,\"CanDelete\":true},\"POS\":{\"CanCreate\":true,\"CanRead\":true,\"CanUpdate\":true,\"CanDelete\":true},\"Reports\":{\"CanCreate\":true,\"CanRead\":true,\"CanUpdate\":true,\"CanDelete\":true},\"Promotions\":{\"CanCreate\":true,\"CanRead\":true,\"CanUpdate\":true,\"CanDelete\":true},\"Customers\":{\"CanCreate\":true,\"CanRead\":true,\"CanUpdate\":true,\"CanDelete\":true},\"PurchaseOrders\":{\"CanCreate\":true,\"CanRead\":true,\"CanUpdate\":true,\"CanDelete\":true},\"AuditLogs\":{\"CanCreate\":true,\"CanRead\":true,\"CanUpdate\":true,\"CanDelete\":true}}}", "Admin" },
                    { 2, "{\"Modules\":{\"UserManagement\":{\"CanCreate\":false,\"CanRead\":true,\"CanUpdate\":true,\"CanDelete\":false},\"Inventory\":{\"CanCreate\":true,\"CanRead\":true,\"CanUpdate\":true,\"CanDelete\":false},\"POS\":{\"CanCreate\":true,\"CanRead\":true,\"CanUpdate\":true,\"CanDelete\":false},\"Reports\":{\"CanCreate\":false,\"CanRead\":true,\"CanUpdate\":false,\"CanDelete\":false},\"Promotions\":{\"CanCreate\":true,\"CanRead\":true,\"CanUpdate\":true,\"CanDelete\":false},\"Customers\":{\"CanCreate\":true,\"CanRead\":true,\"CanUpdate\":true,\"CanDelete\":false},\"PurchaseOrders\":{\"CanCreate\":true,\"CanRead\":true,\"CanUpdate\":true,\"CanDelete\":false},\"AuditLogs\":{\"CanCreate\":false,\"CanRead\":true,\"CanUpdate\":false,\"CanDelete\":false}}}", "Manager" },
                    { 3, "{\"Modules\":{\"UserManagement\":{\"CanCreate\":false,\"CanRead\":false,\"CanUpdate\":false,\"CanDelete\":false},\"Inventory\":{\"CanCreate\":false,\"CanRead\":true,\"CanUpdate\":false,\"CanDelete\":false},\"POS\":{\"CanCreate\":true,\"CanRead\":true,\"CanUpdate\":true,\"CanDelete\":false},\"Reports\":{\"CanCreate\":false,\"CanRead\":true,\"CanUpdate\":false,\"CanDelete\":false},\"Promotions\":{\"CanCreate\":false,\"CanRead\":true,\"CanUpdate\":false,\"CanDelete\":false},\"Customers\":{\"CanCreate\":true,\"CanRead\":true,\"CanUpdate\":true,\"CanDelete\":false},\"PurchaseOrders\":{\"CanCreate\":false,\"CanRead\":false,\"CanUpdate\":false,\"CanDelete\":false},\"AuditLogs\":{\"CanCreate\":false,\"CanRead\":false,\"CanUpdate\":false,\"CanDelete\":false}}}", "Cashier" },
                    { 4, "{\"Modules\":{\"UserManagement\":{\"CanCreate\":false,\"CanRead\":false,\"CanUpdate\":false,\"CanDelete\":false},\"Inventory\":{\"CanCreate\":false,\"CanRead\":true,\"CanUpdate\":false,\"CanDelete\":false},\"POS\":{\"CanCreate\":true,\"CanRead\":true,\"CanUpdate\":false,\"CanDelete\":false},\"Reports\":{\"CanCreate\":false,\"CanRead\":false,\"CanUpdate\":false,\"CanDelete\":false},\"Promotions\":{\"CanCreate\":false,\"CanRead\":true,\"CanUpdate\":false,\"CanDelete\":false},\"Customers\":{\"CanCreate\":false,\"CanRead\":true,\"CanUpdate\":true,\"CanDelete\":false},\"PurchaseOrders\":{\"CanCreate\":false,\"CanRead\":false,\"CanUpdate\":false,\"CanDelete\":false},\"AuditLogs\":{\"CanCreate\":false,\"CanRead\":false,\"CanUpdate\":false,\"CanDelete\":false}}}", "Customer" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RoleName",
                table: "Roles",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_RoleId",
                table: "User",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
