using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WinniElectricidad.AccesoDatos.Repositorios.EF;
using Direccion = WinniElectricidad.LogicaNegocio.Entidades.Direccion;
using ServicioImagen = WinniElectricidad.LogicaNegocio.Entidades.ServicioImagen;
using TipoServicioReserva = WinniElectricidad.LogicaNegocio.Entidades.TipoServicioReserva;
using UsuarioCliente = WinniElectricidad.LogicaNegocio.Entidades.UsuarioCliente;

namespace WinniElectricidad.Tests.Large.Infraestructura;

/// <summary>
/// Utilidad para inicializar datos de prueba en la base de datos
/// usada por los tests E2E. Permite limpiar e insertar entidades
/// en el contexto de prueba configurado en <see cref="ApiTestFactory"/>.
/// </summary>
public static class DbSeeder
{
    public static async Task CleanDatabaseAsync(ApiTestFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WinniElectricidadContext>();
        
        db.Resenias.RemoveRange(db.Resenias);
        db.Reservas.RemoveRange(db.Reservas);
        db.Servicios.RemoveRange(db.Servicios);

        await db.SaveChangesAsync();
    }
    
    /// <summary>
    /// Elimina todos los registros de la tabla <see cref="Servicio"/> y agrega los servicios proporcionados.
    /// </summary>
    /// <param name="factory">Instancia de la fábrica de la API usada por los tests.</param>
    /// <param name="servicios">Colección de servicios de prueba a insertar.</param>
    public static async Task SeedServiciosAsync(ApiTestFactory factory, IEnumerable<LogicaNegocio.Entidades.Servicio> servicios)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WinniElectricidadContext>();

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

    /// <summary>
    /// Crea una reserva de prueba para un usuario cliente con direcciones,
    /// utilizando un servicio existente y una fecha válida (>= 48 h y menor a 30 días).
    /// </summary>
    /// <param name="factory">Instancia de la fábrica de la API usada por los tests.</param>
    /// <param name="fechaReserva">
    /// Fecha/hora deseada para la reserva. Si es null, se usa hoy + 3 días a las 9:00.
    /// </param>
    /// <returns>La reserva creada y persistida en la base de datos de pruebas.</returns>
    public static async Task<LogicaNegocio.Entidades.Reserva> SeedReservaAsync(
        ApiTestFactory factory,
        DateTime? fechaReserva = null)
    {
        var usuario = await SeedUsuarioConDireccionesAsync(factory);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WinniElectricidadContext>();

        await SeedServiciosAsync(factory, [
            new LogicaNegocio.Entidades.Servicio { Titulo = "Electricidad", Descripcion = "Descripción de prueba.", Imagenes = new List<ServicioImagen>(), Activo = true }
        ]);
        var servicio = await db.Servicios.FirstAsync();
        
        var fecha = fechaReserva ?? DateTime.Today.AddDays(3).AddHours(9);

        var reserva = new LogicaNegocio.Entidades.Reserva(
            fecha,
            TipoServicioReserva.Instalacion,
            usuario.IdUsuario,
            usuario.Direcciones.First().IdDireccion,
            new List<LogicaNegocio.Entidades.Servicio> { servicio },
            "Reserva de prueba para tests E2E");

        db.Reservas.Add(reserva);
        await db.SaveChangesAsync();

        return reserva;
    }
}