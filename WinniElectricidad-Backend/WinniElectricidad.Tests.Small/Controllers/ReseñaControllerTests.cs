using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WinniElectricidad.Api.Controllers;
using WinniElectricidad.Api.Servicios;
using WinniElectricidad.Compartido.DTOs.Reseñas;
using WinniElectricidad.Compartido.DTOs.Servicios;
using WinniElectricidad.Compartido.DTOs.Usuarios.Reserva;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reseña;
using WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Reseñas;

namespace WinniElectricidad.Tests.Small.Controllers;

[TestFixture]
public class ReseñaControllerTests
{
    private Mock<IAgregarReseña> _mockAgregarReseña;
    private Mock<IServicioImagenes> _mockServicioImagenes;
    private Mock<IObtenerReseñasAprobadas> _mockObtenerReseñas;
    private Mock<IDesaprobarReseña>  _mockDesaprobar;
    private ReseñaController _controller;

    [SetUp]
    public void Setup()
    {
        _mockAgregarReseña = new Mock<IAgregarReseña>();
        _mockServicioImagenes = new Mock<IServicioImagenes>();
        _mockObtenerReseñas = new Mock<IObtenerReseñasAprobadas>();
        _mockDesaprobar = new Mock<IDesaprobarReseña>();

        _controller = new ReseñaController(_mockAgregarReseña.Object, _mockServicioImagenes.Object, _mockObtenerReseñas.Object, _mockDesaprobar.Object);

        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, "10") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Test]
    public async Task Reseñar_ConDatosValidos_SinImagen_DevuelveOkConReseñaCreada()
    {
        // Arrange
        var dtoEntrada = new ReseñaACrearDto
        {
            IdServicio = 1,
            Descripcion = "Muy bueno",
            Calificacion = 5
        };

        var dtoSalida = new ReseñaCreadaDto
        {
            IdReseña = 123,
            Descripcion = dtoEntrada.Descripcion,
            Calificacion = dtoEntrada.Calificacion,
            FechaReseña = DateTime.UtcNow,
            Cliente = new UsuarioReservaDto()
            {
                Email = "usuario@test.com",
                IdUsuario = 10,
                Nombre = "Cliente Test"
            },
            Servicio = new ServicioActivoDto
            {
                Id = 1,
                Titulo = "Servicio test"
            },
            ImagenUrl = null
        };

        _mockAgregarReseña
            .Setup(s => s.Ejecutar(dtoEntrada, 10, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dtoSalida);

        // Act
        var result = await _controller.Reseñar(dtoEntrada, null, CancellationToken.None);

        // Assert
        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);

        var body = ok!.Value as ReseñaCreadaDto;
        Assert.That(body, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(body!.IdReseña, Is.EqualTo(123));
            Assert.That(body.Descripcion, Is.EqualTo(dtoEntrada.Descripcion));
            Assert.That(body.Calificacion, Is.EqualTo(dtoEntrada.Calificacion));
            Assert.That(body.ImagenUrl, Is.Null);
        });

        _mockServicioImagenes.Verify(s => s.GuardarAsync(It.IsAny<IFormFile>(), "resenias"), Times.Never);
    }

    [Test]
    public async Task Reseñar_CuandoServicioLanzaReseñaException_DevuelveConflict()
    {
        // Arrange
        var dtoEntrada = new ReseñaACrearDto
        {
            IdServicio = 99,
            Descripcion = "Test",
            Calificacion = 3
        };

        _mockAgregarReseña
            .Setup(s => s.Ejecutar(dtoEntrada, 10, null, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ReseñaException("Regla de negocio"));

        // Act
        var result = await _controller.Reseñar(dtoEntrada, null, CancellationToken.None);

        // Assert
        var conflict = result as ConflictObjectResult;
        Assert.That(conflict, Is.Not.Null);
    }

    [Test]
    public async Task Reseñar_CuandoServicioLanzaExcepcionGenerica_Devuelve500()
    {
        // Arrange
        var dtoEntrada = new ReseñaACrearDto
        {
            IdServicio = 1,
            Descripcion = "Test",
            Calificacion = 4
        };

        _mockAgregarReseña
            .Setup(s => s.Ejecutar(dtoEntrada, 10, null, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Error inesperado"));

        // Act
        var result = await _controller.Reseñar(dtoEntrada, null, CancellationToken.None);

        // Assert
        var status = result as ObjectResult;
        Assert.That(status, Is.Not.Null);
        Assert.That(status!.StatusCode, Is.EqualTo(500));
    }
}