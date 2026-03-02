using Templater.Core.Render;

namespace Templater.Core.Parser.Nodes;

public abstract class BaseNode
{
    public abstract string Render(RenderContext context);
}
