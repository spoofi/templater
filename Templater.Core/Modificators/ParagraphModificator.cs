using Templater.Core.Abstractions;
using Templater.Core.Modificators.Abstractions;

namespace Templater.Core.Modificators;

public class ParagraphModificator : IModificator
{
    public string Name => "paragraph";

    /// <summary>
    /// Apply paragraph modificator
    /// </summary>
    /// <param name="input">input text</param>
    /// <param name="arguments">0 - before text; 1 - after text</param>
    /// <returns></returns>
    public object Apply(object? input, params string[] arguments)
    {
        var text = input?.ToString()?.Trim() ?? string.Empty;
        return arguments.Length == 0
            ? text
            : $"{arguments[0]}{text}{arguments[1]}";
    }
}
