using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using WinniElectricidad.Compartido.DTOs.Direcciones;
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
}