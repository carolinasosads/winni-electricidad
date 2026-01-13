using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WinniElectricidad.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class ReseniasConIAa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PuntajeReseniaIa",
                table: "Resenias",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PuntajeReseniaIa",
                table: "Resenias");
        }
    }
}
