using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WinniElectricidad.AccesoDatos.Repositorios.EF;
using Direccion = WinniElectricidad.LogicaNegocio.Entidades.Direccion;
using UsuarioCliente = WinniElectricidad.LogicaNegocio.Entidades.UsuarioCliente;

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
    
    public static async Task<UsuarioCliente> SeedUsuarioConDireccionesAsync(
        ApiTestFactory factory,
        string nombre = "Sofía",
        string passwordHash = "1234567",
        string email = "sofia@test.com",
        string telefono = "1234567")
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WinniElectricidadContext>();

        var existente = await db.Usuarios
            .OfType<UsuarioCliente>()
            .Include(u => u.Direcciones)
            .FirstOrDefaultAsync(u => u.Email == email);

        if (existente != null)
            return existente;

        var direcciones = new List<Direccion>
        {
            new("Rivera", "Canelones", "1001", null),
            new("Durazno", "Paysandú", "1002", "Apto 2")
        };

        var usuario = new UsuarioCliente(nombre, passwordHash, email, telefono, direcciones);

        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();

        return usuario;
    }
}