using JetBrains.Annotations;

namespace Templater.Core.Abstractions;

public interface ITemplateEngine
{
    /// <summary>
    /// Create HTML from template and JSON data
    /// </summary>
    [PublicAPI]
    string CreateHtml(string template, string jsonData);
}
