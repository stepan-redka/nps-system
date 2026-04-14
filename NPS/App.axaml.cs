using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using NPS.Views;
using NPS.Services;
using NPS.Services.Interfaces;

namespace NPS;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var injectService = new InjectService();
            var replaceService = new ReplaceService();
            var detectService = new DetectService();
            var normalizeService = new NormalizeService();

            desktop.MainWindow = new MainWindow(injectService, replaceService, detectService, normalizeService);
        }

        base.OnFrameworkInitializationCompleted();
    }
}