using Moq;
using WinniElectricidad.LogicaAplicacion.Servicios.Usuario;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.Tests.Small.LogicaAplicacion;

[TestFixture]
public class ObtenerDireccionesTests
{
    private Mock<IRepositorioUsuario> _mockRepo;
    private ObtenerDirecciones _servicio;

    [SetUp]
    public void Setup()
    {
        _mockRepo = new Mock<IRepositorioUsuario>();
        _servicio = new ObtenerDirecciones(_mockRepo.Object);
    }

    [Test]
    public async Task Ejecutar_DevuelveDtosMapeadosCorrectamente()
    {
        // Arrange
        var direcciones = new List<Direccion>
        {
            new("Calle 1", "Esquina 1", "123", "A"),
            new("Calle 2", "Esquina 2", "456", null)
        };

        _mockRepo.Setup(r => r.FindAddressByUserId(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(direcciones);

        // Act
        var result = await _servicio.Ejecutar(1);

        // Assert
        Assert.That(result, Has.Exactly(2).Items);
        Assert.Multiple(() =>
        {
            Assert.That(result.First().Calle, Is.EqualTo("Calle 1"));
            Assert.That(result.First().Esquina, Is.EqualTo("Esquina 1"));
        });
    }

    [Test]
    public async Task Ejecutar_ListaVaciaDevuelveVacio()
    {
        _mockRepo.Setup(r => r.FindAddressByUserId(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Direccion>());

        var result = await _servicio.Ejecutar(5);
        Assert.That(result, Is.Empty);
    }
}