using Moq;
using WinniElectricidad.LogicaAplicacion.Servicios.Usuario;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.Tests.Small.LogicaAplicacion;

[TestFixture]
public class ObtenerUsuariosPorServicioTests
{
    private Mock<IRepositorioUsuario> _repoUsuarios;
    private Mock<IRepositorioServicio> _repoServicios;
    private ObtenerUsuariosPorServicio _servicio;

    [SetUp]
    public void SetUp()
    {
        _repoUsuarios = new Mock<IRepositorioUsuario>();
        _repoServicios = new Mock<IRepositorioServicio>();

        _servicio = new ObtenerUsuariosPorServicio(
            _repoUsuarios.Object,
            _repoServicios.Object
        );
    }

    [Test]
    public async Task Ejecutar_ServicioExiste_DevuelveListado()
    {
        // Arrange
        var servicio = new Servicio("Electricidad", "Descripcion detallada", null, "icono") { Id = 1 };

        var usuarios = new List<UsuarioCliente>
        {
            new("Cliente 1", "hash", "c1@test.com", "099232323", [])
            {
                IdUsuario = 10
            }
        };

        _repoServicios
            .Setup(r => r.FindById(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(servicio);

        _repoUsuarios
            .Setup(r => r.FindAllFilteredByService(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuarios);

        // Act
        var result = await _servicio.Ejecutar(1, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.First().Email, Is.EqualTo("c1@test.com"));
    }

    [Test]
    public void Ejecutar_ServicioNoExiste_LanzaArgumentException()
    {
        // Arrange
        _repoServicios
            .Setup(r => r.FindById(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Servicio?)null);

        // Act + Assert
        Assert.ThrowsAsync<ArgumentException>(() =>
            _servicio.Ejecutar(99, CancellationToken.None));
    }
}
