using Moq;
using WinniElectricidad.AccesoDatos.Repositorios.EF;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Notificacion;
using WinniElectricidad.LogicaAplicacion.Servicios.Usuario;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Tokens;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;
using WinniElectricidad.Tests.Medium.Utils;

namespace WinniElectricidad.Tests.Medium.LogicaAplicacion;

[TestFixture]
public class RecuperarContrasenaTests
{
    [Test]
    public async Task EnviarCorreoRecuperacion_EmailValido_CreaTokenYEnviaMail()
    {
        // Arrange
        var (context, connection) = await DbContextHelper.CrearContextoSqliteEnMemoria();
        await using (connection)
        await using (context)
        {
            var usuario = new UsuarioCliente(
                "Cliente Reset",
                "hash",
                "reset@test.com",
                "099000000",
                new List<Direccion>())
            {
                IdUsuario = 10
            };

            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();

            var mockEnviarEmail = new Mock<IEnviarEmail>();

            var servicio = new RecuperarContrasena(
                new RepositorioUsuarios(context),
                new RepositorioOneTimeTokens(context),
                new ServicioOneTimeToken(),
                mockEnviarEmail.Object,
                new ServicioHash()
            );

            // Act
            await servicio.EnviarCorreoRecuperacion("reset@test.com");

            // Assert DB
            Assert.That(context.OneTimeTokens.Count(), Is.EqualTo(1));

            // Assert email
            mockEnviarEmail.Verify(e => e.Ejecutar(
                    "reset@test.com",
                    It.Is<string>(s => s.Contains("Recuperación de contraseña")),
                    It.Is<string>(c => c.Contains("reset-password")),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
    
    [Test]
    public async Task ResetearContrasena_TokenValido_CambiaPasswordYMarcaTokenUsado()
    {
        var (context, connection) = await DbContextHelper.CrearContextoSqliteEnMemoria();
        await using (connection)
        await using (context)
        {
            var hash = new ServicioHash();
            var tokenService = new ServicioOneTimeToken();

            var usuario = new UsuarioCliente(
                "Cliente Reset",
                hash.Hash("vieja123"),
                "reset@test.com",
                "099000000",
                new List<Direccion>())
            {
                IdUsuario = 20
            };

            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();

            var (tokenPlain, tokenHash) = tokenService.Create();

            context.OneTimeTokens.Add(new OneTimeToken
            {
                IdUsuario = 20,
                TokenHash = tokenHash,
                Usado = false,
                Tipo = OneTimeTokenTipo.ReseteoDeContrasena
            });

            await context.SaveChangesAsync();

            var servicio = new RecuperarContrasena(
                new RepositorioUsuarios(context),
                new RepositorioOneTimeTokens(context),
                tokenService,
                Mock.Of<IEnviarEmail>(),
                hash
            );

            // Act
            await servicio.ResetearContrasena("nueva123", tokenPlain);

            // Assert password
            var usuarioActualizado = await context.Usuarios.FindAsync(20);
            Assert.That(hash.VerificarPassword("nueva123", usuarioActualizado!.PasswordHash), Is.True);

            // Assert token usado
            var token = context.OneTimeTokens.Single();
            Assert.That(token.Usado, Is.True);
        }
    }
    
    [Test]
    public async Task ResetearContrasena_TokenInvalido_LanzaOneTimeTokenException()
    {
        var (context, connection) = await DbContextHelper.CrearContextoSqliteEnMemoria();
        await using (connection)
        await using (context)
        {
            var servicio = new RecuperarContrasena(
                new RepositorioUsuarios(context),
                new RepositorioOneTimeTokens(context),
                new ServicioOneTimeToken(),
                Mock.Of<IEnviarEmail>(),
                new ServicioHash()
            );

            Assert.ThrowsAsync<OneTimeTokenException>(() =>
                servicio.ResetearContrasena("nueva123", "token-invalido"));
        }
    }
}