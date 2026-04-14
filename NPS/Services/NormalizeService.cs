using System;
using System.Collections.Generic;
using System.Text;
using NPS.Services.Interfaces;

namespace NPS.Services;

public class NormalizeService : INormalizeService
{
    private readonly Dictionary<char, char> _homoglyphMap = new()
    {
        // Cyrillic to Latin mapping (reversed from ReplaceService)
        ['\u0430'] = 'a',
        ['\u0435'] = 'e',
        ['\u043E'] = 'o',
        ['\u0440'] = 'p',
        ['\u0441'] = 'c',
        ['\u0445'] = 'x',
        ['\u0443'] = 'y',
        ['\u0456'] = 'i',
        ['\u0410'] = 'A',
        ['\u0412'] = 'B',
        ['\u0421'] = 'C',
        ['\u0415'] = 'E',
        ['\u041D'] = 'H',
        ['\u041A'] = 'K',
        ['\u041C'] = 'M',
        ['\u041E'] = 'O',
        ['\u0420'] = 'P',
        ['\u0422'] = 'T',
        ['\u0425'] = 'X',
        ['\u0423'] = 'Y',
        ['\u0406'] = 'I'
    };

    private static readonly char[] InvisibleChars = {
        '\u200B', '\u200C', '\u200D', '\uFEFF'
    };

    private static readonly char[] BidiChars = {
        '\u202A', '\u202B', '\u202C', '\u202D', '\u202E', '\u2066', '\u2067', '\u2068', '\u2069'
    };

    public string NormalizeText(string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;

        var sb = new StringBuilder(text.Length);

        foreach (var c in text)
        {
            if (IsInvisible(c) || IsBidi(c)) continue;

            if (_homoglyphMap.TryGetValue(c, out var mapped))
            {
                sb.Append(mapped);
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    public int CountRemovedInjections(string text)
    {
        if (string.IsNullOrEmpty(text)) return 0;
        int count = 0;
        foreach (var c in text)
        {
            if (IsInvisible(c) || IsBidi(c)) count++;
        }
        return count;
    }

    public int CountNormalizedHomoglyphs(string text)
    {
        if (string.IsNullOrEmpty(text)) return 0;
        int count = 0;
        foreach (var c in text)
        {
            if (_homoglyphMap.ContainsKey(c)) count++;
        }
        return count;
    }

    private bool IsInvisible(char c) => Array.IndexOf(InvisibleChars, c) >= 0;
    private bool IsBidi(char c) => Array.IndexOf(BidiChars, c) >= 0;
}
