namespace Basic;

public interface IValue { }

public sealed record NullValue() : IValue;
public sealed record NumericValue(double Value) : IValue;
public sealed record StringValue(string Value) : IValue;
public sealed record BooleanValue(bool Value) : IValue;

public record Token(TokenType Type, Location Location, IValue Value)
{
    public Token(TokenType type, Location location) : this(type, location, new NullValue()) { }
    public Token(TokenType type, Location location, string value) : this(type, location, new StringValue(value)) { }
    public Token(TokenType type, Location location, double value) : this(type, location, new NumericValue(value)) { }
    public Token(TokenType type, Location location, bool value) : this(type, location, new BooleanValue(value)) { }
}