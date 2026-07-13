using System;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TCP.App.Data;
using TCP.App.Services;
using Xunit;

namespace TCP.Tests;

public class ProjectManagerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly SqliteConnection _connection;

    public ProjectManagerTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();

        // Inject the factory into the singleton instance
        ProjectManager.Instance.DbContextFactory = () => new AppDbContext(options);
        
        // Reset singleton state for a fresh test run
        ProjectManager.Instance.IsDirty = false;
        // _currentScenario is private, but calling LoadScenario or creating clears it properly.
    }

    public void Dispose()
    {
        _connection.Close();
        _context.Dispose();
        ProjectManager.Instance.DbContextFactory = () => new AppDbContext(); // Restore to default
    }

    [Fact]
    public void CreateNewScenario_Adds_To_Database_And_Loads_It()
    {
        // Act
        ProjectManager.Instance.CreateNewScenario("Test PM Scenario");

        // Assert
        var scenario = _context.Scenarios.FirstOrDefault(s => s.Name == "Test PM Scenario");
        Assert.NotNull(scenario);
        
        Assert.NotNull(ProjectManager.Instance.CurrentScenario);
        Assert.Equal("Test PM Scenario", ProjectManager.Instance.CurrentScenario.Name);
        Assert.False(ProjectManager.Instance.IsDirty, "Newly created scenario should not be dirty.");
    }

    [Fact]
    public void DeleteScenario_Removes_From_Database_And_Clears_Current_If_Same()
    {
        // Arrange
        ProjectManager.Instance.CreateNewScenario("To Delete");
        var scenarioId = ProjectManager.Instance.CurrentScenario!.Id;

        // Act
        ProjectManager.Instance.DeleteScenario(scenarioId);

        // Assert
        var exists = _context.Scenarios.Any(s => s.Id == scenarioId);
        Assert.False(exists);
        Assert.Null(ProjectManager.Instance.CurrentScenario);
        Assert.False(ProjectManager.Instance.IsDirty);
    }
    
    [Fact]
    public void GetAllScenarios_Returns_List_From_Database()
    {
        // Arrange
        ProjectManager.Instance.CreateNewScenario("Scen 1");
        ProjectManager.Instance.CreateNewScenario("Scen 2");

        // Act
        var list = ProjectManager.Instance.GetAllScenarios();

        // Assert
        Assert.True(list.Count >= 2);
        Assert.Contains(list, s => s.Name == "Scen 1");
        Assert.Contains(list, s => s.Name == "Scen 2");
    }
}
