using NPS.Services.Interfaces;
using System.Collections.Generic;
using System.Text;

namespace NPS.Services;

public class ReplaceService : IReplaceService
{
    private readonly Dictionary<char, char> _homoglyphs = new()
    {
        ['a'] = '\u0430', ['e'] = '\u0435', ['o'] = '\u043E', 
        ['p'] = '\u0440', ['c'] = '\u0441', ['x'] = '\u0445', ['y'] = '\u0443',
        ['A'] = '\u0410', ['B'] = '\u0412', ['C'] = '\u0421', 
        ['E'] = '\u0415', ['H'] = '\u041D', ['K'] = '\u041A', 
        ['M'] = '\u041C', ['O'] = '\u041E', ['P'] = '\u0420', 
        ['T'] = '\u0422', ['X'] = '\u0425', ['Y'] = '\u0423'
    };
    
    public string ReplaceWithHomoglyphs(string? text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        
        var sb = new StringBuilder(text.Length);

        foreach (var c in text)
        {
            // Метод GetValueOrDefault поверне заміну, якщо вона є, 
            // або сам символ c, якщо його немає в словнику
            sb.Append(_homoglyphs.GetValueOrDefault(c, c));
        }
        
        return sb.ToString();
    }

    public ReplaceService()
    {
    }
}