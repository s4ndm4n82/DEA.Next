using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEA.Next.Data.Migrations
{
    /// <inheritdoc />
    public partial class Migration070220251 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SendBody",
                table: "EmailDetails",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SendBody",
                table: "EmailDetails");
        }
    }
}
