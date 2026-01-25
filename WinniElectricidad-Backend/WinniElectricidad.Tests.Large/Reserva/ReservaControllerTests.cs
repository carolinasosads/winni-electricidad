using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WinniElectricidad.Compartido.DTOs.Reservas;
using WinniElectricidad.Compartido.DTOs.Usuarios.Reserva;
using WinniElectricidad.Tests.Large.Infraestructura;

namespace WinniElectricidad.Tests.Large.Reserva;

[TestFixture]
[NonParallelizable]
public class ReservaControllerTests : LargeTestBase
{
    private const string EndpointBase = "/WinniElectricidadApi/Reserva/";

    [Test]
    public async Task GetHistoricoMensual_AdminToken_ConFiltroQueNoMatchea_DeberiaRetornarListaVacia()
    {
        // Arrange
        await DbSeeder.CleanDatabaseAsync(Factory);

        var admin = await DbSeeder.SeedUsuarioConDireccionesAsync(Factory);
        var token = Jwt.GenerarTokenConRol(admin.IdUsuario, admin.Email, "Administrador");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Sembramos al menos 1 reserva en el mes actual (para que sin filtro habría datos)
        var fechaReserva = DateTime.Today.AddDays(3).Date.AddHours(9);
        await DbSeeder.SeedReservaAsync(Factory, fechaReserva);

        var mes = DateTime.Today.Month;
        var anio = DateTime.Today.Year;

        var endpoint = $"{EndpointBase}historico-mensual/{mes}/{anio}?filtro=___no_existe___";

        // Act
        var resp = await Client.GetAsync(endpoint);

        // Assert
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var reservas = await resp.Content.ReadFromJsonAsync<List<HistoricoReservaDto>>();
        Assert.That(reservas, Is.Not.Null);
        Assert.That(reservas!, Is.Empty);
    }

    [Test]
    public async Task GetHistoricoMensual_AdminToken_ConFiltroQueMatcheaPorEmail_DeberiaRetornarResultados()
    {
        // Arrange
        await DbSeeder.CleanDatabaseAsync(Factory);

        var admin = await DbSeeder.SeedUsuarioConDireccionesAsync(Factory);
        var token = Jwt.GenerarTokenConRol(admin.IdUsuario, admin.Email, "Administrador");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Seed: reserva en el mes actual asociada al usuario seed (usamos su email como filtro)
        var fechaReserva = DateTime.Today.AddDays(3).Date.AddHours(9);
        await DbSeeder.SeedReservaAsync(Factory, fechaReserva);

        var mes = DateTime.Today.Month;
        var anio = DateTime.Today.Year;

        var filtro = Uri.EscapeDataString(admin.Email);
        var endpoint = $"{EndpointBase}historico-mensual/{mes}/{anio}?filtro={filtro}";

        // Act
        var resp = await Client.GetAsync(endpoint);

        // Assert
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var reservas = await resp.Content.ReadFromJsonAsync<List<HistoricoReservaDto>>();
        Assert.That(reservas, Is.Not.Null);
        Assert.That(reservas!, Is.Not.Empty);
    }

    [Test]
    public async Task GetHistoricoFinalizadas_AdminToken_ConFiltroQueNoMatchea_DeberiaRetornarListaVacia()
    {
        // Arrange
        await DbSeeder.CleanDatabaseAsync(Factory);

        var admin = await DbSeeder.SeedUsuarioConDireccionesAsync(Factory);
        var token = Jwt.GenerarTokenConRol(admin.IdUsuario, admin.Email, "Administrador");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Seed una reserva con fecha válida (>= 48h) para no romper la validación del dominio
        var fechaFinalizada = DateTime.Today.AddDays(3).Date.AddHours(9);
        await DbSeeder.SeedReservaAsync(Factory, fechaFinalizada);

        var endpoint = $"{EndpointBase}finalizadas?filtro=___no_existe___";

        // Act
        var resp = await Client.GetAsync(endpoint);

        // Assert
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var reservas = await resp.Content.ReadFromJsonAsync<List<HistoricoReservaDto>>();
        Assert.That(reservas, Is.Not.Null);
        Assert.That(reservas!, Is.Empty);
    }

