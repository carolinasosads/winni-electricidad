using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.DTOs.Servicios;
using WinniElectricidad.LogicaNegocio.Entidades;

namespace WinniElectricidad.Tests.Small.Compartido;

public class ServicioMapperTests
{
    [Test]
    public void MapearServiciosAActivosDtos_MapeaCorrectamente()
    {
        // Arrange
        var servicios = new List<Servicio>
        {
            new Servicio("Electricidad", "Instalaciones eléctricas completas", new List<ServicioImagen>(), "icono") { Id = 1, Activo = true }
        };

        // Act
        var dtos = ServicioMapper.MapearServiciosADtos(true, servicios).ToList();

        // Assert
        Assert.That(dtos, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(dtos[0].Id, Is.EqualTo(1));
            Assert.That(dtos[0].Titulo, Is.EqualTo("Electricidad"));
        });
    }
    
    [Test]
    public void MapearServiciosADtos_MapeaCorrectamente()
    {
        // Arrange
        var servicios = new List<Servicio>
        {
            new Servicio("Electricidad", "Instalaciones eléctricas completas", new List<ServicioImagen>(), "icono") { Id = 1, Activo = true }
        };

        // Act
        var dtos = ServicioMapper
            .MapearServiciosADtos(false, servicios)
            .OfType<ServicioDto>()
            .ToList();
        
        // Assert
        Assert.That(dtos, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(dtos[0].Activo, Is.EqualTo(true));
        });
    }
}