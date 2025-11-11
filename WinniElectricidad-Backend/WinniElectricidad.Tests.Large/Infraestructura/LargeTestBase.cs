using WinniElectricidad.Tests.Large.Utils;

namespace WinniElectricidad.Tests.Large.Infraestructura;

/// <summary>
/// Clase base abstracta para los tests end-to-end (E2E) del sistema Winni Electricidad.
/// </summary>
///
/// <remarks>
/// 
/// Esta clase inicializa un entorno de pruebas real de la API utilizando
/// <see cref="ApiTestFactory"/>, que crea una instancia en memoria de la aplicación web
/// mediante <c>WebApplicationFactory&lt;Program&gt;</c>.
///
/// Todos los tests E2E deben heredar de esta clase para acceder a un <see cref="HttpClient"/>
/// configurado contra la instancia de la API de prueba.
///
/// </remarks>

public class LargeTestBase : IDisposable
{
    /// <summary>
    /// Fábrica que inicializa y configura la instancia de la API para los tests.
    /// </summary>
    protected readonly ApiTestFactory Factory;
    
    /// <summary>
    /// Cliente HTTP utilizado para interactuar con la API durante los tests E2E.
    /// </summary>
    protected readonly HttpClient Client;
    
    /// <summary>
    /// Helper para generar tokens JWT válidos o expirados durante los tests.
    /// </summary>
    protected readonly JwtHelper Jwt;

    protected LargeTestBase()
    {
        Factory = new ApiTestFactory();
        Client = Factory.CreateClient();
        Jwt = new JwtHelper(Factory.Configuration);
    }
    
    [SetUp]
    public virtual void LimpiarHeaders()
    {
        Client.DefaultRequestHeaders.Authorization = null;
    }
    
    public void Dispose()
    {
        Client.Dispose();
        Factory.Dispose();
    }
}