using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WinniElectricidad.AccesoDatos.Repositorios.EF;
using WinniElectricidad.Api.Servicios;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Notificacion;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reseña;
using WinniElectricidad.Tests.Large.Mocks;

namespace WinniElectricidad.Tests.Large.Infraestructura;

/// <summary>
/// Fábrica que inicializa la aplicación WinniElectricidad.Api
/// en un entorno de pruebas en memoria, utilizando <c>appsettings.Testing.json</c>.
/// </summary>
/// <remarks>
/// <para>
/// Esta clase configura un entorno aislado para los tests end-to-end:
/// carga las configuraciones desde <c>appsettings.Testing.json</c>,
/// reemplaza la base de datos por una versión SQLite en memoria,
/// y permite registrar dependencias fake (por ejemplo, servicios externos de correo).
/// </para>
/// <para>
/// Todos los tests E2E deberían crear su <see cref="HttpClient"/> a través de esta fábrica.
/// </para>
/// </remarks>
public class ApiTestFactory : WebApplicationFactory<Program>
{
    static ApiTestFactory()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Testing");

        Environment.SetEnvironmentVariable("MercadoPago__AccessToken", "TEST_ACCESS_TOKEN");

        Environment.SetEnvironmentVariable("MercadoPago_AccessToken", "TEST_ACCESS_TOKEN");
        Environment.SetEnvironmentVariable("MERCADOPAGO_ACCESSTOKEN", "TEST_ACCESS_TOKEN");
    }

    private SqliteConnection? _connection;
    public IConfiguration Configuration { get; private set; } = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.Sources.Clear();
            config.AddJsonFile("appsettings.Testing.json", optional: true);
            config.AddEnvironmentVariables();

            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MercadoPago:AccessToken"] = "TEST_ACCESS_TOKEN"
            });

            Configuration = config.Build();
        });

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<WinniElectricidadContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            services.AddDbContext<WinniElectricidadContext>(options =>
                options.UseSqlite(_connection));

            var moderacionDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IModeracionOpenAi));
            var imagenesDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IServicioImagenes));
            var enviarEmailDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IEnviarEmail));

            if (moderacionDescriptor != null)
                services.Remove(moderacionDescriptor);
            if (imagenesDescriptor != null)
                services.Remove(imagenesDescriptor);
            if (enviarEmailDescriptor != null)
                services.Remove(enviarEmailDescriptor);

            services.AddSingleton<IModeracionOpenAi, ModeracionOpenAiFake>();
            services.AddSingleton<IServicioImagenes, ServicioImagenesFake>();
            services.AddSingleton<IEvaluarPuntajeResenia, EvaluarPuntajeReseniaFake>();
            services.AddSingleton<IEnviarEmail, EnviarEmailFake>();

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<WinniElectricidadContext>();
            db.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _connection?.Dispose();
    }
}