namespace Basic;

public class Parser
{
    private readonly string? _filename;
    private readonly Lexer _lexer;
    private Token _currentToken;
    private Token _nextToken;
    
    private Location BuildLocation()
    {
        return _currentToken.Location with { FileName = _filename };
    }
    
    public Parser(Lexer lexer, string? filename = null)
    {
        _lexer = lexer;
        _filename = filename;
        
        _currentToken = _lexer.NextToken();
        _nextToken = _lexer.NextToken();
    }

    public Statement? ParseStatement()
    {
        if (_currentToken.Type == TokenType.Eof)
            return null;
        
        if (_nextToken.Type == TokenType.Equal)
        {
            return ParseAssignment();
        }

        if (_currentToken.Type == TokenType.If)
        {
            return ParseIf(isElif: false, isNested: _currentToken.Type == TokenType.Else);
        }

        if (_currentToken.Type == TokenType.For)
        {
            return ParseFor();
        }

        if (_currentToken.Type == TokenType.While)
        {
            return ParseWhile();
        }

        var location = BuildLocation();
        var expr = ParseExpression();
        return new ExpressionStatement(expr, location);
    }
    
    private Block ParseBlock()
    {
        var location = BuildLocation();
        var statements = new List<Statement>();
        while (_currentToken.Type != TokenType.End && _currentToken.Type != TokenType.Else)
        {
            var statement = ParseStatement();
            if (statement is not null)
            {
                statements.Add(statement);
            }
            else
            {
                throw new SyntaxError("Expected statement", _currentToken.Location);
            }
        }
        return new Block(statements, location);
    }

    private Assignment ParseAssignment()
    {
        var location = BuildLocation();
        var identifier = ConsumeToken(TokenType.Identifier);
        ConsumeToken(TokenType.Equal);
        var value = ParseExpression();
        if (identifier.Value is not StringValue name)
            throw new SyntaxError("Token value should be a string", _currentToken.Location);
        return new Assignment(location, name.Value, value);
    }
    
    private If ParseIf(bool isElif = false, bool isNested = false)
    {
        var location = BuildLocation();

        if (isElif)
        {
            ConsumeToken(TokenType.Else);
            ConsumeToken(TokenType.If);
        }
        else
        {
            ConsumeToken(TokenType.If);
        }

        var condition = ParseExpression();
        ConsumeToken(TokenType.Then);
        var body = ParseBlock();
    
        if (body is null)
        {
            throw new SyntaxError("Expected block", _currentToken.Location);
        }

        Statement? elseBlock = null;

        if (_currentToken.Type == TokenType.Else)
        {
            if (_nextToken.Type == TokenType.If)
            {
                elseBlock = ParseIf(isElif: true);
            }
            else
            {
                ConsumeToken(TokenType.Else);
                elseBlock = ParseBlock();
            }
        }

        if (!isElif && !isNested)
        {
            ConsumeToken(TokenType.End);
            ConsumeToken(TokenType.If);
        }
    
        return new If(condition, body, elseBlock, location);
    }

    public Statement ParseFor()
    {
        var location = BuildLocation();
        ConsumeToken(TokenType.For);
        var identifier = ConsumeToken(TokenType.Identifier);
        ConsumeToken(TokenType.Equal);
        var start = ParseExpression();
        ConsumeToken(TokenType.To);
        var end = ParseExpression();
        ConsumeToken(TokenType.Then);
        var body = ParseBlock();
        if (identifier.Value is not StringValue name)
            throw new SyntaxError("Token value should be a string", _currentToken.Location);
        ConsumeToken(TokenType.End);
        ConsumeToken(TokenType.For);
        return new ForRange(name.Value, start, end, body, location);
    }

    public Statement ParseWhile()
    {
        var location = BuildLocation();
        ConsumeToken(TokenType.While);
        var condition = ParseExpression();
        ConsumeToken(TokenType.Then);
        var body = ParseBlock();
        ConsumeToken(TokenType.End);
        ConsumeToken(TokenType.While);
        return new While(condition, body, location);
    }
    
    private Expression ParseExpression()
    {
        return ParseExpression(8);
    }
    
    private Expression ParseExpression(int precedence)
    {
        Expression node;

        if (precedence == 1) 
        {
            node = ParseUnary();
        } 
        else 
        {
            node = ParseExpression(precedence - 1); 
        }

        while (IsBinaryOperatorOfPrecedence(_currentToken.Type, precedence))
        {
            var location = BuildLocation();
            var op = _currentToken.Type;
            ConsumeToken();
            var right = ParseExpression(precedence - 1); 
            node = new BinaryExpression(node, op, right, location);
        }

        return node;
    }

