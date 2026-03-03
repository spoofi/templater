using System.Diagnostics.CodeAnalysis;
using System.Text;
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
        var expressionSpan = expression.AsSpan();
        var pipeIndex = expressionSpan.IndexOf('|');

        if (pipeIndex == -1)
        {
            JsonPath = expression.Trim();
            return;
        }

        JsonPath = expressionSpan[..pipeIndex].Trim().ToString();

        var remaining = expressionSpan[(pipeIndex + 1)..];
        while (!remaining.IsEmpty)
        {
            var nextPipe = remaining.IndexOf('|');
            ReadOnlySpan<char> part;
            if (nextPipe == -1)
            {
                part = remaining;
                remaining = ReadOnlySpan<char>.Empty;
            }
            else
            {
                part = remaining[..nextPipe];
                remaining = remaining[(nextPipe + 1)..];
            }

            part = part.Trim();
            if (part.IsEmpty) continue;

            var colonIndex = part.IndexOf(':');
            if (colonIndex == -1)
            {
                Modificators.Add(new ModificatorWithArgs(part.ToString(), Array.Empty<string>()));
            }
            else
            {
                var modName = part[..colonIndex].Trim().ToString();
                var argsStr = part[(colonIndex + 1)..];
                var args = new List<string>();

                // Although arguments are usually simple, we still split them. 
                // To be even more optimal, we could avoid Split(':') if we knew there's only one arg,
                // but usually there are few args.
                foreach (var arg in argsStr.ToString().Split(':', StringSplitOptions.RemoveEmptyEntries)) args.Add(arg);

                Modificators.Add(new ModificatorWithArgs(modName, args.ToArray()));
            }
        }
    }

    [SuppressMessage("ReSharper", "ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator")]
    public override void Render(RenderContext context, StringBuilder sb)
    {
        var value = context.ResolvePath(JsonPath);
        object? result = value;

        // Apply modificators one by one
        foreach (var modificator in Modificators)
            result = context.TryApplyModificator(modificator.Name, modificator.Args, result, out var modifiedResult)
                ? modifiedResult
                : throw new TemplaterInvalidOperationException($"Modificator '{modificator.Name}' not found");

        if (result != null)
            sb.Append(result);
    }

    private record ModificatorWithArgs(string Name, string[] Args);
}
