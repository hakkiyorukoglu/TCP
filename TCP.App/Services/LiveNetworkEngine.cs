using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using TCP.App.Models.Electronics;

namespace TCP.App.Services;

/// <summary>
/// LiveNetworkEngine - Handles live background pinging and HTTP controls.
/// </summary>
public class LiveNetworkEngine
{
    private static LiveNetworkEngine? _instance;
    public static LiveNetworkEngine Instance => _instance ??= new LiveNetworkEngine();

    private bool _isRunning;
    private CancellationTokenSource? _cancellationTokenSource;
    private readonly HttpClient _httpClient;

    public event Action? NetworkStatusUpdated;

    private LiveNetworkEngine()
    {
        _httpClient = new HttpClient();
        _httpClient.Timeout = TimeSpan.FromSeconds(2); // Short timeout for live control
    }

    /// <summary>
    /// Starts the background engine.
    /// </summary>
    public void Start()
    {
        if (_isRunning) return;
        _isRunning = true;
        _cancellationTokenSource = new CancellationTokenSource();
        Task.Run(() => PingLoop(_cancellationTokenSource.Token));
    }

    /// <summary>
    /// Stops the background engine.
    /// </summary>
    public void Stop()
    {
        _isRunning = false;
        _cancellationTokenSource?.Cancel();
    }

    private async Task PingLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await UpdateNetworkStatusAsync();
            NetworkStatusUpdated?.Invoke();
            await Task.Delay(2000, token); // 2-second interval
        }
    }

    private async Task UpdateNetworkStatusAsync()
    {
        var network = NetworkManager.Instance;
        if (network.MainPc == null) return;

        // Step 1: Discover the daisy chain from Main PC onwards
        var reachableModems = new HashSet<Guid>();
        var unreachableModems = new HashSet<Guid>();

        // Find the first modem connected to MainPC
        var firstModem = network.Modems.FirstOrDefault(m => m.IncomingConnectionId == network.MainPc.Id);
        Guid? currentTargetId = firstModem?.Id;
        bool upstreamBroken = false;

        while (currentTargetId != null && currentTargetId != Guid.Empty)
        {
            var modem = network.Modems.FirstOrDefault(m => m.Id == currentTargetId);
            if (modem == null) break;

            if (upstreamBroken)
            {
                // Everything downstream is unreachable
                modem.Status = NetworkStatus.Unreachable;
                unreachableModems.Add(modem.Id);
                currentTargetId = modem.OutgoingConnectionId;
                continue;
            }

            // Otherwise, ping it
            bool isOnline = await PingAsync(modem.IpAddress);
            if (isOnline)
            {
                modem.Status = NetworkStatus.Online;
                reachableModems.Add(modem.Id);
            }
            else
            {
                modem.Status = NetworkStatus.Offline;
                upstreamBroken = true; // Daisy chain breaks here
            }

            currentTargetId = modem.OutgoingConnectionId;
        }

        // Step 2: Update stations based on their parent modem
        foreach (var modem in network.Modems)
        {
            foreach (var station in modem.Stations)
            {
                if (modem.Status == NetworkStatus.Offline || modem.Status == NetworkStatus.Unreachable)
                {
                    station.Status = NetworkStatus.Unreachable;
                }
                else if (modem.Status == NetworkStatus.Online)
                {
                    // Ping the station since upstream is fine
                    bool isOnline = await PingAsync(station.IpAddress);
                    station.Status = isOnline ? NetworkStatus.Online : NetworkStatus.Offline;
                }
            }
        }
    }

    private async Task<bool> PingAsync(string ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress)) return false;
        try
        {
            using var pinger = new Ping();
            var reply = await pinger.SendPingAsync(ipAddress, 1000); // 1-sec timeout
            return reply.Status == IPStatus.Success;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Sends an HTTP GET to turn a relay on/off. (e.g. http://192.168.1.10/relay/1/on)
    /// </summary>
    public async Task<bool> SendRelayCommandAsync(string ipAddress, int relayIndex, bool turnOn)
    {
        try
        {
            string state = turnOn ? "on" : "off";
            string url = $"http://{ipAddress}/relay/{relayIndex}/{state}";
            var response = await _httpClient.GetAsync(url);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            AppLogger.LogException(ex, "LiveNetworkEngine.SendRelayCommandAsync");
            return false;
        }
    }
}
