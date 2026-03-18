using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using NPS;
using System.IO;
using NPS.Services;
using NPS.Services.Interfaces;

namespace NPS.Views;

public partial class DetectWindow : Window
{
private readonly IInjectService _injector;
    private readonly IReplaceService _replacer;    // hardcoded suspicion threshold (0–100)
    private const int SuspicionThreshold = 40;

    public DetectWindow(IInjectService injector, IReplaceService replacer)
    {
        InitializeComponent();
        _injector = injector;
        _replacer = replacer;
    }

    private void OnBackClicked(object? sender, RoutedEventArgs e)
    {
        var main = new MainWindow(_injector, _replacer);
        main.Position = Position;
        main.Width = Width;
        main.Height = Height;
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
    {
        InputTextBox.Text = string.Empty;
        ResetUI();
    }

    private void OnAnalyzeClicked(object? sender, RoutedEventArgs e)
    {
        var text = InputTextBox.Text ?? string.Empty;
        if (string.IsNullOrWhiteSpace(text)) return;

        // TODO: replace with your real engine results
        int zeroWidth  = CountZeroWidth(text);
        int homoglyphs = 0;   // plug in your engine
        int bidi       = CountBidi(text);
        int score      = CalculateScore(zeroWidth, homoglyphs, bidi, text.Length);

        // update metric tiles
        ZeroWidthCount.Text  = zeroWidth.ToString();
        HomoglyphCount.Text  = homoglyphs.ToString();
        BidiCount.Text       = bidi.ToString();
        ScoreValue.Text      = score.ToString();

        // update score bar (max width bound to parent — approximate via fixed 200px)
        ScoreBar.Width = score * 2;
        ScoreBar.Background = score >= SuspicionThreshold
            ? new SolidColorBrush(Color.Parse("#C0392B"))
            : new SolidColorBrush(Color.Parse("#5C6CF0"));

        // verdict banner
        UpdateVerdict(score, zeroWidth, homoglyphs, bidi);

        // findings list
        PopulateFindings(text, zeroWidth, bidi);
    }

    private void UpdateVerdict(int score, int zw, int hg, int bidi)
    {
        bool suspicious = score >= SuspicionThreshold;

        VerdictTitle.Text      = suspicious ? "SUSPICIOUS — THRESHOLD EXCEEDED" : "CLEAN — BELOW THRESHOLD";
        VerdictTitle.Foreground = suspicious
            ? new SolidColorBrush(Color.Parse("#C0392B"))
            : new SolidColorBrush(Color.Parse("#27AE60"));
        VerdictIcon.Text       = suspicious ? "⚠" : "✓";
        VerdictIcon.Foreground = VerdictTitle.Foreground;
        VerdictSub.Text        = suspicious
            ? $"Detected {zw + hg + bidi} anomalies. Score {score}/100 exceeds threshold of {SuspicionThreshold}."
            : $"No significant anomalies. Score {score}/100 is within safe range.";
    }

    private void PopulateFindings(string text, int zw, int bidi)
    {
        FindingsPanel.Children.Clear();
        if (zw == 0 && bidi == 0)
        {
            FindingsPanel.Children.Add(new TextBlock
            {
                Text = "No injections found.",
                Foreground = new SolidColorBrush(Color.Parse("#25253A")),
                FontSize = 10
            });
            return;
        }
        if (zw > 0)  AddFinding($"{zw}× zero-width character(s) found", "#5C6CF0");
        if (bidi > 0) AddFinding($"{bidi}× Bidi override character(s) found", "#C0392B");
    }

    private void AddFinding(string message, string color)
    {
        FindingsPanel.Children.Add(new TextBlock
        {
            Text = $"· {message}",
            Foreground = new SolidColorBrush(Color.Parse(color)),
            FontSize = 10
        });
    }

    private void ResetUI()
    {
        ZeroWidthCount.Text = HomoglyphCount.Text = BidiCount.Text = ScoreValue.Text = "—";
        ScoreBar.Width = 0;
        VerdictTitle.Text = "AWAITING ANALYSIS";
        VerdictTitle.Foreground = new SolidColorBrush(Color.Parse("#28283A"));
        VerdictIcon.Text = "·";
        VerdictIcon.Foreground = VerdictTitle.Foreground;
        VerdictSub.Text = "Paste text and click Analyze to begin.";
        FindingsPanel.Children.Clear();
    }

    // --- placeholder detection helpers (replace with your engine) ---
    private static int CountZeroWidth(string t)
    {
        int n = 0;
        foreach (var c in t)
            if (c == '\u200B' || c == '\u200C' || c == '\u200D' || c == '\uFEFF') n++;
        return n;
    }

    private static int CountBidi(string t)
    {
        int n = 0;
        foreach (var c in t)
            if (c == '\u202A' || c == '\u202B' || c == '\u202C' ||
                c == '\u202D' || c == '\u202E' || c == '\u2066' ||
                c == '\u2067' || c == '\u2068' || c == '\u2069') n++;
        return n;
    }

    private static int CalculateScore(int zw, int hg, int bidi, int length)
    {
        if (length == 0) return 0;
        double raw = (zw * 3.0 + hg * 4.0 + bidi * 5.0) / length * 300;
        return (int)System.Math.Min(raw + (zw + hg + bidi) * 2, 100);
    }
}