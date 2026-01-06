using WinniElectricidad.Compartido.DTOs.Consulta;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Consulta;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Notificacion;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Consultas;

public class CrearConsulta : ICrearConsulta
{
    private readonly IEnviarEmail _enviarEmail;
    private readonly IRepositorioUsuario _repositorioUsuario;

    public CrearConsulta(
        IEnviarEmail enviarEmail,
        IRepositorioUsuario repositorioUsuario)
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

        var asuntoAdmin = "Nueva consulta";

        var cuerpoAdmin = $@"
            <div style='font-family: Arial, sans-serif; color: #333;'>
                <h2>Nueva consulta</h2>

                <p>Se recibió una nueva consulta desde la página principal.</p>

                <p><strong>Nombre:</strong> {dto.Nombre}</p>
                <p><strong>Email:</strong> {dto.Email}</p>
                <p><strong>Teléfono:</strong> {dto.Telefono}</p>

                <p><strong>Mensaje:</strong></p>
                <p>{dto.Mensaje}</p>

                <p>Respondé este correo para contactarte directamente con la persona.</p>
            </div>";

        await _enviarEmail.Ejecutar(admin.Email, asuntoAdmin, cuerpoAdmin, ct);
    }
}