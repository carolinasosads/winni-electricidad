using Microsoft.EntityFrameworkCore;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.AccesoDatos.Repositorios.EF;

public class RepositorioUsuarios : IRepositorioUsuario
{
    private readonly WinniElectricidadContext _db;

    public RepositorioUsuarios(WinniElectricidadContext db)
    {
        _db = db;
    }
    
    public Task<UsuarioBase?> Add(UsuarioBase obj, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<UsuarioBase?> FindById(int id, CancellationToken ct = default)
    {
        var usuario = await _db.Usuarios.Where(x => x.IdUsuario == id).FirstOrDefaultAsync(ct);
        return usuario;
    }
    
    public async Task<IReadOnlyList<UsuarioBase>> FindByIds(List<int> ids, CancellationToken ct = default)
    {
        return await _db.Usuarios
            .Where(u => ids.Contains(u.IdUsuario))
            .ToListAsync(ct);
    }

    public Task Update(UsuarioBase obj, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task Delete(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<UsuarioBase>> FindAll(CancellationToken ct = default)
    {
        return await _db.Usuarios
            .OfType<UsuarioCliente>()
            .AsNoTracking()
            .OrderBy(u => u.IdUsuario)
            .ToListAsync(ct); 
    }
    
    public async Task<IReadOnlyList<UsuarioCliente>> FindAllFilteredByService(int idServicio, CancellationToken ct = default)
    {
        return await _db.Usuarios
            .OfType<UsuarioCliente>()
            .Where(u =>
                u.Reservas.Any(r =>
                    r.Servicios.Any(s => s.Id == idServicio)
                )
            )
            .AsNoTracking()
            .OrderBy(u => u.NombreCompleto)
            .ToListAsync(ct); 
    }

    public async Task<UsuarioBase?> FindbyEmail(string email, CancellationToken ct = default)
    {
        var usuario = await _db.Usuarios.Where(x => x.Email == email).FirstOrDefaultAsync(ct);
        return usuario;
    }
    
    public async Task<UsuarioCliente?> Registro(UsuarioCliente usuarioCliente, CancellationToken ct = default)
    {
        await _db.Usuarios.AddAsync(usuarioCliente, ct);
        await _db.SaveChangesAsync(ct);

        return usuarioCliente;
    }


    public async Task ChangePassword(int idUsuario, string passwordHash, CancellationToken ct = default)
    {
        await _db.Usuarios
            .Where(u => u.IdUsuario == idUsuario)
            .ExecuteUpdateAsync(
                s => s.SetProperty(u => u.PasswordHash, passwordHash),
                ct
            );
    }

    public async Task<IReadOnlyList<Direccion>> FindAddressByUserId(int idUsuario, CancellationToken ct = default)
    {
        return await _db.Usuarios
            .OfType<UsuarioCliente>()                 
            .Where(c => c.IdUsuario == idUsuario)
            .SelectMany(c => c.Direcciones)
            .ToListAsync(ct);
    }
    public async Task<UsuarioAdministrador?> ObtenerAdministrador(CancellationToken ct = default)
    {
        return await _db.Usuarios
            .OfType<UsuarioAdministrador>()
            .Where(u => u.NombreCompleto == "Administrador del Sistema")
            .FirstOrDefaultAsync(ct);   
    }
    
    public async Task<ICollection<UsuarioCliente>> BuscarPorNombreEmailTelefono(string dato, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dato))
            return new List<UsuarioCliente>();

        dato = dato.ToLower().Trim();

        return await _db.Usuarios
            .OfType<UsuarioCliente>()          
            .AsNoTracking()
            .Where(u =>
                (u.NombreCompleto != null && u.NombreCompleto.ToLower().Contains(dato)) ||
                (u.Email != null && u.Email.ToLower().Contains(dato)) ||
                (u.Telefono != null && u.Telefono.ToLower().Contains(dato))
            )
            .OrderBy(u => u.NombreCompleto)
            .ToListAsync(ct);
    }
    
    public async Task<UsuarioCliente?> FindClienteDetalleById(int idUsuario, CancellationToken ct = default)
    {
        return await _db.Usuarios
            .OfType<UsuarioCliente>()
            .AsNoTracking()
            .Include(u => u.Direcciones)
            .Include(u => u.Reservas)
            .Include(u => u.Presupuestos)
            .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario, ct);
    }
}