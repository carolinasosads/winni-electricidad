using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WinniElectricidad.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class EstadoPago : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstadoPago",
                table: "Pagos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstadoPago",
                table: "Pagos");
        }
    }
}
