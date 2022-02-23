namespace ORION.M2.DbContext.Data;

using Microsoft.EntityFrameworkCore;
using ORION.M2.DbContext.Models;

public class ApplicationDbContext : DbContext
{
    public DbSet<Person>? Person { get; set; }
    public DbSet<Permission>? Permission { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var conn = $@"Data Source={baseDir}\\orion.db";
        optionsBuilder.UseSqlite(conn);
    }
}

