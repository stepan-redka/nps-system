using System;
using System.Text;
using NPS.Services.Interfaces;

namespace NPS.Services;

public class InjectService : IInjectService
{
    private const char InvisibleChar = '\u200B'; // Zero Width Space
    private readonly Random _random = new();

    public string InjectInvisibleChars(string text, double frequency)
    {
        if (string.IsNullOrEmpty(text)) return text;

        var sb = new StringBuilder();

        foreach (var c in text)
        {
            sb.Append(c);
            // frequency має бути від 0.0 до 1.0 (наприклад, 0.2 — це 20% ймовірність)
            if (_random.NextDouble() < frequency)
            {
                sb.Append(InvisibleChar);
            }
        }

        return sb.ToString();
    }
}