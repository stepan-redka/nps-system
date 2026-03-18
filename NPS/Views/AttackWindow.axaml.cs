using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System.IO;
using Avalonia.Media;
using Avalonia.Controls.Documents;
using NPS.Services.Interfaces;

namespace NPS.Views;

public partial class AttackWindow : Window
{

    private readonly IInjectService _injector;
    private readonly IReplaceService _replacer;
    private string _lastInjectedText = string.Empty;

    public AttackWindow(IInjectService injector, IReplaceService replacer)
    {
        InitializeComponent();
        _injector = injector;
        _replacer = replacer;
    }

    private void OnBackClicked(object? sender, RoutedEventArgs e)
    {
        var main = new MainWindow(_injector, _replacer);
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
        string originalText = InputTextBox.Text ?? string.Empty;
        double freq = FrequencySlider.Value;
        bool isReverse = DirectionModeCheckBox.IsChecked == true;

        string activeText = originalText;
        int mode = AttackTypeComboBox.SelectedIndex;

        // 0: Injection, 1: Replacement, 2: Combined
        if (mode == 0 || mode == 2)
        {
             activeText = _injector.InjectInvisibleChars(activeText, freq);
        }

        //saved the state for later comparison
        _lastInjectedText = activeText;

        if (mode == 1 || mode == 2)
        {
             activeText = _replacer.ReplaceWithHomoglyphs(activeText, isReverse);
        }
        
        OutputTextBox.Text = activeText;
    }

    private async void OnCopyClicked(object? sender, RoutedEventArgs e)
    {
        if (Clipboard is not null && OutputTextBox.Text is not null)
            await Clipboard.SetTextAsync(OutputTextBox.Text);
    }
    
    private async void OnSaveFileClicked(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Adversarial Text",
            DefaultExtension = "txt",
            SuggestedFileName = "attack_output.txt"
        });

        if (file is not null)
        {
            await using var stream = await file.OpenWriteAsync();
            using var writer = new StreamWriter(stream);
            await writer.WriteAsync(OutputTextBox.Text ?? string.Empty);
        }
    }

    private void OnInspectClicked(object? sender, RoutedEventArgs e)
    {
        VisualizerBlock.Inlines?.Clear();

        string outputText = OutputTextBox.Text ?? string.Empty;
        
        if (string.IsNullOrEmpty(outputText) || string.IsNullOrEmpty(_lastInjectedText)) return;
        
        if (outputText.Length != _lastInjectedText.Length) return;

        // Використовуємо for замість foreach
        for (int i = 0; i < outputText.Length; i++)
        {
            char currentChar = outputText[i];
            
            // 1. Запитуємо InjectService (тут нічого не змінилося)
            if (_injector.IsInjectedChar(currentChar))
            {
                var run = new Run("•") { Foreground = Brushes.Red, FontWeight = Avalonia.Media.FontWeight.Bold };
                VisualizerBlock.Inlines?.Add(run);
            }
            // 2. Шукаємо гомогліфи: якщо символ у фінальному тексті відрізняється від проміжного...
            else if ( currentChar != _lastInjectedText[i] && _replacer.IsTargetHomoglyph(currentChar, DirectionModeCheckBox.IsChecked == true) )
            {
                var run = new Run(currentChar.ToString()) { Foreground = Brushes.Green, FontWeight = Avalonia.Media.FontWeight.Bold };
                VisualizerBlock.Inlines?.Add(run);
            }
            // 3. Звичайний текст
            else
            {
                var run = new Run(currentChar.ToString()) { Foreground = Brushes.Gray };
                VisualizerBlock.Inlines?.Add(run);
            }
        }
    }
}