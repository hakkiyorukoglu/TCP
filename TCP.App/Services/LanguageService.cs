using System;
using System.Windows;
using System.Windows.Threading;

namespace TCP.App.Services;

/// <summary>
/// LanguageService - Dil yönetim servisi
/// 
/// Bu servis runtime'da dili değiştirmeyi yönetir.
/// Application.Current.Resources'a dil ResourceDictionary'lerini merge eder.
/// </summary>
public static class LanguageService
{
    private static string _currentLanguage = "tr-TR";
    
    public static void ApplyLanguage(string languageCode)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
        {
            languageCode = "tr-TR"; // Default fallback
        }
        
        languageCode = languageCode.Trim();
        if (!string.Equals(languageCode, "tr-TR", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(languageCode, "en-US", StringComparison.OrdinalIgnoreCase))
        {
            languageCode = "tr-TR";
        }
        
        try
        {
            var app = Application.Current;
            if (app == null) return;
            
            var dispatcher = app.Dispatcher;
            if (dispatcher == null) return;
            
            dispatcher.Invoke(() =>
            {
                try
                {
                    var resources = app.Resources;
                    if (resources == null) return;
                    
                    var mergedDictionaries = resources.MergedDictionaries;
                    if (mergedDictionaries != null)
                    {
                        // Eski dil dosyalarını kaldır
                        for (int i = mergedDictionaries.Count - 1; i >= 0; i--)
                        {
                            var dict = mergedDictionaries[i];
                            if (dict != null && dict.Source != null)
                            {
                                var sourceString = dict.Source.ToString();
                                if (sourceString.Contains("Strings.tr-TR.xaml") || sourceString.Contains("Strings.en-US.xaml"))
                                {
                                    mergedDictionaries.RemoveAt(i);
                                }
                            }
                        }
                    }
                    
                    // Yeni dil dosyasını yükle
                    string langPath;
                    if (string.Equals(languageCode, "en-US", StringComparison.OrdinalIgnoreCase))
                    {
                        langPath = "pack://application:,,,/TCP.Theming;component/Themes/Languages/Strings.en-US.xaml";
                        _currentLanguage = "en-US";
                    }
                    else
                    {
                        langPath = "pack://application:,,,/TCP.Theming;component/Themes/Languages/Strings.tr-TR.xaml";
                        _currentLanguage = "tr-TR";
                    }
                    
                    var langUri = new Uri(langPath, UriKind.Absolute);
                    var langDictionary = new ResourceDictionary
                    {
                        Source = langUri
                    };
                    
                    if (mergedDictionaries != null)
                    {
                        mergedDictionaries.Add(langDictionary);
                    }
                }
                catch
                {
                    // Fail silently
                }
            }, DispatcherPriority.Normal);
        }
        catch
        {
            // Fail silently
        }
    }
    
    public static string GetCurrentLanguage()
    {
        return _currentLanguage;
    }
}
