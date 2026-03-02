using System.Text.Json;
using System.Text.Json.Nodes;
using Templater.Core.Abstractions;
using Templater.Core.Exceptions;
using Templater.Core.Modificators;
using Templater.Core.Parser;
using Templater.Core.Parser.Nodes;
using Templater.Core.Render;

namespace Templater.Core;

public class TemplaterEngine : ITemplateEngine
{
    private readonly ModificatorsRegistry _modificatorsRegistry = new();
    private readonly JsonNodeOptions _jsonNodeOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly JsonDocumentOptions _jsonDocumentOptions = new() { CommentHandling = JsonCommentHandling.Skip };

    public string CreateHtml(string template, string jsonData)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(template);
        ArgumentException.ThrowIfNullOrWhiteSpace(jsonData);

        var rootData = ParseJson(jsonData);
        var rootNode = ParseTemplate(template);

        using var context = new RenderContext(rootData, _modificatorsRegistry);
        var renderer = new TemplateRenderer(rootNode, context);
        return renderer.Render();
    }

    private JsonNode? ParseJson(string json)
    {
        try
        {
            return JsonNode.Parse(json, _jsonNodeOptions, _jsonDocumentOptions);
        }
        catch (JsonException ex)
        {
            throw new TemplaterInvalidOperationException("Failed to parse JSON data", ex);
        }
    }

    private static RootNode ParseTemplate(string template)
    {
        try
        {
            var tokens = new Tokenizer(template).Tokenize();
            var parser = new TemplateParser(tokens);
            return parser.Parse();
        }
        catch (Exception ex)
        {
            if (ex is TemplaterException)
                throw;

            throw new TemplaterInvalidOperationException("Failed to parse template", ex);
        }
    }
}