    [Test]
    public async Task GetHistoricoFinalizadas_AdminToken_ConFiltroQueMatcheaPorEmail_DeberiaRetornarResultados()
    {
        // Arrange
        await DbSeeder.CleanDatabaseAsync(Factory);

        var admin = await DbSeeder.SeedUsuarioConDireccionesAsync(Factory);
        var token = Jwt.GenerarTokenConRol(admin.IdUsuario, admin.Email, "Administrador");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // 1) Asegurar un servicio válido (IdServicios debe existir)
        const int servicioIdValido = 999;

        using (var scope = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.CreateScope(Factory.Services))
        {
            var db = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions
                .GetRequiredService<WinniElectricidad.AccesoDatos.Repositorios.EF.WinniElectricidadContext>(scope.ServiceProvider);

            var existe = db.Servicios.Any(s => s.Id == servicioIdValido);
            if (!existe)
            {
                db.Servicios.Add(new WinniElectricidad.LogicaNegocio.Entidades.Servicio
                {
                    Id = servicioIdValido,
                    Titulo = "ServicioTestHistorico_" + Guid.NewGuid().ToString("N"),
                    Descripcion = "Servicio para test de reserva historica",
                    Activo = true,
                    Icono = "Otro"
                });

                db.SaveChanges();
            }
        }

        // 2) Crear una reserva histórica (en el pasado)
        var fechaPasada = DateTime.Today.AddDays(-2).Date.AddHours(9);
        var idDireccion = admin.Direcciones.First().IdDireccion;

        // Intento A: 0
        var respCrear = await PostRegistrarHistoricoAsync(
            admin.IdUsuario,
            fechaPasada,
            idDireccion,
            servicioIdValido,
            tipoEnumInt: 0
        );

        // Si por alguna razón 0 no calza con tu enum, intento B: 1
        if (respCrear.StatusCode == HttpStatusCode.BadRequest)
        {
            respCrear = await PostRegistrarHistoricoAsync(
                admin.IdUsuario,
                fechaPasada,
                idDireccion,
                servicioIdValido,
                tipoEnumInt: 1
            );
        }

        Assert.That(respCrear.StatusCode, Is.EqualTo(HttpStatusCode.OK),
            $"RegistrarHistorico falló. Body: {await respCrear.Content.ReadAsStringAsync()}");

        // 3) Ahora sí: finalizadas con filtro por email debería traer algo
        var filtro = Uri.EscapeDataString(admin.Email);
        var endpoint = $"{EndpointBase}finalizadas?filtro={filtro}";

        var resp = await Client.GetAsync(endpoint);

        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var reservas = await resp.Content.ReadFromJsonAsync<List<HistoricoReservaDto>>();
        Assert.That(reservas, Is.Not.Null);
        Assert.That(reservas!, Is.Not.Empty);

        // ---------------- local helper ----------------
        async Task<HttpResponseMessage> PostRegistrarHistoricoAsync(
            int idUsuario,
            DateTime fecha,
            int idDir,
            int idServicio,
            int tipoEnumInt)
        {
            // Enviar tipoServicio como INT (enum) para evitar el error:
            // "The JSON value could not be converted to ... TipoServicioReserva"
            var json = $$"""
                       {
                         "fechaReserva": "{{fecha:O}}",
                         "idDireccion": {{idDir}},
                         "idServicios": [{{idServicio}}],
                         "tipoServicio": {{tipoEnumInt}},
                         "comentario": "seed historico finalizadas"
                       }
                       """;

            var postEndpoint = $"{EndpointBase}admin/registrar-historico/{idUsuario}";
            return await Client.PostAsync(
                postEndpoint,
                new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json")
            );
        }
    }

