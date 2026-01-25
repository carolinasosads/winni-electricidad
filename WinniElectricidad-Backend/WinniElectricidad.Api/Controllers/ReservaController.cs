using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WinniElectricidad.Compartido.DTOs.Reservas;
using WinniElectricidad.Compartido.DTOs.Usuarios.Reserva;
using WinniElectricidad.Compartido.Reservas;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;
using WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Notificaciones;
using WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Reservas;

namespace WinniElectricidad.Api.Controllers;

/// <summary>
/// Controlador que gestiona las operaciones de reserva de presupuesto.
/// </summary>
[Route("WinniElectricidadApi/[controller]")]
[ApiController]
public class ReservaController : ControllerBase
{
    private readonly IObtenerHorariosDisponibles _obtenerHorariosDisponibles;
    private readonly IAgendarReserva _agendarReserva;
    private readonly IObtenerHistoricoMensualReservas _obtenerHistoricoMensualReservas;
    private readonly IObtenerHistoricoFinalizadas _obtenerHistoricoFinalizadas;
    private readonly IObtenerReservasPorEstado _obtenerReservasPorEstado;
    private readonly IAprobarReserva _aprobarReserva;
    private readonly ICancelarReserva _cancelarReserva;
    private readonly IModificarReserva _modificarReserva;
    private readonly IObtenerReservasPorCliente _obtenerReservasPorCliente;
    private readonly IRegistrarReservaHistoricaAdmin _registrarReservaHistoricaAdmin;
    private readonly IObtenerMisReservasClienteConDetalle _obtenerMisReservasClienteConDetalle;

    /// <summary>
    /// Inicializa una nueva instancia del <see cref="ReservaController"/> con las dependencias necesarias.
    /// </summary>
    public ReservaController(IObtenerHorariosDisponibles obtenerHorariosDisponibles, IAgendarReserva agendarReserva, IObtenerHistoricoMensualReservas obtenerHistoricoMensualReservas, IObtenerHistoricoFinalizadas obtenerHistoricoFinalizadas,IObtenerReservasPorEstado obtenerReservasPorEstado,  IAprobarReserva aprobarReserva,
        ICancelarReserva cancelarReserva, IModificarReserva modificarReserva, IObtenerReservasPorCliente obtenerReservasPorCliente, IRegistrarReservaHistoricaAdmin registrarReservaHistoricaAdmin, IObtenerMisReservasClienteConDetalle obtenerMisReservasClienteConDetalle)
    {
        _obtenerHorariosDisponibles = obtenerHorariosDisponibles;
        _agendarReserva = agendarReserva;
        _obtenerHistoricoMensualReservas = obtenerHistoricoMensualReservas;
        _obtenerHistoricoFinalizadas = obtenerHistoricoFinalizadas;
        _obtenerReservasPorEstado = obtenerReservasPorEstado;
        _cancelarReserva = cancelarReserva;
        _aprobarReserva = aprobarReserva;
        _modificarReserva =  modificarReserva;
        _obtenerReservasPorCliente = obtenerReservasPorCliente;
        _registrarReservaHistoricaAdmin = registrarReservaHistoricaAdmin;
        _obtenerMisReservasClienteConDetalle = obtenerMisReservasClienteConDetalle;
    }
    
