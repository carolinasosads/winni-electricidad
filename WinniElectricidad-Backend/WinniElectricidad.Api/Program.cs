using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WinniElectricidad.AccesoDatos.Repositorios.EF;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios;
using WinniElectricidad.LogicaAplicacion.Servicios;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

var builder = WebApplication.CreateBuilder(args);

//TODO: BORRAR ESTO PARA PRODUCCION
builder.Services.AddCors(o =>
{
    o.AddPolicy("Dev", p =>
        p
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// Add services to the container.

// Autenticación
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = jwtSettings["Key"];
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
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<WinniElectricidadContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ConexionWinniElectricidad")));

// Inyección de dependencias

// Servicios
builder.Services.AddScoped<ILoginUsuario, LoginUsuario>();
builder.Services.AddScoped<IServicioToken, ServicioToken>();

// Repositorios
builder.Services.AddScoped<IRepositorioUsuario, RepositorioUsuarios>();

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