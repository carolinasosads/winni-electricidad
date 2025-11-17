using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using WinniElectricidad.Compartido.DTOs.Reservas;
using WinniElectricidad.Tests.Large.Infraestructura;

namespace WinniElectricidad.Tests.Large.Reserva;

[TestFixture]
[NonParallelizable]
public class ReservaControllerTests : LargeTestBase
{
    private const string EndpointBase = "/WinniElectricidadApi/Reserva/";

    [Test]
    public async Task GetDisponibilidad_TokenValido_SinReservas_DeberiaRetornarDiasYHorasDisponibles()
    {
        // Arrange
        const string endpoint = EndpointBase + "disponibilidad";

        await DbSeeder.CleanDatabaseAsync(Factory);
        var usuario = await DbSeeder.SeedUsuarioConDireccionesAsync(Factory);

        var token = Jwt.GenerarTokenValido(usuario.IdUsuario, usuario.Email, "Cliente");
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var resp = await Client.GetAsync(endpoint);

        // Assert
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dias = await resp.Content.ReadFromJsonAsync<List<DiaDisponibilidadDto>>();

        Assert.That(dias, Is.Not.Null);
        Assert.That(dias!, Is.Not.Empty);
        Assert.That(dias, Has.Count.EqualTo(29));

        var primerDiaEsperado = DateTime.Today.AddDays(2).Date;
        Assert.That(dias.First().Fecha.Date, Is.EqualTo(primerDiaEsperado));

        foreach (var dia in dias)
        {
            Assert.That(dia.Horas, Is.Not.Null);

            Assert.Multiple(() =>
            {
                // Horario: 9:00 a 17:00 saltando de 90 minutos: 6 slots
                Assert.That(dia.Horas.Count(), Is.EqualTo(6));

                // Todos los horarios deberían estar disponibles
                Assert.That(dia.Horas.All(h => h.Disponible), Is.True);
            });
        }
    }
    
    [Test]
    public async Task GetDisponibilidad_TokenValido_ConReserva_DeberiaMarcarHorarioComoOcupado()
    {
        // Arrange
        const string endpoint = EndpointBase + "disponibilidad";

        await DbSeeder.CleanDatabaseAsync(Factory);
        var usuario = await DbSeeder.SeedUsuarioConDireccionesAsync(Factory);
        var fechaReserva = DateTime.Today.AddDays(3).AddHours(9);
        await DbSeeder.SeedReservaAsync(Factory, fechaReserva);

        var token = Jwt.GenerarTokenValido(usuario.IdUsuario, usuario.Email, "Cliente");
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var resp = await Client.GetAsync(endpoint);

        // Assert
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dias = await resp.Content.ReadFromJsonAsync<List<DiaDisponibilidadDto>>();
        Assert.That(dias, Is.Not.Null);

        var diaReserva = dias!.Single(d => d.Fecha.Date == fechaReserva.Date);
        var horaReserva = TimeOnly.FromDateTime(fechaReserva);

        var slot = diaReserva.Horas.Single(h => h.Hora == horaReserva);

        Assert.Multiple(() =>
        {
            // Ese horario en particular debe aparecer como no disponible
            Assert.That(slot.Disponible, Is.False);

            // Y el resto de los horarios del mismo día siguen disponibles
            Assert.That(diaReserva.Horas
                .Where(h => h.Hora != horaReserva)
                .All(h => h.Disponible), Is.True);
        });
    }
    
    [Test]
    public async Task GetHorariosDisponibles_SinToken_DeberiaRetornarUnauthorized()
    {
        // Arrange
        const string endpoint = EndpointBase + "disponibilidad";

        // Act
        var resp = await Client.GetAsync(endpoint);

        // Assert
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GetHorariosDisponibles_RolIncorrecto_DeberiaRetornarForbidden()
    {
        // Arrange
        const string endpoint = EndpointBase + "disponibilidad";
        await DbSeeder.CleanDatabaseAsync(Factory);
        var usuario = await DbSeeder.SeedUsuarioConDireccionesAsync(Factory);
        var token = Jwt.GenerarTokenConRol(usuario.IdUsuario, usuario.Email, "Administrador");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var resp = await Client.GetAsync(endpoint);

        // Assert
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }
}