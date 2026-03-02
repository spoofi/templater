using System.Diagnostics.CodeAnalysis;
using Templater.Core.Exceptions;
using Templater.Core.Render;

namespace Templater.Core.Parser.Nodes;

/// <summary>
/// Variable node - renders variable value with modifications if needed
/// </summary>
public class VariableNode : BaseNode
{
    public VariableNode(string expression)
    {
        ArgumentNullException.ThrowIfNull(expression);
        ParseExpression(expression);
    }

    private string JsonPath { get; set; } = string.Empty;
    private List<ModificatorWithArgs> Modificators { get; } = [];


    private void ParseExpression(string expression)
    {
        var parts = expression.Split('|').Select(p => p.Trim()).ToArray();

        if (parts.Length == 0)
            return;

        JsonPath = parts[0];

        for (var i = 1; i < parts.Length; i++)
        {
            var modParts = parts[i].Split(':');
            if (!string.IsNullOrWhiteSpace(modParts[0]))
                Modificators.Add(new ModificatorWithArgs(modParts[0], modParts[1..]));
        }
    }

    [SuppressMessage("ReSharper", "ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator")]
    public override string Render(RenderContext context)
    {
        var value = context.ResolvePath(JsonPath);
        object? result = value;

        // Apply modificators one by one
        foreach (var modificator in Modificators)
            result = context.TryApplyModificator(modificator.Name, modificator.Args, result, out var modifiedResult)
                ? modifiedResult
                : throw new TemplaterInvalidOperationException($"Modificator '{modificator.Name}' not found");

        return result?.ToString() ?? string.Empty;
    }

    private record ModificatorWithArgs(string Name, string[] Args);
}
