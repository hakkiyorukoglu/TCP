using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Media.Imaging;
using TCP.App.Models.Editor;
using TCP.App.Models.Electronics;

namespace TCP.App.Services;

public class EditorLayoutService
{
    private static readonly Lazy<EditorLayoutService> _instance = new(() => new EditorLayoutService());
    public static EditorLayoutService Instance => _instance.Value;

    private EditorLayoutService() { }

    public string GetCurrentLayoutJson(IEnumerable<EditorImage> images, IEnumerable<ILayerItem> placedItems, IEnumerable<TrackRoute> routes)
    {
        try
        {
            var state = new EditorLayoutState();
            
            foreach(var route in routes)
            {
                state.Routes.Add(route);
            }
            
            foreach (var img in images)
            {
                state.Images.Add(new EditorImage
                {
                    Id = img.Id,
                    FilePath = img.FilePath,
                    Name = img.Name,
                    X = img.X,
                    Y = img.Y,
                    Width = img.Width,
                    Height = img.Height,
                    Opacity = img.Opacity,
                    IsLocked = img.IsLocked
                });
            }

            foreach (var item in placedItems)
            {
                double x = 0, y = 0;
                bool isLocked = false;

                if (item is StationInstance st)
                {
                    x = st.X; y = st.Y; isLocked = st.IsLocked;
                }
                else if (item is ComponentInstance comp)
                {
                    x = comp.X; y = comp.Y; isLocked = comp.IsLocked;
                }
                else if (item is MainPcInstance pc)
                {
                    x = pc.X; y = pc.Y; isLocked = pc.IsLocked;
                }
                else if (item is ModemInstance m)
                {
                    x = m.X; y = m.Y; isLocked = m.IsLocked;
                }
                else if (item is RfidTagInstance rfid)
                {
                    x = rfid.X; y = rfid.Y; isLocked = rfid.IsLocked;
                }

                state.PlacedItems.Add(new PlacedItemState
                {
                    ItemId = item.Id,
                    X = x,
                    Y = y,
                    IsLocked = isLocked
                });
            }

            return JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            TerminalService.Instance.LogError($"Failed to generate layout JSON: {ex.Message}");
            return "{}";
        }
    }

    /// <summary>
    /// Saves the current layout state to a JSON file. (Deprecated: ProjectManager handles DB)
    /// </summary>
    public void SaveLayout(string filePath, IEnumerable<EditorImage> images, IEnumerable<ILayerItem> placedItems, IEnumerable<TrackRoute> routes)
    {
        var json = GetCurrentLayoutJson(images, placedItems, routes);
        if (json != "{}")
        {
            File.WriteAllText(filePath, json);
            TerminalService.Instance.LogSuccess($"Layout saved successfully to {Path.GetFileName(filePath)}");
        }
    }

    public EditorLayoutState? LoadFromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json) || json == "{}") return null;

        try
        {
            var state = JsonSerializer.Deserialize<EditorLayoutState>(json);

            if (state != null)
            {
                // Reload bitmaps
                foreach (var img in state.Images)
                {
                    if (File.Exists(img.FilePath))
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.UriSource = new Uri(img.FilePath);
                        bitmap.EndInit();
                        bitmap.Freeze();
                        img.ImageSource = bitmap;
                    }
                }
            }

            return state;
        }
        catch (Exception ex)
        {
            TerminalService.Instance.LogError($"Failed to load layout from JSON: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Loads the layout state from a JSON file.
    /// </summary>
    public EditorLayoutState? LoadLayout(string filePath)
    {
        try
        {
            if (!File.Exists(filePath)) return null;

            var json = File.ReadAllText(filePath);
            var state = LoadFromJson(json);

            if (state != null)
                TerminalService.Instance.LogSuccess($"Layout loaded successfully from {Path.GetFileName(filePath)}");
                
            return state;
        }
        catch (Exception ex)
        {
            TerminalService.Instance.LogError($"Failed to load layout from file: {ex.Message}");
            return null;
        }
    }
}
