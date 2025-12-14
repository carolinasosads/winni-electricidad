using Microsoft.EntityFrameworkCore;
using WinniElectricidad.LogicaNegocio.Entidades;

namespace WinniElectricidad.AccesoDatos.Repositorios.EF;

public class WinniElectricidadContext : DbContext
{
    public WinniElectricidadContext(DbContextOptions<WinniElectricidadContext> options) : base(options){}
    
    public DbSet<UsuarioBase>  Usuarios { get; set; }
    public DbSet<Direccion>  Direcciones { get; set; }
    public DbSet<Reserva>  Reservas { get; set; }
    public DbSet<Presupuesto>  Presupuestos { get; set; }
    public DbSet<Pago>  Pagos { get; set; }
    public DbSet<Notificacion>  Notificaciones { get; set; }
    public DbSet<Servicio>  Servicios { get; set; }
    public DbSet<Reseña>  Resenias { get; set; }
    public DbSet<OneTimeToken> OneTimeTokens { get; set; }
    public DbSet<Settings> Settings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UsuarioBase>()
            .HasDiscriminator<string>("Discriminator")
            .HasValue<UsuarioAdministrador>("UsuarioAdministrador")
            .HasValue<UsuarioCliente>("UsuarioCliente");

        modelBuilder.Entity<Direccion>(e =>
        {
            e.HasOne(d => d.UsuarioCliente)
                .WithMany(u => u.Direcciones)
                .HasForeignKey(d => d.IdUsuarioCliente)
                .OnDelete(DeleteBehavior.Cascade);

            e.Property(d => d.IdUsuarioCliente)
                .HasColumnName("IdUsuario");
        });

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

        modelBuilder.Entity<Servicio>(entity =>
        {
            entity.HasIndex(s => s.Titulo)
                .IsUnique();

            entity.Property(s => s.Titulo)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(s => s.Descripcion)
                .HasMaxLength(300);

            entity.Property(s => s.ImagenUrl)
                .HasMaxLength(500);

            entity.Property(s => s.Activo)
                .HasDefaultValue(true);
        });
        
        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(r => r.IdReserva);

            entity.Property(r => r.FechaReserva)
                .IsRequired();

            entity.Property(r => r.EstadoReserva)
                .HasConversion<int>()
                .IsRequired();

            entity.Property(r => r.TipoServicioReserva)
                .HasConversion<int>()
                .IsRequired();
            
            entity.HasOne(r => r.UsuarioCliente)
                .WithMany(c => c.Reservas)
                .HasForeignKey(r => r.IdUsuarioCliente)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.Direccion)
                .WithMany()
                .HasForeignKey(r => r.IdDireccion)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.Presupuesto)
                .WithOne(p => p.Reserva)
                .HasForeignKey<Presupuesto>(p => p.IdReserva)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(r => r.Servicios)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "ReservaServicio",
                    j => j.HasOne<Servicio>()
                        .WithMany()
                        .HasForeignKey("IdServicio")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<Reserva>()
                        .WithMany()
                        .HasForeignKey("IdReserva")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("IdReserva", "IdServicio");
                        j.ToTable("ReservaServicio");
                    });
//Índice unico para que no haya dos reservas en el mismo horario 
            entity.HasIndex(r => new { r.FechaReserva})
                .IsUnique(); 
        });

        modelBuilder.Entity<Settings>().HasData(
            new Settings
            {
                Id = 1,
                HoraInicio = new TimeOnly(9, 0),
                HoraFin = new TimeOnly(17, 0),
                MinutosEntreTurnos = 90,
                DiasMinimos = 2,
                DiasMaximos = 30
            }
        );

        modelBuilder.Entity<Servicio>().HasData(
                new Servicio
                {
                    Id = 1,
                    Titulo = "Electricidad",
                    Descripcion = "Descripción genérica para el servicio de Electricidad",
                    ImagenUrl = null,
                    Activo = true
                },
                new Servicio
                {
                    Id = 2,
                    Titulo = "Sanitaria",
                    Descripcion = "Descripción genérica para el servicio de Sanitaria",
                    ImagenUrl = null,
                    Activo = true
                },
                new Servicio
                {
                    Id = 3,
                    Titulo = "Climatización",
                    Descripcion = "Descripción genérica para el servicio de Climatización",
                    ImagenUrl = null,
                    Activo = true
                },
                new Servicio
                {
                    Id = 4,
                    Titulo = "Riego",
                    Descripcion = "Descripción genérica para el servicio de Riego",
                    ImagenUrl = null,
                    Activo = true
                }
            );
    }
}