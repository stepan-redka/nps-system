namespace NPS.Services.Interfaces;

public interface IInjectService
{
    string InjectInvisibleChars(string text, double frequency);
    bool IsInjectedChar(char c); 
}

public interface IReplaceService
{
    string ReplaceWithHomoglyphs(string text, bool mode);
    bool IsTargetHomoglyph(char c, bool mode); 
}