using System.Text;
using System.Text.Json.Nodes;
using Templater.Core.Render;

namespace Templater.Core.Parser.Nodes;

/// <summary>
/// For cycle: {% for item in collection %}...{% endfor %}
/// </summary>
public class ForLoopNode : BaseNode
{
    public ForLoopNode(string itemName, string jsonPath)
    {
        ArgumentNullException.ThrowIfNull(itemName);
        ArgumentNullException.ThrowIfNull(jsonPath);

        ItemName = itemName;
        JsonPath = jsonPath;
    }

    private string ItemName { get; }
    private string JsonPath { get; }

    private List<BaseNode> Body { get; } = [];

    public void AddBodyNode(BaseNode node)
    {
        Body.Add(node);
    }

    public override string Render(RenderContext context)
    {
        var collection = context.ResolvePath(JsonPath);

        if (collection is not JsonArray array)
            return string.Empty; // TODO 2026-03-02 A.Suvorov: idk what to do

        var sb = new StringBuilder();

        foreach (var item in array)
            using (context.AddScope())
            {
                context.SetVariable(ItemName, item);

                foreach (var node in Body)
                    sb.Append(node.Render(context));
            }

        return sb.ToString();
    }
}
