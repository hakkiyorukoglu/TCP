using System;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TCP.App.Data;
using TCP.App.Data.Entities;
using Xunit;

namespace TCP.Tests;

public class DatabaseTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly SqliteConnection _connection;

    public DatabaseTests()
    {
        // In-memory veritabanı bağlantısını açık tutmak gerekir, aksi halde silinir
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _connection.Close();
        _context.Dispose();
    }

    [Fact]
    public void Can_Add_And_Retrieve_Scenario()
    {
        // Arrange
        var scenario = new ScenarioDb
        {
            Id = Guid.NewGuid(),
            Name = "Test Scenario",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            NetworkStateJson = "{}",
            EditorLayoutJson = "{}"
        };

        // Act
        _context.Scenarios.Add(scenario);
        _context.SaveChanges();

        var retrieved = _context.Scenarios.FirstOrDefault(s => s.Id == scenario.Id);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal("Test Scenario", retrieved.Name);
    }

    [Fact]
    public void Can_Delete_Scenario()
    {
        // Arrange
        var scenario = new ScenarioDb
        {
            Id = Guid.NewGuid(),
            Name = "To Be Deleted",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            NetworkStateJson = "{}",
            EditorLayoutJson = "{}"
        };
        _context.Scenarios.Add(scenario);
        _context.SaveChanges();

        // Act
        var toDelete = _context.Scenarios.First();
        _context.Scenarios.Remove(toDelete);
        _context.SaveChanges();

        var retrieved = _context.Scenarios.FirstOrDefault(s => s.Id == scenario.Id);

        // Assert
        Assert.Null(retrieved);
        Assert.Empty(_context.Scenarios);
    }
}
