using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WinniElectricidad.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class ServicioIcono : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Icono",
                table: "Servicios",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Otro");

            migrationBuilder.UpdateData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 1,
                column: "Icono",
                value: "Electricidad");

            migrationBuilder.UpdateData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 2,
                column: "Icono",
                value: "Sanitaria");

            migrationBuilder.UpdateData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 3,
                column: "Icono",
                value: "Climatización");

            migrationBuilder.UpdateData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 4,
                column: "Icono",
                value: "Riego");

            migrationBuilder.UpdateData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 5,
                column: "Icono",
                value: "Otro");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icono",
                table: "Servicios");
        }
    }
}
