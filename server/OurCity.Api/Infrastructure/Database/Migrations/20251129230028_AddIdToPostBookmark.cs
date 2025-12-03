using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OurCity.Api.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddIdToPostBookmark : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PostBookmarks",
                table: "PostBookmarks");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "PostBookmarks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostBookmarks",
                table: "PostBookmarks",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PostBookmarks_UserId",
                table: "PostBookmarks",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PostBookmarks",
                table: "PostBookmarks");

            migrationBuilder.DropIndex(
                name: "IX_PostBookmarks_UserId",
                table: "PostBookmarks");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PostBookmarks");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostBookmarks",
                table: "PostBookmarks",
                columns: new[] { "UserId", "PostId" });
        }
    }
}
