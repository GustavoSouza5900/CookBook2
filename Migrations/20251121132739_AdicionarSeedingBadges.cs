using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CookBook.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarSeedingBadges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Badge",
                columns: new[] { "Id", "Description", "IconClass", "Name", "TriggerEvent" },
                values: new object[,]
                {
                    { 1, "Publique sua primeira receita.", "fas fa-star", "Chef Novato", "RECIPE_COUNT_1" },
                    { 2, "Publique 10 receitas.", "fas fa-trophy", "Mestre Culinário", "RECIPE_COUNT_10" },
                    { 3, "Receba 1 curtidas no total.", "fas fa-heart", "Primeira de Muitas", "TOTAL_LIKES_1" },
                    { 4, "Receba 50 curtidas no total.", "fas fa-fire", "Popular", "TOTAL_LIKES_50" },
                    { 5, "Publique seu primeiro comentário", "fas fa-comment", "Opnião Própria", "COMMENT_COUNT_1" },
                    { 6, "Publique 5 comentários.", "fas fa-comments", "Comentarista Ativo", "COMMENT_COUNT_5" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Badge",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Badge",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Badge",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Badge",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Badge",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Badge",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
