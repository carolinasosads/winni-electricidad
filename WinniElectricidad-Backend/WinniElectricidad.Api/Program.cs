using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Resend;
using WinniElectricidad.AccesoDatos.Repositorios.EF;
using WinniElectricidad.Api.Servicio;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Servicio;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Usuario;
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

// Resend
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

// Autenticación
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

// Inyección de dependencias

// Servicios
builder.Services.AddScoped<ILoginUsuario, LoginUsuario>();
builder.Services.AddScoped<IServicioToken, ServicioToken>();
builder.Services.AddScoped<IRegistroUsuario, RegistroUsuario>();
builder.Services.AddHttpClient<IHCaptchaVerifier, HCaptchaServicio>();
builder.Services.AddScoped<IRecuperarContrasena, RecuperarContrasena>();
builder.Services.AddScoped<IServicioOneTimeToken, ServicioOneTimeToken>();
builder.Services.AddScoped<IObtenerServiciosActivos, ObtenerServiciosActivos>();

// Repositorios
builder.Services.AddScoped<IRepositorioUsuario, RepositorioUsuarios>();
builder.Services.AddScoped<IRepositorioOneTimeToken, RepositorioOneTimeTokens>();
builder.Services.AddScoped<IRepositorioServicio, RepositorioServicios>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//TODO: borrar antes de subir a prod
app.UseCors("Dev");

app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>
/// Clase parcial utilizada exclusivamente para habilitar el acceso al punto de entrada de la aplicación
/// desde los proyectos de testing.
/// </summary>
public partial class Program { }