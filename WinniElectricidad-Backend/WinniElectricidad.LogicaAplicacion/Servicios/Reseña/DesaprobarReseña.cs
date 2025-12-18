using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reseña;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Reseña;

public class DesaprobarReseña : IDesaprobarReseña
{
    private readonly IRepositorioReseña _repositorioReseña;

    public DesaprobarReseña(IRepositorioReseña repositorioReseña)
    {
        _repositorioReseña = repositorioReseña;
    }
    
    public async Task Ejecutar(int idReseña, CancellationToken cancellationToken = default)
    {
        var reseña = await _repositorioReseña.FindById(idReseña, cancellationToken);
        
        if (reseña is null)
            throw new ArgumentException("La reseña no existe.");

        reseña.Desaprobar();
        await _repositorioReseña.Update(reseña, cancellationToken);
    }
}