using Templater.Core.Abstractions;

namespace Templater.Core.Modificators;

public class PriceModificator : IModificator
{
    private const string DefaultCurrencySymbol = "$";
    public string Name => "price";

    public object Apply(object? input, params string[] arguments)
    {
        if (input == null)
            return string.Empty;

        var currencySymbol = arguments.Length > 0 ? arguments[0] : DefaultCurrencySymbol;

        switch (input)
        {
            case string str when decimal.TryParse(str, out var decimalValue):
                return $"{currencySymbol}{decimalValue:F0}";
            case IConvertible convertible:
            {
                var value = convertible.ToDecimal(null);
                return $"{currencySymbol}{value:F0}";
            }
            default:
                // if can't parse -> return default currency + input
                return $"{currencySymbol}{input}";
        }
    }
}
