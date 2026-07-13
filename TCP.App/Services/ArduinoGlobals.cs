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

    public bool rfidAvailable(int pin)
    {
        return OnRfidAvailable?.Invoke(pin) ?? false;
    }

    public string readRfid(int pin)
    {
        return OnReadRfid?.Invoke(pin) ?? "";
    }
}