    [Test]
    public async Task GetPorEstado_AdminToken_EstadoInvalido_DeberiaRetornarBadRequest()
    {
        // Arrange
        await DbSeeder.CleanDatabaseAsync(Factory);

        var admin = await DbSeeder.SeedUsuarioConDireccionesAsync(Factory);
        var token = Jwt.GenerarTokenConRol(admin.IdUsuario, admin.Email, "Administrador");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // estado inválido -> debe dar 400 por el TryParse del caso de uso
        var endpoint = $"{EndpointBase}por-estado?estado=ESTADO_QUE_NO_EXISTE";

        // Act
        var resp = await Client.GetAsync(endpoint);

        // Assert
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task GetPorEstado_AdminToken_ConFiltroQueNoMatchea_DeberiaRetornarListaVacia()
    {
        // Arrange
        await DbSeeder.CleanDatabaseAsync(Factory);

        var admin = await DbSeeder.SeedUsuarioConDireccionesAsync(Factory);
        var token = Jwt.GenerarTokenConRol(admin.IdUsuario, admin.Email, "Administrador");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Seed una reserva (no importa el estado para validar filtro: si no matchea, debe ser empty)
        var fecha = DateTime.Today.AddDays(3).Date.AddHours(9);
        await DbSeeder.SeedReservaAsync(Factory, fecha);

        // OJO: usa un estado real de tu enum (ej: Pendiente / Confirmada / Finalizada / Cancelada)
        var endpoint = $"{EndpointBase}por-estado?estado=Pendiente&filtro=___no_existe___";

        // Act
        var resp = await Client.GetAsync(endpoint);

        // Assert
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var reservas = await resp.Content.ReadFromJsonAsync<List<HistoricoReservaDto>>();
        Assert.That(reservas, Is.Not.Null);
        Assert.That(reservas!, Is.Empty);
    }

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

        var primerDiaEsperado = DateTime.Today.AddDays(2).Date;

        if (primerDiaEsperado.DayOfWeek == DayOfWeek.Sunday)
        {
            primerDiaEsperado = primerDiaEsperado.AddDays(1);
        }

        Assert.That(dias.First().Fecha.Date, Is.EqualTo(primerDiaEsperado));

        foreach (var dia in dias)
        {
            Assert.That(dia.Horas, Is.Not.Null);
            if (dia.Fecha.DayOfWeek != DayOfWeek.Sunday)
            {
                Assert.Multiple(() =>
                {
                    // Horario: 9:00 a 17:00 saltando de 90 minutos: 6 slots
                    Assert.That(dia.Horas.Count(), Is.EqualTo(6));

                    // Todos los horarios deberían estar disponibles
                    Assert.That(dia.Horas.All(h => h.Disponible), Is.True);
                });
            }
        }
    }

    [Test]
    public async Task GetDisponibilidad_TokenValido_ConReserva_DeberiaMarcarHorarioComoOcupado()
    {
        // Arrange
        const string endpoint = EndpointBase + "disponibilidad";

        await DbSeeder.CleanDatabaseAsync(Factory);
        var usuario = await DbSeeder.SeedUsuarioConDireccionesAsync(Factory);

        // ====== Fecha válida: no domingo ======
        var fechaReserva = DateTime.Today.AddDays(3);

        // Si cae en domingo, la corro al siguiente día hábil
        while (fechaReserva.DayOfWeek == DayOfWeek.Sunday)
        {
            fechaReserva = fechaReserva.AddDays(1);
        }

        // Le agrego la hora deseada (09:00)
        fechaReserva = fechaReserva.Date.AddHours(9);

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

    [Test]
    public async Task GetMisReservas_TokenValido_DeberiaRetornarListaValida()
    {
        // Arrange
        const string endpoint = EndpointBase + "mis-reservas";

        await DbSeeder.CleanDatabaseAsync(Factory);

        var usuario = await DbSeeder.SeedUsuarioConDireccionesAsync(Factory);

        var fechaReserva = DateTime.Today.AddDays(3);
        while (fechaReserva.DayOfWeek == DayOfWeek.Sunday)
            fechaReserva = fechaReserva.AddDays(1);
        fechaReserva = fechaReserva.Date.AddHours(9);

        // Usamos el seeder existente (NO toca usuario)
        await DbSeeder.SeedReservaAsync(Factory, fechaReserva);

        var token = Jwt.GenerarTokenValido(usuario.IdUsuario, usuario.Email, "Cliente");
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var resp = await Client.GetAsync(endpoint);

        // Assert
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var reservas = await resp.Content.ReadFromJsonAsync<List<ReservaListadoDto>>();

        Assert.That(reservas, Is.Not.Null);
    }

    [Test]
    public async Task GetMisReservas_SinToken_DeberiaRetornarUnauthorized()
    {
        // Arrange
        const string endpoint = EndpointBase + "mis-reservas";

        Client.DefaultRequestHeaders.Authorization = null;

        // Act
        var resp = await Client.GetAsync(endpoint);

        // Assert
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GetMisReservas_RolIncorrecto_DeberiaRetornarForbidden()
    {
        // Arrange
        const string endpoint = EndpointBase + "mis-reservas";

        await DbSeeder.CleanDatabaseAsync(Factory);
        var usuario = await DbSeeder.SeedUsuarioConDireccionesAsync(Factory);

        var token = Jwt.GenerarTokenConRol(usuario.IdUsuario, usuario.Email, "Administrador");
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var resp = await Client.GetAsync(endpoint);

        // Assert
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }
}
