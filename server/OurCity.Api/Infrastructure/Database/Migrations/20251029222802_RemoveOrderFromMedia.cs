using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OurCity.Api.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOrderFromMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "Media");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Media",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
