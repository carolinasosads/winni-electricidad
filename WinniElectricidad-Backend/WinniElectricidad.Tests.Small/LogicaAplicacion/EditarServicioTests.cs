using Moq;
using WinniElectricidad.Compartido.DTOs.Servicios;
using WinniElectricidad.LogicaAplicacion.Servicios.Servicio;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.Tests.Small.LogicaAplicacion;

[TestFixture]
public class EditarServicioTests
{
    private Mock<IRepositorioServicio> _repoMock;
    private EditarServicio _servicio;

    [SetUp]
    public void SetUp()
    {
        _repoMock = new Mock<IRepositorioServicio>();
        _servicio = new EditarServicio(_repoMock.Object);
    }

    [Test]
    public void Ejecutar_ServicioNoExiste_LanzaArgumentException()
    {
        _repoMock
            .Setup(r => r.FindById(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Servicio?)null);

        var dto = new EditarServicioDto();

        Assert.ThrowsAsync<ArgumentException>(() =>
            _servicio.Ejecutar(1, dto, null, CancellationToken.None));
    }

    [Test]
    public void Ejecutar_IndexPrincipalNuevaInvalido_LanzaArgumentException()
    {
        var servicio = new Servicio("Electricidad", "Descripcion detallada", new List<ServicioImagen>());

        _repoMock
            .Setup(r => r.FindById(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(servicio);

        var dto = new EditarServicioDto
        {
            IndexPrincipalNueva = 5
        };

        var imagenes = new List<string> { "/img/1.jpg" };

        Assert.ThrowsAsync<ArgumentException>(() =>
            _servicio.Ejecutar(1, dto, imagenes, CancellationToken.None));
    }

    [Test]
    public async Task Ejecutar_UrlPrincipalExistente_TienePrioridad()
    {
        var servicio = new Servicio(
            "Electricidad",
            "Descripcion detallada",
            new List<ServicioImagen>
            {
                new ServicioImagen("/img/existente.jpg", esPrincipal: false),
                new ServicioImagen("/img/otra.jpg", esPrincipal: true)
            }
        );

        _repoMock
            .Setup(r => r.FindById(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(servicio);

        var dto = new EditarServicioDto
        {
            Titulo = servicio.Titulo,
            Descripcion = servicio.Descripcion,
            ImagenesExistentes = new List<String>
            {
                "/img/existente.jpg",
                "/img/otra.jpg"
            },
            UrlPrincipalExistente = "/img/existente.jpg"
        };

        var imagenes = new List<string> { "/img/nueva.jpg" };

        await _servicio.Ejecutar(1, dto, imagenes, CancellationToken.None);

        _repoMock.Verify(r => r.Update(
                It.Is<Servicio>(s =>
                    s.Imagenes.Any(img =>
                        img.Url == "/img/existente.jpg" &&
                        img.EsPrincipal
                    )
                    &&
                    s.Imagenes
                        .Where(img => img.Url != "/img/existente.jpg")
                        .All(img => img.EsPrincipal == false)
                ),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