    /// <summary>
    /// Obtiene la disponibilidad de horarios para la solicitud de presupuestos o reservas.
    /// </summary>
    /// <remarks>
    /// Este endpoint devuelve los días y horarios disponibles para que el usuario autenticado pueda agendar una cita de presupuesto.
    /// 
    /// **Flujo:**
    /// 1. Consulta la disponibilidad configurada en el sistema mediante el servicio <see cref="IObtenerHorariosDisponibles"/>.  
    /// 2. Devuelve una colección de objetos <see cref="DiaDisponibilidadDto"/> que representan los días y horarios habilitados.  
    ///
    /// **Requiere autenticación:**  
    /// - Solo disponible para usuarios con el rol <c>Cliente</c>.
    ///
    /// **Códigos de respuesta:**
    /// - `200 OK` → Lista de horarios disponibles obtenida correctamente.  
    /// - `500 Internal Server Error` → Error inesperado del servidor.
    /// </remarks>
    /// <param name="cancellationToken">
    /// Token de cancelación que permite interrumpir la operación si es necesario.
    /// </param>
    /// <returns>
    /// Una respuesta HTTP que contiene la lista de días y horarios disponibles para reservas.
    /// </returns>
    /// <response code="200">Lista de horarios disponibles obtenida correctamente.</response>
    /// <response code="500">Error inesperado del servidor.</response>
    [HttpGet("disponibilidad")]
    [ProducesResponseType(typeof(IEnumerable<DiaDisponibilidadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> GetHorariosDisponibles(CancellationToken cancellationToken)
    {
        try
        {
            var horariosDisponibles = await _obtenerHorariosDisponibles.Ejecutar(cancellationToken);
            return Ok(horariosDisponibles);
        } catch (Exception)
        {
            return StatusCode(500, new { message = "Error inesperado." });
        }
    }
    
    /// <summary>
    /// Crea una nueva reserva de presupuesto para el usuario autenticado.
    /// </summary>
    /// <remarks>
    /// Este endpoint permite agendar una cita para solicitar un presupuesto, indicando fecha, hora,
    /// servicios requeridos y la dirección del usuario.
    ///
    /// **Flujo:**
    /// 1. Se valida el token JWT y se obtiene el identificador del usuario autenticado.  
    /// 2. Se procesa la solicitud mediante el servicio <see cref="IAgendarReserva"/>.  
    /// 3. Si la reserva es válida, se crea y retorna un objeto <see cref="ReservaCreadaDto"/>.  
    ///
    /// **Requiere autenticación:**  
    /// - Roles permitidos: <c>Cliente</c> y <c>Administrador</c>.
    ///
    /// **Códigos de respuesta:**
    /// - `200 OK` → Reserva creada correctamente.  
    /// - `400 Bad Request` → La solicitud contiene datos inválidos.  
    /// - `401 Unauthorized` → Token inválido o expirado.  
    /// - `403 Forbidden` → El usuario no tiene permisos para completar la acción.  
    /// - `409 Conflict` → La reserva no puede realizarse (duplicada, horario ocupado, etc.).  
    /// - `500 Internal Server Error` → Error inesperado del servidor.
    /// </remarks>
    /// <param name="nuevaReserva">
    /// Datos necesarios para crear la reserva, incluyendo fecha, horario, dirección y servicios seleccionados.
    /// </param>
    /// <param name="cancellationToken">
    /// Token de cancelación para abortar la operación si es necesario.
    /// </param>
    /// <returns>
    /// Una respuesta HTTP que contiene los datos de la reserva creada.
    /// </returns>
    /// <response code="200">Reserva creada correctamente.</response>
    /// <response code="400">La solicitud contiene datos inválidos.</response>
    /// <response code="401">Token inválido o expirado.</response>
    /// <response code="403">El usuario no tiene permisos suficientes.</response>
    /// <response code="409">Conflicto al intentar crear la reserva.</response>
    /// <response code="500">Error inesperado del servidor.</response>
    [HttpPost("agendar")]
    [ProducesResponseType(typeof(ReservaCreadaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Cliente, Administrador")]
    public async Task<IActionResult> Agendar([FromBody] ReservaACrearDto nuevaReserva, CancellationToken cancellationToken)
    {
        try
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idClaim)) return Unauthorized(new { message = "Token inválido o expirado." });

            var idUsuario = int.Parse(idClaim);
            
            var reservaCreada = await _agendarReserva.Ejecutar(nuevaReserva, idUsuario, cancellationToken);
            
            return Ok(reservaCreada);
        } catch (ReservaException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (EmailNotificacionException)
        {
            return StatusCode(500, new { message = "La reserva fue creada, pero hubo un problema al enviar la notificación por email."});
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Error inesperado." });
        }
    }
    
    /// <summary>
    /// Obtiene el historial de reservas realizadas en un mes y año específicos.
    /// </summary>
    /// <remarks>
    /// Este endpoint devuelve el listado de reservas correspondientes al período indicado,
    /// pensado para el análisis interno del administrador.
    ///
    /// **Flujo:**
    /// 1. Se valida que el mes y el año sean valores válidos.  
    /// 2. Se consulta el servicio <see cref="IObtenerHistoricoMensualReservas"/> con los parámetros recibidos.  
    /// 3. Se retorna una colección de <see cref="HistoricoReservaDto"/> con la información de las reservas.
    ///
    /// **Requiere autenticación:**  
    /// - Rol permitido: <c>Administrador</c>.
    ///
    /// **Códigos de respuesta:**
    /// - `200 OK` → Historial mensual obtenido correctamente.  
    /// - `400 Bad Request` → Parámetros de mes o año inválidos.  
    /// - `500 Internal Server Error` → Error inesperado del servidor.
    /// </remarks>
    /// <param name="mes">
    /// Número de mes (1–12) para el cual se desea obtener el historial.
    /// </param>
    /// <param name="anio">
    /// Año correspondiente al período a consultar.
    /// </param>
    /// <param name="cancellationToken">
    /// Token de cancelación para interrumpir la operación si es necesario.
    /// </param>
    /// <returns>
    /// Una respuesta HTTP que contiene la lista de reservas del mes y año especificados.
    /// </returns>
    /// <response code="200">Historial mensual obtenido correctamente.</response>
    /// <response code="400">Parámetros de mes o año inválidos.</response>
    /// <response code="500">Error inesperado del servidor.</response>
    [HttpGet("historico-mensual/{mes:int}/{anio:int}")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(IEnumerable<HistoricoReservaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> GetHistoricoMensual(int mes, int anio, [FromQuery] string? filtro, CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await _obtenerHistoricoMensualReservas.Ejecutar( mes, anio, filtro, cancellationToken);

            return Ok(resultado);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Error inesperado." });
        }
    }
    
    /// <summary>
    /// Obtiene el historial de todas las reservas finalizadas.
    /// </summary>
    /// <remarks>
    /// Este endpoint recupera las reservas cuyo estado es <c>Finalizada</c>,
    /// para su visualización en el módulo de historial interno.
    ///
    /// **Flujo:**
    /// 1. Se invoca el servicio <see cref="IObtenerHistoricoFinalizadas"/>.  
    /// 2. Se retorna una colección de <see cref="HistoricoReservaDto"/> con la información de cada reserva finalizada.
    ///
    /// **Requiere autenticación:**  
    /// - Rol permitido: <c>Administrador</c>.
    ///
    /// **Códigos de respuesta:**
    /// - `200 OK` → Historial de reservas finalizadas obtenido correctamente.  
    /// - `500 Internal Server Error` → Error inesperado del servidor.
    /// </remarks>
    /// <param name="ct">
    /// Token de cancelación para detener la operación si es necesario.
    /// </param>
    /// <returns>
    /// Una respuesta HTTP que contiene la lista de reservas finalizadas.
    /// </returns>
    /// <response code="200">Historial de reservas finalizadas obtenido correctamente.</response>
    /// <response code="500">Error inesperado del servidor.</response>
    [HttpGet("finalizadas")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(IEnumerable<HistoricoReservaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> GetHistoricoFinalizadas([FromQuery]string? filtro, CancellationToken ct)
    {
        try
        {
            var resultado = await _obtenerHistoricoFinalizadas.Ejecutar(filtro, ct);
            return Ok(resultado);
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Error inesperado." });
        }
    }
    
    /// <summary>
    /// Obtiene las reservas filtradas por estado.
    /// </summary>
    /// <remarks>
    /// Este endpoint permite consultar las reservas según su estado actual
    /// (por ejemplo: Pendiente, Confirmada, Finalizada, Cancelada).
    ///
    /// **Flujo:**
    /// 1. Se recibe el estado como parámetro de consulta (<c>query string</c>).  
    /// 2. Se valida el valor del estado y se invoca el servicio <see cref="IObtenerReservasPorEstado"/>.  
    /// 3. Se retorna una colección de <see cref="HistoricoReservaDto"/> con las reservas que coinciden con el estado solicitado.
    ///
    /// **Requiere autenticación:**  
    /// - Rol permitido: <c>Administrador</c>.
    ///
    /// **Códigos de respuesta:**
    /// - `200 OK` → Reservas obtenidas correctamente.  
    /// - `400 Bad Request` → El estado proporcionado es inválido.  
    /// - `500 Internal Server Error` → Error inesperado del servidor.
    /// </remarks>
    /// <param name="estado">
    /// Estado por el cual se filtrarán las reservas (ej.: <c>Pendiente</c>, <c>Confirmada</c>).
    /// </param>
    /// <param name="ct">
    /// Token de cancelación para detener la operación si es necesario.
    /// </param>
    /// <returns>
    /// Una respuesta HTTP con la lista de reservas que coinciden con el estado solicitado.
    /// </returns>
    /// <response code="200">Reservas obtenidas correctamente.</response>
    /// <response code="400">El estado proporcionado es inválido.</response>
    /// <response code="500">Error inesperado del servidor.</response>
    [HttpGet("por-estado")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(IEnumerable<HistoricoReservaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPorEstado([FromQuery] string estado, [FromQuery]string? filtro, CancellationToken ct)
    {
        try
        {
            var resultado = await _obtenerReservasPorEstado.Ejecutar(estado, filtro, ct);

            return Ok(resultado);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Error inesperado." });
        }
    }

    /// <summary>
    /// Aprueba una reserva de presupuesto específica.
    /// </summary>
    /// <remarks>
    /// Este endpoint permite cambiar el estado de una reserva a <c>Confirmada</c>,
    /// siempre que la misma se encuentre en un estado válido para ser aprobada.
    ///
    /// **Flujo:**
    /// 1. Se recibe el identificador de la reserva por ruta.  
    /// 2. Se invoca el servicio <see cref="IAprobarReserva"/> para aplicar la lógica de negocio.  
    /// 3. Si la operación es exitosa, se retorna un mensaje de confirmación.
    ///
    /// **Requiere autenticación:**  
    /// - Rol permitido: <c>Administrador</c>.
    ///
    /// **Códigos de respuesta:**
    /// - `200 OK` → Reserva aprobada correctamente.  
    /// - `400 Bad Request` → La reserva no existe o no puede ser aprobada en su estado actual.  
    /// - `500 Internal Server Error` → Error inesperado del servidor.
    /// </remarks>
    /// <param name="idReserva">
    /// Identificador de la reserva que se desea aprobar.
    /// </param>
    /// <param name="ct">
    /// Token de cancelación para la operación asíncrona.
    /// </param>
    /// <returns>
    /// Una respuesta HTTP con un mensaje indicando el resultado de la operación.
    /// </returns>
    /// <response code="200">Reserva aprobada correctamente.</response>
    /// <response code="400">La reserva no existe o no puede ser aprobada.</response>
    /// <response code="500">Error inesperado del servidor.</response>
    [HttpPatch("aprobar/{idReserva:int}")]
    [Authorize(Roles = "Administrador,Cliente")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Aprobar([FromRoute] int idReserva, CancellationToken ct)
    {
        try
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idClaim))
                return Unauthorized(new { message = "Token inválido o expirado." });

            var idUsuario = int.Parse(idClaim);
            var esAdmin = User.IsInRole("Administrador");

            await _aprobarReserva.Ejecutar(idReserva, idUsuario, esAdmin, ct);

            return Ok(new
            {
                message = esAdmin ? "Reserva aprobada con éxito." : "Reserva confirmada con éxito."
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Error inesperado." });
        }
    }


    /// <summary>
    /// Cancela una reserva existente por su identificador.
    /// </summary>
    /// <remarks>
    /// Este endpoint permite marcar una reserva como <c>Cancelada</c>, aplicando las reglas
    /// de negocio correspondientes (por ejemplo, que la reserva exista y se encuentre en un
    /// estado que permita la cancelación).
    ///
    /// **Flujo:**
    /// 1. Se recibe el identificador de la reserva por ruta.  
    /// 2. Se invoca el servicio <see cref="ICancelarReserva"/> para ejecutar la cancelación.  
    /// 3. Si la operación es exitosa, se retorna un mensaje de confirmación.
    ///
    /// **Requiere autenticación:**  
    /// - Rol permitido: <c>Administrador</c>.
    ///
    /// **Códigos de respuesta:**
    /// - `200 OK` → Reserva cancelada correctamente.  
    /// - `400 Bad Request` → La reserva no existe o no puede ser cancelada.  
    /// - `500 Internal Server Error` → Error inesperado del servidor.
    /// </remarks>
    /// <param name="idReserva">
    /// Identificador de la reserva que se desea cancelar.
    /// </param>
    /// <param name="ct">
    /// Token de cancelación para la operación asíncrona.
    /// </param>
    /// <returns>
    /// Una respuesta HTTP con un mensaje indicando el resultado de la operación.
    /// </returns>
    /// <response code="200">Reserva cancelada correctamente.</response>
    /// <response code="400">La reserva no existe o no puede ser cancelada.</response>
    /// <response code="500">Error inesperado del servidor.</response>
        [HttpPatch("cancelar/{idReserva:int}")]
        [Authorize(Roles = "Administrador,Cliente")]
        [ProducesResponseType(typeof(IEnumerable<HistoricoReservaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Cancelar([FromRoute] int idReserva, CancellationToken ct)
        {
            try
            {
                await _cancelarReserva.Ejecutar(idReserva, ct);
                return Ok(new { message = "Reserva cancelada con éxito." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Error inesperado." });
            }
        }
    
    
    /// <summary>
    /// Modifica una reserva existente.
    /// </summary>
    /// <remarks>
    /// Este endpoint permite actualizar ciertos datos de una reserva (fecha, horario, dirección,
    /// servicios, etc.), siempre que se encuentre en un estado que lo permita (por ejemplo, Pendiente
    /// o Confirmada).
    ///
    /// **Flujo:**
    /// 1. Se recibe un objeto <see cref="ReservaAModificarDto"/> con los nuevos datos de la reserva.  
    /// 2. Se valida que la reserva exista y que su estado permita la modificación.  
    /// 3. Se invoca el servicio <see cref="IModificarReserva"/> para aplicar los cambios.  
    /// 4. Si la operación es exitosa, se retorna un mensaje de confirmación.
    ///
    /// **Requiere autenticación:**  
    /// - Rol permitido: <c>Administrador</c>.
    ///
    /// **Códigos de respuesta:**
    /// - `200 OK` → Reserva modificada correctamente.  
    /// - `400 Bad Request` → La reserva no existe o los datos proporcionados son inválidos.  
    /// - `500 Internal Server Error` → Error inesperado del servidor.
    /// </remarks>
    /// <param name="dto">
    /// Datos necesarios para modificar la reserva, incluyendo su identificador y los campos a actualizar.
    /// </param>
    /// <param name="ct">
    /// Token de cancelación para la operación asíncrona.
    /// </param>
    /// <returns>
    /// Una respuesta HTTP con un mensaje indicando el resultado de la modificación.
    /// </returns>
    /// <response code="200">Reserva modificada correctamente.</response>
    /// <response code="400">La reserva no existe o los datos son inválidos.</response>
    /// <response code="500">Error inesperado del servidor.</response>
    [HttpPatch("modificar")]
    [Authorize(Roles = "Administrador,Cliente")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Modificar([FromBody] ReservaAModificarDto dto, CancellationToken ct)
    {
        try
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idClaim))
                return Unauthorized(new { message = "Token inválido o expirado." });

            var esAdmin = User.IsInRole("Administrador");

            await _modificarReserva.Ejecutar(dto, esAdmin, ct);

            return Ok(new
            {
                message = esAdmin
                    ? "Cambio sugerido. La reserva quedó pendiente para que el cliente la apruebe."
                    : "Cambio sugerido. La reserva quedó pendiente para revisión del representante."
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Error inesperado." });
        }
    }
    
        /// <summary>
    /// Crea una reserva de presupuesto para un usuario específico (flujo administrador).
    /// </summary>
    /// <remarks>
    /// Solo disponible para usuarios con rol <c>Administrador</c>.
    ///
    /// **Flujo:**
    /// 1. El administrador selecciona un usuario (cliente) en el panel.  
    /// 2. Envía el id de ese usuario y los datos de la reserva.  
    /// 3. Se reutiliza el caso de uso <see cref="IAgendarReserva"/> para crear la reserva
    ///    asociada al usuario seleccionado.
    ///
    /// **Códigos de respuesta:**
    /// - `200 OK` → Reserva creada correctamente.  
    /// - `400 Bad Request` → Datos inválidos.  
    /// - `409 Conflict` → Conflicto de reserva (horario ocupado, etc.).  
    /// - `500 Internal Server Error` → Error inesperado.
    /// </remarks>
    /// <param name="idUsuario">
    /// Identificador del usuario para el que se creará la reserva.
    /// </param>
    /// <param name="nuevaReserva">  

    /// Datos necesarios para crear la reserva (fecha, hora, dirección, servicios).
    /// </param>
    /// <param name="ct">Token de cancelación.</param>
    [HttpPost("admin/agendar/{idUsuario:int}")]
    [Authorize(Roles = "Administrador,Cliente")]
    [ProducesResponseType(typeof(ReservaCreadaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AgendarParaUsuario(
        [FromRoute] int idUsuario,
        [FromBody] ReservaACrearDto nuevaReserva,
        CancellationToken ct)
    {
        try
        {
            if (idUsuario <= 0)
            {
                return BadRequest(new { message = "El id de usuario es inválido." });
            }

            var reservaCreada = await _agendarReserva.Ejecutar(nuevaReserva, idUsuario, ct);

            return Ok(reservaCreada);
        }
        catch (ReservaException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (EmailNotificacionException)
        {
            return StatusCode(500, new { message = "La reserva fue creada, pero hubo un problema al enviar la notificación por email." });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Error inesperado." });
        }
    }

    
    /// <summary>
    /// Obtiene el listado completo de reservas asociadas a un cliente específico.
    /// </summary>
    /// <remarks>
    /// Este endpoint está destinado al uso administrativo y permite:
    /// <list type="bullet">
    /// <item>
    /// Consultar todas las reservas (pasadas y futuras) de un cliente.
    /// </item>
    /// <item>
    /// Visualizar el historial de reservas desde el panel de administración.
    /// </item>
    /// </list>
    ///
    /// <b>Acceso restringido:</b>
    /// <br/>
    /// Requiere autenticación con rol <c>Administrador</c>.
    /// </remarks>

        [HttpGet("Admin/Cliente/{clienteId:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<IEnumerable<ReservaListadoDto>>> GetReservasPorClienteAdmin(int clienteId, CancellationToken ct)
        {
            var reservas = await _obtenerReservasPorCliente.EjecutarAsync(clienteId, ct);
            return Ok(reservas);
        }
    
    /// <summary>
    /// Registra una reserva histórica (ya realizada) para un usuario específico.
    /// </summary>
    /// <remarks>
    /// Este endpoint permite a un administrador crear manualmente reservas pasadas,
    /// generalmente con fines administrativos, históricos o de carga inicial de datos.
    ///
    /// <b>Comportamiento especial:</b>
    /// <list type="bullet">
    /// <item>
    /// No aplica validaciones de agenda ni restricciones de fecha.
    /// </item>
    /// <item>
    /// La reserva se registra directamente como histórica.
    /// </item>
    /// </list>
    ///
    /// <b>Acceso restringido:</b>
    /// <br/>
    /// Requiere autenticación con rol <c>Administrador</c>.
    /// </remarks>

        [HttpPost("admin/registrar-historico/{idUsuario:int}")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(ReservaCreadaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegistrarHistoricoParaUsuario(
            [FromRoute] int idUsuario,
            [FromBody] ReservaACrearDto nuevaReserva,
            CancellationToken ct)
        {
            try
            {
                if (idUsuario <= 0)
                    return BadRequest(new { message = "El id de usuario es inválido." });

                var reservaCreada = await _registrarReservaHistoricaAdmin.Ejecutar(nuevaReserva, idUsuario, ct);

                return Ok(reservaCreada);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Error inesperado." });
            }
        }
    
    
    /// <summary>
    /// Obtiene el listado de reservas asociadas al usuario cliente autenticado.
    /// </summary>
    /// <remarks>
    /// Este endpoint devuelve únicamente las reservas pertenecientes al cliente
    /// que realiza la solicitud, identificado a partir del token JWT.
    /// Incluye el detalle necesario para su visualización en el panel del cliente.
    /// </remarks>
    /// <param name="ct">
    /// Token de cancelación para abortar la operación si la solicitud es cancelada.
    /// </param>
    /// <returns>
    /// Un listado de reservas del cliente autenticado.
    /// </returns>
    /// <response code="200">
    /// Devuelve el listado de reservas del cliente.
    /// </response>
    /// <response code="401">
    /// El token es inválido, expiró o el usuario no está autenticado.
    /// </response>
    /// <response code="500">
    /// Ocurrió un error inesperado al obtener las reservas.
    /// </response>
    [HttpGet("mis-reservas")]
    [Authorize(Roles = "Cliente")]
    [ProducesResponseType(typeof(IEnumerable<ReservaListadoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ReservaListadoDto>>> GetMisReservas(CancellationToken ct)
    {
        try
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idClaim))
                return Unauthorized(new { message = "Token inválido o expirado." });

            var idUsuario = int.Parse(idClaim);

            var reservas = await _obtenerMisReservasClienteConDetalle.Ejecutar(idUsuario, ct);

            return Ok(reservas);
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Error inesperado." });
        }
    }

}