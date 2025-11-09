using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WinniElectricidad.AccesoDatos.Repositorios.EF;

namespace WinniElectricidad.Tests.Medium.Utils;

/// <summary>
/// Proporciona métodos auxiliares para crear contextos de prueba
/// utilizando una base de datos SQLite en memoria.
/// </summary>
public static class DbContextHelper
{
    /// <summary>
    /// Crea una nueva instancia del contexto de base de datos WinniElectricidad utilizando SQLite en memoria. 
    /// La conexión se mantiene abierta mientras el contexto esté en uso.
    /// </summary>
    /// <returns>
    /// Una tupla con:
    /// - El contexto listo para usar.
    /// - La conexión SQLite (debe mantenerse abierta hasta que finalice el test).
    /// </returns>
    public static async Task<(WinniElectricidadContext context, SqliteConnection connection)> CrearContextoSqliteEnMemoria()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<WinniElectricidadContext>()
            .UseSqlite(connection)
            .Options;

        var context = new WinniElectricidadContext(options);

        await context.Database.EnsureCreatedAsync();
        
        await context.Database.ExecuteSqlRawAsync("PRAGMA foreign_keys = OFF;");

        var entityTypes = context.Model.GetEntityTypes();
        var tableNames = entityTypes
            .Select(t => t.GetTableName())
            .Distinct()
            .Where(name => !string.IsNullOrEmpty(name))
            .ToList();
        
        foreach (var table in tableNames)
        {
            await context.Database.ExecuteSqlRawAsync($"DELETE FROM \"{table}\";");
            await context.Database.ExecuteSqlRawAsync($"DELETE FROM sqlite_sequence WHERE name='{table}';");
        }
        
        await context.Database.ExecuteSqlRawAsync("PRAGMA foreign_keys = ON;");

        await context.SaveChangesAsync();
        return (context, connection);
    }
}