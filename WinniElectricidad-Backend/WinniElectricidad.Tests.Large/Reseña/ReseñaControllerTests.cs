using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using WinniElectricidad.Compartido.DTOs.Reseñas;
using WinniElectricidad.Tests.Large.Infraestructura;

namespace WinniElectricidad.Tests.Large.Reseña;

[TestFixture]
[NonParallelizable]
public class ReseñaControllerTests : LargeTestBase
{
    private const string EndpointBase = "/WinniElectricidadApi/Reseña";

    [TestCase(false, TestName = "Reseñar_SinImagen_DeberiaCrearReseña")]
    [TestCase(true,  TestName = "Reseñar_ConImagen_DeberiaCrearReseñaYGuardarImagen")]
    public async Task Reseñar_E2E_FlujoCompleto(bool conImagen)
    {
        // Arrange
        const string endpoint = EndpointBase;

        await DbSeeder.CleanDatabaseAsync(Factory);

        var usuario = await DbSeeder.SeedUsuarioConDireccionesAsync(Factory);
        var token = Jwt.GenerarTokenValido(usuario.IdUsuario, usuario.Email, "Cliente");
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        int idServicio;
        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider
                .GetRequiredService<AccesoDatos.Repositorios.EF.WinniElectricidadContext>();

            var servicio = new LogicaNegocio.Entidades.Servicio(
                "Electricidad E2E",
                "Servicio eléctrico E2E",
                null)
            { Activo = true };

            db.Servicios.Add(servicio);
            await db.SaveChangesAsync();
            idServicio = servicio.Id;
        }

        var form = new MultipartFormDataContent
        {
            { new StringContent("Reseña parametrizada E2E", Encoding.UTF8), "Descripcion" },
            { new StringContent("5"), "Calificacion" },
            { new StringContent(idServicio.ToString()), "IdServicio" }
        };

        if (conImagen)
        {
            var imagenBytes = "fake image content"u8.ToArray();
            var imagenContent = new ByteArrayContent(imagenBytes);
            imagenContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

            form.Add(imagenContent, "imagen", "foto-e2e.jpg");
        }

        // Act
        var resp = await Client.PostAsync(endpoint, form);

        // Assert
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var dto = await resp.Content.ReadFromJsonAsync<ReseñaCreadaDto>();
        Assert.That(dto, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(dto!.IdReseña, Is.GreaterThan(0));
            Assert.That(dto.Descripcion, Is.EqualTo("Reseña parametrizada E2E"));
            Assert.That(dto.Calificacion, Is.EqualTo(5));
            Assert.That(dto.Servicio.Id, Is.EqualTo(idServicio));
            Assert.That(dto.Cliente.IdUsuario, Is.EqualTo(usuario.IdUsuario));

            if (conImagen)
            {
                Assert.That(dto.ImagenUrl, Is.Not.Null);
                Assert.That(dto.ImagenUrl, Does.StartWith("/resenias/"));
            }
            else
            {
                Assert.That(dto.ImagenUrl, Is.Null);
            }
        });

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider
                .GetRequiredService<AccesoDatos.Repositorios.EF.WinniElectricidadContext>();

            var reseñaEnDb = db.Reseñas.Single(r => r.Id == dto!.IdReseña);

            Assert.Multiple(() =>
            {
                Assert.That(reseñaEnDb.IdUsuario, Is.EqualTo(usuario.IdUsuario));
                Assert.That(reseñaEnDb.IdServicio, Is.EqualTo(idServicio));
            });

            if (conImagen)
                Assert.That(reseñaEnDb.ImagenUrl, Is.EqualTo(dto.ImagenUrl));
            else
                Assert.That(reseñaEnDb.ImagenUrl, Is.Null);
        }

        // Assert solo con imagen
        if (conImagen)
        {
            using var scope = Factory.Services.CreateScope();
            var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

            var relative = dto!.ImagenUrl!.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var webRoot = env.WebRootPath ?? "wwwroot";
            var path = Path.Combine(webRoot, relative);

            Assert.That(File.Exists(path), Is.True,
                $"Se esperaba que el archivo exista en: {path}");
        }
    }
}