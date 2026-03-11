namespace NPS.Services.Interfaces;

public interface IInjectService
{
    string InjectInvisibleChars(string text, double frequency);
}

public interface IReplaceService
{
    string ReplaceWithHomoglyphs(string text);
}