using Templater.Core.Abstractions;

namespace Templater.Core.Modificators;

public class ParagraphModificator : IModificator
{
    public string Name => "paragraph";

    public object Apply(object? input, params string[] arguments)
    {
        var text = input?.ToString()?.Trim() ?? string.Empty;
        // 2026-03-02 A.Suvorov: can expand logic. For example, add <p></p> tags
        return text;
    }
}
