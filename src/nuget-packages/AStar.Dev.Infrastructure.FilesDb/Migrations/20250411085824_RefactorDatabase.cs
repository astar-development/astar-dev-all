using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AStar.Dev.Infrastructure.FilesDb.Migrations
{
    /// <inheritdoc />
    public partial class RefactorDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "files");

            migrationBuilder.CreateTable(
                name: "FileAccessDetail",
                schema: "files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DetailsLastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastViewed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SoftDeleted = table.Column<bool>(type: "bit", nullable: false),
                    SoftDeletePending = table.Column<bool>(type: "bit", nullable: false),
                    MoveRequired = table.Column<bool>(type: "bit", nullable: false),
                    HardDeletePending = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileAccessDetail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileClassification",
                schema: "files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileClassification", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModelToIgnore",
                schema: "files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelToIgnore", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TagToIgnore",
                schema: "files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    IgnoreImage = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagToIgnore", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileDetail",
                schema: "files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileAccessDetailId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    DirectoryName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Height = table.Column<int>(type: "int", nullable: false),
                    Width = table.Column<int>(type: "int", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileDetail_FileAccessDetail_FileAccessDetailId",
                        column: x => x.FileAccessDetailId,
                        principalSchema: "files",
                        principalTable: "FileAccessDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FileDetailClassification",
                schema: "files",
                columns: table => new
                {
                    FileId = table.Column<int>(type: "int", nullable: false),
                    ClassificationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileDetailClassification", x => new { x.FileId, x.ClassificationId });
                    table.ForeignKey(
                        name: "FK_FileDetailClassification_FileClassification_ClassificationId",
                        column: x => x.ClassificationId,
                        principalSchema: "files",
                        principalTable: "FileClassification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FileDetailClassification_FileDetail_FileId",
                        column: x => x.FileId,
                        principalSchema: "files",
                        principalTable: "FileDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileDetail_FileAccessDetailId",
                schema: "files",
                table: "FileDetail",
                column: "FileAccessDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_FileDetailClassification_ClassificationId",
                schema: "files",
                table: "FileDetailClassification",
                column: "ClassificationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileDetailClassification",
                schema: "files");

            migrationBuilder.DropTable(
                name: "ModelToIgnore",
                schema: "files");

            migrationBuilder.DropTable(
                name: "TagToIgnore",
                schema: "files");

            migrationBuilder.DropTable(
                name: "FileClassification",
                schema: "files");

            migrationBuilder.DropTable(
                name: "FileDetail",
                schema: "files");

            migrationBuilder.DropTable(
                name: "FileAccessDetail",
                schema: "files");
        }
    }
}
