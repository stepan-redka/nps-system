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

        var distribution = _detector.CalculateAlphabetDistribution(text);
        
        int zeroWidth  = CountZeroWidth(text);
        int bidi       = CountBidi(text);
        int homoglyphs = DetectHomoglyphs(text, distribution);

        // Entropy analysis: detect low entropy (randomness) that might indicate obfuscation
        double entropy = _detector.CalculateEntropy(text);
        // Only penalize if entropy is EXTREMELY low for the given length
        int entropyPenalty = (text.Length > 30 && entropy < 2.2) ? 15 : 0;

        // Alphabet mixing penalty
        int mixingPenalty = DetectAlphabetMixing(distribution);

        int score = CalculateScore(zeroWidth, homoglyphs, bidi, text.Length, entropyPenalty, mixingPenalty);

        // Update UI metrics
        ZeroWidthCount.Text  = zeroWidth.ToString();
        HomoglyphCount.Text  = homoglyphs.ToString();
        BidiCount.Text       = bidi.ToString();
        ScoreValue.Text      = score.ToString();

        // Update score bar
        ScoreBar.Width = score * 2;
        ScoreBar.Background = score >= SuspicionThreshold
            ? new SolidColorBrush(Color.Parse("#C0392B"))
            : new SolidColorBrush(Color.Parse("#4F46E5"));

        // Update distribution charts
        UpdateDistributionCharts(distribution);

        // Verdict banner
        UpdateVerdict(score, zeroWidth, homoglyphs, bidi);

        // Normalization
        PerformNormalization(text);

        // Findings list
        PopulateFindings(text, zeroWidth, bidi, entropy, distribution);
    }

    private void UpdateDistributionCharts(Dictionary<string, int> distribution)
    {
        int total = distribution.Values.Sum();
        if (total == 0) return;

        double latinRatio = (double)distribution["Latin"] / total;
        double cyrillicRatio = (double)distribution["Cyrillic"] / total;
        double otherRatio = (double)distribution["Other"] / total;

        // Animate or set widths (using 200 as base width for the bars)
        LatinBar.Width = latinRatio * 200;
        CyrillicBar.Width = cyrillicRatio * 200;
        OtherBar.Width = otherRatio * 200;

        LatinPct.Text = $"{latinRatio:P0}";
        CyrillicPct.Text = $"{cyrillicRatio:P0}";
        OtherPct.Text = $"{otherRatio:P0}";
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

        VerdictTitle.Text      = suspicious ? "SUSPICIOUS — THREAT DETECTED" : "CLEAN — NO THREAT DETECTED";
        VerdictTitle.Foreground = suspicious
            ? new SolidColorBrush(Color.Parse("#C0392B"))
            : new SolidColorBrush(Color.Parse("#10B981"));
        VerdictIcon.Text       = suspicious ? "⚠" : "✓";
        VerdictIcon.Foreground = VerdictTitle.Foreground;
        VerdictSub.Text        = suspicious
            ? $"Identified {zw + hg + bidi} anomalies. Security score {score}/100 exceeds safety threshold."
            : $"Text analysis completed. Security score {score}/100 is within nominal range.";
    }

    private void PopulateFindings(string text, int zw, int bidi, double entropy, Dictionary<string, int> distribution)
    {
        FindingsPanel.Children.Clear();
        var findings = new List<string>();

        if (zw > 0)  findings.Add($"{zw}× invisible character injection(s)");
        if (bidi > 0) findings.Add($"{bidi}× bidirectional override(s)");

        if (text.Length > 30 && entropy < 2.5) findings.Add($"Low entropy ({entropy:F2}) suggests pattern obfuscation");

        int total = distribution.Values.Sum();
        if (total > 0)
        {
            int latin = distribution["Latin"];
            int cyrillic = distribution["Cyrillic"];
            if (latin > 0 && cyrillic > 0)
            {
                double latinRatio = (double)latin / total;
                double cyrillicRatio = (double)cyrillic / total;
                if (latinRatio < 0.9 && cyrillicRatio < 0.9)
                    findings.Add("Anomalous script mixing (Latin/Cyrillic)");
            }
        }

        if (findings.Count == 0)
        {
            FindingsPanel.Children.Add(new TextBlock
            {
                Text = "No security anomalies detected.",
                Foreground = new SolidColorBrush(Color.Parse("#6B7280")),
                FontSize = 11,
                FontStyle = FontStyle.Italic
            });
            return;
        }

        foreach (var finding in findings)
            AddFinding(finding, "#EF4444");
    }

    private void AddFinding(string message, string color)
    {
        FindingsPanel.Children.Add(new TextBlock
        {
            Text = $"• {message}",
            Foreground = new SolidColorBrush(Color.Parse(color)),
            FontSize = 11,
            Margin = new Avalonia.Thickness(0, 2)
        });
    }

    private void ResetUI()
    {
        ZeroWidthCount.Text = HomoglyphCount.Text = BidiCount.Text = ScoreValue.Text = "0";
        ScoreBar.Width = 0;
        LatinBar.Width = CyrillicBar.Width = OtherBar.Width = 0;
        LatinPct.Text = CyrillicPct.Text = OtherPct.Text = "0%";
        VerdictTitle.Text = "AWAITING ANALYSIS";
        VerdictTitle.Foreground = new SolidColorBrush(Color.Parse("#374151"));
        VerdictIcon.Text = "•";
        VerdictIcon.Foreground = VerdictTitle.Foreground;
        VerdictSub.Text = "Paste text and click Analyze to begin.";
        FindingsPanel.Children.Clear();
        NormalizedTextBox.Text = string.Empty;
        NormalizationStats.Text = "No recovery data yet.";
        CopyNormalizedButton.IsEnabled = false;
    }

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
        double raw = (zw * 4.0 + hg * 5.0 + bidi * 6.0) / length * 350;
        return (int)System.Math.Min(raw + (zw + hg + bidi) * 2 + entropyPenalty + mixingPenalty, 100);
    }

    private int DetectHomoglyphs(string text, Dictionary<string, int> distribution)
    {
        int latin = distribution["Latin"];
        int cyrillic = distribution["Cyrillic"];
        
        if (latin == 0 || cyrillic == 0) return 0; // No mixing, no homoglyph threat

        // Detect characters from the "minority" script that are valid homoglyphs
        bool detectLatinAsHomoglyphs = latin < cyrillic;
        int count = 0;
        var seenChars = new HashSet<char>();

        foreach (char c in text)
        {
            if (!char.IsLetter(c)) continue;
            if (seenChars.Add(c))
            {
                if (detectLatinAsHomoglyphs)
                {
                    if (IsLatin(c) && _replacer.IsTargetHomoglyph(c, true)) count++;
                }
                else
                {
                    if (IsCyrillic(c) && _replacer.IsTargetHomoglyph(c, false)) count++;
                }
            }
        }
        return count;
    }

    private bool IsLatin(char c) => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
    private bool IsCyrillic(char c) => (c >= 'А' && c <= 'я') || c == 'Ё' || c == 'ё' || c == 'і' || c == 'І';

    private int DetectAlphabetMixing(Dictionary<string, int> distribution)
    {
        int total = distribution.Values.Sum();
        if (total == 0) return 0;

        int latin = distribution["Latin"];
        int cyrillic = distribution["Cyrillic"];

        if (latin > 0 && cyrillic > 0)
        {
            double latinRatio = (double)latin / total;
            double cyrillicRatio = (double)cyrillic / total;

            // Only penalize if mixing is significant (at least 2% and less than 98%)
            if (latinRatio > 0.02 && latinRatio < 0.98 && cyrillicRatio > 0.02 && cyrillicRatio < 0.98)
                return 15;
        }

        return 0;
    }
}