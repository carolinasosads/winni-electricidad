using Moq;
using WinniElectricidad.Compartido.DTOs.Servicios;
using WinniElectricidad.LogicaAplicacion.Servicios.Servicio;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.Tests.Small.LogicaAplicacion;

[TestFixture]
public class CrearServicioTests
{
    private Mock<IRepositorioServicio> _repoMock;
    private CrearServicio _servicio;

    [SetUp]
    public void SetUp()
    {
        _repoMock = new Mock<IRepositorioServicio>();
        _servicio = new CrearServicio(_repoMock.Object);
    }

    [Test]
    public async Task Ejecutar_DatosValidos_LlamaAddYDevuelveDto()
    {
        // Arrange
        var dto = new CrearServicioDto
        {
            Titulo = "Electricidad",
            Descripcion = "Instalaciones eléctricas"
        };

        var imagenes = new List<string> { "/img/1.jpg" };

        _repoMock
            .Setup(r => r.Add(It.IsAny<Servicio>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Servicio s, CancellationToken _) =>
            {
                s.Id = 10;
                return s;
            });

        // Act
        var result = await _servicio.Ejecutar(dto, imagenes, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(10));
            Assert.That(result.Titulo, Is.EqualTo(dto.Titulo));
            Assert.That(result.Descripcion, Is.EqualTo(dto.Descripcion));
            Assert.That(result.Imagenes.Count, Is.EqualTo(1));
        });

        _repoMock.Verify(
            r => r.Add(It.IsAny<Servicio>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}