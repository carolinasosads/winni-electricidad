using Moq;
using WinniElectricidad.AccesoDatos.Repositorios.EF;
using WinniElectricidad.Compartido.DTOs.Reservas;
using WinniElectricidad.LogicaAplicacion.Servicios.Reserva;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;
using WinniElectricidad.Tests.Medium.Utils;

namespace WinniElectricidad.Tests.Medium.LogicaAplicacion;

[TestFixture]
public class ObtenerHistoricoMensualReservasTests
{
    [Test]
    public async Task ObtenerHistoricoMensual_MesConReservas_DevuelveSoloLasDelMes()
    {
        // Arrange
        var (context, connection) = await DbContextHelper.CrearContextoSqliteEnMemoria();
        await using (connection)
        await using (context)
        {
            var direccion = new Direccion("Rivera", "Canelones", "1001", null)
            {
                IdDireccion = 10,
                IdUsuarioCliente = 1
            };

            var usuario = new UsuarioCliente(
                "Cliente Historico",
                "hash",
                "cliente@historico.com",
                "099000000",
                new List<Direccion> { direccion }
            )
            {
                IdUsuario = 1
            };

            var servicio = new Servicio(
                titulo: "Electricidad",
                descripcion: "Servicio test descripcion detallada",
                imagenUrl: null
            )
            {
                Id = 10,
                Activo = true
            };

            context.Usuarios.Add(usuario);
            context.Servicios.Add(servicio);
            await context.SaveChangesAsync();

            var reservaEnero = new Reserva
            {
                FechaReserva = new DateTime(2025, 1, 10),
                EstadoReserva = EstadoReserva.Confirmada,
                TipoServicioReserva = TipoServicioReserva.Instalacion,
                IdUsuarioCliente = usuario.IdUsuario,
                IdDireccion = direccion.IdDireccion,
                Servicios = new List<Servicio> { servicio }
            };

            var reservaFebrero = new Reserva
            {
                FechaReserva = new DateTime(2025, 2, 5),
                EstadoReserva = EstadoReserva.Confirmada,
                TipoServicioReserva = TipoServicioReserva.Instalacion,
                IdUsuarioCliente = usuario.IdUsuario,
                IdDireccion = direccion.IdDireccion,
                Servicios = new List<Servicio> { servicio }
            };

            context.Reservas.AddRange(reservaEnero, reservaFebrero);
            await context.SaveChangesAsync();

            var servicioHistorico = new ObtenerHistoricoMensualReservas(
                new RepositorioReservas(context)
            );

            // Act
            var resultado = await servicioHistorico.Ejecutar(1, 2025);

            // Assert
            Assert.That(resultado.Count(), Is.EqualTo(1));
            Assert.That(resultado.First().FechaReserva.Month, Is.EqualTo(1));
        }
    }
    
    [TestCase(0)]
    [TestCase(13)]
    public void ObtenerHistoricoMensual_MesInvalido_LanzaArgumentException(int mes)
    {
        var servicio = new ObtenerHistoricoMensualReservas(
            Mock.Of<IRepositorioReserva>()
        );

        Assert.ThrowsAsync<ArgumentException>(() =>
            servicio.Ejecutar(mes, 2025));
    }
    
    [TestCase(1999)]
    [TestCase(3000)]
    public void ObtenerHistoricoMensual_AnioInvalido_LanzaArgumentException(int anio)
    {
        var servicio = new ObtenerHistoricoMensualReservas(
            Mock.Of<IRepositorioReserva>()
        );

        Assert.ThrowsAsync<ArgumentException>(() =>
            servicio.Ejecutar(1, anio));
    }
    
    [Test]
    public async Task ObtenerHistoricoMensual_SinReservas_DevuelveListaVacia()
    {
        var (context, connection) = await DbContextHelper.CrearContextoSqliteEnMemoria();
        await using (connection)
        await using (context)
        {
            var servicio = new ObtenerHistoricoMensualReservas(
                new RepositorioReservas(context)
            );

            var resultado = await servicio.Ejecutar(3, 2025);

            Assert.That(resultado, Is.Empty);
        }
    }
}