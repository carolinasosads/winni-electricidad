using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Servicio;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Servicio;

public class ActivarServicio : IActivarServicio
{
    private readonly IRepositorioServicio _repositorioServicio;

    public ActivarServicio(IRepositorioServicio repositorioServicio)
    {
        _repositorioServicio = repositorioServicio;
    }
    
    public async Task Ejecutar(int idServicio, CancellationToken cancellationToken = default)
    {
        var servicio = await _repositorioServicio.FindById(idServicio, cancellationToken);
        
        if (servicio is null)
            throw new ArgumentException("El servicio no existe.");

        servicio.Activar();
        await _repositorioServicio.Update(servicio, cancellationToken);
    }
}