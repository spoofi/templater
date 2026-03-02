namespace Templater.Core.Abstractions;

public interface IModificator
{
    /// <summary>
    /// Unique name (used in template)
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Apply modificator
    /// </summary>
    object? Apply(object? input, params string[] arguments);
}
