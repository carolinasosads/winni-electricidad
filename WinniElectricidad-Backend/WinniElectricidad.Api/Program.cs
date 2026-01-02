using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenAI;
using Resend;
using WinniElectricidad.AccesoDatos.Repositorios.EF;
using WinniElectricidad.Api.Servicios;
using WinniElectricidad.Compartido.Configuracion;
using WinniElectricidad.LogicaAplicacion.CasosDeUso.Reservas;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Notificacion;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Pago;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Presupuesto;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reseña;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Servicio;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Usuario;
using WinniElectricidad.LogicaAplicacion.Servicios.Notificacion;
using WinniElectricidad.LogicaAplicacion.Servicios.Pago;
using WinniElectricidad.LogicaAplicacion.Servicios.Presupuesto;
using WinniElectricidad.LogicaAplicacion.Servicios.Reseña;
using WinniElectricidad.LogicaAplicacion.Servicios.Reserva;
using WinniElectricidad.LogicaAplicacion.Servicios.Servicio;
using WinniElectricidad.LogicaAplicacion.Servicios.Usuario;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<HCaptchaOptions>(
    builder.Configuration.GetSection("HCaptcha"));

//TODO: BORRAR ESTO PARA PRODUCCION
builder.Services.AddCors(o =>
{
    o.AddPolicy("Dev", p =>
        p
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// --- Resend ---
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddOptions();
builder.Services.AddHttpClient<ResendClient>();

builder.Services.Configure<ResendClientOptions>(
    builder.Configuration.GetSection("Resend")
);

builder.Services.AddTransient<IResend, ResendClient>();

// --- OpenAI ---
builder.Services.Configure<OpenAiOptions>(
    builder.Configuration.GetSection("OpenAI")
);

builder.Services.AddSingleton<OpenAIClient>(sp =>
{
    var options = sp.GetRequiredService<IOptions<OpenAiOptions>>().Value;
    return new OpenAIClient(options.ApiKey);
});


// --- Autenticación ---
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = jwtSettings["Key"]
          ?? Environment.GetEnvironmentVariable("JWT_KEY");
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

if (string.IsNullOrEmpty(key))
    throw new InvalidOperationException("JWT Key no configurada. Verificar appsettings o variables de entorno.");

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        };
    });
       

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Winni Electricidad",
        Version = "v1",
        Description = "API del proyecto integrador para gestión de Winni Electricidad."
    });
    
    // Configurar el esquema de seguridad JWT
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Ingrese 'Bearer' seguido de un espacio y el token JWT.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    // Agregar requerimiento de seguridad global
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            []
        }
    });
});

builder.Services.AddDbContext<WinniElectricidadContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ConexionWinniElectricidad")));

// --- Inyección de dependencias ---

// -- Servicios --

// - Usuario -
builder.Services.AddScoped<ILoginUsuario, LoginUsuario>();
builder.Services.AddScoped<IServicioToken, ServicioToken>();
builder.Services.AddScoped<IRegistroUsuario, RegistroUsuario>();
builder.Services.AddHttpClient<IHCaptchaVerifier, HCaptchaServicio>();
builder.Services.AddScoped<IRecuperarContrasena, RecuperarContrasena>();
builder.Services.AddScoped<IServicioOneTimeToken, ServicioOneTimeToken>();
// - Servicio -
builder.Services.AddScoped<IObtenerServiciosSegunEstado, ObtenerServiciosSegunEstado>();
builder.Services.AddScoped<IDesactivarServicio, DesactivarServicio>();
builder.Services.AddScoped<IActivarServicio, ActivarServicio>();
// - Direccion -
builder.Services.AddScoped<IObtenerDirecciones, ObtenerDirecciones>();
// - Reserva -
builder.Services.AddScoped<IObtenerHorariosDisponibles, ObtenerHorariosDisponibles>();
builder.Services.AddScoped<IAgendarReserva, AgendarReserva>();
builder.Services.AddScoped<IObtenerHistoricoMensualReservas, ObtenerHistoricoMensualReservas>();
builder.Services.AddScoped<IObtenerHistoricoFinalizadas, ObtenerHistoricoFinalizadas>();
builder.Services.AddScoped<IObtenerReservasPorEstado, ObtenerReservasPorEstado>();
builder.Services.AddScoped<IAprobarReserva, AprobarReserva>();
builder.Services.AddScoped<ICancelarReserva, CancelarReserva>();
builder.Services.AddScoped<IModificarReserva, ModificarReserva>();
builder.Services.AddScoped<IObtenerMisReservasClienteConDetalle, ObtenerMisReservasClienteConDetalle>();
// - Notificacion -
builder.Services.AddScoped<IEnviarEmail, EnviarEmail>();
builder.Services.AddScoped<IServicioHash, ServicioHash>();
// - Reseña -
builder.Services.AddScoped<IAgregarReseña, AgregarReseña>();
builder.Services.AddScoped<IServicioImagenes, ServicioImagenes>();
builder.Services.AddScoped<IObtenerReseñasAprobadas, ObtenerReseñasAprobadas>();
builder.Services.AddScoped<IDesaprobarReseña, DesaprobarReseña>();
builder.Services.AddScoped<IModeracionOpenAi, ModeracionOpenAi>();
// - Administrador -
builder.Services.AddScoped<ICrearUsuarioDesdeAdmin, CrearUsuarioDesdeAdmin>();
builder.Services.AddScoped<IBuscarUsuarios, BuscarUsuarios>();
builder.Services.AddScoped<IObtenerReservasPorCliente, ObtenerReservasPorCliente>();
builder.Services.AddScoped<ICrearPresupuesto, CrearPresupuesto>();
builder.Services.AddScoped<IRegistrarReservaHistoricaAdmin, RegistrarReservaHistoricaAdmin>();
builder.Services.AddScoped<IListarTodosLosUsuarios, ListarTodosLosUsuarios>();
builder.Services.AddScoped<IObtenerDetalleUsuario, ObtenerDetalleUsuario>();
builder.Services.AddScoped<IObtenerPresupuestoPorReserva, ObtenerPresupuestoPorReserva>();
builder.Services.AddScoped<IObtenerPresupuesto, ObtenerPresupuesto>();
builder.Services.AddScoped<IRegistrarPagoPresupuesto, RegistrarPagoPresupuesto>();

// -- Repositorios --
builder.Services.AddScoped<IRepositorioUsuario, RepositorioUsuarios>();
builder.Services.AddScoped<IRepositorioOneTimeToken, RepositorioOneTimeTokens>();
builder.Services.AddScoped<IRepositorioServicio, RepositorioServicios>();
builder.Services.AddScoped<IRepositorioReserva, RepositorioReservas>();
builder.Services.AddScoped<IRepositorioSettings, RepositorioSettings>();
builder.Services.AddScoped<IRepositorioReseña, RepositorioReseñas>();
builder.Services.AddScoped<IRepositorioPresupuesto, RepositorioPresupuestos>();
builder.Services.AddScoped<IRepositorioPago, RepositorioPagos>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

//TODO: borrar antes de subir a prod
app.UseCors("Dev");

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>
/// Clase parcial utilizada exclusivamente para habilitar el acceso al punto de entrada de la aplicación
/// desde los proyectos de testing.
/// </summary>
public partial class Program { }