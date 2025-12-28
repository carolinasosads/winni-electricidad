using WinniElectricidad.AccesoDatos.Repositorios.EF;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Usuario;
using WinniElectricidad.LogicaAplicacion.Servicios.Usuario;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;
using WinniElectricidad.Tests.Medium.Utils;

namespace WinniElectricidad.Tests.Medium.LogicaAplicacion;

[TestFixture]
public class LoginUsuarioTests
{
    [Test]
    public async Task Login_CredencialesValidas_DeberiaRetornarUsuarioLogueado()
    {
        var (context, connection) = await DbContextHelper.CrearContextoSqliteEnMemoria();
        await using (connection)
        await using (context)
        {
            var hash = new ServicioHash();
            var passwordPlano = "123456";
            var passwordHash = hash.Hash(passwordPlano);

            var usuario = new UsuarioCliente(
                "Cliente Login",
                passwordHash,
                "login@test.com",
                "099000000",
                new List<Direccion>()
            )
            {
                IdUsuario = 1
            };

            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();

            var repo = new RepositorioUsuarios(context);
            var servicio = new LoginUsuario(repo, hash);

            var result = await servicio.Login("login@test.com", passwordPlano);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Email, Is.EqualTo("login@test.com"));
            Assert.That(result.Id, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task Login_EmailNoExiste_DeberiaRetornarNull()
    {
        var (context, connection) = await DbContextHelper.CrearContextoSqliteEnMemoria();
        await using (connection)
        await using (context)
        {
            var servicio = new LoginUsuario(
                new RepositorioUsuarios(context),
                new ServicioHash()
            );

            var result = await servicio.Login("noexiste@test.com", "123");

            Assert.That(result, Is.Null);
        }
    }

    [Test]
    public async Task Login_PasswordIncorrecta_DeberiaRetornarNull()
    {
        var (context, connection) = await DbContextHelper.CrearContextoSqliteEnMemoria();
        await using (connection)
        await using (context)
        {
            var hash = new ServicioHash();

            var usuario = new UsuarioCliente(
                "Cliente Login",
                hash.Hash("correcta"),
                "login@test.com",
                "099000000",
                new List<Direccion>()
            )
            {
                IdUsuario = 2
            };

            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();

            var servicio = new LoginUsuario(
                new RepositorioUsuarios(context),
                hash
            );

            var result = await servicio.Login("login@test.com", "incorrecta");

            Assert.That(result, Is.Null);
        }
    }

    [Test]
    public async Task Login_EmailNull_DeberiaRetornarNull()
    {
        // Arrange
        var (context, connection) = await DbContextHelper.CrearContextoSqliteEnMemoria();
        await using (connection)
        await using (context)
        {
            var servicio = new LoginUsuario(
                new RepositorioUsuarios(context),
                new ServicioHash()
            );

            // Act
            var result = await servicio.Login(null!, "123");

            // Assert
            Assert.That(result, Is.Null);
        }
    }

    [Test]
    public async Task Login_PasswordNull_DeberiaRetornarNull()
    {
        // Arrange
        var (context, connection) = await DbContextHelper.CrearContextoSqliteEnMemoria();
        await using (connection)
        await using (context)
        {
            var servicio = new LoginUsuario(
                new RepositorioUsuarios(context),
                new ServicioHash()
            );

            // Act
            var result = await servicio.Login("test@test.com", null!);

            // Assert
            Assert.That(result, Is.Null);
        }
    }
}