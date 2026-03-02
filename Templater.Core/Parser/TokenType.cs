namespace Templater.Core.Parser;

public enum TokenType
{
    /// <summary>
    /// Text
    /// </summary>
    Text,

    /// <summary>
    /// {{ variable }}
    /// </summary>
    Variable,

    /// <summary>
    /// {% for ... %}, etc.
    /// </summary>
    BlockStart,

    /// <summary>
    /// {% endfor %}, etc.
    /// </summary>
    BlockEnd
}
