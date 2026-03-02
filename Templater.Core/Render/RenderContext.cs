using System.Text.Json.Nodes;
using Templater.Core.Exceptions;
using Templater.Core.Modificators;

namespace Templater.Core.Render;

public class RenderContext : IDisposable
{
    private readonly JsonNode? _rootData;
    private readonly ModificatorsRegistry _modificatorsRegistry;
    private readonly Stack<Dictionary<string, JsonNode?>> _scopes = new();
    private bool _disposed;

    public RenderContext(JsonNode? rootData, ModificatorsRegistry modificatorsRegistry)
    {
        _rootData = rootData;
        _modificatorsRegistry = modificatorsRegistry;
        CreateScope(); // add root scope for context
    }

    /// <summary>
    /// Add a new scope for nested contexts (for cycles)
    /// </summary>
    public IDisposable AddScope()
    {
        ThrowIfDisposed();
        CreateScope();
        return new ScopePop(_scopes);
    }

    private class ScopePop(Stack<Dictionary<string, JsonNode?>> scopes) : IDisposable
    {
        public void Dispose()
        {
            scopes.Pop();
        }
    }

    public void SetVariable(string name, JsonNode? value)
    {
        ThrowIfDisposed();
        var scope = _scopes.Peek();
        if (!scope.TryAdd(name, value))
            throw new TemplaterInvalidOperationException($"Variable '{name}' already exists in the current scope");
    }

    public JsonNode? ResolvePath(string path)
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(path))
            return null;

        // search in all scopes. From top to bottom
        foreach (var scope in _scopes)
        {
            var parts = path.Split('.');
            if (!scope.TryGetValue(parts[0], out var current))
                continue;

            for (var i = 1; i < parts.Length; i++)
                if (current is JsonObject obj && obj.TryGetPropertyValue(parts[i], out var next))
                {
                    current = next;
                }
                else
                {
                    current = null;
                    break;
                }

            if (current != null)
                return current;
        }

        // Search in root data: "product.name" -> data["product"]["name"]
        var rootParts = path.Split('.');
        var rootCurrent = _rootData;

        foreach (var part in rootParts)
            if (rootCurrent is JsonObject obj && obj.TryGetPropertyValue(part, out var next))
                rootCurrent = next;
            else
                return null;

        return rootCurrent;
    }

    public bool TryApplyModificator(string modName, string[] args, object? input, out object? result)
    {
        ThrowIfDisposed();

        if (_modificatorsRegistry.TryGetModificator(modName, out var modificator))
        {
            result = modificator!.Apply(input, args);
            return true;
        }

        result = null;
        return false;
    }

    private void CreateScope()
    {
        _scopes.Push(new Dictionary<string, JsonNode?>(StringComparer.OrdinalIgnoreCase));
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _scopes.Clear();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
