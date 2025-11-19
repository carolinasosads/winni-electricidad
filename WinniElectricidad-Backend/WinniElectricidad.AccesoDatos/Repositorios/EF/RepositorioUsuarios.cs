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

    public Task Update(UsuarioBase obj, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task Delete(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<UsuarioBase>> FindAll(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<UsuarioBase?> FindbyEmail(string email, CancellationToken ct = default)
    {
        var usuario = await _db.Usuarios.Where(x => x.Email == email).FirstOrDefaultAsync(ct);
        return usuario;
    }

    public async Task<UsuarioBase?> Login(string email, string password, CancellationToken ct = default)
    {
        var usuarioBuscado = await FindbyEmail(email, ct);
        
        if (usuarioBuscado is not null && usuarioBuscado.PasswordHash == password)
        {
            return  usuarioBuscado;
        }
        
        return null;
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
}