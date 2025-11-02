using Microsoft.EntityFrameworkCore;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.AccesoDatos.Repositorios.EF;

public class RepositorioUsuarios : IRepositorioUsuario
{
    private WinniElectricidadContext _db;

    public RepositorioUsuarios(WinniElectricidadContext db)
    {
        _db = db;
    }
    
    public void Add(UsuarioBase obj)
    {
        throw new NotImplementedException();
    }

    public UsuarioBase FindById(int id)
    {
        throw new NotImplementedException();
    }

    public void Update(UsuarioBase obj)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<UsuarioBase> FindAll()
    {
        throw new NotImplementedException();
    }

    public async Task<UsuarioBase?> FindbyEmail(string email)
    {
        var usuario = await _db.Usuarios.FirstOrDefaultAsync(x => x.Email == email);
        return usuario;
    }

    public async Task<UsuarioBase?> Login(string email, string password)
    {
        var usuarioBuscado = await FindbyEmail(email);
        
        if (usuarioBuscado is not null && usuarioBuscado.PasswordHash == password)
        {
            return  usuarioBuscado;
        }
        
        return null;
    }
}