using System.Text;
using Templater.Core.Render;

namespace Templater.Core.Parser.Nodes;

/// <summary>
/// Root node - renders all children
/// </summary>
public class RootNode : BaseNode
{
    public List<BaseNode> Children { get; } = [];

    public override string Render(RenderContext context)
    {
        var sb = new StringBuilder();
        foreach (var child in Children)
            sb.Append(child.Render(context));
        return sb.ToString();
    }
}
