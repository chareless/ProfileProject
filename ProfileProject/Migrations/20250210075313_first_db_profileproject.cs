using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProfileProject.Migrations
{
    /// <inheritdoc />
    public partial class first_db_profileproject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Surname",
                table: "Users",
                newName: "NameSurname");

            migrationBuilder.RenameColumn(
                name: "MobilePhone",
                table: "Users",
                newName: "MobilePhone2");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateWhen",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "MobilePhone1",
                table: "Users",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateWhen",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateWhen",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MobilePhone1",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdateWhen",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "NameSurname",
                table: "Users",
                newName: "Surname");

            migrationBuilder.RenameColumn(
                name: "MobilePhone2",
                table: "Users",
                newName: "MobilePhone");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
