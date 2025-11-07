using Microsoft.EntityFrameworkCore;
using WinniElectricidad.LogicaNegocio.Entidades;

namespace WinniElectricidad.AccesoDatos.Repositorios.EF;

public class WinniElectricidadContext : DbContext
{
    public WinniElectricidadContext(DbContextOptions<WinniElectricidadContext> options) : base(options){}
    
    public DbSet<UsuarioBase>  Usuarios { get; set; }
    public DbSet<Direccion>  Direcciones { get; set; }
    public DbSet<Reserva>  Reservas { get; set; }
    //public DbSet<Presupuesto>  Presupuestos { get; set; }
    public DbSet<Pago>  Pagos { get; set; }
    public DbSet<Notificacion>  Notificaciones { get; set; }
    public DbSet<OneTimeToken> OneTimeTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<UsuarioBase>()
            .HasDiscriminator<string>("Discriminator")
            .HasValue<UsuarioAdministrador>("UsuarioAdministrador")
            .HasValue<UsuarioCliente>("UsuarioCliente");

        modelBuilder.Entity<OneTimeToken>(e =>
        {
            e.ToTable("OneTimeTokens");
            e.HasKey(x => x.Id);
            e.Property(x => x.TokenHash)
                .HasMaxLength(64)
                .IsRequired();
            e.Property(x => x.Tipo)
                .HasConversion<string>()
                .IsRequired();
            e.Property(x => x.ExpiraEl).IsRequired();
            e.Property(x => x.Usado).HasDefaultValue(false);
            e.Property(x => x.CreadoEl).IsRequired();
            e.HasIndex(x => x.TokenHash).IsUnique();
            e.HasIndex(x => new { x.IdUsuario, x.Tipo, x.Usado, x.ExpiraEl });
            e.HasOne(x => x.Usuario)
                .WithMany()
                .HasForeignKey(x => x.IdUsuario)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}