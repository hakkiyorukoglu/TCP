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

    public void SetIncomingConnection(Guid targetModemId, Guid? newSourceId)
    {
        if (targetModemId == newSourceId) return; // Prevent self-loop

        var targetModem = _modems.FirstOrDefault(m => m.Id == targetModemId);
        if (targetModem == null) return;

        if (newSourceId.HasValue && newSourceId == targetModem.OutgoingConnectionId) return; // Prevent incoming=outgoing

        var oldSourceId = targetModem.IncomingConnectionId;
        if (oldSourceId == newSourceId) return;

        // 1. Clean up old source's outgoing
        if (oldSourceId.HasValue && oldSourceId.Value != _mainPc.Id)
        {
            var oldSource = _modems.FirstOrDefault(m => m.Id == oldSourceId.Value);
            if (oldSource != null && oldSource.OutgoingConnectionId == targetModemId)
            {
                oldSource.OutgoingConnectionId = null;
            }
        }

        // 2. Set new incoming
        targetModem.IncomingConnectionId = newSourceId;

        // 3. Establish new incoming connection from source
        if (newSourceId.HasValue)
        {
            if (newSourceId.Value == _mainPc.Id)
            {
                // MainPC can only connect to one modem. Clean up others.
                foreach (var m in _modems)
                {
                    if (m.Id != targetModemId && m.IncomingConnectionId == _mainPc.Id)
                    {
                        m.IncomingConnectionId = null;
                    }
                }
            }
            else
            {
                var newSource = _modems.FirstOrDefault(m => m.Id == newSourceId.Value);
                if (newSource != null)
                {
                    // If new source was pointing somewhere else, break that target's incoming
                    if (newSource.OutgoingConnectionId.HasValue && newSource.OutgoingConnectionId.Value != targetModemId)
                    {
                        var oldTarget = _modems.FirstOrDefault(m => m.Id == newSource.OutgoingConnectionId.Value);
                        if (oldTarget != null) oldTarget.IncomingConnectionId = null;
                    }
                    newSource.OutgoingConnectionId = targetModemId;
                }
            }
        }
        SaveNetwork();
    }

    public void SetOutgoingConnection(Guid sourceModemId, Guid? newTargetId)
    {
        if (sourceModemId == newTargetId) return; // Prevent self-loop

        var sourceModem = _modems.FirstOrDefault(m => m.Id == sourceModemId);
        if (sourceModem == null) return;

        if (newTargetId.HasValue && newTargetId == sourceModem.IncomingConnectionId) return; // Prevent incoming=outgoing

        var oldTargetId = sourceModem.OutgoingConnectionId;
        if (oldTargetId == newTargetId) return;

        // 1. Clean up old target's incoming
        if (oldTargetId.HasValue)
        {
            var oldTarget = _modems.FirstOrDefault(m => m.Id == oldTargetId.Value);
            if (oldTarget != null && oldTarget.IncomingConnectionId == sourceModemId)
            {
                oldTarget.IncomingConnectionId = null;
            }
        }

        // 2. Set new outgoing
        sourceModem.OutgoingConnectionId = newTargetId;

        // 3. Establish new outgoing connection to target
        if (newTargetId.HasValue)
        {
            var newTarget = _modems.FirstOrDefault(m => m.Id == newTargetId.Value);
            if (newTarget != null)
            {
                // If new target was receiving from somewhere else, break that source's outgoing
                if (newTarget.IncomingConnectionId.HasValue && newTarget.IncomingConnectionId.Value != sourceModemId)
                {
                    if (newTarget.IncomingConnectionId.Value != _mainPc.Id)
                    {
                        var oldSource = _modems.FirstOrDefault(m => m.Id == newTarget.IncomingConnectionId.Value);
                        if (oldSource != null) oldSource.OutgoingConnectionId = null;
                    }
                }
                newTarget.IncomingConnectionId = sourceModemId;
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
