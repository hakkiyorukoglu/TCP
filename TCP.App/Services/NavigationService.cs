using System;
using TCP.App.ViewModels;

namespace TCP.App.Services;

/// <summary>
/// NavigationService - Navigation yönetim servisi
/// 
/// Bu servis uygulama içi navigation'ı yönetir.
/// View'ler arası geçişler bu servis üzerinden yapılır.
/// 
/// MVVM Pattern:
/// - ViewModel'ler bu servisi kullanarak navigation yapar
/// - View'ler navigation'dan haberdar değildir (separation of concerns)
/// 
/// Single Responsibility: Navigation state ve view değişimi yönetimi
/// </summary>
public class NavigationService
{
    /// <summary>
    /// Mevcut ViewModel
    /// Şu an aktif olan view'in ViewModel'i
    /// </summary>
    public ViewModelBase? CurrentViewModel { get; private set; }
    
    /// <summary>
    /// Navigation event
    /// ViewModel değiştiğinde tetiklenir
    /// </summary>
    public event Action<ViewModelBase>? ViewModelChanged;
    
    /// <summary>
    /// Belirtilen ViewModel'e navigate eder
    /// </summary>
    public void NavigateTo(ViewModelBase viewModel)
    {
        CurrentViewModel = viewModel;
        ViewModelChanged?.Invoke(viewModel);
    }
}

/// <summary>
/// ViewModelBase - Tüm ViewModel'lerin base sınıfı
/// 
/// Ortak ViewModel işlevselliği için base class.
/// NavigationService için marker interface görevi görür.
/// </summary>
public abstract class ViewModelBase
{
    // Gelecekte ortak ViewModel işlevselliği buraya eklenecek
    // NavigationService bu base class'ı kullanarak type-safe navigation sağlar
}
