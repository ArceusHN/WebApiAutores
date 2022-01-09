using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiAutores.Migrations
{
    public partial class AutoresLibros_Correcion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Autor_Autores_AutorId",
                table: "Autor");

            migrationBuilder.DropForeignKey(
                name: "FK_Autor_Libros_LibroId",
                table: "Autor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Autor",
                table: "Autor");

            migrationBuilder.RenameTable(
                name: "Autor",
                newName: "AutoresLibros");

            migrationBuilder.RenameIndex(
                name: "IX_Autor_LibroId",
                table: "AutoresLibros",
                newName: "IX_AutoresLibros_LibroId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AutoresLibros",
                table: "AutoresLibros",
                columns: new[] { "AutorId", "LibroId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AutoresLibros_Autores_AutorId",
                table: "AutoresLibros",
                column: "AutorId",
                principalTable: "Autores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AutoresLibros_Libros_LibroId",
                table: "AutoresLibros",
                column: "LibroId",
                principalTable: "Libros",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AutoresLibros_Autores_AutorId",
                table: "AutoresLibros");

            migrationBuilder.DropForeignKey(
                name: "FK_AutoresLibros_Libros_LibroId",
                table: "AutoresLibros");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AutoresLibros",
                table: "AutoresLibros");

            migrationBuilder.RenameTable(
                name: "AutoresLibros",
                newName: "Autor");

            migrationBuilder.RenameIndex(
                name: "IX_AutoresLibros_LibroId",
                table: "Autor",
                newName: "IX_Autor_LibroId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Autor",
                table: "Autor",
                columns: new[] { "AutorId", "LibroId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Autor_Autores_AutorId",
                table: "Autor",
                column: "AutorId",
                principalTable: "Autores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Autor_Libros_LibroId",
                table: "Autor",
                column: "LibroId",
                principalTable: "Libros",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
