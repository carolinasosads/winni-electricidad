using WinniElectricidad.Compartido.DTOs.Reservas;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Notificacion;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Reserva;

public class ModificarReserva : IModificarReserva
{
    private IEnviarEmail _enviarEmail;
    private readonly IRepositorioReserva _repositorioReserva;
    private readonly IRepositorioUsuario _repositorioUsuario;
    public ModificarReserva(IRepositorioReserva repositorioReserva, IEnviarEmail enviarEmail, IRepositorioUsuario repositorioUsuario)
    {
        _repositorioReserva = repositorioReserva;
        _enviarEmail = enviarEmail;
        _repositorioUsuario = repositorioUsuario;
    }

    public async Task Ejecutar(ReservaAModificarDto dto, CancellationToken cancellationToken = default)
    {
        var reserva = await _repositorioReserva.ObtenerReservaPorId(dto.IdReserva, cancellationToken);

        if (reserva is null)
            throw new ArgumentException("La reserva no existe.");

        var fechaAnterior = reserva.FechaReserva;

        var desde = dto.NuevaFecha.AddMinutes(-5);
        var hasta = dto.NuevaFecha.AddMinutes(5);

        var reservasEnRango = await _repositorioReserva
            .FindAllBetweenDates(desde, hasta, cancellationToken);
        reserva.Reprogramar(dto.NuevaFecha, reservasEnRango);

        await _repositorioReserva.ActualizarReserva(reserva, cancellationToken);
        var cliente = reserva.UsuarioCliente 
                      ?? throw new InvalidOperationException("El cliente asociado a la reserva no existe.");

        var fechaAnteriorString = fechaAnterior.ToString("dd/MM/yyyy HH:mm");
        var fechaNuevaString = reserva.FechaReserva.ToString("dd/MM/yyyy HH:mm");

       var direccion = reserva.Direccion?.ToString() ?? "Sin dirección registrada";
        var comentario = string.IsNullOrWhiteSpace(reserva.Comentario)
            ? "Sin comentarios adicionales." : reserva.Comentario;
        var tipoServicio = reserva.TipoServicioReserva.ToString();

        var cuerpoCliente = $@"
            <div style='font-family: Arial, sans-serif; color: #333;'>
                <h2>Reserva reprogramada</h2>

                <p>Hola {cliente.NombreCompleto},</p>

                <p>Te informamos que tu reserva ha sido <strong>reprogramada con una fecha sugerida</strong>.</p>

                <p><strong>Fecha anterior:</strong> {fechaAnteriorString}</p>
                <p><strong>Nueva fecha sugerida:</strong> {fechaNuevaString}</p>

                <p>Esta nueva fecha es una <strong>propuesta</strong> y nos estaremos comunicando contigo a la brevedad
                para confirmar si te queda bien o coordinar otro horario en caso de ser necesario.</p>

                <p>Tu reserva volverá a estado <strong>Pendiente</strong> hasta que confirmemos juntos la nueva fecha.</p>

                <p>Ante cualquier duda o ajuste que quieras realizar, no dudes en responder este correo.</p>

                <p>¡Gracias por confiar en Winni Electricidad!</p>
            </div>";

        await _enviarEmail.Ejecutar(cliente.Email, "Winni Electricidad - Reserva reprogramada", cuerpoCliente, cancellationToken);

 
        var admin = await _repositorioUsuario.ObtenerAdministrador(cancellationToken);

        var cuerpoAdmin = $@"
            <div style='font-family: Arial, sans-serif; color: #333;'>
                <h2>Reserva reprogramada (sugerencia de nueva fecha)</h2>

                <p><strong>Cliente:</strong> {cliente.NombreCompleto}</p>
                <p><strong>Email:</strong> {cliente.Email}</p>
                <p><strong>Teléfono:</strong> {cliente.Telefono}</p>

                <p><strong>Tipo de servicio:</strong> {tipoServicio}</p>
                <p><strong>Dirección:</strong> {direccion}</p>

                <p><strong>Fecha anterior:</strong> {fechaAnteriorString}</p>
                <p><strong>Nueva fecha sugerida:</strong> {fechaNuevaString}</p>

                <p><strong>Comentario del cliente:</strong> {comentario}</p>

                <p>Recordá contactar al cliente para confirmar si la nueva fecha le queda bien
                o coordinar una alternativa.</p>
            </div>";
        await _enviarEmail.Ejecutar(admin.Email, "Reserva reprogramada – Nueva fecha sugerida", cuerpoAdmin, cancellationToken);
    }
}
