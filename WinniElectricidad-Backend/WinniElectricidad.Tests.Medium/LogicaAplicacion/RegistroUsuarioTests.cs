using Microsoft.EntityFrameworkCore;
using Moq;
using WinniElectricidad.AccesoDatos.Repositorios.EF;
using WinniElectricidad.Compartido.DTOs.Usuarios;
using WinniElectricidad.LogicaAplicacion.Servicios.Usuario;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Usuarios;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;
using WinniElectricidad.Tests.Medium.Utils;

namespace WinniElectricidad.Tests.Medium.LogicaAplicacion;

[TestFixture]
public class RegistroUsuarioTests
{
    [Test]
    public async Task Registro_DatosValidos_CreaUsuarioYDevuelveDto()
    {
        var (context, connection) = await DbContextHelper.CrearContextoSqliteEnMemoria();
        await using (connection)
        await using (context)
        {
            var servicio = new RegistroUsuario(
                new RepositorioUsuarios(context),
                new ServicioHash()
            );

            var dto = new UsuarioRegistroDto
            {
                NombreCompleto = "Sofía",
                Email = "registro@test.com",
                Password = "123456",
                Telefono = "099000000"
            };

            var result = await servicio.Registro(dto);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Email, Is.EqualTo("registro@test.com"));

            var usuario = await context.Usuarios.SingleAsync();
            Assert.That(usuario.Email, Is.EqualTo("registro@test.com"));
            Assert.That(usuario.PasswordHash, Is.Not.EqualTo("123456"));
        }
    }
    
    [Test]
    public async Task Registro_EmailDuplicado_LanzaEmailEnUsoException()
    {
        var (context, connection) = await DbContextHelper.CrearContextoSqliteEnMemoria();
        await using (connection)
        await using (context)
        {
            context.Usuarios.Add(new UsuarioCliente(
                    "Existente",
                    "hash",
                    "dup@test.com",
                    "099789654",
                    new List<Direccion>())
                { IdUsuario = 1 });

            await context.SaveChangesAsync();

            var servicio = new RegistroUsuario(
                new RepositorioUsuarios(context),
                new ServicioHash()
            );

            var dto = new UsuarioRegistroDto
            {
                NombreCompleto = "Nuevo",
                Email = "dup@test.com",
                Password = "123456"
            };

            Assert.ThrowsAsync<EmailEnUsoException>(() =>
                servicio.Registro(dto));
        }
    }
    
    [TestCase("")]
    [TestCase("123")]
    public void Registro_PasswordInvalida_LanzaArgumentException(string password)
    {
        var servicio = new RegistroUsuario(
            Mock.Of<IRepositorioUsuario>(),
            new ServicioHash()
        );

        var dto = new UsuarioRegistroDto
        {
            Email = "test@test.com",
            Password = password
        };

        Assert.ThrowsAsync<ArgumentException>(() =>
            servicio.Registro(dto));
    }
}
