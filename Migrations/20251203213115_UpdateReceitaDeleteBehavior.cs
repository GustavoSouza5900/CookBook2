using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CookBook.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReceitaDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receita_AspNetUsers_UserId",
                table: "Receita");

            migrationBuilder.AddForeignKey(
                name: "FK_Receita_AspNetUsers_UserId",
                table: "Receita",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receita_AspNetUsers_UserId",
                table: "Receita");

            migrationBuilder.AddForeignKey(
                name: "FK_Receita_AspNetUsers_UserId",
                table: "Receita",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
