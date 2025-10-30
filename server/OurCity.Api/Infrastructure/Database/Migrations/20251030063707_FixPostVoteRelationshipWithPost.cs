using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OurCity.Api.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class FixPostVoteRelationshipWithPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostPostVote");

            migrationBuilder.CreateIndex(
                name: "IX_PostVotes_PostId",
                table: "PostVotes",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostVotes_Posts_PostId",
                table: "PostVotes",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostVotes_Posts_PostId",
                table: "PostVotes");

            migrationBuilder.DropIndex(
                name: "IX_PostVotes_PostId",
                table: "PostVotes");

            migrationBuilder.CreateTable(
                name: "PostPostVote",
                columns: table => new
                {
                    PostsId = table.Column<Guid>(type: "uuid", nullable: false),
                    VotesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostPostVote", x => new { x.PostsId, x.VotesId });
                    table.ForeignKey(
                        name: "FK_PostPostVote_PostVotes_VotesId",
                        column: x => x.VotesId,
                        principalTable: "PostVotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostPostVote_Posts_PostsId",
                        column: x => x.PostsId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostPostVote_VotesId",
                table: "PostPostVote",
                column: "VotesId");
        }
    }
}
