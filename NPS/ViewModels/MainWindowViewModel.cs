using System.Diagnostics;
using System.Windows.Input;
using NPS.Services;
using NPS.Services.Interfaces;

namespace NPS.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IAttackService _attackService;
    private readonly IDefenceService _defenceService;
    private readonly IAnalyticsService _analyticsService;

    public ICommand AttackCommand { get; }
    public ICommand DetectCommand { get; }
    public ICommand MetricsCommand { get; }

    public MainWindowViewModel(
        IAttackService attackService,
        IDefenceService defenceService,
        IAnalyticsService analyticsService)
    {
        _attackService = attackService;
        _defenceService = defenceService;
        _analyticsService = analyticsService;

        AttackCommand = new RelayCommand(OnAttack);
        DetectCommand = new RelayCommand(OnDetect);
        MetricsCommand = new RelayCommand(OnMetrics);
    }

    private void OnAttack()
    {
        Debug.WriteLine("Attack Simulation Launched!");
        // TODO: call _attackService
    }

    private void OnDetect()
    {
        Debug.WriteLine("Detection Simulation Launched!");
        // TODO: call _defenceService
    }

    private void OnMetrics()
    {
        // TODO: call _analyticsService
    }
}
