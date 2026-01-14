using Moq;
using WinniElectricidad.Compartido.DTOs.Reservas;
using WinniElectricidad.Compartido.Reservas;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Notificacion;
using WinniElectricidad.LogicaAplicacion.Servicios.Reserva;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Reservas;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.Tests.Small.LogicaAplicacion;

[TestFixture]
public class AgendarReservaTests
{
    private Mock<IRepositorioReserva> _mockRepoReservas;
    private Mock<IRepositorioServicio> _mockRepoServicios;
    private Mock<IRepositorioUsuario> _mockRepoUsuarios;
    private Mock<IEnviarEmail> _mockEnviarEmail;
    private AgendarReserva _servicio;

    [SetUp]
    public void SetUp()
    {
        _mockRepoReservas = new Mock<IRepositorioReserva>();
        _mockRepoServicios = new Mock<IRepositorioServicio>();
        _mockRepoUsuarios = new Mock<IRepositorioUsuario>();
        _mockEnviarEmail = new Mock<IEnviarEmail>();

        _servicio = new AgendarReserva(
            _mockRepoReservas.Object,
            _mockRepoUsuarios.Object,
            _mockRepoServicios.Object,
            _mockEnviarEmail.Object
        );
    }

    [Test]
    public async Task Ejecutar_ReservaValida_DevuelveDto()
    {
        // Arrange
        var fecha = DateTime.Today.AddDays(2);

        // Si cae domingo, mover al día siguiente
        while (fecha.DayOfWeek == DayOfWeek.Sunday)
        {
            fecha = fecha.AddDays(1);
        }

        fecha = fecha.AddHours(10);

        var dto = new ReservaACrearDto
        {
            FechaReserva = fecha,
            TipoServicio = TipoServicioReserva.Instalacion,
            IdDireccion = 10,
            IdServicios = new List<int> { 1 },
            Comentario = "Prueba"
        };

        var usuario = new UsuarioCliente("Sofía", "1234567", "test@test.com", "099111111", new List<Direccion>())
        {
            IdUsuario = 1
        };

        var direcciones = new List<Direccion>
        {
            new() { IdDireccion = 10, IdUsuarioCliente = 1, Calle = "X", Esquina = "Y" }
        };

        var servicios = new List<Servicio>
        {
            new("Electricidad", "Instalaciones completas", null, "icono")
            {
                Activo = true
            }
        };

        var admin = new UsuarioAdministrador("Admin", "9876543", "admin@test.com", "099000000")
        {
            IdUsuario = 2
        };

        _mockRepoUsuarios.Setup(r => r.FindById(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);

        _mockRepoUsuarios.Setup(r => r.FindAddressByUserId(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(direcciones);

        _mockRepoUsuarios.Setup(r => r.ObtenerAdministrador(It.IsAny<CancellationToken>()))
            .ReturnsAsync(admin);

        _mockRepoServicios.Setup(r => r.FindByIds(It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(servicios);

        _mockRepoReservas.Setup(r => r.HorarioOcupado(fecha, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockRepoReservas.Setup(r => r.UsuarioTieneReservaEnHorario(1, fecha, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockRepoReservas.Setup(r => r.UsuarioTieneReservaEnDia(1, fecha, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockRepoReservas
            .Setup(r => r.Add(It.IsAny<Reserva>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Reserva r, CancellationToken _) =>
            {
                r.IdReserva = 123;
                return r;
            });

        _mockRepoReservas.Setup(r => r.FindById(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                return new Reserva(
                    fechaReserva: fecha,
                    tipo: TipoServicioReserva.Instalacion,
                    idUsuarioCliente: 1,
                    idDireccion: 10,
                    servicios: servicios,
                    comentario: dto.Comentario
                )
                {
                    IdReserva = 123,
                    Direccion = direcciones[0],
                    Servicios = servicios
                };
            });

        // Act
        var result = await _servicio.Ejecutar(dto, 1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.FechaReserva, Is.EqualTo(fecha));
            Assert.That(result.Servicios, Has.Count.EqualTo(1));
        });
        
        _mockEnviarEmail.Verify(e => e.Ejecutar(
                usuario.Email,
                "Winni Electricidad - Reserva de presupuesto",
                It.Is<string>(c => c.Contains("Reserva recibida")),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _mockEnviarEmail.Verify(e => e.Ejecutar(
                admin.Email,
                "Nueva reserva de presupuesto agendada",
                It.Is<string>(c => c.Contains("Nueva reserva de presupuesto agendada")),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Test]
    public void Ejecutar_SinAdministrador_LanzaReservaException()
    {
        // Arrange
        var fecha = DateTime.Today.AddDays(2).AddHours(10);

        if (fecha.DayOfWeek == DayOfWeek.Sunday)
        {
            fecha = fecha.AddDays(1);
        }

        var dto = new ReservaACrearDto
        {
            FechaReserva = fecha,
            TipoServicio = TipoServicioReserva.Instalacion,
            IdDireccion = 10,
            IdServicios = new List<int> { 1 },
            Comentario = "Prueba"
        };

        var usuario = new UsuarioCliente("Sofía", "1234567", "test@test.com", "099111111", new List<Direccion>())
        {
            IdUsuario = 1
        };

        _mockRepoUsuarios.Setup(r => r.FindById(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);

        _mockRepoUsuarios.Setup(r => r.FindAddressByUserId(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Direccion>
            {
                new() { IdDireccion = 10, IdUsuarioCliente = 1, Calle = "X", Esquina = "Y" }
            });

        _mockRepoServicios.Setup(r => r.FindByIds(It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Servicio>
            {
                new("Electricidad", "Instalaciones completas", null, "icono") { Activo = true }
            });

        _mockRepoReservas.Setup(r => r.HorarioOcupado(fecha, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockRepoReservas.Setup(r => r.UsuarioTieneReservaEnHorario(1, fecha, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockRepoReservas.Setup(r => r.UsuarioTieneReservaEnDia(1, fecha, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockRepoUsuarios.Setup(r => r.ObtenerAdministrador(It.IsAny<CancellationToken>()))
            .ReturnsAsync((UsuarioAdministrador?)null);

        // Act + Assert
        Assert.ThrowsAsync<ReservaException>(() => _servicio.Ejecutar(dto, 1));

        _mockEnviarEmail.Verify(
            e => e.Ejecutar(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

}
