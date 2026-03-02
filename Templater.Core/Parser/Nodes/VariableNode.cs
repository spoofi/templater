using System.Diagnostics.CodeAnalysis;
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
    private List<string> Modificators { get; } = [];


    private void ParseExpression(string expression)
    {
        // TODO 2026-03-02 A.Suvorov: добавить аргументы для модификатора (like | price:EUR )
        var parts = expression.Split('|').Select(p => p.Trim()).ToArray();

        if (parts.Length == 0)
            return;

        JsonPath = parts[0];

        for (var i = 1; i < parts.Length; i++)
            if (!string.IsNullOrWhiteSpace(parts[i]))
                Modificators.Add(parts[i]);
    }

    [SuppressMessage("ReSharper", "ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator")]
    public override string Render(RenderContext context)
    {
        var value = context.ResolvePath(JsonPath);
        object? result = value;

        // Apply modificators one by one
        foreach (var modName in Modificators)
            result = context.TryApplyModificator(modName, result, out var modifiedResult)
                ? modifiedResult
                : throw new InvalidOperationException($"Modificator '{modName}' not found");

        return result?.ToString() ?? string.Empty;
    }
}
