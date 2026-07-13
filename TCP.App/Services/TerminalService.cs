using System;
using System.Collections.ObjectModel;
using System.Windows;
using TCP.App.Models;

namespace TCP.App.Services
{
    public class TerminalService
    {
        private static TerminalService? _instance;
        public static TerminalService Instance => _instance ??= new TerminalService();

        public ObservableCollection<TerminalMessage> Messages { get; } = new ObservableCollection<TerminalMessage>();
        public event Action<bool>? VisibilityChanged;

        private TerminalService()
        {
            LogSystem("Terminal initialized v1.0.0");
        }

        public void SetVisibility(bool isVisible)
        {
            VisibilityChanged?.Invoke(isVisible);
        }

        public void Log(string message, TerminalLogType type = TerminalLogType.Info)
        {
            var msg = new TerminalMessage { Message = message, LogType = type };
            if (Application.Current?.Dispatcher != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Messages.Add(msg);
                    if (Messages.Count > 500) Messages.RemoveAt(0);
                });
            }
            else
            {
                Messages.Add(msg);
                if (Messages.Count > 500) Messages.RemoveAt(0);
            }
        }

        public void LogInfo(string message) => Log(message, TerminalLogType.Info);
        public void LogSuccess(string message) => Log(message, TerminalLogType.Success);
        public void LogWarning(string message) => Log(message, TerminalLogType.Warning);
        public void LogError(string message) => Log(message, TerminalLogType.Error);
        public void LogSystem(string message) => Log(message, TerminalLogType.System);

        public void ExecuteCommand(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return;

            // Log user command
            Log($"> {input}", TerminalLogType.Command);

            var parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return;

            var cmd = parts[0].ToLowerInvariant();

            try
            {
                switch (cmd)
                {
                    case "clear":
                    case "cls":
                        Application.Current.Dispatcher.Invoke(() => Messages.Clear());
                        break;

                    case "set":
                        if (parts.Length >= 3)
                        {
                            var target = parts[1].ToLowerInvariant();
                            var value = parts[2].ToLowerInvariant();
                            
                            if (target == "theme")
                            {
                                if (value == "dark")
                                {
                                    Wpf.Ui.Appearance.ApplicationThemeManager.Apply(Wpf.Ui.Appearance.ApplicationTheme.Dark);
                                    LogSuccess("Theme changed to Dark");
                                }
                                else if (value == "light")
                                {
                                    Wpf.Ui.Appearance.ApplicationThemeManager.Apply(Wpf.Ui.Appearance.ApplicationTheme.Light);
                                    LogSuccess("Theme changed to Light");
                                }
                                else
                                {
                                    LogError($"Invalid theme: {value}. Use 'dark' or 'light'.");
                                }
                            }
                            else if (target == "lang")
                            {
                                if (value == "en" || value == "tr")
                                {
                                    TCP.App.Services.LanguageService.ApplyLanguage(value == "en" ? "en-US" : "tr-TR");
                                    LogSuccess($"Language changed to {value.ToUpper()}");
                                }
                                else
                                {
                                    LogError($"Invalid language: {value}. Use 'en' or 'tr'.");
                                }
                            }
                            else
                            {
                                LogError($"Unknown set target: {target}");
                            }
                        }
                        else
                        {
                            LogError("Usage: set [theme|lang] [value]");
                        }
                        break;
                        
                    case "help":
                        LogSystem("Available commands:");
                        LogSystem("  clear/cls - Clears the terminal");
                        LogSystem("  set theme [dark|light] - Changes application theme");
                        LogSystem("  set lang [en|tr] - Changes application language");
                        break;

                    default:
                        LogError($"Unknown command: {cmd}");
                        break;
                }
            }
            catch (Exception ex)
            {
                LogError($"Command execution failed: {ex.Message}");
            }
        }
    }
}
