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

    /// <summary>
    /// Saves the current layout state to a JSON file.
    /// </summary>
    public void SaveLayout(string filePath, IEnumerable<EditorImage> images, IEnumerable<ILayerItem> placedItems, IEnumerable<TrackRoute> routes)
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

                state.PlacedItems.Add(new PlacedItemState
                {
                    ItemId = item.Id,
                    X = x,
                    Y = y,
                    IsLocked = isLocked
                });
            }

            var json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
            TerminalService.Instance.LogSuccess($"Layout saved successfully to {Path.GetFileName(filePath)}");
        }
        catch (Exception ex)
        {
            TerminalService.Instance.LogError($"Failed to save layout: {ex.Message}");
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

            TerminalService.Instance.LogSuccess($"Layout loaded successfully from {Path.GetFileName(filePath)}");
            return state;
        }
        catch (Exception ex)
        {
            TerminalService.Instance.LogError($"Failed to load layout: {ex.Message}");
            return null;
        }
    }
}
