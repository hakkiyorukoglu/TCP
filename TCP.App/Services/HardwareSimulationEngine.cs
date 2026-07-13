using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TCP.App.Services;

public class HardwareSimulationEngine
{
    private CancellationTokenSource? _cts;
    private Task? _simulationTask;

    public void StartSimulation(string scriptCode, ArduinoGlobals globals)
    {
        if (string.IsNullOrWhiteSpace(scriptCode))
            return;

        _cts = new CancellationTokenSource();

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
            catch (Exception ex)
            {
                Console.WriteLine("Simülasyon Hatası: " + ex.Message);
            }
        }, _cts.Token);
    }

    public void StopSimulation()
    {
        _cts?.Cancel();
    }
}
