using Microsoft.EntityFrameworkCore;
using WinniElectricidad.LogicaNegocio.Entidades;

namespace WinniElectricidad.AccesoDatos.Repositorios.EF;

public class WinniElectricidadContext : DbContext
{
    public WinniElectricidadContext(DbContextOptions<WinniElectricidadContext> options) : base(options){}
    
    public WinniElectricidadContext(){}
    
    public DbSet<UsuarioBase>  Usuarios { get; set; }
    public DbSet<Direccion>  Direcciones { get; set; }
    public DbSet<Reserva>  Reservas { get; set; }
    public DbSet<Presupuesto>  Presupuestos { get; set; }
    public DbSet<Pago>  Pagos { get; set; }
    public DbSet<Notificacion>  Notificaciones { get; set; }
}