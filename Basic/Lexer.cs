using System.Text;

namespace Basic;

public class Lexer
{
    private readonly InputBuffer _inputBuffer;
    private readonly string? _filename;
    private readonly StringBuilder _stringBuilder = new(256);

    public Lexer(string input, string? filename = null)
    {
        _inputBuffer = new InputBuffer(input);
        _filename = filename;
    }

    public Token NextToken()
    {
        while (true)
        {
            while (!_inputBuffer.IsEmpty() && char.IsWhiteSpace(_inputBuffer.Peek()))
            {
                _inputBuffer.Pop();
            }

            if (_inputBuffer.IsEmpty())
            {
                return new Token(TokenType.Eof, _inputBuffer.Location);
            }

            var ch = _inputBuffer.Peek();

            if (char.IsNumber(ch))
            {
                return ReadNumber();
            }

            if (char.IsLetter(ch))
            {
                return ReadIdentifierOrKeyword();
            }

            if (ch is '\"' or '\'')
            {
                return ReadString(ch);
            }

            if (ch == '#')
            {
                while (!_inputBuffer.IsEmpty() && _inputBuffer.Peek() != '\n')
                {
                    _inputBuffer.Pop();
                }

                continue;
            }

            var location = _inputBuffer.Location;

            if (ch == '+')
            {
                _inputBuffer.Pop();
                return new Token(TokenType.Plus, location);
            }
            
            if (ch == '-')
            {
                _inputBuffer.Pop();
                return new Token(TokenType.Minus, location);
            }
            
            if (ch == '*')
            {
                _inputBuffer.Pop();
                return new Token(TokenType.Star, location);
            }
            
            if (ch == '/')
            {
                _inputBuffer.Pop();
                return new Token(TokenType.Slash, location);
            }
            
            if (ch == '>')
            {
                _inputBuffer.Pop();
                if (_inputBuffer.Peek() == '=')
                {
                    _inputBuffer.Pop();
                    return new Token(TokenType.GreaterEqual, location);   
                }
                return new Token(TokenType.Greater, location);
            }

            if (ch == '<')
            {
                _inputBuffer.Pop();
                if (_inputBuffer.Peek() == '=')
                {
                    _inputBuffer.Pop();
                    return new Token(TokenType.LessEqual, location);   
                }
                return new Token(TokenType.Less, location);
            }
            
            if (ch == '=')
            {
                _inputBuffer.Pop();
                if (_inputBuffer.Peek() == '=')
                {
                    _inputBuffer.Pop();
                    return new Token(TokenType.EqualEqual, location);   
                }
                return new Token(TokenType.Equal, location);
            }
            
            if (ch == '!')
            {
                _inputBuffer.Pop();
                if (_inputBuffer.Peek() == '=')
                {
                    _inputBuffer.Pop();
                    return new Token(TokenType.BangEqual, location);   
                }
                return new Token(TokenType.Bang, location);
            }
            
            if (ch == '&')
            {
                _inputBuffer.Pop();
                if (_inputBuffer.Peek() == '&')
                {
                    _inputBuffer.Pop();
                    return new Token(TokenType.AmpersandAmpersand, location);   
                }
                throw new SyntaxError("Unexpected character.", location);
            }
            
            if (ch == '|')
            {
                _inputBuffer.Pop();
                if (_inputBuffer.Peek() == '|')
                {
                    _inputBuffer.Pop();
                    return new Token(TokenType.PipePipe, location);   
                }
                throw new SyntaxError($"Unexpected character '{_inputBuffer.Peek()}'.", location);
            }
            
            if (ch == '(')
            {
                _inputBuffer.Pop();
                return new Token(TokenType.LeftParen, location);
            }
            
            if (ch == ')')
            {
                _inputBuffer.Pop();
                return new Token(TokenType.RightParen, location);
            }

            if (ch == '[')
            {
                _inputBuffer.Pop();
                return new Token(TokenType.LeftBracket, location);
            }
            
            if (ch == ']')
            {
                _inputBuffer.Pop();
                return new Token(TokenType.RightBracket, location);
            }
            
            if (ch == ',')
            {
                _inputBuffer.Pop();
                return new Token(TokenType.Comma, location);
            }
        }
    }

