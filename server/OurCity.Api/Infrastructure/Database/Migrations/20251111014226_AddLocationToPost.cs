using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OurCity.Api.Infrastructure.Database.App.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationToPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Posts",
                type: "double precision",
                nullable: true
            );

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Posts",
                type: "double precision",
                nullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Latitude", table: "Posts");

            migrationBuilder.DropColumn(name: "Longitude", table: "Posts");
        }
    }
}
