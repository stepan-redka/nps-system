using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using NPS;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using NPS.Services;
using NPS.Services.Interfaces;

namespace NPS.Views;

public partial class DetectWindow : Window
{
    private readonly IInjectService _injector;
    private readonly IReplaceService _replacer;
    private readonly IDetectService _detector;
    private readonly INormalizeService _normalizer;
    private const int SuspicionThreshold = 40;

    public DetectWindow(IInjectService injector, IReplaceService replacer, IDetectService detector, INormalizeService normalizer)
    {
        InitializeComponent();
        _injector = injector;
        _replacer = replacer;
        _detector = detector;
        _normalizer = normalizer;
    }

    private void OnBackClicked(object? sender, RoutedEventArgs e)
    {
        var main = new MainWindow(_injector, _replacer, _detector, _normalizer);
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

        int zeroWidth  = CountZeroWidth(text);
        int homoglyphs = DetectHomoglyphs(text);
        int bidi       = CountBidi(text);

        // Entropy analysis: detect low entropy (randomness) that might indicate obfuscation
        double entropy = _detector.CalculateEntropy(text);
        // Reduce sensitivity: only penalize if entropy is extremely low (e.g., < 2.5 for short strings or highly repetitive content)
        int entropyPenalty = (text.Length > 20 && entropy < 2.5) ? 15 : 0;

        // Alphabet mixing: detect unusual language mixing
        var distribution = _detector.CalculateAlphabetDistribution(text);
        int mixingPenalty = DetectAlphabetMixing(distribution);

        int score = CalculateScore(zeroWidth, homoglyphs, bidi, text.Length, entropyPenalty, mixingPenalty);

        // update metric tiles
        ZeroWidthCount.Text  = zeroWidth.ToString();
        HomoglyphCount.Text  = homoglyphs.ToString();
        BidiCount.Text       = bidi.ToString();
        ScoreValue.Text      = score.ToString();

        // update score bar
        ScoreBar.Width = score * 2;
        ScoreBar.Background = score >= SuspicionThreshold
            ? new SolidColorBrush(Color.Parse("#C0392B"))
            : new SolidColorBrush(Color.Parse("#5C6CF0"));

        // verdict banner
        UpdateVerdict(score, zeroWidth, homoglyphs, bidi);

        // normalization
        PerformNormalization(text);

        // findings list
        PopulateFindings(text, zeroWidth, bidi, entropy, distribution);
    }

    private void PerformNormalization(string text)
    {
        string cleanText = _normalizer.NormalizeText(text);
        int removed = _normalizer.CountRemovedInjections(text);
        int restored = _normalizer.CountNormalizedHomoglyphs(text);

        NormalizedTextBox.Text = cleanText;
        NormalizationStats.Text = $"Restored {restored} characters and removed {removed} hidden symbols.";
        CopyNormalizedButton.IsEnabled = !string.IsNullOrEmpty(cleanText);
    }

    private async void OnCopyNormalizedClicked(object? sender, RoutedEventArgs e)
    {
        if (Clipboard is not null && !string.IsNullOrEmpty(NormalizedTextBox.Text))
        {
            await Clipboard.SetTextAsync(NormalizedTextBox.Text);
        }
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

    private void PopulateFindings(string text, int zw, int bidi, double entropy, Dictionary<string, int> distribution)
    {
        FindingsPanel.Children.Clear();
        var findings = new List<string>();

        if (zw > 0)  findings.Add($"{zw}× zero-width character(s)");
        if (bidi > 0) findings.Add($"{bidi}× Bidi override character(s)");

        if (entropy < 3.5) findings.Add($"Low entropy ({entropy:F2}) suggests obfuscation");

        int total = distribution.Values.Sum();
        if (total > 0)
        {
            int latin = distribution["Latin"];
            int cyrillic = distribution["Cyrillic"];
            if (latin > 0 && cyrillic > 0 && latin < total * 0.9 && cyrillic < total * 0.9)
                findings.Add("Mixed Latin/Cyrillic detected");
        }

        if (findings.Count == 0)
        {
            FindingsPanel.Children.Add(new TextBlock
            {
                Text = "No anomalies detected.",
                Foreground = new SolidColorBrush(Color.Parse("#25253A")),
                FontSize = 10
            });
            return;
        }

        foreach (var finding in findings)
            AddFinding(finding, findings.IndexOf(finding) == 0 ? "#5C6CF0" : "#C0392B");
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
        NormalizedTextBox.Text = string.Empty;
        NormalizationStats.Text = "No recovery data yet.";
        CopyNormalizedButton.IsEnabled = false;
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

    private static int CalculateScore(int zw, int hg, int bidi, int length, int entropyPenalty, int mixingPenalty)
    {
        if (length == 0) return 0;
        double raw = (zw * 3.0 + hg * 4.0 + bidi * 5.0) / length * 300;
        return (int)System.Math.Min(raw + (zw + hg + bidi) * 2 + entropyPenalty + mixingPenalty, 100);
    }

    private int DetectHomoglyphs(string text)
    {
        int count = 0;
        var seenCombos = new HashSet<(char, char)>();

        for (int i = 0; i < text.Length - 1; i++)
        {
            char curr = text[i];
            char next = text[i + 1];

            // Check if current char is a homoglyph replacement (exists in replacer mapping)
            if (!char.IsLetter(curr) || !char.IsLetter(next))
                continue;

            var combo = (curr, next);
            if (seenCombos.Add(combo))
            {
                // Test if this could be a homoglyph (basic heuristic)
                if (_replacer.IsTargetHomoglyph(curr, false) || _replacer.IsTargetHomoglyph(curr, true))
                    count++;
            }
        }

        return count;
    }

    private int DetectAlphabetMixing(Dictionary<string, int> distribution)
    {
        int total = distribution.Values.Sum();
        if (total == 0) return 0;

        int latin = distribution["Latin"];
        int cyrillic = distribution["Cyrillic"];

        // Penalize if both scripts present in significant amounts (10-90% each)
        if (latin > 0 && cyrillic > 0)
        {
            double latinRatio = (double)latin / total;
            double cyrillicRatio = (double)cyrillic / total;

            if (latinRatio > 0.1 && latinRatio < 0.9 && cyrillicRatio > 0.1 && cyrillicRatio < 0.9)
                return 10;
        }

        return 0;
    }
}