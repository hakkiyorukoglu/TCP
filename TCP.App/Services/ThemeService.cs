using System;
using System.Windows;

namespace TCP.App.Services;

/// <summary>
/// ThemeService - Tema yönetim servisi
/// 
/// TCP-0.8.1: Theme Selection Fix
/// 
/// Bu servis runtime'da tema değiştirmeyi yönetir.
/// Application.Current.Resources'a tema ResourceDictionary'lerini merge eder.
/// 
/// Single Responsibility: Runtime theme application
/// </summary>
public static class ThemeService
{
    /// <summary>
    /// Mevcut tema adı
    /// </summary>
    private static string _currentTheme = "Dark";
    
    /// <summary>
    /// Tema uygula
    /// 
    /// TCP-0.8.1: Theme Selection Fix
    /// 
    /// Behavior:
    /// - Eski tema ResourceDictionary'sini kaldırır
    /// - Yeni tema ResourceDictionary'sini yükler ve merge eder
    /// - Theme.Dark.xaml veya Theme.Light.xaml
    /// 
    /// Safety:
    /// - Invalid theme name → fallback to Dark
    /// - Exception catch edilir, app crash etmez
    /// </summary>
    public static void ApplyTheme(string themeName)
    {
        if (string.IsNullOrWhiteSpace(themeName))
        {
            themeName = "Dark"; // Default fallback
        }
        
        // Theme name'i normalize et (case-insensitive)
        themeName = themeName.Trim();
        if (!string.Equals(themeName, "Dark", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(themeName, "Light", StringComparison.OrdinalIgnoreCase))
        {
            themeName = "Dark"; // Invalid theme → fallback to Dark
        }
        
        try
        {
            var app = Application.Current;
            if (app == null)
            {
                return; // App null ise işlem yapma
            }
            
            var resources = app.Resources;
            if (resources == null)
            {
                return; // Resources null ise işlem yapma
            }
            
            // Eski tema ResourceDictionary'sini kaldır
            // MergedDictionaries içinde Theme.Dark.xaml veya Theme.Light.xaml'ı bul ve kaldır
            var mergedDictionaries = resources.MergedDictionaries;
            if (mergedDictionaries != null)
            {
                // Tema dosyalarını bul ve kaldır
                for (int i = mergedDictionaries.Count - 1; i >= 0; i--)
                {
                    var dict = mergedDictionaries[i];
                    if (dict != null && dict.Source != null)
                    {
                        var sourceString = dict.Source.ToString();
                        if (sourceString.Contains("Theme.Dark.xaml") || sourceString.Contains("Theme.Light.xaml"))
                        {
                            mergedDictionaries.RemoveAt(i);
                        }
                    }
                }
            }
            
            // Yeni tema ResourceDictionary'sini yükle ve merge et
            string themePath;
            if (string.Equals(themeName, "Dark", StringComparison.OrdinalIgnoreCase))
            {
                themePath = "pack://application:,,,/TCP.Theming;component/Themes/Variants/Theme.Dark.xaml";
                _currentTheme = "Dark";
            }
            else
            {
                themePath = "pack://application:,,,/TCP.Theming;component/Themes/Variants/Theme.Light.xaml";
                _currentTheme = "Light";
            }
            
            var themeUri = new Uri(themePath, UriKind.Absolute);
            var themeDictionary = new ResourceDictionary
            {
                Source = themeUri
            };
            
            if (mergedDictionaries != null)
            {
                mergedDictionaries.Add(themeDictionary);
            }
        }
        catch (Exception)
        {
            // Exception durumunda sessizce fail eder
            // App crash etmez
            // Loglama yapılabilir ama şimdilik sessizce fail ediyoruz
        }
    }
    
    /// <summary>
    /// Mevcut tema adını al
    /// </summary>
    public static string GetCurrentTheme()
    {
        return _currentTheme;
    }
}
