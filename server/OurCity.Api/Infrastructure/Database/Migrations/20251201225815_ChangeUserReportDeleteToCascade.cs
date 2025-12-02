using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OurCity.Api.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUserReportDeleteToCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserReports_AspNetUsers_ReporterId",
                table: "UserReports");

            migrationBuilder.DropForeignKey(
                name: "FK_UserReports_AspNetUsers_TargetUserId",
                table: "UserReports");

            migrationBuilder.AddForeignKey(
                name: "FK_UserReports_AspNetUsers_ReporterId",
                table: "UserReports",
                column: "ReporterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserReports_AspNetUsers_TargetUserId",
                table: "UserReports",
                column: "TargetUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserReports_AspNetUsers_ReporterId",
                table: "UserReports");

            migrationBuilder.DropForeignKey(
                name: "FK_UserReports_AspNetUsers_TargetUserId",
                table: "UserReports");

            migrationBuilder.AddForeignKey(
                name: "FK_UserReports_AspNetUsers_ReporterId",
                table: "UserReports",
                column: "ReporterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserReports_AspNetUsers_TargetUserId",
                table: "UserReports",
                column: "TargetUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
