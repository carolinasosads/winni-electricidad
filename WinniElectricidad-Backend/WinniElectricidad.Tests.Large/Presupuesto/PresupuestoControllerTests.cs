using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using WinniElectricidad.AccesoDatos.Repositorios.EF;
using WinniElectricidad.Compartido.DTOs.Presupuesto;
using WinniElectricidad.Tests.Large.Infraestructura;

namespace WinniElectricidad.Tests.Large.Presupuestos;

[TestFixture]
[NonParallelizable]
public class PresupuestoControllerTests : LargeTestBase
{
    private const string EndpointBase = "/WinniElectricidadApi/Presupuesto/";

    [SetUp]
    public override void LimpiarHeaders()
    {
        base.LimpiarHeaders();
    }

    [Test]
    public async Task CrearPresupuesto_Admin_AReservaExistente_DeberiaRetornar201YGuardarEnDb()
    {
        // Arrange
        await DbSeeder.CleanDatabaseAsync(Factory);
        var reserva = await DbSeeder.SeedReservaAsync(Factory);
        var endpoint = $"{EndpointBase}reserva/{reserva.IdReserva}";

        var token = Jwt.GenerarTokenValido(idUsuario: 999, email: "admin@test.com", rol: "Administrador");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var dto = new PresupuestoCrearDto
        {
            MontoTotal = 1500,
            NotasInternas = "Cliente pidió presupuesto detallado"
        };

        // Act
        var resp = await Client.PostAsJsonAsync(endpoint, dto);

        // Assert (HTTP)
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var creado = await resp.Content.ReadFromJsonAsync<PresupuestoDto>();
        Assert.That(creado, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(creado!.MontoTotal, Is.EqualTo(dto.MontoTotal));
            Assert.That(creado.IdReserva, Is.EqualTo(reserva.IdReserva));
            Assert.That(creado.Id, Is.GreaterThan(0));
        });

        // Assert (DB)
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WinniElectricidadContext>();

        var presupuestoDb = db.Presupuestos.FirstOrDefault(p => p.IdReserva == reserva.IdReserva);
        Assert.That(presupuestoDb, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(presupuestoDb!.Monto, Is.EqualTo(dto.MontoTotal));
            Assert.That(presupuestoDb.Notas, Is.EqualTo(dto.NotasInternas));
            Assert.That(presupuestoDb.IdUsuario, Is.EqualTo(reserva.IdUsuarioCliente));
        });
    }

    [Test]
    public async Task CrearPresupuesto_SinToken_DeberiaRetornar401()
    {
        // Arrange
        await DbSeeder.CleanDatabaseAsync(Factory);
        var reserva = await DbSeeder.SeedReservaAsync(Factory);
        var endpoint = $"{EndpointBase}reserva/{reserva.IdReserva}";

        var dto = new PresupuestoCrearDto
        {
            MontoTotal = 1000,
            NotasInternas = null
        };

        // Act
        var resp = await Client.PostAsJsonAsync(endpoint, dto);

        // Assert
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task CrearPresupuesto_RolCliente_DeberiaRetornar403()
    {
        // Arrange
        await DbSeeder.CleanDatabaseAsync(Factory);
        var reserva = await DbSeeder.SeedReservaAsync(Factory);
        var endpoint = $"{EndpointBase}reserva/{reserva.IdReserva}";

        var token = Jwt.GenerarTokenValido(
            idUsuario: reserva.IdUsuarioCliente,
            email: "cliente@test.com",
            rol: "Cliente"
        );
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var dto = new PresupuestoCrearDto
        {
            MontoTotal = 1000,
            NotasInternas = null
        };

        // Act
        var resp = await Client.PostAsJsonAsync(endpoint, dto);

        // Assert
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    public async Task CrearPresupuesto_MontoInvalido_DeberiaRetornar400()
    {
        // Arrange
        await DbSeeder.CleanDatabaseAsync(Factory);
        var reserva = await DbSeeder.SeedReservaAsync(Factory);
        var endpoint = $"{EndpointBase}reserva/{reserva.IdReserva}";

        var token = Jwt.GenerarTokenValido(idUsuario: 999, email: "admin@test.com", rol: "Administrador");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var dto = new PresupuestoCrearDto
        {
            MontoTotal = 0,
            NotasInternas = null
        };

        // Act
        var resp = await Client.PostAsJsonAsync(endpoint, dto);

        // Assert
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task CrearPresupuesto_ReservaInexistente_HoyDevuelve500()
    {
        // Arrange
        await DbSeeder.CleanDatabaseAsync(Factory);
        var endpoint = $"{EndpointBase}reserva/999999";

        var token = Jwt.GenerarTokenValido(idUsuario: 999, email: "admin@test.com", rol: "Administrador");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var dto = new PresupuestoCrearDto
        {
            MontoTotal = 1000,
            NotasInternas = null
        };

        // Act
        var resp = await Client.PostAsJsonAsync(endpoint, dto);

        // Assert
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }

    [Test]
    public async Task CrearPresupuesto_DosVecesParaMismaReserva_HoyDevuelve500PorRestriccionUnoAUno()
    {
        // Arrange
        await DbSeeder.CleanDatabaseAsync(Factory);
        var reserva = await DbSeeder.SeedReservaAsync(Factory);
        var endpoint = $"{EndpointBase}reserva/{reserva.IdReserva}";

        var token = Jwt.GenerarTokenValido(idUsuario: 999, email: "admin@test.com", rol: "Administrador");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var dto1 = new PresupuestoCrearDto
        {
            MontoTotal = 1200,
            NotasInternas = null
        };

        var dto2 = new PresupuestoCrearDto
        {
            MontoTotal = 1300,
            NotasInternas = null
        };

        // Act
        var resp1 = await Client.PostAsJsonAsync(endpoint, dto1);
        var resp2 = await Client.PostAsJsonAsync(endpoint, dto2);

        // Assert
        Assert.That(resp1.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        Assert.That(resp2.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }
}
