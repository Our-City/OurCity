using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OurCity.Api.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class SeedTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("08e4cb83-1d93-4e0c-bc4c-30c2aee497b8"), "Education" },
                    { new Guid("0a7f2a8d-504c-4b17-8448-7a274a1bba44"), "Tourism" },
                    { new Guid("1f59e1d4-37b7-4ad2-9f6f-431a5e8cf8b7"), "Community Events" },
                    { new Guid("3a46f0b5-238f-4e41-bbb4-254bdb14f92e"), "Housing" },
                    { new Guid("3b84d6d5-4d4e-4e09-8a90-6c2d257ae14c"), "Construction" },
                    { new Guid("41a6f4ac-8a91-4209-b40e-8b14b9a01873"), "Infrastructure" },
                    { new Guid("4f6329f1-3201-4a94-b41c-cf74ed91f777"), "Business" },
                    { new Guid("5f8b0e26-33a1-4e9f-a3c5-7e78f32f804a"), "Entertainment" },
                    { new Guid("6b8e5470-5a3e-48a7-a3e3-142e7e8b2e02"), "Transportation" },
                    { new Guid("7dd62a06-5a7c-44ff-a2f1-299a507d21aa"), "Environment" },
                    { new Guid("8c7b9a39-b4a9-40a3-85ce-034d97a2a6c2"), "Parks & Recreation" },
                    { new Guid("91f75b8d-bf32-46af-a6a9-8f89417cbbd0"), "Shopping" },
                    { new Guid("9e4f0c3f-02e4-4c88-bf89-9cc7cf7b63c3"), "Events" },
                    { new Guid("c4db2614-0d47-4c16-89da-fd8c97a216f4"), "Food & Dining" },
                    { new Guid("c6d13b79-0a6a-4db3-a219-1c6240b9ef82"), "Culture" },
                    { new Guid("e122c911-8cbe-45e0-9d91-9353ed685c61"), "Sports" },
                    { new Guid("f1f8e911-61db-45a2-b9df-7dc6de4c9a0d"), "Safety" },
                    { new Guid("f6d81e88-8332-4ee7-96b9-8517f7d7a2d9"), "Healthcare" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("08e4cb83-1d93-4e0c-bc4c-30c2aee497b8"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("0a7f2a8d-504c-4b17-8448-7a274a1bba44"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("1f59e1d4-37b7-4ad2-9f6f-431a5e8cf8b7"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("3a46f0b5-238f-4e41-bbb4-254bdb14f92e"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("3b84d6d5-4d4e-4e09-8a90-6c2d257ae14c"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("41a6f4ac-8a91-4209-b40e-8b14b9a01873"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("4f6329f1-3201-4a94-b41c-cf74ed91f777"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("5f8b0e26-33a1-4e9f-a3c5-7e78f32f804a"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("6b8e5470-5a3e-48a7-a3e3-142e7e8b2e02"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("7dd62a06-5a7c-44ff-a2f1-299a507d21aa"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("8c7b9a39-b4a9-40a3-85ce-034d97a2a6c2"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("91f75b8d-bf32-46af-a6a9-8f89417cbbd0"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("9e4f0c3f-02e4-4c88-bf89-9cc7cf7b63c3"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("c4db2614-0d47-4c16-89da-fd8c97a216f4"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("c6d13b79-0a6a-4db3-a219-1c6240b9ef82"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("e122c911-8cbe-45e0-9d91-9353ed685c61"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("f1f8e911-61db-45a2-b9df-7dc6de4c9a0d"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("f6d81e88-8332-4ee7-96b9-8517f7d7a2d9"));
        }
    }
}
