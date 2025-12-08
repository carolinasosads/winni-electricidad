using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Notificacion;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Reserva;

public class AprobarReserva : IAprobarReserva
{
    private readonly IEnviarEmail _enviarEmail;
    private readonly IRepositorioReserva _repositorioReserva;
    private readonly IRepositorioUsuario _repositorioUsuario;


    public AprobarReserva(IRepositorioReserva repoReserva, IRepositorioUsuario repositorioUsuario,
        IEnviarEmail enviarEmail)
    {
        _repositorioReserva = repoReserva;
        _enviarEmail = enviarEmail;
        _repositorioUsuario = repositorioUsuario;
    }
    
    public async Task Ejecutar(int idReserva, CancellationToken cancellationToken = default)
    {
        var reserva = await _repositorioReserva.ObtenerReservaPorId(idReserva, cancellationToken);
        
        if (reserva is null)
            throw new ArgumentException("La reserva no existe.");

        reserva.Aprobar();
        await _repositorioReserva.ActualizarReserva(reserva, cancellationToken);

       var cliente = reserva.UsuarioCliente;

        var fecha = reserva.FechaReserva.ToString("dd/MM/yyyy HH:mm");
        var direccion = reserva.Direccion?.ToString() ?? "Sin dirección registrada";
        var comentario = string.IsNullOrWhiteSpace(reserva.Comentario)
            ? "Sin comentarios adicionales." : reserva.Comentario;
        var tipoServicio = reserva.TipoServicioReserva.ToString();
        
        var cuerpoEmailCliente = $@"
            <div style='font-family: Arial, sans-serif; color: #333;'>
                <h2>Reserva confirmada</h2>
                <p>Hola {cliente.NombreCompleto},</p>
                <p>Tu reserva para el <strong>{fecha}</strong> fue <strong>confirmada</strong>.</p>
                <p>¡Gracias por confiar en Winni Electricidad!</p>
            </div>";

        await _enviarEmail.Ejecutar(cliente.Email, "Winni Electricidad - Reserva confirmada", cuerpoEmailCliente, cancellationToken);

        var admin = await _repositorioUsuario.ObtenerAdministrador(cancellationToken);
        
        var cuerpoEmailAdmin = $@"
            <div style='font-family: Arial, sans-serif; color: #333;'>
                <h2>Reserva confirmada</h2>

                <p><strong>Cliente:</strong> {cliente.NombreCompleto}</p>
                <p><strong>Email:</strong> {cliente.Email}</p>
                <p><strong>Teléfono:</strong> {cliente.Telefono}</p>

                <p><strong>Tipo de servicio:</strong> {tipoServicio}</p>
                <p><strong>Dirección:</strong> {direccion}</p>

                <p><strong>Fecha de la reserva:</strong> {fecha}</p>

                <p><strong>Comentario del cliente:</strong> {comentario}</p>

                <p>La reserva fue marcada como <strong>confirmada</strong> en el sistema.</p>
            </div>";

        await _enviarEmail.Ejecutar(admin.Email, "Reserva confirmada – Notificación interna", cuerpoEmailAdmin, cancellationToken);
    }
}