using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WinniElectricidad.AccesoDatos.Repositorios.EF;

namespace WinniElectricidad.Tests.Large.Infraestructura;

/// <summary>
/// Fábrica que inicializa la aplicación WinniElectricidad.Api
/// en un entorno de pruebas en memoria, utilizando <c>appsettings.Test.json</c>.
/// </summary>
/// <remarks>
/// <para>
/// Esta clase configura un entorno aislado para los tests end-to-end:
/// carga las configuraciones desde <c>appsettings.Test.json</c>,
/// reemplaza la base de datos por una versión SQLite en memoria,
/// y permite registrar dependencias fake (por ejemplo, servicios externos de correo).
/// </para>
/// <para>
/// Todos los tests E2E deberían crear su <see cref="HttpClient"/> a través de esta fábrica.
/// </para>
/// </remarks>
public class ApiTestFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddJsonFile("appsettings.Test.json", optional: false);
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