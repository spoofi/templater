using Templater.Core.Render;

namespace Templater.Core.Parser.Nodes;

/// <summary>
/// Text - render as is
/// </summary>
public class TextNode(string content) : BaseNode
{
    public override string Render(RenderContext context)
    {
        return content;
    }
}
