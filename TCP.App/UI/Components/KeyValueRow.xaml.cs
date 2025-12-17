using System.Windows;
using System.Windows.Controls;

namespace TCP.App.UI.Components;

/// <summary>
/// KeyValueRow.xaml.cs - Reusable Key/Value row component code-behind
/// 
/// TCP-0.6.1: Board Details Components (Reusable)
/// 
/// Zero business logic - only dependency properties for binding.
/// </summary>
public partial class KeyValueRow : UserControl
{
    /// <summary>
    /// Key dependency property
    /// </summary>
    public static readonly DependencyProperty KeyProperty =
        DependencyProperty.Register(nameof(Key), typeof(string), typeof(KeyValueRow),
            new PropertyMetadata(string.Empty));
    
    /// <summary>
    /// Value dependency property
    /// </summary>
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(string), typeof(KeyValueRow),
            new PropertyMetadata(string.Empty));
    
    /// <summary>
    /// Key - Label text
    /// </summary>
    public string Key
    {
        get => (string)GetValue(KeyProperty);
        set => SetValue(KeyProperty, value);
    }
    
    /// <summary>
    /// Value - Value text
    /// </summary>
    public string Value
    {
        get => (string)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
    
    /// <summary>
    /// Constructor
    /// </summary>
    public KeyValueRow()
    {
        InitializeComponent();
    }
}
