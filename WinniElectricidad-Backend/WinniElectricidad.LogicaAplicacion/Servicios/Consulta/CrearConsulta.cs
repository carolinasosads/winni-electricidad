using WinniElectricidad.Compartido.DTOs.Consulta;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Consulta;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Notificacion;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Consultas;

public class CrearConsulta : ICrearConsulta
{
    private readonly IEnviarEmail _enviarEmail;
    private readonly IRepositorioUsuario _repositorioUsuario;

    public CrearConsulta(IEnviarEmail enviarEmail, IRepositorioUsuario repositorioUsuario)
    {
        _enviarEmail = enviarEmail;
        _repositorioUsuario = repositorioUsuario;
    }

    public async Task Ejecutar(ConsultaCrearDto dto, CancellationToken ct = default)
    {
        var admin = await _repositorioUsuario.ObtenerAdministrador(ct);

        if (admin is null)
            throw new InvalidOperationException(
                "No hay un usuario administrador para recibir la consulta."
            );

        if (dto.IdCliente.HasValue && dto.IdCliente.Value > 0)
        {
            var cliente = await _repositorioUsuario.FindById(dto.IdCliente.Value, ct);

            if (cliente is null)
                throw new InvalidOperationException("No se encontró el cliente asociado a la consulta.");

            dto.Nombre = string.IsNullOrWhiteSpace(dto.Nombre) ? cliente.NombreCompleto : dto.Nombre;
            dto.Email = string.IsNullOrWhiteSpace(dto.Email) ? cliente.Email : dto.Email;
            dto.Telefono = string.IsNullOrWhiteSpace(dto.Telefono) ? cliente.Telefono : dto.Telefono;
        }

        var nombre = (dto.Nombre ?? "").Trim();
        var email = (dto.Email ?? "").Trim();
        var telefono = (dto.Telefono ?? "").Trim();
        var mensaje = (dto.Mensaje ?? "").Trim();

        if (string.IsNullOrWhiteSpace(mensaje))
            throw new InvalidOperationException("El mensaje es obligatorio.");

        if (!dto.IdCliente.HasValue || dto.IdCliente.Value <= 0)
        {
            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(telefono))
            {
                throw new InvalidOperationException("Nombre, Email y Teléfono son obligatorios para usuarios no logueados.");
            }
        }

        var asuntoAdmin = "Nueva consulta";

        var footer = _enviarEmail.GetFooter();

        var cuerpoAdmin = $@"
            <div style=""font-family: Arial, sans-serif; background-color:#f4f6f8; padding:24px;"">
              <div style=""max-width:600px; margin:0 auto; background-color:#ffffff; border-radius:8px; overflow:hidden;"">
    
                <!-- Header -->
                <div style=""background-color:#1f3a5f; color:#ffffff; padding:16px 24px;"">
                  <h2 style=""margin:0; font-size:20px;"">Winni Electricidad</h2>
                </div>
    
                <!-- Body -->
                <div style=""padding:24px; color:#333333;"">
                  <h3 style=""margin-top:0; color:#1f3a5f;"">
                    Nueva consulta
                  </h3>
    
                  <p style=""margin:0 0 16px 0;"">
                    Se recibió una nueva consulta desde la página principal.
                  </p>
    
                  <p style=""margin:0 0 8px 0;""><strong>Nombre:</strong> {nombre}</p>
                  <p style=""margin:0 0 8px 0;""><strong>Email:</strong> {email}</p>
                  <p style=""margin:0 0 16px 0;""><strong>Teléfono:</strong> {telefono}</p>
    
                  <p style=""margin:0 0 8px 0;""><strong>Mensaje:</strong></p>
                  <p style=""margin:0 0 16px 0;"">{mensaje}</p>
    
                  <p style=""margin:0;"">
                    Utiliza el correo que se encuentra arriba para contactarte directamente con la persona.
                  </p>
    
                  {footer}
                </div>
    
              </div>
            </div>";

        await _enviarEmail.Ejecutar(admin.Email, asuntoAdmin, cuerpoAdmin, ct);
    }
}
