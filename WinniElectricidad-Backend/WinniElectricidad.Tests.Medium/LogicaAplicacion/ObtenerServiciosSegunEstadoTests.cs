using WinniElectricidad.AccesoDatos.Repositorios.EF;
using WinniElectricidad.LogicaAplicacion.Servicios.Servicio;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.Tests.Medium.Utils;

namespace WinniElectricidad.Tests.Medium.LogicaAplicacion;

[TestFixture]
public class ObtenerServiciosSegunEstadoTests
{
    [Test]
    public async Task Ejecutar_DevuelveSoloServiciosActivosMapeados()
    {
        // Arrange
        var (context, connection) = await DbContextHelper.CrearContextoSqliteEnMemoria();
        await using (connection)
        await using (context)
        {
            context.Servicios.AddRange(
                new Servicio("Electricidad", "Instalaciones eléctricas completas", null),
                new Servicio("Sanitaria", "Servicio sanitario completo", null) { Activo = false }
            );
            await context.SaveChangesAsync();

            var repo = new RepositorioServicios(context);
            var obtener = new ObtenerServiciosSegunEstado(repo);

            // Act
            var result = await obtener.Ejecutar(true);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Titulo, Is.EqualTo("Electricidad"));
        }
    }
    
    [Test]
    public async Task Ejecutar_DevuelveTodosLosServicios_SiActivoEsFalse()
    {
        // Arrange
        var (context, connection) = await DbContextHelper.CrearContextoSqliteEnMemoria();
        await using (connection)
        await using (context)
        {
            context.Servicios.AddRange(
                new Servicio("Electricidad", "Instalaciones eléctricas completas", null),
                new Servicio("Sanitaria", "Servicio sanitario completo", null) { Activo = false }
            );
            await context.SaveChangesAsync();

            var repo = new RepositorioServicios(context);
            var obtener = new ObtenerServiciosSegunEstado(repo);

            // Act
            var result = await obtener.Ejecutar(false);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
        }
    }
}