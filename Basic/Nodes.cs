namespace Basic;

public abstract record Node(Location Location);

public abstract record Statement(Location Location) : Node(Location);

public abstract record Expression(Location Location) : Node(Location);

public record Block(List<Statement> Statements, Location Location) : Statement(Location);

public record Assignment(Location Location, string Name, Expression Value) : Statement(Location);

public record If(Expression Condition, Statement Then, Statement? Else, Location Location) : Statement(Location);

public record ForRange(string Name, Expression Start, Expression End, Statement Body, Location Location) : Statement(Location);

public record While(Expression Condition, Statement Body, Location Location) : Statement(Location);

public record Break(Location Location) : Statement(Location);

public record ExpressionStatement(Expression Expression, Location Location) : Statement(Location);

public record UnaryExpression(TokenType Operator, Expression Operand, Location Location) : Expression(Location);

public record BinaryExpression(Expression Left, TokenType Operator, Expression Right, Location Location) : Expression(Location);

public record Number(double Value, Location Location) : Expression(Location);

public record String(string Value, Location Location) : Expression(Location);

public record Boolean(bool Value, Location Location) : Expression(Location);

public record Identifier(string Value, Location Location) : Expression(Location);

public record CallExpression(string Name, List<Expression> Arguments, Location Location) : Expression(Location);

public record FunctionDefinition(string Name, List<string> Parameters, Statement Body, Location Location) : Statement(Location);

public record Return(Expression? Value, Location Location) : Statement(Location);