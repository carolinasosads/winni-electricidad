using WinniElectricidad.LogicaNegocio.Entidades;

namespace WinniElectricidad.Tests.Small.LogicaNegocio;

[TestFixture]
public class ServicioTests
{
    [Test]
    public void CrearServicio_Valido_NoLanzaExcepcion()
    {
        // Arrange & act
        var servicio = new Servicio("Electricidad", "Instalaciones eléctricas completas", null);

        // Assert
        Assert.That(servicio.Activo, Is.True);
        Assert.That(servicio.Titulo, Is.EqualTo("Electricidad"));
    }

    [Test]
    public void CrearServicio_TituloVacio_LanzaArgumentException()
    {
        // Arrange, act & assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new Servicio("", "Descripción válida y larga", null));

        Assert.That(ex.Message, Does.Contain("Ingrese un título válido"));
    }

    [Test]
    public void CrearServicio_DescripcionCorta_LanzaArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new Servicio("Sanitaria", "Muy corta", null));

        Assert.That(ex.Message, Does.Contain("Ingrese una descripción más detallada"));
    }

    [Test]
    public void CambiarActivo_CambiaElEstadoDelServicio()
    {
        var servicio = new Servicio("Riego", "Instalación de riego completo", null);
        servicio.Activo = false;

        Assert.IsFalse(servicio.Activo);
    }
}