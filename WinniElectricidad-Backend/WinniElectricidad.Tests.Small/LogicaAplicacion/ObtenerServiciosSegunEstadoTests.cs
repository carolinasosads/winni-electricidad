using Moq;
using WinniElectricidad.LogicaAplicacion.Servicios.Servicio;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.Tests.Small.LogicaAplicacion;

[TestFixture]
public class ObtenerServiciosSegunEstadoTests
{
    private Mock<IRepositorioServicio> _mockRepo;
    private ObtenerServiciosSegunEstado _servicio;

    [SetUp]
    public void Setup()
    {
        _mockRepo = new Mock<IRepositorioServicio>();
        _servicio = new ObtenerServiciosSegunEstado(_mockRepo.Object);
    }

    [Test]
    public async Task Ejecutar_CuandoHayActivos_DevuelveDtos()
    {
        // Arrange
        var lista = new List<Servicio> {
            new("Electricidad", "Instalaciones eléctricas completas", new List<ServicioImagen>(), "icono")
        };
        _mockRepo.Setup(r => r.FindAllSegunEstado(true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lista);

        // Act
        var result = await _servicio.Ejecutar(true);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Titulo, Is.EqualTo("Electricidad"));
        });
    }

    [Test]
    public async Task Ejecutar_CuandoNoHayActivos_DevuelveListaVacia()
    {
        // Arrange
        _mockRepo.Setup(r => r.FindAllSegunEstado(true,It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Servicio>());

        // Act
        var result = await _servicio.Ejecutar(true);

        // Assert
        Assert.That(result, Is.Empty);
    }
}