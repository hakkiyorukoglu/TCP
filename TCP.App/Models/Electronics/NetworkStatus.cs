namespace TCP.App.Models.Electronics;

/// <summary>
/// Represents the live network status of a connected device (Modem/Station).
/// </summary>
public enum NetworkStatus
{
    /// <summary>
    /// Device is not reachable and not tracked.
    /// </summary>
    Offline,
    
    /// <summary>
    /// Device is responding to ping/HTTP requests.
    /// </summary>
    Online,
    
    /// <summary>
    /// Device cannot be reached because an upstream device is offline.
    /// </summary>
    Unreachable,
    
    /// <summary>
    /// Device has encountered an error or timeout.
    /// </summary>
    Error
}
