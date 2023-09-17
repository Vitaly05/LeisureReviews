using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeisureReviews.Migrations
{
    /// <inheritdoc />
    public partial class CreateIllustration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IllustrationId",
                table: "Reviews");

            migrationBuilder.CreateTable(
                name: "Illustrations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReviewId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Illustrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Illustrations_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Illustrations_ReviewId",
                table: "Illustrations",
                column: "ReviewId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Illustrations");

            migrationBuilder.AddColumn<string>(
                name: "IllustrationId",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
