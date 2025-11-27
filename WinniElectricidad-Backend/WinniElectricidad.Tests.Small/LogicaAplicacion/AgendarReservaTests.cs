using Moq;
using Resend;
using WinniElectricidad.Compartido.Reservas;
using WinniElectricidad.LogicaAplicacion.Servicios.Reserva;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.Tests.Small.LogicaAplicacion;

[TestFixture]
public class AgendarReservaTests
{
    private Mock<IRepositorioReserva> _mockRepoReservas;
    private Mock<IRepositorioServicio> _mockRepoServicios;
    private Mock<IRepositorioUsuario> _mockRepoUsuarios;
    private Mock<IResend> _mockResend;              
    private AgendarReserva _servicio;

    [SetUp]
    public void SetUp()
    {
        _mockRepoReservas = new Mock<IRepositorioReserva>();
        _mockRepoServicios = new Mock<IRepositorioServicio>();
        _mockRepoUsuarios = new Mock<IRepositorioUsuario>();
        _mockResend = new Mock<IResend>();          

        _servicio = new AgendarReserva(
            _mockRepoReservas.Object,
            _mockRepoUsuarios.Object,
            _mockRepoServicios.Object,
            _mockResend.Object                    
        );
    }

    [Test]
    public async Task Ejecutar_ReservaValida_DevuelveDto()
    {
        // Arrange
        var fechaBase = DateTime.Today.AddDays(3);

        // Si justo cae domingo, se corro a lunes
        if (fechaBase.DayOfWeek == DayOfWeek.Sunday)
        {
            fechaBase = fechaBase.AddDays(1);
        }

        var fecha = fechaBase.AddHours(10);
        var dto = new ReservaACrearDto()
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
            new("Electricidad", "Instalaciones completas", null)
        };

        var admin = new UsuarioAdministrador(
            "Admin",
            "1234567",
            "admin@test.com",
            "099000000"
        )
        {
            IdUsuario = 2
        };

        _mockRepoUsuarios.Setup(r => r.FindById(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);

        _mockRepoUsuarios.Setup(r => r.FindAddressByUserId(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(direcciones);

        _mockRepoServicios.Setup(r => r.FindByIds(It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(servicios);

        _mockRepoReservas.Setup(r => r.HorarioOcupado(fecha, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockRepoReservas.Setup(r => r.UsuarioTieneReservaEnHorario(1, fecha, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockRepoReservas.Setup(r => r.UsuarioTieneReservaEnDia(1, fecha, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockRepoReservas.Setup(r => r.Add(It.IsAny<Reserva>(), It.IsAny<CancellationToken>()));
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

        _mockRepoUsuarios.Setup(r => r.ObtenerAdministrador(It.IsAny<CancellationToken>()))
            .ReturnsAsync(admin);

        _mockResend
            .Setup(r => r.EmailSendAsync(
                It.IsAny<EmailMessage>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ResendResponse<Guid>)default!);
        

        // Act
        var result = await _servicio.Ejecutar(dto, 1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.FechaReserva, Is.EqualTo(fecha));
            Assert.That(result.Servicios, Has.Count.EqualTo(1));
        });
    }
}
