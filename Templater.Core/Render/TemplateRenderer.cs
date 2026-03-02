using System.Text;
using Templater.Core.Parser.Nodes;

namespace Templater.Core.Render;

public class TemplateRenderer
{
    private readonly RootNode _root;
    private readonly RenderContext _context;

    public TemplateRenderer(RootNode root, RenderContext context)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(context);

        _root = root;
        _context = context;
    }

    public string Render()
    {
        var result = _root.Render(_context);
        return PostProcess(result);
    }

    private static string PostProcess(string html)
    {
        var lines = html.Split('\n');
        var sb = new StringBuilder();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;
            
            sb.Append(line.TrimEnd());
            sb.Append('\n');
        }

        return sb.ToString();
    }
}
