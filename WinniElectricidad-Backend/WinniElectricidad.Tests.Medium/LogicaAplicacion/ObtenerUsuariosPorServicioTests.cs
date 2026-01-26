using WinniElectricidad.AccesoDatos.Repositorios.EF;
using WinniElectricidad.LogicaAplicacion.Servicios.Usuario;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.Tests.Medium.Utils;

namespace WinniElectricidad.Tests.Medium.LogicaAplicacion;

[TestFixture]
public class ObtenerUsuariosPorServicioTests
{
    [Test]
    public async Task Ejecutar_ConReservasAsociadas_DevuelveClientes()
    {
        var (context, connection) = await DbContextHelper.CrearContextoSqliteEnMemoria();
        await using (connection)
        await using (context)
        {
            var servicio = new Servicio("Electricidad", "Descripcion detallada", null, "icono")
            {
                Id = 1,
                Activo = true
            };

            context.Servicios.Add(servicio);

            var direccion = new Direccion(
                calle: "Rivera",
                esquina: "Canelones",
                numero: "1234",
                apto: null
            );

            var cliente = new UsuarioCliente(
                "Cliente Test",
                "hash",
                "cliente@test.com",
                "099123456",
                new List<Direccion> { direccion }
            )
            {
                IdUsuario = 10
            };

            context.Usuarios.Add(cliente);
            await context.SaveChangesAsync();

            var reserva = new Reserva(
                fechaReserva: DateTime.Today.AddDays(3),
                tipo: TipoServicioReserva.Instalacion,
                idUsuarioCliente: cliente.IdUsuario,
                idDireccion: direccion.IdDireccion,
                servicios: new List<Servicio> { servicio },
                comentario: "Reserva test"
            );

            context.Reservas.Add(reserva);
            await context.SaveChangesAsync();

            var repoUsuarios = new RepositorioUsuarios(context);
            var repoServicios = new RepositorioServicios(context);
            var casoUso = new ObtenerUsuariosPorServicio(repoUsuarios, repoServicios);

            // Act
            var result = await casoUso.Ejecutar(servicio.Id, CancellationToken.None);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Email, Is.EqualTo("cliente@test.com"));
        }
    }
}