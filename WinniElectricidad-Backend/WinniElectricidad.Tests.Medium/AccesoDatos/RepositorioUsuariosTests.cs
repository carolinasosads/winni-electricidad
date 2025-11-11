using WinniElectricidad.AccesoDatos.Repositorios.EF;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.Tests.Medium.Utils;

namespace WinniElectricidad.Tests.Medium.AccesoDatos;

[TestFixture]
public class RepositorioUsuariosTests
{
    [Test]
    public async Task FindAddressByUserId_DevuelveSoloLasDelUsuario()
    {
        var (context, connection) = await DbContextHelper.CrearContextoSqliteEnMemoria();
        await using (connection)
        await using (context)
        {
            var direccionesUsuario1 = new List<Direccion>
            {
                new("Calle 1", "Esquina 1", "111", null),
                new("Calle 2", "Esquina 2", "222", "A")
            };
            var direccionesUsuario2 = new List<Direccion>
            {
                new("Otra", "Distinta", "333", null)
            };
            
            var usuario1 = new UsuarioCliente("Sofía", "1234567", "sofia@test.com", "1234567", direccionesUsuario1) { IdUsuario = 1 };
            var usuario2 = new UsuarioCliente("Carolina", "1234567", "caro@test.com", "1234567", direccionesUsuario2) { IdUsuario = 2 };

            context.Usuarios.AddRange(usuario1, usuario2);
            await context.SaveChangesAsync();

            var repo = new RepositorioUsuarios(context);

            // Act
            var result = await repo.FindAddressByUserId(usuario1.IdUsuario);

            // Assert
            Assert.That(result, Has.Exactly(2).Items);
            Assert.That(result.All(d => d.Calle.StartsWith("Calle")), Is.True);
        }
    }
}