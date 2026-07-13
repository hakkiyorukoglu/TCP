using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Windows;
using TCP.App.Data;
using TCP.App.Data.Entities;
using TCP.App.Models.Editor;
using TCP.App.ViewModels;

namespace TCP.App.Services;

public class ProjectManager
{
    private static ProjectManager? _instance;
    public static ProjectManager Instance => _instance ??= new ProjectManager();

    public Func<AppDbContext> DbContextFactory { get; set; } = () => new AppDbContext();
    private AppDbContext CreateDbContext() => DbContextFactory();

    private ScenarioDb? _currentScenario;
    public ScenarioDb? CurrentScenario => _currentScenario;

    private bool _isDirty;
    public bool IsDirty
    {
        get => _isDirty;
        set
        {
            if (_isDirty != value)
            {
                _isDirty = value;
                IsDirtyChanged?.Invoke();
            }
        }
    }

    public event Action? IsDirtyChanged;
    public event Action? ScenarioLoaded;

    private ProjectManager()
    {
        // Subscribe to network changes
        NetworkManager.Instance.NetworkChanged += () => IsDirty = true;
    }

    public void MarkDirty()
    {
        IsDirty = true;
    }

    public List<ScenarioDb> GetAllScenarios()
    {
        try
        {
            using var db = CreateDbContext();
            return db.Scenarios.OrderByDescending(s => s.UpdatedAt).ToList();
        }
        catch (Exception ex)
        {
            TerminalService.Instance.LogError($"Failed to load scenarios list: {ex.Message}");
            return new List<ScenarioDb>();
        }
    }

    public void NewScenario(string name = "Yeni Senaryo")
    {
        if (!PromptForSaveIfDirty()) return;

        _currentScenario = new ScenarioDb { Name = name };
        
        // Clear network
        NetworkManager.Instance.ClearNetwork();
        
        // Editor will be cleared by the ViewModel listening to ScenarioLoaded
        IsDirty = false;
        ScenarioLoaded?.Invoke();
        TerminalService.Instance.LogSuccess($"Started new scenario: {name}");
    }

    public void CreateNewScenario(string name)
    {
        try
        {
            var scenario = new ScenarioDb
            {
                Id = Guid.NewGuid(),
                Name = name,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                NetworkStateJson = "{}",
                EditorLayoutJson = "{}"
            };

            using var db = CreateDbContext();
            db.Scenarios.Add(scenario);
            db.SaveChanges();

            LoadScenario(scenario.Id);
            TerminalService.Instance.LogSuccess($"New scenario '{name}' created.");
        }
        catch (Exception ex)
        {
            TerminalService.Instance.LogError($"Failed to create scenario: {ex.Message}");
        }
    }

    public void DeleteScenario(Guid id)
    {
        try
        {
            using var db = CreateDbContext();
            var scenario = db.Scenarios.FirstOrDefault(s => s.Id == id);
            if (scenario != null)
            {
                db.Scenarios.Remove(scenario);
                db.SaveChanges();
                
                if (_currentScenario != null && _currentScenario.Id == id)
                {
                    _currentScenario = null;
                    NetworkManager.Instance.ClearNetwork();
                    if (LoadEditorJsonAction != null) LoadEditorJsonAction.Invoke("{}");
                    IsDirty = false;
                    ScenarioLoaded?.Invoke();
                }
                
                TerminalService.Instance.LogSuccess($"Scenario '{scenario.Name}' deleted.");
            }
        }
        catch (Exception ex)
        {
            TerminalService.Instance.LogError($"Failed to delete scenario: {ex.Message}");
        }
    }

    public void LoadScenario(Guid id)
    {
        if (!PromptForSaveIfDirty()) return;

        try
        {
            using var db = CreateDbContext();
            var scenario = db.Scenarios.FirstOrDefault(s => s.Id == id);
            
            if (scenario != null)
            {
                _currentScenario = scenario;
                
                // Load Network
                NetworkManager.Instance.LoadFromJson(scenario.NetworkStateJson);
                
                // Load Editor
                if (LoadEditorJsonAction != null)
                {
                    LoadEditorJsonAction.Invoke(scenario.EditorLayoutJson);
                }
                
                IsDirty = false;
                ScenarioLoaded?.Invoke();
                TerminalService.Instance.LogSuccess($"Scenario '{scenario.Name}' loaded successfully.");
            }
        }
        catch (Exception ex)
        {
            TerminalService.Instance.LogError($"Failed to load scenario: {ex.Message}");
        }
    }

    // Editor ViewModel will register these so ProjectManager can ask for state
    public Func<string>? GetEditorJsonFunc { get; set; }
    public Action<string>? LoadEditorJsonAction { get; set; }

    public void SaveScenario()
    {
        if (_currentScenario == null) return;

        try
        {
            var networkJson = NetworkManager.Instance.GetJsonState();
            
            string editorJson = "{}";
            if (GetEditorJsonFunc != null)
            {
                editorJson = GetEditorJsonFunc.Invoke();
            }

            using var db = CreateDbContext();
            
            var existing = db.Scenarios.FirstOrDefault(s => s.Id == _currentScenario.Id);
            if (existing != null)
            {
                existing.Name = _currentScenario.Name;
                existing.UpdatedAt = DateTime.Now;
                existing.NetworkStateJson = networkJson;
                existing.EditorLayoutJson = editorJson;
                db.Scenarios.Update(existing);
            }
            else
            {
                _currentScenario.UpdatedAt = DateTime.Now;
                _currentScenario.NetworkStateJson = networkJson;
                _currentScenario.EditorLayoutJson = editorJson;
                db.Scenarios.Add(_currentScenario);
            }

            db.SaveChanges();
            IsDirty = false;
            TerminalService.Instance.LogSuccess($"Scenario '{_currentScenario.Name}' saved.");
        }
        catch (Exception ex)
        {
            TerminalService.Instance.LogError($"Failed to save scenario: {ex.Message}");
        }
    }

    /// <summary>
    /// Prompts user to save if there are unsaved changes. 
    /// Returns false if user CANCELS the operation.
    /// Returns true if user saves, discards, or there were no changes.
    /// </summary>
    public bool PromptForSaveIfDirty()
    {
        if (!IsDirty) return true;

        var result = MessageBox.Show(
            "Kaydedilmemiş değişiklikleriniz var. Değişiklikleri kaydetmek istiyor musunuz?", 
            "Uyarı", 
            MessageBoxButton.YesNoCancel, 
            MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            SaveScenario();
            return true;
        }
        else if (result == MessageBoxResult.No)
        {
            return true; // Discard and proceed
        }

        return false; // Cancel
    }
}
