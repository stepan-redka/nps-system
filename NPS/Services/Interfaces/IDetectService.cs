namespace NPS.Services.Interfaces;

using System.Collections.Generic;

public interface IDetectService
{
    Dictionary<char, int> CalculateFrequencies(string text);
    double CalculateProbabilityOfSymbol(char symbol, int totalLength);
    double CalculateEntropy(string text);
    Dictionary<string, int> CalculateAlphabetDistribution(string text);
}