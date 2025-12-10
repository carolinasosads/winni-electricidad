using WinniElectricidad.AccesoDatos.Repositorios.EF;
using WinniElectricidad.Compartido.DTOs.Reseñas;
using WinniElectricidad.LogicaAplicacion.Servicios.Reseña;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Reseñas;
using WinniElectricidad.Tests.Medium.Utils;

namespace WinniElectricidad.Tests.Medium.LogicaAplicacion;

[TestFixture]
public class AgregarReseñaTests
{
    [Test]
    public async Task Ejecutar_FlujoCompleto_UsuarioYServicioExisten_CreaReseñaYDevuelveDto()
    {
        // Arrange
        var (context, connection) = await DbContextHelper.CrearContextoSqliteEnMemoria();
        await using (connection)
        await using (context)
        {
            var usuario = new UsuarioCliente(
                nombreCompleto: "Cliente Medium",
                passwordHash: "987654321",
                email: "cliente@medium.com",
                telefono: "098111111",
                direcciones: new List<Direccion>())
            {
                IdUsuario = 5
            };

            var servicio = new Servicio("Electricidad", "Instalaciones completas", null)
            {
                Id = 50,
                Activo = true
            };

            context.Usuarios.Add(usuario);
            context.Servicios.Add(servicio);
            await context.SaveChangesAsync();

            var repoReseñas = new RepositorioReseñas(context);
            var repoUsuarios = new RepositorioUsuarios(context);
            var repoServicios = new RepositorioServicios(context);

            var servicioAgregar = new AgregarReseña(
                repositorioReseña: repoReseñas,
                repositorioUsuario: repoUsuarios,
                repositorioServicio: repoServicios
            );

            var dtoEntrada = new ReseñaACrearDto
            {
                IdServicio = 50,
                Descripcion = "Medium test reseña",
                Calificacion = 5
            };

            var imagenUrl = "/resenias/medium.jpg";

            // Act
            var result = await servicioAgregar.Ejecutar(
                dtoEntrada,
                idUsuario: 5,
                imagenUrl: imagenUrl,
                ct: CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.IdReseña, Is.GreaterThan(0));
                Assert.That(result.Descripcion, Is.EqualTo(dtoEntrada.Descripcion));
                Assert.That(result.Calificacion, Is.EqualTo(dtoEntrada.Calificacion));
                Assert.That(result.ImagenUrl, Is.EqualTo(imagenUrl));
                Assert.That(result.Cliente.IdUsuario, Is.EqualTo(5));
                Assert.That(result.Servicio.Id, Is.EqualTo(50));
            });

            var reseñaEnDb = context.Reseñas.Single(r => r.Id == result.IdReseña);
            Assert.Multiple(() =>
            {
                Assert.That(reseñaEnDb.Descripcion, Is.EqualTo("Medium test reseña"));
                Assert.That(reseñaEnDb.Calificacion, Is.EqualTo(5));
                Assert.That(reseñaEnDb.IdUsuario, Is.EqualTo(5));
                Assert.That(reseñaEnDb.IdServicio, Is.EqualTo(50));
                Assert.That(reseñaEnDb.ImagenUrl, Is.EqualTo(imagenUrl));
            });
        }
    }

    [Test]
    public async Task Ejecutar_UsuarioNoExiste_LanzaUnauthorizedAccessExceptionYNoGuarda()
    {
        // Arrange
        var (context, connection) = await DbContextHelper.CrearContextoSqliteEnMemoria();
        await using (connection)
        await using (context)
        {
            var servicio = new Servicio("Sanitaria", "Sanitaria completa", null)
            {
                Id = 60,
                Activo = true
            };

            context.Servicios.Add(servicio);
            await context.SaveChangesAsync();

            var repoReseñas = new RepositorioReseñas(context);
            var repoUsuarios = new RepositorioUsuarios(context);
            var repoServicios = new RepositorioServicios(context);

            var servicioAgregar = new AgregarReseña(
                repositorioReseña: repoReseñas,
                repositorioUsuario: repoUsuarios,
                repositorioServicio: repoServicios
            );

            var dtoEntrada = new ReseñaACrearDto
            {
                IdServicio = 60,
                Descripcion = "Test sin usuario",
                Calificacion = 3
            };

            // Act + Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                servicioAgregar.Ejecutar(
                    dtoEntrada,
                    idUsuario: 999,
                    imagenUrl: null,
                    ct: CancellationToken.None));

            Assert.That(context.Reseñas.Count(), Is.EqualTo(0));
        }
    }

    [Test]
    public async Task Ejecutar_ServicioNoExiste_LanzaReseñaExceptionYNoGuarda()
    {
        // Arrange
        var (context, connection) = await DbContextHelper.CrearContextoSqliteEnMemoria();
        await using (connection)
        await using (context)
        {
            var usuario = new UsuarioCliente(
                nombreCompleto: "Cliente Medium",
                passwordHash: "1111111",
                email: "cliente@servicio-noexiste.com",
                telefono: "097000000",
                direcciones: new List<Direccion>())
            {
                IdUsuario = 7
            };

            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();

            var repoReseñas = new RepositorioReseñas(context);
            var repoUsuarios = new RepositorioUsuarios(context);
            var repoServicios = new RepositorioServicios(context);

            var servicioAgregar = new AgregarReseña(
                repositorioReseña: repoReseñas,
                repositorioUsuario: repoUsuarios,
                repositorioServicio: repoServicios
            );

            var dtoEntrada = new ReseñaACrearDto
            {
                IdServicio = 999,
                Descripcion = "Test servicio inexistente",
                Calificacion = 2
            };

            // Act + Assert
            Assert.ThrowsAsync<ReseñaException>(() =>
                servicioAgregar.Ejecutar(
                    dtoEntrada,
                    idUsuario: 7,
                    imagenUrl: null,
                    ct: CancellationToken.None));

            Assert.That(context.Reseñas.Count(), Is.EqualTo(0));
        }
    }
}