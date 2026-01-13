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

        Assert.That(ex.Message, Does.Contain("Ingresa un título válido"));
    }

    [Test]
    public void CrearServicio_DescripcionCorta_LanzaArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new Servicio("Sanitaria", "Muy corta", null));

        Assert.That(ex.Message, Does.Contain("Ingresa una descripción más detallada"));
    }

    [Test]
    public void CambiarActivo_CambiaElEstadoDelServicio()
    {
        var servicio = new Servicio("Riego", "Instalación de riego completo", null);
        servicio.Activo = false;

        Assert.IsFalse(servicio.Activo);
    }
    
    [Test]
    public void Actualizar_MarcaSoloUnaImagenComoPrincipal()
    {
        var servicio = new Servicio(
            "Electricidad",
            "Descripcion larga valida",
            new List<ServicioImagen>
            {
                new ServicioImagen("/img/a.jpg", esPrincipal: true),
                new ServicioImagen("/img/b.jpg"),
            }
        );

        servicio.Actualizar(
            nuevoTitulo: "Electricidad",
            nuevaDescripcion: "Descripcion larga valida",
            imagenesUrls: new List<string> { "/img/a.jpg", "/img/b.jpg" },
            nuevasImagenes: null,
            urlPrincipalFinal: "/img/b.jpg"
        );

        Assert.That(servicio.Imagenes.Count(i => i.EsPrincipal), Is.EqualTo(1));
        Assert.That(
            servicio.Imagenes.Single(i => i.EsPrincipal).Url,
            Is.EqualTo("/img/b.jpg")
        );
    }
    
    [Test]
    public void Actualizar_UrlPrincipalInexistente_NoRompeYEligeFallback()
    {
        var servicio = new Servicio(
            "Electricidad",
            "Descripcion larga valida",
            new List<ServicioImagen>
            {
                new ServicioImagen("/img/a.jpg", esPrincipal: true),
                new ServicioImagen("/img/b.jpg"),
            }
        );

        servicio.Actualizar(
            nuevoTitulo: "Electricidad",
            nuevaDescripcion: "Descripcion larga valida",
            imagenesUrls: new List<string> { "/img/a.jpg", "/img/b.jpg" },
            nuevasImagenes: null,
            urlPrincipalFinal: "/img/que-no-existe.jpg"
        );

        Assert.That(servicio.Imagenes.Count(i => i.EsPrincipal), Is.EqualTo(1));
    }
    
    [Test]
    public void Actualizar_EliminaImagenesQueNoEstanEnLista()
    {
        var servicio = new Servicio(
            "Electricidad",
            "Descripcion larga valida",
            new List<ServicioImagen>
            {
                new ServicioImagen("/img/a.jpg", esPrincipal: true),
                new ServicioImagen("/img/b.jpg"),
            }
        );

        servicio.Actualizar(
            "Electricidad",
            "Descripcion larga valida",
            imagenesUrls: new List<string> { "/img/a.jpg" },
            nuevasImagenes: null,
            urlPrincipalFinal: null
        );

        Assert.That(servicio.Imagenes.Count, Is.EqualTo(1));
        Assert.That(servicio.Imagenes.Single().Url, Is.EqualTo("/img/a.jpg"));
    }
    
    [Test]
    public void Actualizar_SinImagenes_LanzaInvalidOperationException()
    {
        var servicio = new Servicio(
            "Electricidad",
            "Descripcion larga valida",
            new List<ServicioImagen>
            {
                new ServicioImagen("/img/a.jpg", esPrincipal: true)
            }
        );

        Assert.Throws<InvalidOperationException>(() =>
            servicio.Actualizar(
                "Electricidad",
                "Descripcion larga valida",
                imagenesUrls: new List<string>(),
                nuevasImagenes: null,
                urlPrincipalFinal: null
            )
        );
    }
}