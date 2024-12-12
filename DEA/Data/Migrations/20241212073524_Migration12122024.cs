using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEA.Next.Data.Migrations
{
    /// <inheritdoc />
    public partial class Migration12122024 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    CustomerName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UserName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Token = table.Column<string>(type: "character varying(800)", maxLength: 800, nullable: false),
                    Queue = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TemplateKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DocumentId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DocumentEncoding = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    MaxBatchSize = table.Column<int>(type: "integer", nullable: false),
                    SendEmail = table.Column<bool>(type: "boolean", nullable: false),
                    SendSubject = table.Column<bool>(type: "boolean", nullable: false),
                    FieldOneValue = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FieldOneName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FieldTwoValue = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FieldTwoName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Domain = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FileDeliveryMethod = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Extension = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    CustomerDetailsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentDetails_CustomerDetails_CustomerDetailsId",
                        column: x => x.CustomerDetailsId,
                        principalTable: "CustomerDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EmailInboxPath = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CustomerDetailsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailDetails_CustomerDetails_CustomerDetailsId",
                        column: x => x.CustomerDetailsId,
                        principalTable: "CustomerDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FtpDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FtpType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FtpProfile = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FtpHost = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FtpUser = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FtpPassword = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FtpPort = table.Column<int>(type: "integer", nullable: false),
                    FtpMainFolder = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FtpSubFolder = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FtpFolderLoop = table.Column<bool>(type: "boolean", nullable: false),
                    FtpRemoveFiles = table.Column<bool>(type: "boolean", nullable: false),
                    FtpMoveToSubFolder = table.Column<bool>(type: "boolean", nullable: false),
                    CustomerDetailsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FtpDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FtpDetails_CustomerDetails_CustomerDetailsId",
                        column: x => x.CustomerDetailsId,
                        principalTable: "CustomerDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDetails_CustomerDetailsId",
                table: "DocumentDetails",
                column: "CustomerDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailDetails_CustomerDetailsId",
                table: "EmailDetails",
                column: "CustomerDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_FtpDetails_CustomerDetailsId",
                table: "FtpDetails",
                column: "CustomerDetailsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentDetails");

            migrationBuilder.DropTable(
                name: "EmailDetails");

            migrationBuilder.DropTable(
                name: "FtpDetails");

            migrationBuilder.DropTable(
                name: "CustomerDetails");
        }
    }
}
