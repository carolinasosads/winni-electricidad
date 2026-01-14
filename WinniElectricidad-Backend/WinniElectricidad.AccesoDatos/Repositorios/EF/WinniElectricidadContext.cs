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
    public DbSet<ServicioImagen> ServicioImagenes { get; set; }
    public DbSet<Reseña>  Resenias { get; set; }
    public DbSet<OneTimeToken> OneTimeTokens { get; set; }
    public DbSet<Settings> Settings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // -------------------- USUARIOS --------------------
        modelBuilder.Entity<UsuarioBase>()
            .HasDiscriminator<string>("Discriminator")
            .HasValue<UsuarioAdministrador>("UsuarioAdministrador")
            .HasValue<UsuarioCliente>("UsuarioCliente");
        
        modelBuilder.Entity<UsuarioCliente>()
            .HasMany(u => u.Reseñas)
            .WithOne()
            .HasForeignKey(r => r.IdUsuario)
            .OnDelete(DeleteBehavior.Restrict);

        // -------------------- DIRECCIONES --------------------
        modelBuilder.Entity<Direccion>(e =>
        {
            e.HasOne(d => d.UsuarioCliente)
                .WithMany(u => u.Direcciones)
                .HasForeignKey(d => d.IdUsuarioCliente)
                .OnDelete(DeleteBehavior.Cascade);

            e.Property(d => d.IdUsuarioCliente)
                .HasColumnName("IdUsuario");
        });

        // -------------------- ONE TIME TOKEN --------------------
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

        // -------------------- SERVICIOS --------------------
        modelBuilder.Entity<Servicio>(entity =>
        {
            entity.HasIndex(s => s.Titulo)
                .IsUnique();

            entity.Property(s => s.Titulo)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(s => s.Descripcion)
                .HasMaxLength(300);

            entity.Property(s => s.Activo)
                .HasDefaultValue(true);
            
            entity.HasMany(s => s.Reseñas)
                .WithOne()
                .HasForeignKey(r => r.IdServicio)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.Property(s => s.Icono)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Otro");
        });
        
        // -------------------- SERVICIO IMAGENES --------------------
        modelBuilder.Entity<ServicioImagen>(entity =>
        {
            entity.HasKey(i => i.Id);

            entity.Property(i => i.Url)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(i => i.EsPrincipal)
                .IsRequired();

            entity.HasOne(i => i.Servicio)
                .WithMany(s => s.Imagenes)
                .HasForeignKey(i => i.ServicioId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // -------------------- RESERVAS --------------------
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
            entity.HasIndex(r => r.FechaReserva)
                .IsUnique()
                .HasFilter($"[EstadoReserva] <> {(int)EstadoReserva.Cancelada}");
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
        // -------------------- PRESUPUESTOS --------------------
        modelBuilder.Entity<Presupuesto>(entity =>
        {
            entity.Property(p => p.MontoPagado)
                .HasPrecision(18, 2);

            entity.Property(p => p.Monto)
                .HasPrecision(18, 2);
            
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Monto)
                .HasPrecision(18, 2);

            entity.Property(p => p.Notas)
                .HasMaxLength(500);

            entity.Property(p => p.FechaPresupuesto)
                .IsRequired();

            entity.Property(p => p.IdUsuario)
                .IsRequired();
            
            entity.HasOne(p => p.Reserva)
                .WithOne(r => r.Presupuesto)
                .HasForeignKey<Presupuesto>(p => p.IdReserva)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // -------------------- PAGOS --------------------
        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(p => p.IdPago);

            entity.Property(p => p.Monto)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(p => p.FechaHoraRealizado)
                .IsRequired();

            entity.HasOne(p => p.Usuario)
                .WithMany(u => u.Pagos)
                .HasForeignKey(p => p.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.Presupuesto)
                .WithMany(x => x.Pagos)
                .HasForeignKey(p => p.IdPresupuesto)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Servicio>().HasData(
                new Servicio
                {
                    Id = 1,
                    Titulo = "Electricidad",
                    Descripcion = "Instalaciones, reparaciones y mantenimiento eléctrico en hogares y comercios.",
                    Activo = true,
                    Icono = "Electricidad"
                },
                new Servicio
                {
                    Id = 2,
                    Titulo = "Sanitaria",
                    Descripcion = "Instalación y reparación de cañerías, griferías y artefactos sanitarios.",
                    Activo = true,
                    Icono = "Sanitaria"
                },
                new Servicio
                {
                    Id = 3,
                    Titulo = "Climatización",
                    Descripcion = "Instalación y mantenimiento de sistemas de aire acondicionado y calefacción.",
                    Activo = true,
                    Icono = "Climatización"
                },
                new Servicio
                {
                    Id = 4,
                    Titulo = "Riego",
                    Descripcion = "Instalación y mantenimiento de sistemas de riego para jardines.",
                    Activo = true,
                    Icono = "Riego"
                },
                new Servicio
                {
                    Id = 5,
                    Titulo = "Otro",
                    Descripcion = "Servicios técnicos generales sujetos a evaluación previa.",
                    Activo = true,
                    Icono = "Otro"
                }
        );

        modelBuilder.Entity<ServicioImagen>().HasData(
            // ==================== ELECTRICIDAD ====================
            new { Id = 1, Url = "https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/electricidad.jpg",  EsPrincipal = true,  ServicioId = 1 },
            new { Id = 2, Url = "https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/electricidad2.jpg", EsPrincipal = false, ServicioId = 1 },
            new { Id = 3, Url = "https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/electricidad3.jpg", EsPrincipal = false, ServicioId = 1 },
            new { Id = 4, Url = "https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/electricidad4.jpg", EsPrincipal = false, ServicioId = 1 },
            new { Id = 5, Url = "https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/electricidad5.jpg", EsPrincipal = false, ServicioId = 1 },
            new { Id = 6, Url = "https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/electricidad6.jpg", EsPrincipal = false, ServicioId = 1 },
            new { Id = 7, Url = "https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/electricidad7.jpg", EsPrincipal = false, ServicioId = 1 },
            new { Id = 8, Url = "https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/electricidad8.jpg", EsPrincipal = false, ServicioId = 1 },

            // ==================== SANITARIA ====================
            new { Id = 9,  Url = "https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/sanitaria.jpg",  EsPrincipal = true,  ServicioId = 2 },
            new { Id = 10, Url = "https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/sanitaria2.jpg", EsPrincipal = false, ServicioId = 2 },
            new { Id = 11, Url = "https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/sanitaria3.jpg", EsPrincipal = false, ServicioId = 2 },
            new { Id = 12, Url = "https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/sanitaria4.jpg", EsPrincipal = false, ServicioId = 2 },
            new { Id = 13, Url = "https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/sanitaria5.jpg", EsPrincipal = false, ServicioId = 2 },

            // ==================== CLIMATIZACIÓN ====================
            new { Id = 14, Url = "https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/climatizacion.jpg",  EsPrincipal = true,  ServicioId = 3 },
            new { Id = 15, Url = "https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/climatizacion2.jpg", EsPrincipal = false, ServicioId = 3 },
            new { Id = 16, Url = "https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/climatizacion3.jpg", EsPrincipal = false, ServicioId = 3 },

            // ==================== RIEGO ====================
            new { Id = 17, Url = "https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/riego.jpg",  EsPrincipal = true,  ServicioId = 4 },
            new { Id = 18, Url = "https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/riego2.jpg", EsPrincipal = false, ServicioId = 4 },

            // ==================== OTROS ====================
            new { Id = 19, Url = "https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/otros.jpg",  EsPrincipal = true,  ServicioId = 5 },
            new { Id = 20, Url = "https://winnielectricidadstorage.blob.core.windows.net/imagenes/servicios/otros2.jpg", EsPrincipal = false, ServicioId = 5 }
        );
    }
}