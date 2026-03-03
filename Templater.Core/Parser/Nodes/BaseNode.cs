using System.Text;
using Templater.Core.Render;

namespace Templater.Core.Parser.Nodes;

public abstract class BaseNode
{
    public abstract void Render(RenderContext context, StringBuilder sb);
}
