namespace NPS.Services;
using Interfaces;
using System;
using System.Collections.Generic;

public class DetectService : IDetectService
{
    private readonly Dictionary<char, int> _frequencies = new Dictionary<char, int>();
    
    public Dictionary<char, int> CalculateFrequencies(string text)
    {
        _frequencies.Clear();
        foreach (var item in text)
        {
            if (_frequencies.ContainsKey(item))
        {
            _frequencies[item]++;
        }
        else
        {
            _frequencies[item] = 1;
        }
        }
    return _frequencies;
    }

    public double CalculateProbabilityOfSymbol(char symbol, int totalLength)
    {
        if (totalLength == 0 || !_frequencies.ContainsKey(symbol)) return 0.0;
        return (double)_frequencies[symbol] / totalLength;
    }

    //формула Шенно: H(X) = -Σ P(x_i) * log2(P(x_i))
    public double CalculateEntropy(string text)
    {
        if (string.IsNullOrEmpty(text)) return 0.0;

        CalculateFrequencies(text); 
        
        double entropy = 0.0;
        int length = text.Length;

        // 2. Проходимося по всіх унікальних символах (ключах нашого словника)
        foreach (char symbol in _frequencies.Keys)
        {
            double probability = CalculateProbabilityOfSymbol(symbol, length);
            if (probability > 0)
            {
                entropy += probability * Math.Log(probability, 2); // log base 2
            }
        }
        //логарифми дають від'ємне значення для ймовірностей ([0, 1]), 
        //тому повертаємо -entropy, щоб отримати позитивну ентропію
        return -entropy; 
    }
}