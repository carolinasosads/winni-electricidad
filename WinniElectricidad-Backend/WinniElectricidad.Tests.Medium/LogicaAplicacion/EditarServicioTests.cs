using Microsoft.EntityFrameworkCore;
using WinniElectricidad.AccesoDatos.Repositorios.EF;
using WinniElectricidad.Compartido.DTOs.Servicios;
using WinniElectricidad.LogicaAplicacion.Servicios.Servicio;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.Tests.Medium.Utils;

namespace WinniElectricidad.Tests.Medium.LogicaAplicacion;

[TestFixture]
public class EditarServicioTests
{
    [Test]
    public async Task Ejecutar_FlujoCompleto_EditaServicioYPersiste()
    {
        var (context, connection) = await DbContextHelper.CrearContextoSqliteEnMemoria();
        await using (connection)
        await using (context)
        {
            var servicio = new Servicio(
                "Electricidad",
                "Descripcion original",
                new List<ServicioImagen>(),
                "icono")
            {
                Activo = true
            };

            context.Servicios.Add(servicio);
            await context.SaveChangesAsync();

            var repo = new RepositorioServicios(context);
            var editarServicio = new EditarServicio(repo);

            var dto = new EditarServicioDto
            {
                Titulo = "Electricidad Editada",
                Descripcion = "Desc editada y detallada",
                IndexPrincipalNueva = 0
            };

            var imagenes = new List<string> { "/img/1.jpg" };

            // Act
            var result = await editarServicio.Ejecutar(
                servicio.Id,
                dto,
                imagenes,
                CancellationToken.None);

            // Assert
            Assert.That(result.Titulo, Is.EqualTo("Electricidad Editada"));

            var servicioDb = await context.Servicios
                .Include(s => s.Imagenes)
                .FirstAsync();

            Assert.Multiple(() =>
            {
                Assert.That(servicioDb.Descripcion, Is.EqualTo("Desc editada y detallada"));

                Assert.That(servicioDb.Imagenes, Is.Not.Empty);

                var principal = servicioDb.Imagenes.SingleOrDefault(i => i.EsPrincipal);

                Assert.That(principal, Is.Not.Null);
                Assert.That(principal!.Url, Is.EqualTo("/img/1.jpg"));

                Assert.That(
                    servicioDb.Imagenes
                        .Where(i => i != principal)
                        .All(i => i.EsPrincipal == false),
                    Is.True
                );
            });
        }
    }

    [Test]
    public async Task Ejecutar_ServicioNoExiste_LanzaException()
    {
        var (context, connection) = DbContextHelper
            .CrearContextoSqliteEnMemoria().Result;

        await using (connection)
        await using (context)
        {
            var repo = new RepositorioServicios(context);
            var editarServicio = new EditarServicio(repo);

            Assert.ThrowsAsync<ArgumentException>(() =>
                editarServicio.Ejecutar(
                    999,
                    new EditarServicioDto(),
                    null,
                    CancellationToken.None));
        }
    }
}
