using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using TCP.App.Models.Electronics;

namespace TCP.App.Services;

public class NetworkState
{
    public MainPcInstance MainPc { get; set; } = new();
    public ObservableCollection<ModemInstance> Modems { get; set; } = new();
}

/// <summary>
/// NetworkManager - Manages the Main PC and Modems, which in turn manage Stations and Components.
/// </summary>
public class NetworkManager
{
    private static NetworkManager? _instance;

    public static NetworkManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new NetworkManager();
            }
            return _instance;
        }
    }

    private readonly string _saveFilePath;
    private MainPcInstance _mainPc;
    private readonly ObservableCollection<ModemInstance> _modems;

    public MainPcInstance MainPc => _mainPc;
    public ObservableCollection<ModemInstance> Modems => _modems;

    public event Action? NetworkChanged;

    private NetworkManager()
    {
        var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TCP");
        if (!Directory.Exists(appData))
        {
            Directory.CreateDirectory(appData);
        }
        _saveFilePath = Path.Combine(appData, "modems.json");
        _modems = new ObservableCollection<ModemInstance>();
        _mainPc = new MainPcInstance();

        LoadNetwork();
    }

    public void AddModem(ModemInstance modem)
    {
        _modems.Add(modem);
        SaveNetwork();
    }

    public void RemoveModem(Guid id)
    {
        var modem = _modems.FirstOrDefault(m => m.Id == id);
        if (modem != null)
        {
            // Remove incoming/outgoing references from other modems
            foreach(var m in _modems)
            {
                if (m.IncomingConnectionId == id) m.IncomingConnectionId = null;
                if (m.OutgoingConnectionId == id) m.OutgoingConnectionId = null;
            }
            
            _modems.Remove(modem);
            SaveNetwork();
        }
    }

    // Two-way binding helper for connections
    public void LinkModems(Guid? sourceId, Guid? targetId)
    {
        // Enforce MainPc can only connect to 1 modem. If another modem is already using MainPc as source, break it.
        if (sourceId.HasValue && sourceId.Value == _mainPc.Id && targetId.HasValue)
        {
            var alreadyConnected = _modems.FirstOrDefault(m => m.IncomingConnectionId == _mainPc.Id);
            if (alreadyConnected != null && alreadyConnected.Id != targetId.Value)
            {
                alreadyConnected.IncomingConnectionId = null;
            }
        }

        // First, clear old incoming links to the target
        if (targetId.HasValue && targetId.Value != _mainPc.Id)
        {
            var target = _modems.FirstOrDefault(m => m.Id == targetId.Value);
            if (target != null)
            {
                // If target was connected to someone else previously, we break that
                if (target.IncomingConnectionId.HasValue)
                {
                    var oldSource = _modems.FirstOrDefault(m => m.Id == target.IncomingConnectionId.Value);
                    if (oldSource != null && oldSource.OutgoingConnectionId == targetId)
                    {
                        oldSource.OutgoingConnectionId = null;
                    }
                }
                target.IncomingConnectionId = sourceId;
            }
        }

        // Now set the outgoing of source
        if (sourceId.HasValue && sourceId.Value != _mainPc.Id)
        {
            var source = _modems.FirstOrDefault(m => m.Id == sourceId.Value);
            if (source != null)
            {
                source.OutgoingConnectionId = targetId;
            }
        }
        SaveNetwork();
    }

    public void SaveNetwork()
    {
        try
        {
            var state = new NetworkState
            {
                MainPc = _mainPc,
                Modems = _modems
            };
            var json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_saveFilePath, json);
            NetworkChanged?.Invoke();
        }
        catch (Exception ex)
        {
            TerminalService.Instance.LogError($"Ağ durumu kaydedilemedi: {ex.Message}");
        }
    }
    
    // For backward compatibility from previous steps
    public void SaveModems() => SaveNetwork();

    private void LoadNetwork()
    {
        try
        {
            if (File.Exists(_saveFilePath))
            {
                var json = File.ReadAllText(_saveFilePath);
                
                try 
                {
                    // Try to load as NetworkState
                    var state = JsonSerializer.Deserialize<NetworkState>(json);
                    if (state != null)
                    {
                        if (state.MainPc != null) _mainPc = state.MainPc;
                        if (state.Modems != null)
                        {
                            foreach (var m in state.Modems) _modems.Add(m);
                        }
                        return;
                    }
                }
                catch
                {
                    // If it fails, fallback to List<ModemInstance> (previous version)
                    var loaded = JsonSerializer.Deserialize<List<ModemInstance>>(json);
                    if (loaded != null)
                    {
                        foreach (var m in loaded)
                        {
                            _modems.Add(m);
                        }
                        SaveNetwork(); // Resave in new format
                    }
                }
            }
        }
        catch (Exception ex)
        {
            TerminalService.Instance.LogError($"Ağ durumu yüklenemedi: {ex.Message}");
        }
    }
}
