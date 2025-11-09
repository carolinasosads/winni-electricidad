using System.Net;
using System.Net.Http.Json;
using WinniElectricidad.Compartido.DTOs.Servicios;
using WinniElectricidad.Tests.Large.Infraestructura;

namespace WinniElectricidad.Tests.Large.Servicio;

public class ServicioController : LargeTestBase
{
    private const string EndpointBase = "/WinniElectricidadApi/Servicio/";
    
    [Test]
    public async Task GetServiciosActivos_DeberiaRetornarSoloActivos()
    {
        // ARRANGE
        const string endpoint = EndpointBase + "activos";

        await DbSeeder.SeedServiciosAsync(Factory, new[]
        {
            new LogicaNegocio.Entidades.Servicio { Titulo = "Electricidad", Descripcion = "Descripción de prueba.", ImagenUrl = null, Activo = true },
            new LogicaNegocio.Entidades.Servicio { Titulo = "Sanitaria",   Descripcion = "Descripción de prueba.", ImagenUrl = null, Activo = true },
            new LogicaNegocio.Entidades.Servicio { Titulo = "Riego",       Descripcion = "Descripción de prueba.", ImagenUrl = null, Activo = false },
        });

        // ACT
        var resp = await Client.GetAsync(endpoint);

        // ASSERT
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var servicios = await resp.Content.ReadFromJsonAsync<List<ServicioActivoDto>>();
        Assert.That(servicios, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(servicios, Has.Count.EqualTo(2));
            Assert.That(servicios.Any(s => s.Titulo == "Riego"), Is.False);
        });
    }
}