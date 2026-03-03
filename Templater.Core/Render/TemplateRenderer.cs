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
        var sb = new StringBuilder();
        _root.Render(_context, sb);
        return PostProcess(sb.ToString());
    }

    private static string PostProcess(string html)
    {
        if (string.IsNullOrEmpty(html))
            return string.Empty;

        var sb = new StringBuilder(html.Length);
        var span = html.AsSpan();

        var start = 0;
        while (start < span.Length)
        {
            var end = span[start..].IndexOf('\n');
            ReadOnlySpan<char> line;
            if (end == -1)
            {
                line = span[start..];
                start = span.Length;
            }
            else
            {
                line = span.Slice(start, end);
                start += end + 1;
            }

            if (line.IsWhiteSpace())
                continue;

            sb.Append(line.TrimEnd());
            sb.Append('\n');
        }

        return sb.ToString();
    }
}
