using System;

namespace TCP.App.Services;

public class ArduinoGlobals
{
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
        // Standard Arduino delay
        System.Threading.Thread.Sleep(ms);
    }
}
