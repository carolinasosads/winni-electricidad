using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WinniElectricidad.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class Servicio_con_varias_imagenes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagenUrl",
                table: "Servicios");

            migrationBuilder.CreateTable(
                name: "ServicioImagenes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EsPrincipal = table.Column<bool>(type: "bit", nullable: false),
                    ServicioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicioImagenes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServicioImagenes_Servicios_ServicioId",
                        column: x => x.ServicioId,
                        principalTable: "Servicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ServicioImagenes",
                columns: new[] { "Id", "EsPrincipal", "ServicioId", "Url" },
                values: new object[,]
                {
                    { 1, true, 1, "https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/electricidad.jpg" },
                    { 2, true, 2, "https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/sanitaria.jpg" },
                    { 3, true, 3, "https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/climatizacion.jpg" },
                    { 4, true, 4, "https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/riego.jpg" },
                    { 5, true, 5, "https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/otros.jpg" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServicioImagenes_ServicioId",
                table: "ServicioImagenes",
                column: "ServicioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServicioImagenes");

            migrationBuilder.AddColumn<string>(
                name: "ImagenUrl",
                table: "Servicios",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImagenUrl",
                value: "/servicios/electricidad.jpeg");

            migrationBuilder.UpdateData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImagenUrl",
                value: "/servicios/sanitaria.jpeg");

            migrationBuilder.UpdateData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImagenUrl",
                value: "/servicios/climatizacion.jpeg");

            migrationBuilder.UpdateData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImagenUrl",
                value: "/servicios/riego.jpg");

            migrationBuilder.UpdateData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImagenUrl",
                value: "/servicios/otros.jpeg");
        }
    }
}
