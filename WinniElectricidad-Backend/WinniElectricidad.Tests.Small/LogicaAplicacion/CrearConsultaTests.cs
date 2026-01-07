using Moq;
using WinniElectricidad.Compartido.DTOs.Consulta;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Consulta;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Notificacion;
using WinniElectricidad.LogicaAplicacion.Servicios.Consultas;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.Tests.Small.LogicaAplicacion;

[TestFixture]
public class CrearConsultaTests
{
    private Mock<IEnviarEmail> _mockEnviarEmail;
    private Mock<IRepositorioUsuario> _mockRepoUsuario;
    private CrearConsulta _servicio;

    [SetUp]
    public void SetUp()
    {
        _mockEnviarEmail = new Mock<IEnviarEmail>();
        _mockRepoUsuario = new Mock<IRepositorioUsuario>();

        _servicio = new CrearConsulta(
            _mockEnviarEmail.Object,
            _mockRepoUsuario.Object
        );
    }

    [Test]
    public void Ejecutar_SinAdministrador_LanzaInvalidOperationException()
    {
        // Arrange
        var dto = new ConsultaCrearDto
        {
            Nombre = "Juan",
            Email = "juan@test.com",
            Telefono = "099111111",
            Mensaje = "Consulta de prueba"
        };

        _mockRepoUsuario
            .Setup(r => r.ObtenerAdministrador(It.IsAny<CancellationToken>()))
            .ReturnsAsync((UsuarioAdministrador?)null);

        // Act + Assert
        Assert.ThrowsAsync<InvalidOperationException>(() =>
            _servicio.Ejecutar(dto, CancellationToken.None));

        _mockEnviarEmail.Verify(
            e => e.Ejecutar(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task Ejecutar_DatosValidos_EnviaEmailAlAdministrador()
    {
        // Arrange
        var admin = new UsuarioAdministrador(
            "Admin",
            "hash",
            "admin@test.com",
            "099000000")
        {
            IdUsuario = 1
        };

        var dto = new ConsultaCrearDto
        {
            Nombre = "Maria",
            Email = "maria@test.com",
            Telefono = "098222222",
            Mensaje = "Hola, tengo una consulta"
        };

        _mockRepoUsuario
            .Setup(r => r.ObtenerAdministrador(It.IsAny<CancellationToken>()))
            .ReturnsAsync(admin);

        // Act
        await _servicio.Ejecutar(dto, CancellationToken.None);

        // Assert
        _mockEnviarEmail.Verify(
            e => e.Ejecutar(
                admin.Email,
                It.Is<string>(s => s.Contains("Nueva consulta")),
                It.Is<string>(c =>
                    c.Contains(dto.Nombre) &&
                    c.Contains(dto.Email) &&
                    c.Contains(dto.Telefono!) &&
                    c.Contains(dto.Mensaje)),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
