using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using WinniElectricidad.Compartido.DTOs.Direcciones;
using WinniElectricidad.Compartido.DTOs.Usuarios.ListadoUsuarios;
using WinniElectricidad.Tests.Large.Infraestructura;

namespace WinniElectricidad.Tests.Large.Usuario;

[TestFixture]
[NonParallelizable]
public class UsuarioControllerTests : LargeTestBase
{
    private const string EndpointBase = "/WinniElectricidadApi/Usuario/";

    [Test]
    public async Task GetDirecciones_UsuarioClienteAutenticado_DeberiaRetornarSusDirecciones()
    {
        // Arrange
        const string endpoint = EndpointBase + "direcciones";
        var usuario = await DbSeeder.SeedUsuarioConDireccionesAsync(Factory);

        var token = Jwt.GenerarTokenValido(usuario.IdUsuario, usuario.Email, "Cliente");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var resp = await Client.GetAsync(endpoint);

        // Assert
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var direcciones = await resp.Content.ReadFromJsonAsync<List<DireccionDetalleDto>>();
        Assert.That(direcciones, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(direcciones, Has.Count.EqualTo(2));
            Assert.That(direcciones!.Any(d => d.Calle == "Rivera"));
            Assert.That(direcciones!.Any(d => d.Calle == "Durazno"));
        });
    }

    [Test]
    public async Task GetDirecciones_SinToken_DeberiaRetornarUnauthorized()
    {
        // Arrange
        const string endpoint = EndpointBase + "direcciones";

        // Act
        var resp = await Client.GetAsync(endpoint);

        // Assert
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GetDirecciones_TokenExpirado_DeberiaRetornarUnauthorized()
    {
        // Arrange
        const string endpoint = EndpointBase + "direcciones";
        var usuario = await DbSeeder.SeedUsuarioConDireccionesAsync(Factory);
        var token = Jwt.GenerarTokenExpirado(usuario.IdUsuario, usuario.Email, "Cliente");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var resp = await Client.GetAsync(endpoint);

        // Assert
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GetDirecciones_RolIncorrecto_DeberiaRetornarForbidden()
    {
        // Arrange
        const string endpoint = EndpointBase + "direcciones";
        var usuario = await DbSeeder.SeedUsuarioConDireccionesAsync(Factory);
        var token = Jwt.GenerarTokenConRol(usuario.IdUsuario, usuario.Email, "Administrador");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var resp = await Client.GetAsync(endpoint);

        // Assert
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }
    
    [Test]
    public async Task GetClientesPorServicio_AdminValido_Devuelve200()
    {
        const string endpoint = EndpointBase + "admin/clientes/servicio/";
        await DbSeeder.CleanDatabaseAsync(Factory);

        var usuario = await DbSeeder.SeedUsuarioConDireccionesAsync(Factory);
        var reserva = await DbSeeder.SeedReservaAsync(Factory);

        var token = Jwt.GenerarTokenConRol(
            usuario.IdUsuario,
            usuario.Email,
            "Administrador");

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var servicioId = reserva.Servicios.First().Id;

        // Act
        var resp = await Client.GetAsync(endpoint + servicioId);

        // Assert
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var clientes =
            await resp.Content.ReadFromJsonAsync<List<ListadoUsuariosDto>>();

        Assert.That(clientes, Is.Not.Null);
        Assert.That(clientes!.Any(c => c.Email == usuario.Email), Is.True);
    }

    [Test]
    public async Task GetClientesPorServicio_ServicioNoExiste_Devuelve400()
    {
        const string endpoint = EndpointBase + "admin/clientes/servicio/";
        var token = Jwt.GenerarTokenConRol(1, "admin@test.com", "Administrador");
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var resp = await Client.GetAsync(endpoint + "999");

        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task GetClientesPorServicio_SinToken_Devuelve401()
    {
        const string endpoint = EndpointBase + "admin/clientes/servicio/";
        Client.DefaultRequestHeaders.Authorization = null;

        var resp = await Client.GetAsync(endpoint + "1");

        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GetClientesPorServicio_RolIncorrecto_Devuelve403()
    {
        const string endpoint = EndpointBase + "admin/clientes/servicio/";
        var token = Jwt.GenerarTokenConRol(1, "cliente@test.com", "Cliente");
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var resp = await Client.GetAsync(endpoint + "1");

        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }
}