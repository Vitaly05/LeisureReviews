using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeisureReviews.Migrations
{
    /// <inheritdoc />
    public partial class CreateLeisure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rates_Reviews_ReviewId",
                table: "Rates");

            migrationBuilder.DropColumn(
                name: "Leisure",
                table: "Reviews");

            migrationBuilder.RenameColumn(
                name: "ReviewId",
                table: "Rates",
                newName: "LeisureId");

            migrationBuilder.RenameIndex(
                name: "IX_Rates_ReviewId",
                table: "Rates",
                newName: "IX_Rates_LeisureId");

            migrationBuilder.AddColumn<string>(
                name: "LeisureId",
                table: "Reviews",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Leisures",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leisures", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_LeisureId",
                table: "Reviews",
                column: "LeisureId");

            migrationBuilder.CreateIndex(
                name: "IX_Leisures_Name",
                table: "Leisures",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Rates_Leisures_LeisureId",
                table: "Rates",
                column: "LeisureId",
                principalTable: "Leisures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Leisures_LeisureId",
                table: "Reviews",
                column: "LeisureId",
                principalTable: "Leisures",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rates_Leisures_LeisureId",
                table: "Rates");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Leisures_LeisureId",
                table: "Reviews");

            migrationBuilder.DropTable(
                name: "Leisures");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_LeisureId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "LeisureId",
                table: "Reviews");

            migrationBuilder.RenameColumn(
                name: "LeisureId",
                table: "Rates",
                newName: "ReviewId");

            migrationBuilder.RenameIndex(
                name: "IX_Rates_LeisureId",
                table: "Rates",
                newName: "IX_Rates_ReviewId");

            migrationBuilder.AddColumn<string>(
                name: "Leisure",
                table: "Reviews",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Rates_Reviews_ReviewId",
                table: "Rates",
                column: "ReviewId",
                principalTable: "Reviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
