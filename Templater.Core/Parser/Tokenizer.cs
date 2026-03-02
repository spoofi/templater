using System.Diagnostics.CodeAnalysis;
using System.Text;

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
        if (_position + pattern.Length > _template.Length)
            return false;

        for (var i = 0; i < pattern.Length; i++)
            if (_template[_position + i] != pattern[i])
                return false;

        return true;
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
        var sb = new StringBuilder();

        while (_position < _template.Length)
        {
            if (IsMatch(VariableStartTag) || IsMatch(BlockStartTag))
                break;

            sb.Append(_template[_position]);
            Skip(1);
        }

        return new Token(TokenType.Text, sb.ToString(), startLine, startColumn);
    }

    private Token ReadVariable()
    {
        var startLine = _line;
        var startColumn = _column;

        Skip(VariableStartTag.Length);

        var sb = new StringBuilder();
        while (_position < _template.Length && !IsMatch(VariableEndTag))
        {
            sb.Append(_template[_position]);
            Skip(1);
        }

        Skip(VariableEndTag.Length);

        return new Token(TokenType.Variable, sb.ToString().Trim(), startLine, startColumn);
    }

    private Token ReadBlock()
    {
        var startLine = _line;
        var startColumn = _column;

        Skip(BlockStartTag.Length);

        var sb = new StringBuilder();
        while (_position < _template.Length && !IsMatch(BlockEndTag))
        {
            sb.Append(_template[_position]);
            Skip(1);
        }

        Skip(BlockEndTag.Length);

        var content = sb.ToString().Trim();
        var type = content.StartsWith(BlockEndTextStartsWith)
            ? TokenType.BlockEnd
            : TokenType.BlockStart;

        return new Token(type, content, startLine, startColumn);
    }
}
