using System.Collections.Generic;

namespace TCP.App.Models.Electronics;

public class BoardItem
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Dictionary<string, string> SummaryData { get; set; } = new();
}
