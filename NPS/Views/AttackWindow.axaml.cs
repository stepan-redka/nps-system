using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System.IO;

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
    
    private async void OnLoadFileClicked(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Text File",
            AllowMultiple = false
        });

        if (files.Count >= 1)
        {
            await using var stream = await files[0].OpenReadAsync();
            using var reader = new StreamReader(stream);
            InputTextBox.Text = await reader.ReadToEndAsync();
        }
    }

    private void OnClearClicked(object? sender, RoutedEventArgs e)
        => InputTextBox.Text = string.Empty;

    private void OnRunClicked(object? sender, RoutedEventArgs e)
    {
        // TODO: pass InputTextBox.Text to your attack engine
        OutputTextBox.Text = "// results here";
    }

    private async void OnCopyClicked(object? sender, RoutedEventArgs e)
    {
        if (Clipboard is not null && OutputTextBox.Text is not null)
            await Clipboard.SetTextAsync(OutputTextBox.Text);
    }
}