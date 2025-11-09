using Microsoft.EntityFrameworkCore;
using WinniElectricidad.AccesoDatos.Repositorios.EF;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.Tests.Medium.Utils;

namespace WinniElectricidad.Tests.Medium.AccesoDatos;

[TestFixture]
public class RepositorioServiciosTests
{
    [Test]
    public async Task FindAllActive_FiltraSoloActivos()
    {
        // Arrange
        var (context, connection) = await DbContextHelper.CrearContextoSqliteEnMemoria();
        await using (connection)
        await using (context)
        {
            context.Servicios.AddRange(
                new Servicio("Electricidad", "Instalaciones completas", null) { Activo = true },
                new Servicio("Sanitaria", "Servicio sanitario completo", null) { Activo = false },
                new Servicio("Riego", "Sistema de riego automatizado", null) { Activo = true }
            );
            await context.SaveChangesAsync();

            var repo = new RepositorioServicios(context);

            // Act
            var result = await repo.FindAllActive();

            // Assert
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.All(s => s.Activo), Is.True);
        }
    }

    [Test]
    public async Task FindAll_DevuelveTodosLosServicios()
    {
        // Arrange
        var (context, connection) = await DbContextHelper.CrearContextoSqliteEnMemoria();
        await using (connection)
        await using (context)
        {
            context.Servicios.AddRange(
                new Servicio("Electricidad", "Instalaciones completas", null),
                new Servicio("Sanitaria", "Servicio sanitario completo", null)
            );
            await context.SaveChangesAsync();
            
            var repo = new RepositorioServicios(context);

            // Act
            var result = await repo.FindAll();

            // Assert
            Assert.That(result, Has.Count.EqualTo(2));
        }
    }
}