using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using TCP.App.Services;

namespace TCP.App.Services;

public class Esp32HttpClient
{
    private static Esp32HttpClient? _instance;
    public static Esp32HttpClient Instance => _instance ??= new Esp32HttpClient();

    private readonly HttpClient _httpClient;

    private Esp32HttpClient()
    {
        _httpClient = new HttpClient();
        _httpClient.Timeout = TimeSpan.FromSeconds(15);
    }

    /// <summary>
    /// Sends a GET request to toggle a specific relay pin on the ESP32.
    /// Example URL: http://192.168.1.10/relay?pin=5&state=1
    /// </summary>
    public async Task<bool> ToggleRelayAsync(string ipAddress, string pin, bool state)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ipAddress) || string.IsNullOrWhiteSpace(pin))
                return false;

            // Strip "D" prefix if user entered something like "D5" instead of "5"
            string pinNumber = pin.Replace("D", "", StringComparison.OrdinalIgnoreCase).Trim();
            string stateValue = state ? "1" : "0";
            
            string url = $"http://{ipAddress}/relay?pin={pinNumber}&state={stateValue}";

            var response = await _httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                TerminalService.Instance.LogSuccess($"[OTA] Röle komutu başarıyla iletildi: {ipAddress} - Pin {pinNumber} -> {stateValue}");
                return true;
            }
            else
            {
                TerminalService.Instance.LogError($"[OTA] Röle komutu reddedildi ({response.StatusCode}): {ipAddress}");
                return false;
            }
        }
        catch (Exception ex)
        {
            TerminalService.Instance.LogError($"[OTA] Röle komutu başarısız oldu ({ipAddress}): {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Uploads a compiled .bin firmware file via multipart/form-data to the ESP32 OTA endpoint.
    /// Example URL: http://192.168.1.10/update
    /// Form field: "update"
    /// </summary>
    public async Task<bool> UploadOtaFirmwareAsync(string ipAddress, string filePath, IProgress<int>? progress)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ipAddress) || !File.Exists(filePath))
            {
                TerminalService.Instance.LogError($"[OTA] Dosya bulunamadı veya IP adresi geçersiz: {filePath}");
                return false;
            }

            string url = $"http://{ipAddress}/update";
            
            var fileInfo = new FileInfo(filePath);
            var fileStream = File.OpenRead(filePath);
            
            progress?.Report(10);

            using var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            
            // Standard ESP32 Update libraries usually expect "update" as the name of the file field
            content.Add(streamContent, "update", fileInfo.Name);

            TerminalService.Instance.LogInfo($"[OTA] {ipAddress} adresine yazılım yükleniyor... ({fileInfo.Length / 1024} KB)");
            progress?.Report(50);
            
            // IMPORTANT: Timeout might need to be increased for large firmware files.
            using var uploadClient = new HttpClient { Timeout = TimeSpan.FromMinutes(2) };
            var response = await uploadClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                progress?.Report(100);
                TerminalService.Instance.LogSuccess($"[OTA] {ipAddress} yazılım güncellemesi tamamlandı! Cihaz yeniden başlatılıyor...");
                return true;
            }
            else
            {
                string errorBody = await response.Content.ReadAsStringAsync();
                TerminalService.Instance.LogError($"[OTA] {ipAddress} yükleme reddedildi ({response.StatusCode}): {errorBody}");
                return false;
            }
        }
        catch (Exception ex)
        {
            TerminalService.Instance.LogError($"[OTA] Yükleme hatası ({ipAddress}): {ex.Message}");
            return false;
        }
    }
}
