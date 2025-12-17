using System;
using System.IO;

namespace TCP.App.Services;

/// <summary>
/// AppLogger - Minimal logging utility
/// 
/// TCP-0.9.3: Error Guardrails (No-crash policy)
/// 
/// Bu servis uygulama hatalarını log dosyasına yazar.
/// Basit, synchronous logging (no async complexity).
/// 
/// Single Responsibility: Error logging
/// </summary>
public static class AppLogger
{
    /// <summary>
    /// Log dosyası yolu
    /// %AppData%/TCP/logs/app.log
    /// TCP-0.9.3: Error Guardrails (No-crash policy)
    /// </summary>
    private static readonly string _logFilePath;
    
    /// <summary>
    /// Static constructor - Log dosya yolunu oluştur
    /// TCP-0.9.3: Error Guardrails (No-crash policy)
    /// </summary>
    static AppLogger()
    {
        try
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var tcpFolder = Path.Combine(appDataPath, "TCP");
            var logsFolder = Path.Combine(tcpFolder, "logs");
            
            // Logs klasörünü oluştur (yoksa)
            if (!Directory.Exists(logsFolder))
            {
                Directory.CreateDirectory(logsFolder);
            }
            
            _logFilePath = Path.Combine(logsFolder, "app.log");
        }
        catch
        {
            // Log dosya yolu oluşturulamazsa fallback
            _logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TCP_app.log");
        }
    }
    
    /// <summary>
    /// Exception logla
    /// TCP-0.9.3: Error Guardrails (No-crash policy)
    /// 
    /// Exception bilgilerini log dosyasına yazar.
    /// Hata oluşursa sessizce fail eder (app crash etmez).
    /// </summary>
    public static void LogException(Exception ex, string? context = null)
    {
        try
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var logEntry = $"[{timestamp}] EXCEPTION";
            
            if (!string.IsNullOrWhiteSpace(context))
            {
                logEntry += $" | Context: {context}";
            }
            
            logEntry += $"\nType: {ex.GetType().Name}";
            logEntry += $"\nMessage: {ex.Message}";
            logEntry += $"\nStackTrace:\n{ex.StackTrace}";
            logEntry += "\n" + new string('-', 80) + "\n";
            
            // Append to log file
            File.AppendAllText(_logFilePath, logEntry);
        }
        catch
        {
            // Log yazma başarısız olursa sessizce fail eder
            // App crash etmez
        }
    }
    
    /// <summary>
    /// Info mesajı logla
    /// TCP-0.9.3: Error Guardrails (No-crash policy)
    /// </summary>
    public static void LogInfo(string message)
    {
        try
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var logEntry = $"[{timestamp}] INFO: {message}\n";
            
            File.AppendAllText(_logFilePath, logEntry);
        }
        catch
        {
            // Log yazma başarısız olursa sessizce fail eder
        }
    }
}
