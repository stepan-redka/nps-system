using NPS.Services.Interfaces;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace NPS.Services;

public class ReplaceService : IReplaceService
{
    private readonly Dictionary<char, char> _latinToCyrillic = new()
{
    // lowercase
    ['a'] = '\u0430', // а
    ['e'] = '\u0435', // е
    ['o'] = '\u043E', // о
    ['p'] = '\u0440', // р
    ['c'] = '\u0441', // с
    ['x'] = '\u0445', // х
    ['y'] = '\u0443', // у
    ['i'] = '\u0456', // і  
    // uppercase
    ['A'] = '\u0410', // А
    ['B'] = '\u0412', // В
    ['C'] = '\u0421', // С
    ['E'] = '\u0415', // Е
    ['H'] = '\u041D', // Н
    ['K'] = '\u041A', // К
    ['M'] = '\u041C', // М
    ['O'] = '\u041E', // О
    ['P'] = '\u0420', // Р
    ['T'] = '\u0422', // Т
    ['X'] = '\u0425', // Х
    ['Y'] = '\u0423', // У
    ['I'] = '\u0406'  // І  
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

        if (mode == false) 
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

    public bool IsTargetHomoglyph(char c, bool mode)
    {
        if (mode == true) 
        {
            return _latinToCyrillic.ContainsKey(c);
        }
        else 
        {
            return _cyrillicToLatin.ContainsKey(c);
        }
    }
}