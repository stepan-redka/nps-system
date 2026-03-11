using Avalonia.Controls;
using Avalonia.Interactivity;

namespace NPS.Views;

public partial class AttackWindow : Window
{
    public AttackWindow()
    {
        InitializeComponent();
    }

    private void OnBackClicked(object? sender, RoutedEventArgs e)
    {
        var main = new MainWindow();
        main.Position = Position;
        main.Show();
        Close();
    }
}