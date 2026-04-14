namespace NPS.Services.Interfaces;

public interface INormalizeService
{
    string NormalizeText(string text);
    int CountRemovedInjections(string text);
    int CountNormalizedHomoglyphs(string text);
}
