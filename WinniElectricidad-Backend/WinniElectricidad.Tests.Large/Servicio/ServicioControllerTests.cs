using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WinniElectricidad.AccesoDatos.Repositorios.EF;
using WinniElectricidad.Compartido.DTOs.Servicios;
using WinniElectricidad.LogicaNegocio.Entidades;
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
            new LogicaNegocio.Entidades.Servicio { Titulo = "Electricidad", Descripcion = "Descripción de prueba.", Imagenes = new List<ServicioImagen>(), Activo = true },
            new LogicaNegocio.Entidades.Servicio { Titulo = "Sanitaria",   Descripcion = "Descripción de prueba.", Imagenes = new List<ServicioImagen>(), Activo = true },
            new LogicaNegocio.Entidades.Servicio { Titulo = "Riego",       Descripcion = "Descripción de prueba.", Imagenes = new List<ServicioImagen>(), Activo = false },
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
            new LogicaNegocio.Entidades.Servicio { Titulo = "Electricidad", Descripcion = "Descripción de prueba.", Imagenes = new List<ServicioImagen>(), Activo = true },
            new LogicaNegocio.Entidades.Servicio { Titulo = "Sanitaria",   Descripcion = "Descripción de prueba.", Imagenes = new List<ServicioImagen>(), Activo = true },
            new LogicaNegocio.Entidades.Servicio { Titulo = "Riego",       Descripcion = "Descripción de prueba.", Imagenes = new List<ServicioImagen>(), Activo = false },
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
                null, 
                "icono")
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
                null, 
                "icono")
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
                    null, 
                    "icono")
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
    
    [Test]
    public async Task CrearServicio_TokenAdminValido_DatosValidos_DeberiaCrearServicio()
    {
        // Arrange
        await DbSeeder.CleanDatabaseAsync(Factory);

        var token = Jwt.GenerarTokenConRol(1, "admin@test.com", "Administrador");
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var form = new MultipartFormDataContent
        {
            { new StringContent("Electricidad"), "Titulo" },
            { new StringContent("Servicio eléctrico general"), "Descripcion" },
            { new StringContent("Otro"), "Icono" }
        };

        // imagen fake
        var imageContent = new ByteArrayContent(new byte[] { 1, 2, 3 });
        imageContent.Headers.ContentType =
            new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");

        form.Add(imageContent, "imagenes", "test.png");

        // Act
        var resp = await Client.PostAsync(EndpointBase, form);

        // Assert
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var servicio = await resp.Content.ReadFromJsonAsync<ServicioDto>();

        Assert.Multiple(() =>
        {
            Assert.That(servicio, Is.Not.Null);
            Assert.That(servicio!.Titulo, Is.EqualTo("Electricidad"));
            Assert.That(servicio.Descripcion, Is.EqualTo("Servicio eléctrico general"));
            Assert.That(servicio.Imagenes, Is.Not.Null);
            Assert.That(servicio.Imagenes, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public async Task EditarServicio_TokenAdminValido_DatosValidos_DeberiaEditarServicio()
    {
        // Arrange
        await DbSeeder.CleanDatabaseAsync(Factory);

        // Seed servicio inicial
        await DbSeeder.SeedServiciosAsync(Factory, new[]
        {
            new LogicaNegocio.Entidades.Servicio
            {
                Titulo = "Electricidad",
                Descripcion = "Desc original",
                Activo = true,
                Imagenes = new List<LogicaNegocio.Entidades.ServicioImagen>()
            }
        });

        var token = Jwt.GenerarTokenConRol(1, "admin@test.com", "Administrador");
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var form = new MultipartFormDataContent
        {
            { new StringContent("Electricidad Editada"), "Titulo" },
            { new StringContent("Descripción editada"), "Descripcion" },
            { new StringContent(""), "UrlPrincipalExistente" }
        };

        var nuevaImagen = new ByteArrayContent(new byte[] { 9, 9, 9 });
        nuevaImagen.Headers.ContentType =
            new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");

        form.Add(nuevaImagen, "imagenes", "nueva.png");
        
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider
            .GetRequiredService<WinniElectricidadContext>();

        var servicioId = await db.Servicios.Select(id => id.Id).FirstAsync();
        
        var endpoint = EndpointBase + $"{servicioId}";

        // Act
        var resp = await Client.PutAsync(endpoint, form);
        
        // Assert
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var servicioEditado =
            await resp.Content.ReadFromJsonAsync<ServicioActivoDto>();

        Assert.Multiple(() =>
        {
            Assert.That(servicioEditado, Is.Not.Null);
            Assert.That(servicioEditado!.Titulo, Is.EqualTo("Electricidad Editada"));
            Assert.That(servicioEditado.Descripcion, Is.EqualTo("Descripción editada"));
            Assert.That(servicioEditado.Imagenes, Is.Not.Null);
        });
    }
}