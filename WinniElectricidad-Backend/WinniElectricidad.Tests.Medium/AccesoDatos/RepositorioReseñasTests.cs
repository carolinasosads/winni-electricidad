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

            var reseñaEnDb = context.Reseñas.Single(r => r.Id == agregada.Id);

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
            context.Reseñas.AddRange(
                new Reseña("Reseña aprobada 1", 5, 1, 1, null)
                {
                    Estado = EstadoReseña.Aprobada
                },
                new Reseña("Reseña eliminada", 3, 2, 1, null)
                {
                    Estado = EstadoReseña.Eliminada
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