using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using TCP.App.Data.Entities;

namespace TCP.App.Data;

public class AppDbContext : DbContext
{
    public DbSet<ScenarioDb> Scenarios { get; set; } = null!;
    public DbSet<CustomCodeDb> CustomCodes { get; set; } = null!;

    public AppDbContext()
    {
        InitializeDatabase();
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        // SQLite veritabanı dosyası için dizini oluşturur
        var dbPath = GetDatabasePath();
        var dir = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        // Veritabanını oluşturur veya günceller
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite($"Data Source={GetDatabasePath()}");
        }
    }

    private string GetDatabasePath()
    {
        var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TCP");
        return Path.Combine(appData, "tcp_scenarios.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // İleride konfigürasyon eklenebilir
    }
}
