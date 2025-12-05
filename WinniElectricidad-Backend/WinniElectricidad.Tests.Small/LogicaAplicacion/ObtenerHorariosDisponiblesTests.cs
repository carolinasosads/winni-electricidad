using Moq;
using WinniElectricidad.LogicaAplicacion.Servicios.Reserva;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.Tests.Small.LogicaAplicacion;

[TestFixture]
public class ObtenerHorariosDisponiblesTests
{
    private Mock<IRepositorioReserva> _mockRepo;
    private ObtenerHorariosDisponibles _servicio;
    
    [SetUp]
    public void Setup()
    {
        _mockRepo = new Mock<IRepositorioReserva>();
        _mockRepo.Setup(r =>
                r.FindAllBetweenDates(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reserva>());
        _servicio = new ObtenerHorariosDisponibles(_mockRepo.Object);
    }
    
    [Test]
    public async Task Ejecutar_DeberiaMarcarComoOcupadoCuandoExisteReserva()
    {
        // Arrange
        var hoy = DateTime.Today;

        var fechaReserva = hoy.AddDays(2);

        // Si cae domingo, pasamos al siguiente día hábil
        while (fechaReserva.DayOfWeek == DayOfWeek.Sunday)
        {
            fechaReserva = fechaReserva.AddDays(1);
        }
        fechaReserva = fechaReserva.Date.AddHours(9);

        var reservas = new List<Reserva>
        {
            new(
                fechaReserva,
                TipoServicioReserva.Instalacion,
                idUsuarioCliente: 1,
                idDireccion: 1,
                servicios: new List<Servicio> { new Servicio { Titulo = "Electricidad" } },
                comentario: null)
        };

        _mockRepo.Setup(r =>
                r.FindAllBetweenDates(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(reservas);

        // Act
        var diasDisponibles = (await _servicio.Ejecutar()).ToList();

        var dia = diasDisponibles.First(d => d.Fecha.Date == fechaReserva.Date);
        var hora = dia.Horas.First(h => h.Hora == TimeOnly.FromDateTime(fechaReserva));

        // Assert
        Assert.That(hora.Disponible, Is.False);
    }
    
    [Test]
    public async Task Ejecutar_SinReservas_DeberiaDevolverTodosLosHorariosDisponibles()
    {
        // Act
        var dias = await _servicio.Ejecutar();

        // Assert
        Assert.That(dias, Is.Not.Null);

        foreach (var dia in dias)
        {
            if (dia.Fecha.DayOfWeek != DayOfWeek.Sunday)
            {
                Assert.That(dia.Horas.All(h => h.Disponible), Is.True);
            }
        }
    }
    
    [Test]
    public async Task Ejecutar_DeberiaComenzarEnDiaMas48Horas()
    {
        // Act
        var dias = (await _servicio.Ejecutar()).ToList();

        // Assert
        Assert.That(dias, Is.Not.Null);
        Assert.That(dias, Is.Not.Empty);

        var esperado = DateTime.Today.AddDays(2).Date;

        if (esperado.DayOfWeek == DayOfWeek.Sunday)
        {
            esperado = esperado.AddDays(1);
        }

        Assert.Multiple(() =>
        {
            // Primer día debe ser hoy + 2 días
            Assert.That(dias.First().Fecha.Date, Is.EqualTo(esperado));

            // No debe incluir hoy
            Assert.That(dias.Any(d => d.Fecha.Date == DateTime.Today.Date), Is.False);

            // No debe incluir mañana
            Assert.That(dias.Any(d => d.Fecha.Date == DateTime.Today.AddDays(1).Date), Is.False);
        });
    }

    [Test]
    public async Task Ejecutar_DeberiaGenerarHorariosCada90Minutos()
    {
        // Act
        var dias = await _servicio.Ejecutar();

        var primerDia = dias.First();

        var horas = primerDia.Horas
            .Select(h => h.Hora)
            .ToList();

        // Assert count
        Assert.That(horas, Has.Count.EqualTo(6));

        Assert.Multiple(() =>
        {
            // Assert secuencia exacta de horarios
            Assert.That(horas[0], Is.EqualTo(new TimeOnly(9, 0)));
            Assert.That(horas[1], Is.EqualTo(new TimeOnly(10, 30)));
            Assert.That(horas[2], Is.EqualTo(new TimeOnly(12, 0)));
            Assert.That(horas[3], Is.EqualTo(new TimeOnly(13, 30)));
            Assert.That(horas[4], Is.EqualTo(new TimeOnly(15, 0)));
            Assert.That(horas[5], Is.EqualTo(new TimeOnly(16, 30)));
        });

        // Assert diferencia de 90 minutos
        Assert.Multiple(() =>
        {
            Assert.That(horas[1].ToTimeSpan() - horas[0].ToTimeSpan(), Is.EqualTo(TimeSpan.FromMinutes(90)));
            Assert.That(horas[2].ToTimeSpan() - horas[1].ToTimeSpan(), Is.EqualTo(TimeSpan.FromMinutes(90)));
            Assert.That(horas[3].ToTimeSpan() - horas[2].ToTimeSpan(), Is.EqualTo(TimeSpan.FromMinutes(90)));
            Assert.That(horas[4].ToTimeSpan() - horas[3].ToTimeSpan(), Is.EqualTo(TimeSpan.FromMinutes(90)));
            Assert.That(horas[5].ToTimeSpan() - horas[4].ToTimeSpan(), Is.EqualTo(TimeSpan.FromMinutes(90)));
        });
    }
}