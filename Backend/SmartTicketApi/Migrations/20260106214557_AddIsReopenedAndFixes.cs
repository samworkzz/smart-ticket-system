using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartTicketApi.Migrations
{
    /// <inheritdoc />
    public partial class AddIsReopenedAndFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReopened",
                table: "Tickets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReopened",
                table: "Tickets");
        }
    }
}
