using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using TCP.App.Models.Electronics;

namespace TCP.App.Services;

/// <summary>
/// DeviceManager - Manages user-created device instances.
/// Singleton service for saving and loading custom devices.
/// </summary>
public class DeviceManager
{
    private static DeviceManager? _instance;

    public static DeviceManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DeviceManager();
            }
            return _instance;
        }
    }

    private readonly string _saveFilePath;
    private readonly ObservableCollection<DeviceInstance> _devices;

    public ObservableCollection<DeviceInstance> Devices => _devices;

    private DeviceManager()
    {
        var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TCP");
        if (!Directory.Exists(appData))
        {
            Directory.CreateDirectory(appData);
        }
        _saveFilePath = Path.Combine(appData, "devices.json");
        _devices = new ObservableCollection<DeviceInstance>();

        LoadDevices();
    }

    public void AddDevice(DeviceInstance device)
    {
        _devices.Add(device);
        SaveDevices();
    }

    public void UpdateDevice(DeviceInstance device)
    {
        var existing = _devices.FirstOrDefault(d => d.Id == device.Id);
        if (existing != null)
        {
            // Just saving state since object is mutable
            SaveDevices();
        }
    }

    public void RemoveDevice(Guid id)
    {
        var device = _devices.FirstOrDefault(d => d.Id == id);
        if (device != null)
        {
            _devices.Remove(device);
            SaveDevices();
        }
    }

    public void SaveDevices()
    {
        try
        {
            var json = JsonSerializer.Serialize(_devices, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_saveFilePath, json);
        }
        catch (Exception ex)
        {
            TerminalService.Instance.LogError($"Failed to save devices: {ex.Message}");
        }
    }

    private void LoadDevices()
    {
        try
        {
            if (File.Exists(_saveFilePath))
            {
                var json = File.ReadAllText(_saveFilePath);
                var loaded = JsonSerializer.Deserialize<List<DeviceInstance>>(json);
                if (loaded != null)
                {
                    foreach (var d in loaded)
                    {
                        _devices.Add(d);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            TerminalService.Instance.LogError($"Failed to load devices: {ex.Message}");
        }
    }
}
