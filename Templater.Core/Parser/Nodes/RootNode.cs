using System.Text;
using Templater.Core.Render;

namespace Templater.Core.Parser.Nodes;

/// <summary>
/// Root node - renders all children
/// </summary>
public class RootNode : BaseNode
{
    public List<BaseNode> Children { get; } = [];

    public override void Render(RenderContext context, StringBuilder sb)
    {
        foreach (var child in Children)
            child.Render(context, sb);
    }
}
