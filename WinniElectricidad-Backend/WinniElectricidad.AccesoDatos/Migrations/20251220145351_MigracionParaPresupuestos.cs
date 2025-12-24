using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WinniElectricidad.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class MigracionParaPresupuestos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescripcionTrabajo",
                table: "Presupuestos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MontoPagado",
                table: "Presupuestos",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescripcionTrabajo",
                table: "Presupuestos");

            migrationBuilder.DropColumn(
                name: "MontoPagado",
                table: "Presupuestos");
        }
    }
}
