using System;
using System.IO;
using System.Windows.Markup;

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        string xaml1 = @"<Style xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
            <Style.Triggers>
                <DataTrigger Binding=""{Binding}"" Value=""{Binding}""><Setter Property=""Opacity"" Value=""1""/></DataTrigger>
            </Style.Triggers>
        </Style>";

        string xaml2 = @"<Style xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
            <Setter Property=""Tag"" Value=""{Binding}"" />
        </Style>";
        
        string xaml3 = @"<Style xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
            <Style.Triggers>
                <DataTrigger Binding=""{Binding}""><Setter Property=""Opacity"" Value=""{Binding}""/></DataTrigger>
            </Style.Triggers>
        </Style>";

        Test(xaml1, "Test 1 (DataTrigger Value={Binding})");
        Test(xaml2, "Test 2 (Setter Value={Binding})");
        Test(xaml3, "Test 3 (DataTrigger Setter Value={Binding})");
    }

    static void Test(string xaml, string name)
    {
        try {
            XamlReader.Parse(xaml);
            Console.WriteLine(name + " OK");
        } catch (Exception ex) {
            Console.WriteLine(name + " ERROR: " + ex.Message);
        }
    }
}
