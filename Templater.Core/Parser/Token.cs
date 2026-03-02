namespace Templater.Core.Parser;

public class Token(TokenType type, string value, int line, int column)
{
    public TokenType Type { get; } = type;
    public string Value { get; } = value;
    public int Line { get; } = line;
    public int Column { get; } = column;

    public override string ToString() => $"[{Type}] '{Value}' at {Line}:{Column}";
}
