using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WinniElectricidad.AccesoDatos.Migrations
{
    public partial class Fix_Resenias_FKs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropForeignKey(
                name: "FK_Resenias_Servicios_ServicioId",
                table: "Resenias");

            migrationBuilder.DropForeignKey(
                name: "FK_Resenias_Usuarios_UsuarioClienteIdUsuario",
                table: "Resenias");

            migrationBuilder.DropIndex(
                name: "IX_Resenias_ServicioId",
                table: "Resenias");

            migrationBuilder.DropIndex(
                name: "IX_Resenias_UsuarioClienteIdUsuario",
                table: "Resenias");

            migrationBuilder.DropColumn(
                name: "ServicioId",
                table: "Resenias");

            migrationBuilder.DropColumn(
                name: "UsuarioClienteIdUsuario",
                table: "Resenias");


            migrationBuilder.CreateIndex(
                name: "IX_Resenias_IdServicio",
                table: "Resenias",
                column: "IdServicio");

            migrationBuilder.CreateIndex(
                name: "IX_Resenias_IdUsuario",
                table: "Resenias",
                column: "IdUsuario");


            migrationBuilder.AddForeignKey(
                name: "FK_Resenias_Servicios_IdServicio",
                table: "Resenias",
                column: "IdServicio",
                principalTable: "Servicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Resenias_Usuarios_IdUsuario",
                table: "Resenias",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropForeignKey(
                name: "FK_Resenias_Servicios_IdServicio",
                table: "Resenias");

            migrationBuilder.DropForeignKey(
                name: "FK_Resenias_Usuarios_IdUsuario",
                table: "Resenias");

            migrationBuilder.DropIndex(
                name: "IX_Resenias_IdServicio",
                table: "Resenias");

            migrationBuilder.DropIndex(
                name: "IX_Resenias_IdUsuario",
                table: "Resenias");


            migrationBuilder.AddColumn<int>(
                name: "ServicioId",
                table: "Resenias",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioClienteIdUsuario",
                table: "Resenias",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resenias_ServicioId",
                table: "Resenias",
                column: "ServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_Resenias_UsuarioClienteIdUsuario",
                table: "Resenias",
                column: "UsuarioClienteIdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Resenias_Servicios_ServicioId",
                table: "Resenias",
                column: "ServicioId",
                principalTable: "Servicios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Resenias_Usuarios_UsuarioClienteIdUsuario",
                table: "Resenias",
                column: "UsuarioClienteIdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario");
        }
    }
}
