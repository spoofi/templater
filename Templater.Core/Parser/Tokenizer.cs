using System.Diagnostics.CodeAnalysis;

namespace Templater.Core.Parser;

public class Tokenizer
{
    private readonly string _template;
    private int _position;
    private int _line = 1;
    private int _column = 1;
    private const string VariableStartTag = "{{";
    private const string VariableEndTag = "}}";
    private const string BlockStartTag = "{%";
    private const string BlockEndTag = "%}";
    private const string BlockEndTextStartsWith = "end";
    private const char NewLineChar = '\n';

    public Tokenizer(string template)
    {
        ArgumentNullException.ThrowIfNull(template);
        _template = template;
    }

    public IEnumerable<Token> Tokenize()
    {
        while (_position < _template.Length)
            if (IsMatch(VariableStartTag))
                yield return ReadVariable();
            else if (IsMatch(BlockStartTag))
                yield return ReadBlock();
            else
                yield return ReadText();
    }

    [SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
    private bool IsMatch(string pattern)
    {
        return _template.AsSpan(_position).StartsWith(pattern);
    }

    private void Skip(int count)
    {
        for (var i = 0; i < count; i++)
            if (_position < _template.Length)
            {
                if (_template[_position] == NewLineChar)
                {
                    _line++;
                    _column = 1;
                }
                else
                {
                    _column++;
                }

                _position++;
            }
    }

    private Token ReadText()
    {
        var startLine = _line;
        var startColumn = _column;
        var startPosition = _position;

        while (_position < _template.Length)
        {
            if (IsMatch(VariableStartTag) || IsMatch(BlockStartTag))
                break;

            Skip(1);
        }

        return new Token(TokenType.Text, _template[startPosition.._position], startLine, startColumn);
    }

    private Token ReadVariable()
    {
        var startLine = _line;
        var startColumn = _column;

        Skip(VariableStartTag.Length);

        var startPosition = _position;
        while (_position < _template.Length && !IsMatch(VariableEndTag)) Skip(1);

        var content = _template.AsSpan(startPosition, _position - startPosition).Trim().ToString();
        Skip(VariableEndTag.Length);

        return new Token(TokenType.Variable, content, startLine, startColumn);
    }

    private Token ReadBlock()
    {
        var startLine = _line;
        var startColumn = _column;

        Skip(BlockStartTag.Length);

        var startPosition = _position;
        while (_position < _template.Length && !IsMatch(BlockEndTag)) Skip(1);

        var content = _template.AsSpan(startPosition, _position - startPosition).Trim();
        Skip(BlockEndTag.Length);

        var type = content.StartsWith(BlockEndTextStartsWith)
            ? TokenType.BlockEnd
            : TokenType.BlockStart;

        return new Token(type, content.ToString(), startLine, startColumn);
    }
}
