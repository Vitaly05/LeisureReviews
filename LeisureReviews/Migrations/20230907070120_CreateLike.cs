using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeisureReviews.Migrations
{
    /// <inheritdoc />
    public partial class CreateLike : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLikesReview");

            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReviewId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Likes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Likes_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Likes_ReviewId",
                table: "Likes",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_UserId",
                table: "Likes",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.CreateTable(
                name: "UserLikesReview",
                columns: table => new
                {
                    LikedReviewsId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LikedUsersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLikesReview", x => new { x.LikedReviewsId, x.LikedUsersId });
                    table.ForeignKey(
                        name: "FK_UserLikesReview_AspNetUsers_LikedUsersId",
                        column: x => x.LikedUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLikesReview_Reviews_LikedReviewsId",
                        column: x => x.LikedReviewsId,
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLikesReview_LikedUsersId",
                table: "UserLikesReview",
                column: "LikedUsersId");
        }
    }
}
