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

        var pathSpan = path.AsSpan();

        // search in all scopes. From top to bottom
        foreach (var scope in _scopes)
        {
            var current = ResolveInScope(scope, pathSpan);
            if (current != null)
                return current;
        }

        // Search in root data
        return ResolveInRoot(_rootData, pathSpan);
    }

    private static JsonNode? ResolveInScope(Dictionary<string, JsonNode?> scope, ReadOnlySpan<char> path)
    {
        var dotIndex = path.IndexOf('.');
        if (dotIndex == -1)
            return scope.GetValueOrDefault(path.ToString());

        if (!scope.TryGetValue(path[..dotIndex].ToString(), out var current) || current == null)
            return null;

        return ResolveInJsonNode(current, path[(dotIndex + 1)..]);
    }

    private static JsonNode? ResolveInRoot(JsonNode? root, ReadOnlySpan<char> path)
    {
        if (root == null)
            return null;

        var dotIndex = path.IndexOf('.');
        if (dotIndex == -1)
            return root is JsonObject obj && obj.TryGetPropertyValue(path.ToString(), out var value) ? value : null;

        if (root is not JsonObject rootObj ||
            !rootObj.TryGetPropertyValue(path[..dotIndex].ToString(), out var current) || current == null)
            return null;

        return ResolveInJsonNode(current, path[(dotIndex + 1)..]);
    }

    private static JsonNode? ResolveInJsonNode(JsonNode node, ReadOnlySpan<char> path)
    {
        var current = node;
        var remainingPath = path;

        while (true)
        {
            var nextDot = remainingPath.IndexOf('.');
            if (nextDot == -1)
                return current is JsonObject obj && obj.TryGetPropertyValue(remainingPath.ToString(), out var value)
                    ? value
                    : null;

            if (current is JsonObject currentObj
                && currentObj.TryGetPropertyValue(remainingPath[..nextDot].ToString(), out var next))
            {
                current = next;
                remainingPath = remainingPath[(nextDot + 1)..];
                if (current == null)
                    return null;
            }
            else
            {
                return null;
            }
        }
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
