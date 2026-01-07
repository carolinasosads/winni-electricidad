using Moq;
using WinniElectricidad.Compartido.DTOs.Reseñas;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reseña;
using WinniElectricidad.LogicaAplicacion.Servicios.Reseña;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Reseñas;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.Tests.Small.LogicaAplicacion;

[TestFixture]
public class AgregarReseñaSmallTests
{
    private Mock<IRepositorioReseña> _mockRepoReseña;
    private Mock<IRepositorioUsuario> _mockRepoUsuario;
    private Mock<IRepositorioServicio> _mockRepoServicio;
    private Mock<IModeracionOpenAi> _mockServicioModeracion;
    private AgregarReseña _servicio;

    [SetUp]
    public void SetUp()
    {
        _mockRepoReseña = new Mock<IRepositorioReseña>();
        _mockRepoUsuario = new Mock<IRepositorioUsuario>();
        _mockRepoServicio = new Mock<IRepositorioServicio>();
        _mockServicioModeracion = new Mock<IModeracionOpenAi>();

        _servicio = new AgregarReseña(
            _mockRepoReseña.Object,
            _mockRepoUsuario.Object,
            _mockRepoServicio.Object,
            _mockServicioModeracion.Object);
    }

    [Test]
    public void Ejecutar_UsuarioNoExiste_LanzaUnauthorizedAccessException()
    {
        // Arrange
        var dto = new ReseñaACrearDto
        {
            IdServicio = 1,
            Descripcion = "Test",
            Calificacion = 5
        };

        _mockRepoUsuario
            .Setup(r => r.FindById(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UsuarioCliente?)null);

        // Act + Assert
        Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _servicio.Ejecutar(dto, 10, null, CancellationToken.None));

        _mockRepoReseña.Verify(
            r => r.Add(It.IsAny<Reseña>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public void Ejecutar_ServicioNoExiste_LanzaReseñaException()
    {
        // Arrange
        var dto = new ReseñaACrearDto
        {
            IdServicio = 99,
            Descripcion = "Test",
            Calificacion = 4
        };

        var usuario = new UsuarioCliente(
            "Cliente Test",
            "1234567",
            "mail@test.com",
            "099000000",
            new List<Direccion>())
        {
            IdUsuario = 10
        };

        _mockRepoUsuario
            .Setup(r => r.FindById(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);

        _mockRepoServicio
            .Setup(r => r.FindById(dto.IdServicio, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Servicio?)null);

        // Act + Assert
        Assert.ThrowsAsync<ReseñaException>(() =>
            _servicio.Ejecutar(dto, 10, null, CancellationToken.None));

        _mockRepoReseña.Verify(
            r => r.Add(It.IsAny<Reseña>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task Ejecutar_DatosValidos_DevuelveReseñaCreadaYAgregaReseña()
    {
        // Arrange
        var dto = new ReseñaACrearDto
        {
            IdServicio = 1,
            Descripcion = "Muy bueno",
            Calificacion = 5
        };

        var usuario = new UsuarioCliente(
            "Cliente Test",
            "1234567",
            "mail@test.com",
            "099000000",
            new List<Direccion>())
        {
            IdUsuario = 10
        };

        var servicio = new Servicio("Electricidad", "Instalaciones de electricidad", new List<ServicioImagen>())
        {
            Id = 1
        };

        _mockRepoUsuario
            .Setup(r => r.FindById(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);

        _mockRepoServicio
            .Setup(r => r.FindById(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(servicio);

        _mockRepoReseña
            .Setup(r => r.Add(It.IsAny<Reseña>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Reseña r, CancellationToken _) =>
            {
                r.Id = 123;
                return r;
            });

        var imagenUrl = "/resenias/test.jpg";

        // Act
        var result = await _servicio.Ejecutar(dto, 10, imagenUrl, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.IdReseña, Is.EqualTo(123));
            Assert.That(result.Descripcion, Is.EqualTo(dto.Descripcion));
            Assert.That(result.Calificacion, Is.EqualTo(dto.Calificacion));

            Assert.That(result.Servicio.Id, Is.EqualTo(1));
            Assert.That(result.Cliente.IdUsuario, Is.EqualTo(10));

            Assert.That(result.ImagenUrl, Is.EqualTo(imagenUrl));
        });

        _mockRepoReseña.Verify(
            r => r.Add(It.IsAny<Reseña>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}