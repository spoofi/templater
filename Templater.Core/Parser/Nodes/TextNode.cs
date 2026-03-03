using System.Text;
using Templater.Core.Render;

namespace Templater.Core.Parser.Nodes;

/// <summary>
/// Text - render as is
/// </summary>
public class TextNode(string content) : BaseNode
{
    public override void Render(RenderContext context, StringBuilder sb)
    {
        sb.Append(content);
    }
}
