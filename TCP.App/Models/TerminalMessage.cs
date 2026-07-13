using System;
using System.Windows.Media;

namespace TCP.App.Models
{
    public enum TerminalLogType
    {
        Info,
        Warning,
        Error,
        Success,
        Command,
        System
    }

    public class TerminalMessage
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Message { get; set; } = string.Empty;
        public TerminalLogType LogType { get; set; } = TerminalLogType.Info;

        public string FormattedTime => Timestamp.ToString("HH:mm:ss");

        public Brush Color
        {
            get
            {
                // Basic colors - we'll bind this to dynamic resources later or use these hardcoded ones for terminal aesthetic
                return LogType switch
                {
                    TerminalLogType.Error => Brushes.Red,
                    TerminalLogType.Warning => Brushes.Orange,
                    TerminalLogType.Success => Brushes.LightGreen,
                    TerminalLogType.Command => Brushes.Cyan,
                    TerminalLogType.System => Brushes.DarkGray,
                    _ => Brushes.WhiteSmoke
                };
            }
        }
    }
}
