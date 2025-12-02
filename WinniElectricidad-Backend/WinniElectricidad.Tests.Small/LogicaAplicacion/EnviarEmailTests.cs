using Moq;
using Resend;
using WinniElectricidad.LogicaAplicacion.Servicios.Notificacion;
using WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Notificaciones;

namespace WinniElectricidad.Tests.Small.LogicaAplicacion;

[TestFixture]
public class EnviarEmailTests
{
    private Mock<IResend> _mockResend;
    private EnviarEmail _servicio;

    [SetUp]
    public void Setup()
    {
        _mockResend = new Mock<IResend>();
        _servicio = new EnviarEmail(_mockResend.Object);
    }

    [Test]
    public async Task Ejecutar_ConstruyeEmailYLLamaAResend()
    {
        var destinatario = "dest@test.com";
        var asunto = "Asunto de prueba";
        var cuerpo = "<p>Hola</p>";

        EmailMessage? mensajeEnviado = null;

        _mockResend
            .Setup(r => r.EmailSendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .Callback<EmailMessage, CancellationToken>((msg, _) =>
            {
                mensajeEnviado = msg;
            })
            .ReturnsAsync(new ResendResponse<Guid>(Guid.Empty, null));

        // Act
        await _servicio.Ejecutar(destinatario, asunto, cuerpo);

        // Assert
        Assert.That(mensajeEnviado, Is.Not.Null);

        var enviado = mensajeEnviado!;

        Assert.Multiple(() =>
        {
            Assert.That(enviado.To.FirstOrDefault()!.ToString(), Is.EqualTo(destinatario));
            Assert.That(enviado.Subject, Is.EqualTo(asunto));
            Assert.That(enviado.HtmlBody, Is.EqualTo(cuerpo));
            Assert.That(enviado.From.ToString(), Is.EqualTo("Winni Electricidad <no-replay@no-replay.winnielectricidad.tech>"));
        });

        _mockResend.Verify(
            r => r.EmailSendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Test]
    public void Ejecutar_ErrorEnResend_LanzaEmailNotificacionException()
    {
        // Arrange
        _mockResend
            .Setup(r => r.EmailSendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Fallo externo"));

        // Act + Assert
        Assert.ThrowsAsync<EmailNotificacionException>(() =>
            _servicio.Ejecutar("dest@test.com", "Asunto", "<p>cuerpo</p>")
        );
    }
}