using System.Collections.Concurrent;
using Templater.Core.Abstractions;
using Templater.Core.Modificators.Abstractions;

namespace Templater.Core.Modificators;

public class ModificatorsRegistry
{
    private readonly ConcurrentDictionary<string, IModificator> _modificators = new(StringComparer.OrdinalIgnoreCase);

    public ModificatorsRegistry()
    {
        RegisterModificator(new PriceModificator());
        RegisterModificator(new ParagraphModificator());
    }

    private void RegisterModificator(IModificator modificator)
    {
        ArgumentNullException.ThrowIfNull(modificator);
        ArgumentException.ThrowIfNullOrWhiteSpace(modificator.Name);

        _modificators.TryAdd(modificator.Name, modificator);
    }

    public bool TryGetModificator(string name, out IModificator? mod) =>
        _modificators.TryGetValue(name, out mod);
}
