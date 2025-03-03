using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HM_SkincareApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class jj : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookmarks_Labels_LabelId",
                table: "Bookmarks");

            migrationBuilder.DropTable(
                name: "Labels");

            migrationBuilder.RenameColumn(
                name: "LabelId",
                table: "Bookmarks",
                newName: "LabelId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookmarks_LabelId",
                table: "Bookmarks",
                newName: "IX_Bookmarks_LabelId");

            migrationBuilder.CreateTable(
                name: "Labels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Labels", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Bookmarks_Labels_LabelId",
                table: "Bookmarks",
                column: "LabelId",
                principalTable: "Labels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookmarks_Labels_LabelId",
                table: "Bookmarks");

            migrationBuilder.DropTable(
                name: "Labels");

            migrationBuilder.RenameColumn(
                name: "LabelId",
                table: "Bookmarks",
                newName: "LabelId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookmarks_LabelId",
                table: "Bookmarks",
                newName: "IX_Bookmarks_LabelId");

            migrationBuilder.CreateTable(
                name: "Labels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Labels", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Bookmarks_Labels_LabelId",
                table: "Bookmarks",
                column: "LabelId",
                principalTable: "Labels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
