using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TCP.App.Services;

public class HardwareSimulationEngine : IDisposable
{
    private CancellationTokenSource? _cts;
    private Task? _simulationTask;

    public void StartSimulation(string scriptCode, ArduinoGlobals globals)
    {
        if (string.IsNullOrWhiteSpace(scriptCode))
            return;

        // Ensure any previous run is stopped before starting a new one
        StopSimulation();

        _cts = new CancellationTokenSource();
        globals.CancellationToken = _cts.Token; // Bind the token for cancellable delay()

        var options = ScriptOptions.Default
            .WithImports("System", "System.Threading", "System.Threading.Tasks");

        // We wrap the script to allow global setup() and loop() to run exactly like Arduino
        string executorCode = scriptCode + @"
            // We invoke setup if it exists
            setup();
            while(true) {
                loop();
                delay(10); // Prevent 100% CPU utilization
            }
        ";

        _simulationTask = Task.Run(async () =>
        {
            try
            {
                await CSharpScript.EvaluateAsync(executorCode, options, globals: globals, cancellationToken: _cts.Token);
            }
            catch (CompilationErrorException ex)
            {
                // In a real scenario we could log this to the UI
                Console.WriteLine("Derleme Hatası: " + string.Join(Environment.NewLine, ex.Diagnostics));
            }
            catch (TaskCanceledException)
            {
                // Task was stopped normally
            }
            catch (OperationCanceledException)
            {
                // Task was stopped normally via our own delay() exception
            }
            catch (Exception ex)
            {
                Console.WriteLine("Simülasyon Hatası: " + ex.Message);
            }
        }, _cts.Token);
    }

    public void StopSimulation()
    {
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
    }

    public void Dispose()
    {
        StopSimulation();
    }
}
