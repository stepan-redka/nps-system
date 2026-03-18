using NPS.Services.Interfaces;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows;

namespace NPS.Services;

public class ReplaceService : IReplaceService
{
    private readonly Dictionary<char, char> _latinToCyrillic = new()
    {
        ['a'] = '\u0430', ['e'] = '\u0435', ['o'] = '\u043E', 
        ['p'] = '\u0440', ['c'] = '\u0441', ['x'] = '\u0445', ['y'] = '\u0443',
        ['A'] = '\u0410', ['B'] = '\u0412', ['C'] = '\u0421', 
        ['E'] = '\u0415', ['H'] = '\u041D', ['K'] = '\u041A', 
        ['M'] = '\u041C', ['O'] = '\u041E', ['P'] = '\u0420', 
        ['T'] = '\u0422', ['X'] = '\u0425', ['Y'] = '\u0423'
    };
    
    private readonly Dictionary<char, char> _cyrillicToLatin;

    public ReplaceService()
    {
        _cyrillicToLatin = _latinToCyrillic.ToDictionary(pair => pair.Value, pair => pair.Key);
    }
    
    public string ReplaceWithHomoglyphs(string? text, bool mode)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
    
        Dictionary<char, char> activeDictionary;

        if (mode == true) 
        {
            activeDictionary = _latinToCyrillic; 
        }
        else 
        {
            activeDictionary = _cyrillicToLatin; 
        }
    
        var sb = new StringBuilder(text.Length);

        foreach (var c in text)
        {
            sb.Append(activeDictionary.GetValueOrDefault(c, c));
        }
    
        return sb.ToString();
    }
}