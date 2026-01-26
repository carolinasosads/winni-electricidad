using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using WinniElectricidad.Compartido.DTOs.Recordatorios;
using WinniElectricidad.Tests.Large.Infraestructura;

namespace WinniElectricidad.Tests.Large.Notificacion;

[TestFixture]
[NonParallelizable]
public class NotificacionControllerTests : LargeTestBase
{
    private const string Endpoint =
        "/WinniElectricidadApi/Notificacion/enviar-recordatorio";

    [Test]
    public async Task EnviarRecordatorio_AdminValido_Devuelve200()
    {
        var token = Jwt.GenerarTokenConRol(1, "admin@test.com", "Administrador");
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var dto = new RecordatorioDto
        {
            TituloServicio = "Electricidad",
            Texto = "Texto de prueba",
            EmailClientesParaEnviar = new() { "cliente@test.com" }
        };

        var resp = await Client.PostAsJsonAsync(Endpoint, dto);

        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task EnviarRecordatorio_DatosInvalidos_Devuelve400()
    {
        var token = Jwt.GenerarTokenConRol(1, "admin@test.com", "Administrador");
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var dto = new RecordatorioDto
        {
            TituloServicio = "",
            Texto = "",
            EmailClientesParaEnviar = new()
        };

        var resp = await Client.PostAsJsonAsync(Endpoint, dto);

        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task EnviarRecordatorio_SinToken_Devuelve401()
    {
        Client.DefaultRequestHeaders.Authorization = null;

        var resp = await Client.PostAsync(Endpoint, null);

        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task EnviarRecordatorio_RolIncorrecto_Devuelve403()
    {
        var token = Jwt.GenerarTokenConRol(1, "cliente@test.com", "Cliente");
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var resp = await Client.PostAsync(Endpoint, null);

        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }
}
