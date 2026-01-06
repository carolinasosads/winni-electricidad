using Moq;
using WinniElectricidad.Compartido.DTOs.Consulta;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Notificacion;
using WinniElectricidad.LogicaAplicacion.Servicios.Consultas;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.Tests.Medium.LogicaAplicacion;

[TestFixture]
public class CrearConsultaMediumTests
{
    [Test]
    public async Task Ejecutar_AdminExiste_EnviaEmailAlAdministrador()
    {
        // Arrange
        var admin = new UsuarioAdministrador(
            "Admin Medium",
            "hash",
            "admin@medium.com",
            "099111111"
        )
        {
            IdUsuario = 1
        };

        var repoUsuariosMock = new Mock<IRepositorioUsuario>();
        repoUsuariosMock
            .Setup(r => r.ObtenerAdministrador(It.IsAny<CancellationToken>()))
            .ReturnsAsync(admin);

        var enviarEmailMock = new Mock<IEnviarEmail>();
        enviarEmailMock
            .Setup(m => m.Ejecutar(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var servicio = new CrearConsulta(
            enviarEmail: enviarEmailMock.Object,
            repositorioUsuario: repoUsuariosMock.Object
        );

        var dto = new ConsultaCrearDto
        {
            Nombre = "Carolina",
            Email = "caro@test.com",
            Telefono = "098123456",
            Mensaje = "Hola, tengo una consulta general."
        };

        // Act
        await servicio.Ejecutar(dto, CancellationToken.None);

        // Assert
        enviarEmailMock.Verify(m => m.Ejecutar(
                admin.Email,
                "Nueva consulta",
                It.Is<string>(html =>
                    html.Contains("Nueva consulta") &&
                    html.Contains(dto.Nombre) &&
                    html.Contains(dto.Email) &&
                    html.Contains(dto.Telefono!) &&
                    html.Contains(dto.Mensaje)
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [Test]
    public async Task Ejecutar_NoHayAdmin_LanzaInvalidOperationException_YNoEnviaEmail()
    {
        // Arrange
        var repoUsuariosMock = new Mock<IRepositorioUsuario>();
        repoUsuariosMock
            .Setup(r => r.ObtenerAdministrador(It.IsAny<CancellationToken>()))
            .ReturnsAsync((UsuarioAdministrador?)null);

        var enviarEmailMock = new Mock<IEnviarEmail>();
        enviarEmailMock
            .Setup(m => m.Ejecutar(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var servicio = new CrearConsulta(
            enviarEmail: enviarEmailMock.Object,
            repositorioUsuario: repoUsuariosMock.Object
        );

        var dto = new ConsultaCrearDto
        {
            Nombre = "Sin Admin",
            Email = "sinadmin@test.com",
            Telefono = "099000000",
            Mensaje = "Probando sin admin."
        };

        // Act + Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(() =>
            servicio.Ejecutar(dto, CancellationToken.None));

        Assert.That(ex!.Message, Is.EqualTo("No hay un usuario administrador para recibir la consulta."));

        enviarEmailMock.Verify(m => m.Ejecutar(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
}
