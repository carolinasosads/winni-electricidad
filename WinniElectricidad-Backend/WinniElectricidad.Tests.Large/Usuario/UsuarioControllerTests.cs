using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using WinniElectricidad.AccesoDatos.Repositorios.EF;
using WinniElectricidad.Compartido.DTOs.Direcciones;
using WinniElectricidad.Compartido.DTOs.Usuarios.Login;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Usuario;
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
    public async Task Login_Correcto_DeberiaRetornarToken()
    {
        // Arrange
        const string endpoint = EndpointBase + "login";
        await DbSeeder.CleanDatabaseAsync(Factory);

        var usuario = await DbSeeder.SeedUsuarioConDireccionesAsync(
            Factory,
            email: "login@e2e.com"
        );

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider
                .GetRequiredService<WinniElectricidadContext>();

            var hash = scope.ServiceProvider.GetRequiredService<IServicioHash>();

            usuario.PasswordHash = hash.Hash("123456");
            db.Usuarios.Update(usuario);
            await db.SaveChangesAsync();
        }

        // Act
        var resp = await Client.PostAsJsonAsync(endpoint, new
        {
            Email = "login@e2e.com",
            Password = "123456"
        });

        // Assert
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dto = await resp.Content.ReadFromJsonAsync<UsuarioLogueadoTokenDto>();
        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto!.Token, Is.Not.Empty);
            Assert.That(dto.Email, Is.EqualTo("login@e2e.com"));
        });
    }

    [Test]
    public async Task Login_CredencialesInvalidas_DeberiaRetornar401()
    {
        // Arrange
        const string endpoint = EndpointBase + "login";

        // Act
        var resp = await Client.PostAsJsonAsync(endpoint, new
        {
            Email = "noexiste@test.com",
            Password = "123"
        });

        // Assert
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
    
    [Test]
    public async Task ResetPassword_TokenInvalido_DeberiaRetornar400()
    {
        const string endpoint = EndpointBase + "reset-password";

        var resp = await Client.PostAsJsonAsync(endpoint, new
        {
            Password = "nueva123",
            TokenPlain = "token-invalido"
        });

        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
}