using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProfileProject.Migrations
{
    /// <inheritdoc />
    public partial class log : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoginControls",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoginDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginControls", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LoginControls_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAccessLogs",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    ControllerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccessTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisitedUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccessLogs", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoginControls_UserId",
                table: "LoginControls",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoginControls");

            migrationBuilder.DropTable(
                name: "UserAccessLogs");
        }
    }
}
