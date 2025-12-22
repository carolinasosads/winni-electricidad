using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using WinniElectricidad.Compartido.DTOs.Servicios;
using WinniElectricidad.Tests.Large.Infraestructura;

namespace WinniElectricidad.Tests.Large.Servicio;

public class ServicioControllerTests : LargeTestBase
{
    private const string EndpointBase = "/WinniElectricidadApi/Servicio/";
    
    [Test]
    public async Task GetServiciosActivos_DeberiaRetornarSoloActivos()
    {
        // Arrange
        const string endpoint = EndpointBase + "activos";

        await DbSeeder.CleanDatabaseAsync(Factory);
        await DbSeeder.SeedServiciosAsync(Factory, new[]
        {
            new LogicaNegocio.Entidades.Servicio { Titulo = "Electricidad", Descripcion = "Descripción de prueba.", ImagenUrl = null, Activo = true },
            new LogicaNegocio.Entidades.Servicio { Titulo = "Sanitaria",   Descripcion = "Descripción de prueba.", ImagenUrl = null, Activo = true },
            new LogicaNegocio.Entidades.Servicio { Titulo = "Riego",       Descripcion = "Descripción de prueba.", ImagenUrl = null, Activo = false },
        });

        // Act
        var resp = await Client.GetAsync(endpoint);

        // Assert
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var servicios = await resp.Content.ReadFromJsonAsync<List<ServicioActivoDto>>();
        Assert.That(servicios, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(servicios, Has.Count.EqualTo(2));
            Assert.That(servicios.Any(s => s.Titulo == "Riego"), Is.False);
        });
    }
    
    [Test]
    public async Task GetServicios_DeberiaRetornarTodosLosServicios()
    {
        // Arrange
        const string endpoint = EndpointBase;
        var usuario = await DbSeeder.SeedUsuarioConDireccionesAsync(Factory);

        var token = Jwt.GenerarTokenValido(usuario.IdUsuario, usuario.Email, "Administrador");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        await DbSeeder.CleanDatabaseAsync(Factory);
        await DbSeeder.SeedServiciosAsync(Factory, new[]
        {
            new LogicaNegocio.Entidades.Servicio { Titulo = "Electricidad", Descripcion = "Descripción de prueba.", ImagenUrl = null, Activo = true },
            new LogicaNegocio.Entidades.Servicio { Titulo = "Sanitaria",   Descripcion = "Descripción de prueba.", ImagenUrl = null, Activo = true },
            new LogicaNegocio.Entidades.Servicio { Titulo = "Riego",       Descripcion = "Descripción de prueba.", ImagenUrl = null, Activo = false },
        });

        // Act
        var resp = await Client.GetAsync(endpoint);

        // Assert
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var servicios = await resp.Content.ReadFromJsonAsync<List<ServicioActivoDto>>();
        Assert.That(servicios, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(servicios, Has.Count.EqualTo(3));
            Assert.That(servicios.Any(s => s.Titulo == "Riego"), Is.True);
        });
    }
    
    [Test]
    public async Task DesactivarServicio_AdminAutenticado_DeberiaCambiarActivoAFalso()
    {
        // Arrange
        await DbSeeder.CleanDatabaseAsync(Factory);

        int idServicio;

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider
                .GetRequiredService<AccesoDatos.Repositorios.EF.WinniElectricidadContext>();

            var servicio = new LogicaNegocio.Entidades.Servicio(
                "Electricidad",
                "Servicio de prueba",
                null)
            { Activo = true };

            db.Servicios.Add(servicio);
            await db.SaveChangesAsync();

            idServicio = servicio.Id;
        }

        var token = Jwt.GenerarTokenValido(1, "admin@test.com", "Administrador");
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var endpoint = EndpointBase + $"desactivar/{idServicio}";

        // Act
        var resp = await Client.PatchAsync(endpoint, null);

        // Assert
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider
                .GetRequiredService<AccesoDatos.Repositorios.EF.WinniElectricidadContext>();

            var servicioDb = db.Servicios.Single(s => s.Id == idServicio);

            Assert.That(servicioDb.Activo, Is.False);
        }
    }

    [Test]
    public async Task ActivarServicio_AdminAutenticado_DeberiaCambiarActivoATrue()
    {
        // Arrange
        await DbSeeder.CleanDatabaseAsync(Factory);

        int idServicio;

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider
                .GetRequiredService<AccesoDatos.Repositorios.EF.WinniElectricidadContext>();

            var servicio = new LogicaNegocio.Entidades.Servicio(
                "Sanitaria",
                "Servicio de prueba",
                null)
            { Activo = false };

            db.Servicios.Add(servicio);
            await db.SaveChangesAsync();

            idServicio = servicio.Id;
        }

        var token = Jwt.GenerarTokenValido(1, "admin@test.com", "Administrador");
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var endpoint = EndpointBase + $"activar/{idServicio}";

        // Act
        var resp = await Client.PatchAsync(endpoint, null);

        // Assert
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider
                .GetRequiredService<AccesoDatos.Repositorios.EF.WinniElectricidadContext>();

            var servicioDb = db.Servicios.Single(s => s.Id == idServicio);

            Assert.That(servicioDb.Activo, Is.True);
        }
    }
    
    [Test]
    public async Task DesactivarServicio_LuegoNoDebeAparecerEnServiciosActivos()
    {
        // Arrange
        await DbSeeder.CleanDatabaseAsync(Factory);

        int idServicio;

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider
                .GetRequiredService<AccesoDatos.Repositorios.EF.WinniElectricidadContext>();

            var servicio = new LogicaNegocio.Entidades.Servicio(
                    "Electricidad",
                    "Servicio activo inicialmente",
                    null)
                { Activo = true };

            db.Servicios.Add(servicio);
            await db.SaveChangesAsync();

            idServicio = servicio.Id;
        }

        var tokenAdmin = Jwt.GenerarTokenValido(1, "admin@test.com", "Administrador");
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenAdmin);

        // Act 1: desactivar
        var respDesactivar =
            await Client.PatchAsync(EndpointBase + $"desactivar/{idServicio}", null);

        // Assert 1
        Assert.That(respDesactivar.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        // Act 2: obtener servicios activos (endpoint público)
        Client.DefaultRequestHeaders.Authorization = null;

        var respActivos = await Client.GetAsync(EndpointBase + "activos");

        // Assert 2
        Assert.That(respActivos.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var serviciosActivos =
            await respActivos.Content.ReadFromJsonAsync<List<ServicioActivoDto>>();

        Assert.That(serviciosActivos, Is.Not.Null);
        Assert.That(serviciosActivos!.Any(s => s.Id == idServicio), Is.False);
    }
}