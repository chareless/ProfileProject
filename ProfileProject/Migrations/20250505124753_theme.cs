using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProfileProject.Migrations
{
    /// <inheritdoc />
    public partial class theme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserThemes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ThemeContrast = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    CaptionShow = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    DarkLayout = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    RtlLayout = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    BoxContainer = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    PresetTheme = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    Layout = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    CreateWhen = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateWhen = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserThemes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserThemes");
        }
    }
}
