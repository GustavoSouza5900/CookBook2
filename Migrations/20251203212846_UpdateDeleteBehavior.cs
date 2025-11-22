using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CookBook.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comentario_AspNetUsers_UserId",
                table: "Comentario");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceitaCurtida_AspNetUsers_UserId",
                table: "ReceitaCurtida");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceitaSalva_AspNetUsers_UserId",
                table: "ReceitaSalva");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBadge_AspNetUsers_UserId",
                table: "UserBadge");

            migrationBuilder.AddForeignKey(
                name: "FK_Comentario_AspNetUsers_UserId",
                table: "Comentario",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceitaCurtida_AspNetUsers_UserId",
                table: "ReceitaCurtida",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceitaSalva_AspNetUsers_UserId",
                table: "ReceitaSalva",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBadge_AspNetUsers_UserId",
                table: "UserBadge",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comentario_AspNetUsers_UserId",
                table: "Comentario");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceitaCurtida_AspNetUsers_UserId",
                table: "ReceitaCurtida");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceitaSalva_AspNetUsers_UserId",
                table: "ReceitaSalva");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBadge_AspNetUsers_UserId",
                table: "UserBadge");

            migrationBuilder.AddForeignKey(
                name: "FK_Comentario_AspNetUsers_UserId",
                table: "Comentario",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceitaCurtida_AspNetUsers_UserId",
                table: "ReceitaCurtida",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceitaSalva_AspNetUsers_UserId",
                table: "ReceitaSalva",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBadge_AspNetUsers_UserId",
                table: "UserBadge",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
