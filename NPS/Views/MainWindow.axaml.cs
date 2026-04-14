using Avalonia.Controls;
using Avalonia.Interactivity;
using NPS.Views;
using NPS.Services.Interfaces;

namespace NPS;

public partial class MainWindow : Window
{
    private readonly IInjectService _injector;
    private readonly IReplaceService _replacer;
    private readonly IDetectService _detector;
    private readonly INormalizeService _normalizer;

    public MainWindow(IInjectService injector, IReplaceService replacer, IDetectService detector, INormalizeService normalizer)
    {
        InitializeComponent();
        ScaleToScreen();

        _injector = injector;
        _replacer = replacer;
        _detector = detector;
        _normalizer = normalizer;
    }

    private void ScaleToScreen()
    {
        var screen = Screens.Primary;
        if (screen is null) return;

        // 55% of screen width, 50% of screen height
        var targetW = screen.WorkingArea.Width * 0.55;
        var targetH = screen.WorkingArea.Height * 0.50;

        Width  = targetW;
        Height = targetH;

        // clamp minimums so it never gets too tiny
        MinWidth  = 520;
        MinHeight = 280;

        WindowStartupLocation = WindowStartupLocation.CenterScreen;
    }

   private void OnAttackClicked(object? sender, RoutedEventArgs e)
    {
        var window = new AttackWindow(_injector, _replacer, _detector, _normalizer);
        window.Position = Position;
        window.Width    = Width;
        window.Height   = Height;
        window.Show();
        Close();
    }

    private void OnDetectClicked(object? sender, RoutedEventArgs e)
    {
        var window = new DetectWindow(_injector, _replacer, _detector, _normalizer);
        window.Position = Position;
        window.Width    = Width;
        window.Height   = Height;
        window.Show();
        Close();
    }
}