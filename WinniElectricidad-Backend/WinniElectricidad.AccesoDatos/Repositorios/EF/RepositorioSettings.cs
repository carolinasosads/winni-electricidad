using Microsoft.EntityFrameworkCore;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.AccesoDatos.Repositorios.EF;

public class RepositorioSettings : IRepositorioSettings
{
    private readonly WinniElectricidadContext _db;

    public RepositorioSettings(WinniElectricidadContext db)
    {
        _db = db;
    }
    
    public async Task<Settings> Obtener(CancellationToken ct = default)
    {
        return (await _db.Settings.FirstOrDefaultAsync(ct))!;
    }
}