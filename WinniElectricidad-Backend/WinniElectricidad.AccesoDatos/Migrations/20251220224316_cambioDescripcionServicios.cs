using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WinniElectricidad.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class cambioDescripcionServicios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 1,
                column: "Descripcion",
                value: "Instalaciones, reparaciones y mantenimiento eléctrico en hogares y comercios.");

            migrationBuilder.UpdateData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 2,
                column: "Descripcion",
                value: "Instalación y reparación de cañerías, griferías y artefactos sanitarios.");

            migrationBuilder.UpdateData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 3,
                column: "Descripcion",
                value: "Instalación y mantenimiento de sistemas de aire acondicionado y calefacción.");

            migrationBuilder.UpdateData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 4,
                column: "Descripcion",
                value: "Instalación y mantenimiento de sistemas de riego para jardines.");

            migrationBuilder.UpdateData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 5,
                column: "Descripcion",
                value: "Servicios técnicos generales sujetos a evaluación previa.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 1,
                column: "Descripcion",
                value: "Instalaciones, reparaciones y mantenimiento eléctrico en hogares y comercios. Solucionamos fallas, mejoras de seguridad y nuevas conexiones.");

            migrationBuilder.UpdateData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 2,
                column: "Descripcion",
                value: "Reparación e instalación de cañerías, griferías y artefactos sanitarios. Atendemos pérdidas, obstrucciones y trabajos de mantenimiento general.");

            migrationBuilder.UpdateData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 3,
                column: "Descripcion",
                value: "Instalación, mantenimiento y reparación de sistemas de aire acondicionado y calefacción para asegurar confort todo el año.");

            migrationBuilder.UpdateData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 4,
                column: "Descripcion",
                value: "Diseño, instalación y mantenimiento de sistemas de riego para jardines y espacios verdes, optimizando el uso del agua.");

            migrationBuilder.UpdateData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 5,
                column: "Descripcion",
                value: "Trabajos técnicos generales y servicios específicos no contemplados en las categorías principales, sujetos a evaluación previa.");
        }
    }
}
