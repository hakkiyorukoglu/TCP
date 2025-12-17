using System.Windows;
using System.Windows.Controls;

namespace TCP.App.UI.Components;

/// <summary>
/// BoardDetailCard.xaml.cs - Reusable board detail card component code-behind
/// 
/// TCP-0.6.1: Board Details Components (Reusable)
/// 
/// Zero business logic - only dependency properties for binding.
/// </summary>
public partial class BoardDetailCard : UserControl
{
    /// <summary>
    /// Title dependency property
    /// </summary>
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(BoardDetailCard),
            new PropertyMetadata(string.Empty));
    
    /// <summary>
    /// Title - Card header text
    /// </summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    
    // Note: Content property is inherited from ContentControl base class
    
    /// <summary>
    /// Constructor
    /// </summary>
    public BoardDetailCard()
    {
        InitializeComponent();
    }
}
