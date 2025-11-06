using Microsoft.EntityFrameworkCore;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Usuarios;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.AccesoDatos.Repositorios.EF;

public class RepositorioUsuarios : IRepositorioUsuario
{
    private readonly WinniElectricidadContext _db;

    public RepositorioUsuarios(WinniElectricidadContext db)
    {
        _db = db;
    }
    
    public Task Add(UsuarioBase obj, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<UsuarioBase?> FindById(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
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
    public async Task<UsuarioCliente?> Registro(UsuarioCliente usuarioCliente)
    {
        await _db.Usuarios.AddAsync(usuarioCliente);
        await _db.SaveChangesAsync();

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
}