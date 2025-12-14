using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WinniElectricidad.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class Resenias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Resenias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Calificacion = table.Column<int>(type: "int", nullable: false),
                    FechaPublicacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImagenUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    IdServicio = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    ServicioId = table.Column<int>(type: "int", nullable: true),
                    UsuarioClienteIdUsuario = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resenias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resenias_Servicios_ServicioId",
                        column: x => x.ServicioId,
                        principalTable: "Servicios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Resenias_Usuarios_UsuarioClienteIdUsuario",
                        column: x => x.UsuarioClienteIdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_FechaReserva",
                table: "Reservas",
                column: "FechaReserva",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resenias_ServicioId",
                table: "Resenias",
                column: "ServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_Resenias_UsuarioClienteIdUsuario",
                table: "Resenias",
                column: "UsuarioClienteIdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Resenias");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_FechaReserva",
                table: "Reservas");
        }
    }
}
