using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLeaveFieldsToRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AccountInactive",
                table: "Requests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateOnly>(
                name: "LeaveEnd",
                table: "Requests",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LeaveProgress",
                table: "Requests",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "LeaveStart",
                table: "Requests",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountInactive",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "LeaveEnd",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "LeaveProgress",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "LeaveStart",
                table: "Requests");
        }
    }
}