    private Expression ParseUnary()
    {
        Expression? node = null;

        while (IsUnaryOperator(_currentToken.Type) || _currentToken.Type == TokenType.LeftParen)
        {
            var location = BuildLocation();

            if (IsUnaryOperator(_currentToken.Type))
            {
                var op = _currentToken.Type;
                ConsumeToken();
                node = new UnaryExpression(op, ParseUnary(), location); 
            }
            else if (_currentToken.Type == TokenType.LeftParen)
            {
                ConsumeToken();
                node = ParseExpression(8);
                if (_currentToken.Type != TokenType.RightParen)
                {
                    throw new SyntaxError("Expected right parenthesis", _currentToken.Location);
                }
                ConsumeToken();
            }
        }

        if (node is not null)
            return node;

        return ParseAtom();
    }
    
    private Number ParseNumber()
    {
        var location = BuildLocation();

        var numberToken = ConsumeToken(TokenType.Number);
        
        if (numberToken.Value is not NumericValue numericValue)
            throw new SyntaxError("Token value should be a float", _currentToken.Location);

        return new Number(numericValue.Value, location);
    }
    
    private String ParseString()
    {
        var location = BuildLocation();

        var stringToken = ConsumeToken(TokenType.String);
        
        if (stringToken.Value is not StringValue stringValue)
            throw new SyntaxError("Token value should be a string", _currentToken.Location);
        
        return new String(stringValue.Value, location);
    }

    private Boolean ParseBoolean()
    {
        var location = BuildLocation();

        var boolean = ConsumeToken(TokenType.Boolean);

        if (boolean.Value is not BooleanValue booleanValue)
            throw new SyntaxError("Token value should be a boolean", _currentToken.Location);
        
        return new Boolean(booleanValue.Value, location);
    }
    
    private Identifier ParseIdentifier()
    {
        var location = BuildLocation();

        var identifierToken = ConsumeToken(TokenType.Identifier);

        if (identifierToken.Value is not StringValue stringLiteral)
            throw new SyntaxError("Token value should be a string", _currentToken.Location);
        
        return new Identifier(stringLiteral.Value, location);
    }
    
    private List<Expression> ParseArguments()
    {
        var arguments = new List<Expression>();
        while (_currentToken.Type != TokenType.RightParen)
        {
            arguments.Add(ParseArgument());
            if (_currentToken.Type == TokenType.Comma)
            {
                ConsumeToken(TokenType.Comma);
            }
        }
        return arguments;
    }

    private Expression ParseArgument()
    {
        var expression = ParseExpression();
        return expression;
    }
    
    private CallExpression ParseCallExpression(Identifier callee)
    {
        var location = BuildLocation();
        ConsumeToken(TokenType.LeftParen);
        var arguments = ParseArguments();
        ConsumeToken(TokenType.RightParen);
        return new CallExpression(callee.Value, arguments, location);
    }
    
    private Expression ParseAtom()
    {
        Expression node;

        if (_currentToken.Type == TokenType.LeftParen)
            node = ParseParenthesizedExpression();
        else if (_currentToken.Type == TokenType.Number)
            node = ParseNumber();
        else if (_currentToken.Type == TokenType.String)
            node = ParseString();
        else if (_currentToken.Type == TokenType.Boolean)
            node = ParseBoolean();
        else if (_currentToken.Type == TokenType.Identifier)
        {
            node = ParseIdentifier();
            
            if (_currentToken.Type == TokenType.LeftParen)
                node = ParseCallExpression((Identifier)node);
        }
        else
            throw new SyntaxError($"Unexpected {_currentToken.Type}", _currentToken.Location);
        

        
        return node;
    }
    
    private Expression ParseParenthesizedExpression()
    {
        ConsumeToken(TokenType.LeftParen);
        var node = ParseExpression();
        ConsumeToken(TokenType.RightParen);
        return node;
    }
    
    private static bool IsUnaryOperator(TokenType type)
    {
        var unaryOperators = new List<TokenType>
        {
            TokenType.Minus, TokenType.Bang
        };
        return unaryOperators.Contains(type);
    }

    private static bool IsBinaryOperatorOfPrecedence(TokenType type, int precedence)
    {
        var precedenceMap = new Dictionary<int, List<TokenType>>
        {
            { 2, new List<TokenType> { TokenType.Star, TokenType.Slash } },
            { 3, new List<TokenType> { TokenType.Plus, TokenType.Minus } },
            { 4, new List<TokenType> { TokenType.EqualEqual, TokenType.BangEqual, TokenType.Less, TokenType.LessEqual, TokenType.Greater, TokenType.GreaterEqual } },
            { 5, new List<TokenType> { TokenType.AmpersandAmpersand } },
            { 6, new List<TokenType> { TokenType.PipePipe } }
        };

        return precedenceMap.TryGetValue(precedence, out var operators) && operators.Contains(type);
    }
    
    private Token ConsumeToken()
    {
        var token = _currentToken;
        _currentToken = _nextToken;
        _nextToken = _lexer.NextToken();
        return token;
    }
    
    private Token ConsumeToken(TokenType expectedType)
    {
        if (_currentToken.Type == expectedType)
        {
            return ConsumeToken();
        }

        throw new SyntaxError($"Unexpected token: Expected {expectedType}, got {_currentToken.Type}", _currentToken.Location);
    }
}