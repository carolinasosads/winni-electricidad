using WinniElectricidad.AccesoDatos.Repositorios.EF;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.Tests.Medium.Utils;

namespace WinniElectricidad.Tests.Medium.AccesoDatos;

[TestFixture]
public class RepositorioReseñasTests
{
    [Test]
    public async Task Add_PersisteReseñaConDatosBasicos()
    {
        // Arrange
        var (context, connection) = await DbContextHelper.CrearContextoSqliteEnMemoria();
        await using (connection)
        await using (context)
        {
            context.Usuarios.Add(new UsuarioCliente
            {
                IdUsuario = 10,
                Email = "test@test.com",
                NombreCompleto = "Usuario Test",
                PasswordHash = "Test",
                Telefono = "Test"
            });

            context.Servicios.Add(new Servicio
            {
                Id = 50,
                Titulo = "Servicio Test",
                Activo = true
            });
            
            var reseña = new Reseña(
                descripcion: "Muy buen servicio",
                calificacion: 5,
                idUsuario: 10,
                idServicio: 50,
                imagen: "/resenias/test.jpg"
            );

            var repo = new RepositorioReseñas(context);

            // Act
            var agregada = await repo.Add(reseña, CancellationToken.None);

            // Assert
            Assert.That(agregada.Id, Is.GreaterThan(0));

            var reseñaEnDb = context.Resenias.Single(r => r.Id == agregada.Id);

            Assert.Multiple(() =>
            {
                Assert.That(reseñaEnDb.Descripcion, Is.EqualTo("Muy buen servicio"));
                Assert.That(reseñaEnDb.Calificacion, Is.EqualTo(5));
                Assert.That(reseñaEnDb.IdUsuario, Is.EqualTo(10));
                Assert.That(reseñaEnDb.IdServicio, Is.EqualTo(50));
                Assert.That(reseñaEnDb.ImagenUrl, Is.EqualTo("/resenias/test.jpg"));
                Assert.That(reseñaEnDb.Estado, Is.EqualTo(EstadoReseña.Aprobada));
            });
        }
    }

    [Test]
    public async Task FindAll_FiltraSoloAprobadas()
    {
        // Arrange
        var (context, connection) = await DbContextHelper.CrearContextoSqliteEnMemoria();
        await using (connection)
        await using (context)
        {
            context.Usuarios.AddRange(
                new UsuarioCliente { IdUsuario = 1, NombreCompleto = "U1", Email = "u1@test.com", PasswordHash = "Test", Telefono = "Test"},
                new UsuarioCliente { IdUsuario = 2, NombreCompleto = "U2", Email = "u2@test.com", PasswordHash = "Test", Telefono = "Test"},
                new UsuarioCliente { IdUsuario = 3, NombreCompleto = "U3", Email = "u3@test.com", PasswordHash = "Test", Telefono = "Test"}
            );

            context.Servicios.AddRange(
                new Servicio { Id = 1, Titulo = "Servicio 1", Activo = true },
                new Servicio { Id = 2, Titulo = "Servicio 2", Activo = true }
            );
            
            context.Resenias.AddRange(
                new Reseña("Reseña aprobada 1", 5, 1, 1, null)
                {
                    Estado = EstadoReseña.Aprobada
                },
                new Reseña("Reseña desaprobada", 3, 2, 1, null)
                {
                    Estado = EstadoReseña.Desaprobada
                },
                new Reseña("Reseña aprobada 2", 4, 3, 2, null)
                {
                    Estado = EstadoReseña.Aprobada
                }
            );
            await context.SaveChangesAsync();

            var repo = new RepositorioReseñas(context);

            // Act
            var result = await repo.FindAll(CancellationToken.None);

            // Assert
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.All(r => r.Estado == EstadoReseña.Aprobada), Is.True);
        }
    }
}