using Microsoft.AspNetCore.Mvc;
using Moq;
using WinniElectricidad.Api.Controllers;
using WinniElectricidad.Compartido.DTOs.Servicios;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Servicio;

namespace WinniElectricidad.Tests.Small.Controllers;

[TestFixture]
public class ServicioControllerTests
{
    private Mock<IObtenerServiciosSegunEstado> _mockServicio;
    private Mock<IActivarServicio> _mockServicioActivar;
    private Mock<IDesactivarServicio> _mockServicioDesactivar;
    private Mock<ICrearServicio> _mockServicioCrear;

    private ServicioController _controller;

    [SetUp]
    public void Setup()
    {
        _mockServicio = new Mock<IObtenerServiciosSegunEstado>();
        _mockServicioDesactivar = new Mock<IDesactivarServicio>();
        _mockServicioActivar = new Mock<IActivarServicio>();
        _mockServicioCrear = new Mock<ICrearServicio>();
        _controller = new ServicioController(_mockServicio.Object,  _mockServicioDesactivar.Object, _mockServicioActivar.Object, _mockServicioCrear.Object);
    }

    [Test]
    public async Task GetServiciosDisponibles_DevuelveOkConLista()
    {
        // Arrange
        var lista = new List<ServicioActivoDto> {
            new() { Id = 1, Titulo = "Electricidad" },
            new() { Id = 2, Titulo = "Sanitaria" }
        };
        _mockServicio.Setup(s => s.Ejecutar(true,It.IsAny<CancellationToken>()))
            .ReturnsAsync(lista);

        // Act
        var result = await _controller.GetServiciosDisponibles(CancellationToken.None);

        // Assert
        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        var servicios = ok!.Value as ICollection<ServicioActivoDto>;
        Assert.That(servicios, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetServiciosDisponibles_EnCasoDeError_Devuelve500()
    {
        // Arrange
        _mockServicio.Setup(s => s.Ejecutar(true, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Error"));
        
        // Act
        var result = await _controller.GetServiciosDisponibles(CancellationToken.None);

        // Assert
        var status = result as ObjectResult;
        Assert.That(status, Is.Not.Null);
        Assert.That(status!.StatusCode, Is.EqualTo(500));
    }
}