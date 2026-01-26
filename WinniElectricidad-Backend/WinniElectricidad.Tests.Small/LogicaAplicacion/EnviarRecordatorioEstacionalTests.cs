using Moq;
using WinniElectricidad.LogicaAplicacion.Servicios.Notificacion;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Notificacion;

namespace WinniElectricidad.Tests.Small.LogicaAplicacion;

[TestFixture]
public class EnviarRecordatorioEstacionalTests
{
    private Mock<IEnviarEmail> _enviarEmailMock;
    private EnviarRecordatorioEstacional _servicio;

    [SetUp]
    public void SetUp()
    {
        _enviarEmailMock = new Mock<IEnviarEmail>();
        _enviarEmailMock
            .Setup(e => e.GetFooterRecordatorioEstacional())
            .Returns("<footer />");

        _servicio = new EnviarRecordatorioEstacional(_enviarEmailMock.Object);
    }

    [Test]
    public async Task Ejecutar_DatosValidos_EnviaEmails()
    {
        var emails = new List<string> { "a@test.com", "b@test.com" };

        await _servicio.Ejecutar(
            "Electricidad",
            "Texto de prueba",
            emails,
            CancellationToken.None);

        _enviarEmailMock.Verify(e =>
            e.EjecutarMultiple(
                It.IsAny<List<string>>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Exactly(1));
    }

    [Test]
    public void Ejecutar_SinTitulo_LanzaArgumentException()
    {
        Assert.ThrowsAsync<ArgumentException>(() =>
            _servicio.Ejecutar(
                "",
                "Texto",
                new List<string> { "a@test.com" },
                CancellationToken.None));
    }

    [Test]
    public void Ejecutar_SinTexto_LanzaArgumentException()
    {
        Assert.ThrowsAsync<ArgumentException>(() =>
            _servicio.Ejecutar(
                "Electricidad",
                "",
                new List<string> { "a@test.com" },
                CancellationToken.None));
    }

    [Test]
    public void Ejecutar_SinDestinatarios_LanzaArgumentException()
    {
        Assert.ThrowsAsync<ArgumentException>(() =>
            _servicio.Ejecutar(
                "Electricidad",
                "Texto",
                new List<string>(),
                CancellationToken.None));
    }
}
