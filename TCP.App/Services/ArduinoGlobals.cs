using System;
using System.Threading;

namespace TCP.App.Services;

public class ArduinoGlobals
{
    // Added token for cancellable delays
    public CancellationToken CancellationToken { get; set; }

    // Common Arduino constants
    public int HIGH = 1;
    public int LOW = 0;
    public int OUTPUT = 1;
    public int INPUT = 0;
    
    // Callbacks to interact with the UI/ViewModel
    public Action<int, int>? OnDigitalWrite { get; set; }
    public Func<int, int>? OnDigitalRead { get; set; }
    public Action<string>? OnLog { get; set; }

    public void print(string msg) { OnLog?.Invoke(msg); }
    public void println(string msg) { OnLog?.Invoke(msg + "\n"); }

    // Arduino standard functions
    public void pinMode(int pin, int mode)
    {
        // For simulation purposes, we might not strictly need to track pinMode
        // but the function must exist so user scripts compile.
    }

    public void digitalWrite(int pin, int value)
    {
        OnDigitalWrite?.Invoke(pin, value);
    }

    public int digitalRead(int pin)
    {
        return OnDigitalRead?.Invoke(pin) ?? 0;
    }

    public void delay(int ms)
    {
        // Use WaitOne with cancellation token to allow immediate abort
        if (CancellationToken != CancellationToken.None)
        {
            if (CancellationToken.WaitHandle.WaitOne(ms))
            {
                // WaitOne returns true if the handle was signaled (i.e., cancelled)
                throw new OperationCanceledException(CancellationToken);
            }
        }
        else
        {
            Thread.Sleep(ms);
        }
    }

    // --- MFRC522 (RFID) Benzeri API ---
    public Func<int, bool>? OnRfidAvailable { get; set; }
    public Func<int, string>? OnReadRfid { get; set; }

    private string? _pendingRfidScan;

    public void InjectRfidScan(string uid)
    {
        _pendingRfidScan = uid;
    }

    public bool rfidAvailable(int pin)
    {
        // First check our injected scan, else fall back to UI callback (if any)
        if (!string.IsNullOrEmpty(_pendingRfidScan))
            return true;

        return OnRfidAvailable?.Invoke(pin) ?? false;
    }

    public string readRfid(int pin)
    {
        if (!string.IsNullOrEmpty(_pendingRfidScan))
        {
            string tag = _pendingRfidScan;
            _pendingRfidScan = null; // consume it
            return tag;
        }

        return OnReadRfid?.Invoke(pin) ?? "";
    }

    // --- Network API Stubs ---
    public Action<string>? OnSend { get; set; }
    public Func<string>? OnReceive { get; set; }

    public void send(string message) 
    { 
        OnLog?.Invoke($"[Ağ Gönderimi]: {message}"); 
        OnSend?.Invoke(message);
    }
    public void send(Guid modemId, Guid stationId, string message) 
    { 
        OnLog?.Invoke($"[Ağ Gönderimi] {modemId} -> {stationId}: {message}"); 
        OnSend?.Invoke(message); // Simplification for now
    }
    public string inMsg() 
    { 
        return OnReceive?.Invoke() ?? ""; 
    }
}
