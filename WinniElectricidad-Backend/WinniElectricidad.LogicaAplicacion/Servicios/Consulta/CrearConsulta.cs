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

        var cuerpoAdmin = $@"
            <div style='font-family: Arial, sans-serif; color: #333;'>
                <h2>Nueva consulta</h2>

                <p>Se recibió una nueva consulta desde la página principal.</p>

                <p><strong>Nombre:</strong> {nombre}</p>
                <p><strong>Email:</strong> {email}</p>
                <p><strong>Teléfono:</strong> {telefono}</p>

                <p><strong>Mensaje:</strong></p>
                <p>{mensaje}</p>

                <p>Respondé este correo para contactarte directamente con la persona.</p>
            </div>";

        await _enviarEmail.Ejecutar(admin.Email, asuntoAdmin, cuerpoAdmin, ct);
    }
}