    private Token ReadIdentifierOrKeyword()
    {
        var loc = _inputBuffer.Location;

        _stringBuilder.Clear();

        while (!_inputBuffer.IsEmpty() && IsIdentifierChar(_inputBuffer.Peek()))
        {
            _stringBuilder.Append(_inputBuffer.Peek());
            _inputBuffer.Pop();
        }

        var identifier = _stringBuilder.ToString();

        if (identifier == "IF")
        {
            return new Token(TokenType.If, loc);
        }

        if (identifier == "DO")
        {
            return new Token(TokenType.Do, loc);
        }
        
        if (identifier == "ELSE")
        {
            return new Token(TokenType.Else, loc);
        }
        
        if (identifier == "END")
        {
            return new Token(TokenType.End, loc);
        }
        
        if (identifier == "FOR")
        {
            return new Token(TokenType.For, loc);
        }
        
        if (identifier == "IN")
        {
            return new Token(TokenType.In, loc);
        }
        
        if (identifier == "TO")
        {
            return new Token(TokenType.To, loc);
        }
        
        if (identifier == "WHILE")
        {
            return new Token(TokenType.While, loc);
        }
        
        if (identifier == "DEF")
        {
            return new Token(TokenType.Def, loc);
        }
        
        if (identifier == "RETURN")
        {
            return new Token(TokenType.Return, loc);
        }
        
        if (identifier == "BREAK")
        {
            return new Token(TokenType.Break, loc);
        }
        
        if (identifier == "CONTINUE")
        {
            return new Token(TokenType.Continue, loc);
        }

        if (identifier == "TRUE")
        {
            return new Token(TokenType.Boolean, loc, true);
        }
        
        if (identifier == "FALSE")
        {
            return new Token(TokenType.Boolean, loc, false);
        }
        
        return new Token(TokenType.Identifier, loc, identifier);
    }
    
    private Token ReadNumber()
    {
        var loc = _inputBuffer.Location;

        _stringBuilder.Clear();

        while (!_inputBuffer.IsEmpty() && char.IsNumber(_inputBuffer.Peek()))
        {
            _stringBuilder.Append(_inputBuffer.Peek());
            _inputBuffer.Pop();
        }

        if (char.IsNumber(_inputBuffer.Peek(1)))
        {
            _stringBuilder.Append(_inputBuffer.Peek());
            _inputBuffer.Pop();

            while (!_inputBuffer.IsEmpty() && char.IsNumber(_inputBuffer.Peek()))
            {
                _stringBuilder.Append(_inputBuffer.Peek());
                _inputBuffer.Pop();
            }
        }

        return new Token(TokenType.Number, loc, double.Parse(_stringBuilder.ToString()));
    }
    
    private Token ReadString(char quote)
    {
        var location = _inputBuffer.Location;

        _inputBuffer.Pop();

        _stringBuilder.Clear();

        while (true)
        {
            var ch = _inputBuffer.Peek();

            if (ch is '\n' or '\0')
            {
                throw new SyntaxError("Unterminated string.", location);
            }

            if (ch == quote)
            {
                _inputBuffer.Pop();
                break;
            }
            
            if (ch == '\\')
            {
                _inputBuffer.Pop();
                _stringBuilder.Append('\\');
                _stringBuilder.Append(_inputBuffer.Peek());
                _inputBuffer.Pop();
            }
            else
            {
                _stringBuilder.Append(ch);
                _inputBuffer.Pop();
            }
        }

        return new Token(TokenType.String, location, _stringBuilder.ToString());
    }

    private static bool IsIdentifierChar(char ch)
    {
        return char.IsLetterOrDigit(ch) || ch == '_';
    }
}