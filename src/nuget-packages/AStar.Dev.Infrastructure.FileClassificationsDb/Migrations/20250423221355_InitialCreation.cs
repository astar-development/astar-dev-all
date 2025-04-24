using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AStar.Dev.Infrastructure.FileClassificationsDb.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "fileClassifications");

            migrationBuilder.CreateTable(
                name: "FileDetailClassification",
                schema: "fileClassifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileDetailClassification", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileToFileClassificationMapping",
                schema: "fileClassifications",
                columns: table => new
                {
                    FileId = table.Column<int>(type: "int", nullable: false),
                    ClassificationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileToFileClassificationMapping", x => new { x.FileId, x.ClassificationId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileDetailClassification",
                schema: "fileClassifications");

            migrationBuilder.DropTable(
                name: "FileToFileClassificationMapping",
                schema: "fileClassifications");
        }
    }
}
