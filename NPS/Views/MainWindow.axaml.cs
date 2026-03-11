using Avalonia.Controls;
using NPS.Services;
using NPS.ViewModels;

namespace NPS;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel(
            new AttackService(),
            new DefenceService(),
            new AnalyticsService());
    }
}