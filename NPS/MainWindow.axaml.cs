using Avalonia.Controls;
using System.Diagnostics;
using Avalonia.Interactivity;

namespace NPS;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    
    private void OnAttackClicked(object? sender, RoutedEventArgs e)
    {
        // For now, let's just print to the console to prove it works
        Debug.WriteLine("Attack Simulation Launched!");
        
        // TODO: Call your Unicode logic here
        // var engine = new UnicodeAttackEngine();
        // engine.Simulate();
    }
    
    private void OnDetectClicked(object? sender, RoutedEventArgs e)
    {
        Debug.WriteLine("Delete Simulation Launched!");
    }

    private void OnMetricsClicked(object? sender, RoutedEventArgs e)
    {
        
    }
}