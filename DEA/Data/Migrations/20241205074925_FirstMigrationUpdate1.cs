using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEA.Next.Data.Migrations
{
    /// <inheritdoc />
    public partial class FirstMigrationUpdate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileDeliveryMethod",
                table: "CustomerDetails",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileDeliveryMethod",
                table: "CustomerDetails");
        }
    }
}
