using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.LogicaNegocio.Entidades;

namespace WinniElectricidad.Tests.Small.Compartido;

public class ServicioMapperTests
{
    [Test]
    public void MapearServiciosADtos_MapeaCorrectamente()
    {
        // Arrange
        var servicios = new List<Servicio>
        {
            new Servicio("Electricidad", "Instalaciones eléctricas completas", null) { Id = 1, Activo = true }
        };

        // Act
        var dtos = ServicioMapper.MapearServiciosADtos(servicios).ToList();

        // Assert
        Assert.That(dtos, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(dtos[0].Id, Is.EqualTo(1));
            Assert.That(dtos[0].Titulo, Is.EqualTo("Electricidad"));
        });
    }
}