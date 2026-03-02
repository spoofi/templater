using Templater.Core.Exceptions;
using Templater.Core.Parser.Nodes;

namespace Templater.Core.Parser;

public class TemplateParser
{
    private readonly List<Token> _tokens;
    private int _position;

    public TemplateParser(IEnumerable<Token> tokens)
    {
        ArgumentNullException.ThrowIfNull(tokens);
        _tokens = tokens.ToList();
    }

    public RootNode Parse()
    {
        var root = new RootNode();

        while (_position < _tokens.Count)
        {
            var node = ParseNode();
            if (node != null)
                root.Children.Add(node);
        }

        return root;
    }

    private BaseNode? ParseNode()
    {
        if (_position >= _tokens.Count)
            return null;

        var token = _tokens[_position];

        return token.Type switch
        {
            TokenType.Text => ParseText(),
            TokenType.Variable => ParseVariable(),
            TokenType.BlockStart when token.Value.StartsWith("for ") => ParseForLoop(),
            TokenType.BlockEnd => SkipBlockEnd(),
            _ => ParseText()
        };
    }

    private TextNode ParseText()
    {
        var token = Consume(TokenType.Text);
        return new TextNode(token.Value);
    }

    private VariableNode ParseVariable()
    {
        var token = Consume(TokenType.Variable);
        return new VariableNode(token.Value);
    }

    private ForLoopNode ParseForLoop()
    {
        var startToken = Consume(TokenType.BlockStart);

        // Parsing: "for product in products"
        var parts = startToken.Value.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 4 || parts[0] != "for" || parts[2] != "in")
            throw new TemplaterInvalidSyntaxException(
                $"Invalid 'for' syntax: {startToken.Value} at {startToken.Line}:{startToken.Column}");

        var itemName = parts[1];
        var collectionPath = parts[3];
        var forNode = new ForLoopNode(itemName, collectionPath);

        // Parse body
        while (_position < _tokens.Count)
        {
            var current = _tokens[_position];

            if (current is { Type: TokenType.BlockEnd, Value: "endfor" })
            {
                Consume(TokenType.BlockEnd);
                break;
            }

            var child = ParseNode();
            if (child != null)
                forNode.AddBodyNode(child);
        }

        return forNode;
    }

    private BaseNode? SkipBlockEnd()
    {
        // skip the already handled end of a block
        Consume(TokenType.BlockEnd);
        return null;
    }

    private Token Consume(TokenType expectedTokenType)
    {
        if (_position >= _tokens.Count)
            throw new TemplaterInvalidOperationException($"Unexpected end of template, expected {expectedTokenType}");

        var token = _tokens[_position++];
        if (token.Type != expectedTokenType)
            throw new TemplaterInvalidOperationException(
                $"Expected {expectedTokenType}, got {token.Type} at {token.Line}:{token.Column}");

        return token;
    }
}
