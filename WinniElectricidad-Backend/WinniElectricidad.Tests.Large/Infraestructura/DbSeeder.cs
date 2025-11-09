using Microsoft.Extensions.DependencyInjection;
using WinniElectricidad.AccesoDatos.Repositorios.EF;

namespace WinniElectricidad.Tests.Large.Infraestructura;

/// <summary>
/// Utilidad para inicializar datos de prueba en la base de datos
/// usada por los tests E2E. Permite limpiar e insertar entidades
/// en el contexto de prueba configurado en <see cref="ApiTestFactory"/>.
/// </summary>
public static class DbSeeder
{
    /// <summary>
    /// Elimina todos los registros de la tabla <see cref="Servicio"/> y agrega los servicios proporcionados.
    /// </summary>
    /// <param name="factory">Instancia de la fábrica de la API usada por los tests.</param>
    /// <param name="servicios">Colección de servicios de prueba a insertar.</param>
    public static async Task SeedServiciosAsync(ApiTestFactory factory, IEnumerable<LogicaNegocio.Entidades.Servicio> servicios)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WinniElectricidadContext>();

        db.Servicios.RemoveRange(db.Servicios);
        await db.SaveChangesAsync();

        db.Servicios.AddRange(servicios);
        await db.SaveChangesAsync();
    }
}