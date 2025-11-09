using Moq;
using WinniElectricidad.LogicaAplicacion.Servicios.Servicio;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.Tests.Small.LogicaAplicacion;

[TestFixture]
public class ObtenerServiciosActivosTests
{
    private Mock<IRepositorioServicio> _mockRepo;
    private ObtenerServiciosActivos _servicio;

    [SetUp]
    public void Setup()
    {
        _mockRepo = new Mock<IRepositorioServicio>();
        _servicio = new ObtenerServiciosActivos(_mockRepo.Object);
    }

    [Test]
    public async Task Ejecutar_CuandoHayActivos_DevuelveDtos()
    {
        // Arrange
        var lista = new List<Servicio> {
            new("Electricidad", "Instalaciones eléctricas completas", null)
        };
        _mockRepo.Setup(r => r.FindAllActive(It.IsAny<CancellationToken>()))
            .ReturnsAsync(lista);

        // Act
        var result = await _servicio.Ejecutar();

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
        _mockRepo.Setup(r => r.FindAllActive(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Servicio>());

        // Act
        var result = await _servicio.Ejecutar();

        // Assert
        Assert.That(result, Is.Empty);
    }
}