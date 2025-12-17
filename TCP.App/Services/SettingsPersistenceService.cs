using System;
using System.IO;
using System.Text.Json;
using TCP.App.Models;

namespace TCP.App.Services;

/// <summary>
/// SettingsPersistenceService - Settings persistence servisi
/// 
/// TCP-0.8.1: Settings Persistence v1 (Local)
/// 
/// Bu servis kullanıcı ayarlarını JSON dosyasına kaydeder ve yükler.
/// %AppData%/TCP/settings.json dosyasını kullanır.
/// 
/// Safety Rules:
/// - Tüm file IO try/catch içinde
/// - Load başarısız olursa default settings döner
/// - Save başarısız olursa sessizce fail eder (exception throw etmez)
/// 
/// Single Responsibility: Settings persistence (load/save)
/// </summary>
public class SettingsPersistenceService
{
    /// <summary>
    /// Settings dosyası yolu
    /// %AppData%/TCP/settings.json
    /// </summary>
    private readonly string _settingsFilePath;
    
    /// <summary>
    /// JSON serialization options
    /// </summary>
    private readonly JsonSerializerOptions _jsonOptions;
    
    /// <summary>
    /// Constructor
    /// Settings dosya yolunu oluşturur
    /// </summary>
    public SettingsPersistenceService()
    {
        // %AppData%/TCP/settings.json
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var tcpFolder = Path.Combine(appDataPath, "TCP");
        _settingsFilePath = Path.Combine(tcpFolder, "settings.json");
        
        // JSON options: Pretty print ve case-insensitive
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };
    }
    
    /// <summary>
    /// Settings dosyasından ayarları yükle
    /// 
    /// Safety:
    /// - Dosya yoksa → default settings döner
    /// - JSON corrupt ise → exception catch edilir → default settings döner
    /// - Hiçbir durumda exception throw etmez
    /// </summary>
    public AppSettings Load()
    {
        try
        {
            // Dosya yoksa default settings döner
            if (!File.Exists(_settingsFilePath))
            {
                return new AppSettings();
            }
            
            // Dosyayı oku
            var jsonContent = File.ReadAllText(_settingsFilePath);
            
            // JSON deserialize et
            var settings = JsonSerializer.Deserialize<AppSettings>(jsonContent, _jsonOptions);
            
            // Deserialize başarısız olursa (null dönerse) default döner
            return settings ?? new AppSettings();
        }
        catch (Exception)
        {
            // Herhangi bir exception durumunda (corrupt JSON, IO error, vb.)
            // Sessizce default settings döner
            // UI'ya exception throw edilmez
            return new AppSettings();
        }
    }
    
    /// <summary>
    /// Settings dosyasına ayarları kaydet
    /// 
    /// Safety:
    /// - Directory yoksa oluşturur
    /// - Save başarısız olursa sessizce fail eder (exception throw etmez)
    /// </summary>
    public void Save(AppSettings settings)
    {
        if (settings == null)
        {
            return; // Null check
        }
        
        try
        {
            // Directory yoksa oluştur
            var directory = Path.GetDirectoryName(_settingsFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            // JSON serialize et
            var jsonContent = JsonSerializer.Serialize(settings, _jsonOptions);
            
            // Dosyaya yaz
            File.WriteAllText(_settingsFilePath, jsonContent);
        }
        catch (Exception)
        {
            // Save başarısız olursa sessizce fail eder
            // UI'ya exception throw edilmez
            // Loglama yapılabilir ama şimdilik sessizce fail ediyoruz
        }
    }
}
