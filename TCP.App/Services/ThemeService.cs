using System;
using System.Windows;
using System.Windows.Threading;

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
    /// TCP-0.8.1: Safe Theme Apply with Save Button
    /// 
    /// Behavior:
    /// - Eski tema ResourceDictionary'sini kaldırır
    /// - Yeni tema ResourceDictionary'sini yükler ve merge eder
    /// - Theme.Dark.xaml veya Theme.Light.xaml
    /// 
    /// Safety:
    /// - Invalid theme name → fallback to Dark
    /// - Exception catch edilir, app crash etmez
    /// - Dispatcher.Invoke ile UI thread'de çalışır
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
            
            // TCP-0.8.1: Dispatcher.Invoke ile UI thread'de çalıştır
            // Bu sayede ResourceDictionary değişikliği güvenli bir şekilde yapılır
            var dispatcher = app.Dispatcher;
            if (dispatcher == null)
            {
                return; // Dispatcher null ise işlem yapma
            }
            
            dispatcher.Invoke(() =>
            {
                try
                {
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
                }
            }, DispatcherPriority.Normal);
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
    
    /// <summary>
    /// Initial theme loading (startup safe)
    /// TCP-0.8.1 Hotfix-2: Startup Safe Resources
    /// 
    /// Bu metod MainWindow Loaded event'inde çağrılır.
    /// Startup'ta StaticResource resolution failure'ı önler.
    /// 
    /// Behavior:
    /// - Try load persisted theme from settings
    /// - If fails → load Dark theme
    /// - If still fails → continue without theme (NO CRASH)
    /// </summary>
    public static void LoadInitialTheme()
    {
        try
        {
            var settings = App.LoadedSettings;
            string themeToLoad = "Dark"; // Default
            
            if (settings != null && !string.IsNullOrWhiteSpace(settings.Theme))
            {
                themeToLoad = settings.Theme;
            }
            
            // Theme'i güvenli bir şekilde yükle
            ApplyTheme(themeToLoad);
        }
        catch
        {
            // Exception durumunda sessizce fail eder
            // App crash etmez
            // Default styling ile devam eder
        }
    }
}
