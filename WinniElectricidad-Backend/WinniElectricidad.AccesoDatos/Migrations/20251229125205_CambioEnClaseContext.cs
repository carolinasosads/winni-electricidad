using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WinniElectricidad.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class CambioEnClaseContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reservas_FechaReserva",
                table: "Reservas");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_FechaReserva",
                table: "Reservas",
                column: "FechaReserva",
                unique: true,
                filter: "[EstadoReserva] <> 2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reservas_FechaReserva",
                table: "Reservas");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_FechaReserva",
                table: "Reservas",
                column: "FechaReserva",
                unique: true);
        }
    }
}
