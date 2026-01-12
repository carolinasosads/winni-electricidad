using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WinniElectricidad.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class Seed_ServicioImagenes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            SET IDENTITY_INSERT ServicioImagenes ON;

            -- ==================== ELECTRICIDAD (ServicioId = 1) ====================
            IF NOT EXISTS (SELECT 1 FROM ServicioImagenes WHERE Id = 1)
                INSERT INTO ServicioImagenes (Id, Url, EsPrincipal, ServicioId)
                VALUES (1, 'https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/electricidad.jpg', 1, 1);

            IF NOT EXISTS (SELECT 1 FROM ServicioImagenes WHERE Id = 6)
                INSERT INTO ServicioImagenes (Id, Url, EsPrincipal, ServicioId)
                VALUES (6, 'https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/electricidad6.jpg', 0, 1);

            IF NOT EXISTS (SELECT 1 FROM ServicioImagenes WHERE Id = 7)
                INSERT INTO ServicioImagenes (Id, Url, EsPrincipal, ServicioId)
                VALUES (7, 'https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/electricidad7.jpg', 0, 1);

            IF NOT EXISTS (SELECT 1 FROM ServicioImagenes WHERE Id = 8)
                INSERT INTO ServicioImagenes (Id, Url, EsPrincipal, ServicioId)
                VALUES (8, 'https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/electricidad8.jpg', 0, 1);

            -- ==================== SANITARIA (ServicioId = 2) ====================
            IF NOT EXISTS (SELECT 1 FROM ServicioImagenes WHERE Id = 2)
                INSERT INTO ServicioImagenes (Id, Url, EsPrincipal, ServicioId)
                VALUES (2, 'https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/sanitaria.jpg', 1, 2);

            IF NOT EXISTS (SELECT 1 FROM ServicioImagenes WHERE Id = 10)
                INSERT INTO ServicioImagenes (Id, Url, EsPrincipal, ServicioId)
                VALUES (10, 'https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/sanitaria2.jpg', 0, 2);

            IF NOT EXISTS (SELECT 1 FROM ServicioImagenes WHERE Id = 11)
                INSERT INTO ServicioImagenes (Id, Url, EsPrincipal, ServicioId)
                VALUES (11, 'https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/sanitaria3.jpg', 0, 2);

            IF NOT EXISTS (SELECT 1 FROM ServicioImagenes WHERE Id = 12)
                INSERT INTO ServicioImagenes (Id, Url, EsPrincipal, ServicioId)
                VALUES (12, 'https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/sanitaria4.jpg', 0, 2);

            IF NOT EXISTS (SELECT 1 FROM ServicioImagenes WHERE Id = 13)
                INSERT INTO ServicioImagenes (Id, Url, EsPrincipal, ServicioId)
                VALUES (13, 'https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/sanitaria5.jpg', 0, 2);

            -- ==================== CLIMATIZACIÓN (ServicioId = 3) ====================
            IF NOT EXISTS (SELECT 1 FROM ServicioImagenes WHERE Id = 3)
                INSERT INTO ServicioImagenes (Id, Url, EsPrincipal, ServicioId)
                VALUES (3, 'https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/climatizacion.jpg', 1, 3);

            IF NOT EXISTS (SELECT 1 FROM ServicioImagenes WHERE Id = 15)
                INSERT INTO ServicioImagenes (Id, Url, EsPrincipal, ServicioId)
                VALUES (15, 'https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/climatizacion2.jpg', 0, 3);

            IF NOT EXISTS (SELECT 1 FROM ServicioImagenes WHERE Id = 16)
                INSERT INTO ServicioImagenes (Id, Url, EsPrincipal, ServicioId)
                VALUES (16, 'https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/climatizacion3.jpg', 0, 3);

            -- ==================== RIEGO (ServicioId = 4) ====================
            IF NOT EXISTS (SELECT 1 FROM ServicioImagenes WHERE Id = 4)
                INSERT INTO ServicioImagenes (Id, Url, EsPrincipal, ServicioId)
                VALUES (4, 'https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/riego.jpg', 1, 4);

            IF NOT EXISTS (SELECT 1 FROM ServicioImagenes WHERE Id = 18)
                INSERT INTO ServicioImagenes (Id, Url, EsPrincipal, ServicioId)
                VALUES (18, 'https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/riego2.jpg', 0, 4);

            -- ==================== OTROS (ServicioId = 5) ====================
            IF NOT EXISTS (SELECT 1 FROM ServicioImagenes WHERE Id = 5)
                INSERT INTO ServicioImagenes (Id, Url, EsPrincipal, ServicioId)
                VALUES (5, 'https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/otros.jpg', 1, 5);

            IF NOT EXISTS (SELECT 1 FROM ServicioImagenes WHERE Id = 20)
                INSERT INTO ServicioImagenes (Id, Url, EsPrincipal, ServicioId)
                VALUES (20, 'https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/otros2.jpg', 0, 5);

            SET IDENTITY_INSERT ServicioImagenes OFF;
            ");
        }
    }
}
