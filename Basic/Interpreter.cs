using System.Text;

namespace Basic;

public class Interpreter
{
    private readonly IWindow _window;
    private readonly Stack<Environment> _environments = new();
    private Environment CurrentEnvironment => _environments.Peek();
    
    public Interpreter(IWindow? window = null)
    {
        _window = window;
        PushEnvironment();
    }
    
    public async Task ExecuteStatement(Statement statement, CancellationToken stoppingToken = default)
    {
        await Task.Run(() =>
        {
            switch (statement)
            {
                case Block block:
                    ExecuteBlock(block, stoppingToken);
                    break;
                case Assignment assignment:
                    ExecuteAssignment(assignment, stoppingToken);
                    break;
                case If @if:
                    ExecuteIf(@if, stoppingToken);
                    break;
                case ForRange forRange:
                    ExecuteForRange(forRange, stoppingToken);
                    break;
                case While @while:
                    ExecuteWhile(@while, stoppingToken);
                    break;
                case ExpressionStatement call:
                    ExecuteExpressionStatement(call, stoppingToken);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }, stoppingToken);
    }
    
    private void ExecuteBlock(Block block, CancellationToken stoppingToken)
    {
        foreach (var statement in block.Statements)
        {
            ExecuteStatement(statement, stoppingToken);
        }
    }
    
    private void ExecuteAssignment(Assignment assignment, CancellationToken stoppingToken)
    {
        var value = EvaluateExpression(assignment.Value, stoppingToken);
        CurrentEnvironment.SetVariable(assignment.Name, value);
    }

    private void ExecuteIf(If @if, CancellationToken stoppingToken)
    {
        var condition = EvaluateExpression(@if.Condition, stoppingToken);
        
        if (condition is not SingleValueObj singleValueObj)
            throw new Exception("Expected single value object");

        if (bool.TryParse(singleValueObj.Value, out var boolValue))
        {
            if (boolValue)
            {
                ExecuteStatement(@if.Then, stoppingToken);
            }
            else if (@if.Else is not null)
            {
                ExecuteStatement(@if.Else, stoppingToken);
            }
            
            return;
        }
        
        throw new Exception("Expected boolean");
    }
    
    private void ExecuteForRange(ForRange forRange, CancellationToken stoppingToken)
    {
        var start = EvaluateExpression(forRange.Start, stoppingToken);
        var end = EvaluateExpression(forRange.End, stoppingToken);
        
        if (start is not SingleValueObj startSingleValueObj)
            throw new Exception("Expected single value object");

        if (end is not SingleValueObj endSingleValueObj)
            throw new Exception("Expected single value object");

        if (double.TryParse(startSingleValueObj.Value, out var startDoubleValue) && double.TryParse(endSingleValueObj.Value, out var endDoubleValue))
        {
            for (var i = startDoubleValue; i <= endDoubleValue; i++)
            {
                CurrentEnvironment.SetVariable(forRange.Name, new SingleValueObj(i.ToString()));
                ExecuteStatement(forRange.Body, stoppingToken);
            }
            
            return;
        }
        
        throw new Exception("Expected number");
    }

    private void ExecuteWhile(While @while, CancellationToken stoppingToken)
    {
        while (true)
        {
            var condition = EvaluateExpression(@while.Condition, stoppingToken);
            
            if (condition is not SingleValueObj singleValueObj)
                throw new Exception("Expected single value object");

            if (bool.TryParse(singleValueObj.Value, out var boolValue))
            {
                if (!boolValue)
                    break;
                
                ExecuteStatement(@while.Body, stoppingToken);
                continue;
            }
            
            throw new Exception("Expected boolean");
        }
    }
    
    private void ExecuteExpressionStatement(ExpressionStatement expressionStatement, CancellationToken stoppingToken)
    {
        EvaluateExpression(expressionStatement.Expression, stoppingToken);
    }

    private Obj EvaluateCallExpression(CallExpression call, CancellationToken stoppingToken)
    {
        if (call.Name == "print")
        {
            foreach (var argument in call.Arguments)
            {
                var value = EvaluateExpression(argument, stoppingToken);
                Console.Write(value);
            }
            
            Console.WriteLine();
            return new NullObj();
        }

        if (call.Name == "clear")
        {
            _window.Clear();
            return new NullObj();
        }

        if (call.Name == "present")
        {
            _window.Present();
            return new NullObj();
        }
        
        if (call.Name == "drawCircle")
        {
            if (call.Arguments.Count != 3)
                throw new Exception("Expected 3 arguments");
            
            var x = EvaluateExpression(call.Arguments[0], stoppingToken);
            var y = EvaluateExpression(call.Arguments[1], stoppingToken);
            var r = EvaluateExpression(call.Arguments[2], stoppingToken);
            
            if (x is not SingleValueObj xSingleValueObj)
                throw new Exception("Expected single value object");

            if (y is not SingleValueObj ySingleValueObj)
                throw new Exception("Expected single value object");

            if (r is not SingleValueObj rSingleValueObj)
                throw new Exception("Expected single value object");

            if (int.TryParse(xSingleValueObj.Value, out var xIntValue) && int.TryParse(ySingleValueObj.Value, out var yIntValue) && int.TryParse(rSingleValueObj.Value, out var rIntValue))
            {
                _window.DrawCircle(xIntValue, yIntValue, rIntValue);
                return new NullObj();
            }
            
            throw new Exception("Expected number");
        }
        
        if (call.Name == "drawText")
        {
            if (call.Arguments.Count != 3)
                throw new Exception("Expected 3 arguments");
            
            var x = EvaluateExpression(call.Arguments[0], stoppingToken);
            var y = EvaluateExpression(call.Arguments[1], stoppingToken);
            var text = EvaluateExpression(call.Arguments[2], stoppingToken);
            
            if (x is not SingleValueObj xSingleValueObj)
                throw new Exception("Expected single value object");

            if (y is not SingleValueObj ySingleValueObj)
                throw new Exception("Expected single value object");

            if (text is not SingleValueObj textSingleValueObj)
                throw new Exception("Expected single value object");

            if (int.TryParse(xSingleValueObj.Value, out var xIntValue) && int.TryParse(ySingleValueObj.Value, out var yIntValue))
            {
                _window.DrawText(xIntValue, yIntValue, textSingleValueObj.Value);
                return new NullObj();
            }
            
            throw new Exception("Expected number");
        }

        if (call.Name == "drawSprite")
        {
            if (call.Arguments.Count != 3)
                throw new Exception("Expected 3 arguments");
            
            var filename = EvaluateExpression(call.Arguments[0], stoppingToken);
            var x = EvaluateExpression(call.Arguments[1], stoppingToken);
            var y = EvaluateExpression(call.Arguments[2], stoppingToken);
            
            if (filename is not SingleValueObj filenameSingleValueObj)
                throw new Exception("Expected single value object");

            if (x is not SingleValueObj xSingleValueObj)
                throw new Exception("Expected single value object");

            if (y is not SingleValueObj ySingleValueObj)
                throw new Exception("Expected single value object");

            if (int.TryParse(xSingleValueObj.Value, out var xIntValue) && int.TryParse(ySingleValueObj.Value, out var yIntValue))
            {
                _window.DrawSprite(filenameSingleValueObj.Value, xIntValue, yIntValue);
                return new NullObj();
            }
            
            throw new Exception("Expected number");
        }

        if (call.Name == "playSound")
        {
            if (call.Arguments.Count != 1)
                throw new Exception("Expected 1 argument");
            
            var filename = EvaluateExpression(call.Arguments[0], stoppingToken);
            
            if (filename is not SingleValueObj filenameSingleValueObj)
                throw new Exception("Expected single value object");

            _window.PlaySound(filenameSingleValueObj.Value);
            return new NullObj();
        }

        if (call.Name == "random")
        {
            if (call.Arguments.Count != 2)
                throw new Exception("Expected 2 arguments");
            
            var min = EvaluateExpression(call.Arguments[0], stoppingToken);
            var max = EvaluateExpression(call.Arguments[1], stoppingToken);
            
            if (min is not SingleValueObj minSingleValueObj)
                throw new Exception("Expected single value object");

            if (max is not SingleValueObj maxSingleValueObj)
                throw new Exception("Expected single value object");

            if (int.TryParse(minSingleValueObj.Value, out var minIntValue) && int.TryParse(maxSingleValueObj.Value, out var maxIntValue))
            {
                var random = new Random();
                var value = random.Next(minIntValue, maxIntValue + 1);
                return new SingleValueObj(value.ToString());
            }
            
            throw new Exception("Expected number");
        }
        
        throw new NotImplementedException();
    }

    private Obj EvaluateExpression(Expression expression, CancellationToken stoppingToken)
    {
        switch (expression)
        {
            case UnaryExpression unaryExpression:
                return EvaluateUnaryExpression(unaryExpression, stoppingToken);
            case BinaryExpression binaryExpression:
                return EvaluateBinaryExpression(binaryExpression, stoppingToken);
            case Number number:
                return new SingleValueObj(number.Value.ToString());
            case String @string:
                return new SingleValueObj(ProcessString(@string.Value));
            case Boolean boolean:
                return new SingleValueObj(boolean.Value.ToString());
            case Identifier identifier:
                return CurrentEnvironment.GetVariable(identifier.Value);
            case CallExpression callExpression:
                return EvaluateCallExpression(callExpression, stoppingToken);
            default:
                throw new NotImplementedException();
        }
    }
    
    private Obj EvaluateUnaryExpression(UnaryExpression unaryExpression, CancellationToken stoppingToken)
    {
        var operand = EvaluateExpression(unaryExpression.Operand, stoppingToken);
        
        if (operand is not SingleValueObj singleValueObj)
            throw new Exception("Expected single value object");

        if (double.TryParse(singleValueObj.Value, out var doubleValue))
        {
            return unaryExpression.Operator switch
            {
                TokenType.Minus => new SingleValueObj((-doubleValue).ToString()),
                TokenType.Bang => new SingleValueObj((doubleValue > 0).ToString()),
                _ => throw new NotImplementedException()
            };
        }
        
        if (bool.TryParse(singleValueObj.Value, out var boolValue))
        {
            return unaryExpression.Operator switch
            {
                TokenType.Minus => throw new Exception("Cannot negate a boolean"),
                TokenType.Bang => new SingleValueObj((!boolValue).ToString()),
                _ => throw new NotImplementedException()
            };
        }
        
        throw new Exception("Expected number or boolean");
    }

    private Obj EvaluateBinaryExpression(BinaryExpression binaryExpression, CancellationToken stoppingToken)
    {
        var lhs = EvaluateExpression(binaryExpression.Left, stoppingToken);
        var rhs = EvaluateExpression(binaryExpression.Right, stoppingToken);
        
        if (lhs is not SingleValueObj lhsSingleValueObj)
            throw new Exception("Expected single value object");
        
        if (rhs is not SingleValueObj rhsSingleValueObj)
            throw new Exception("Expected single value object");
        
        if (double.TryParse(lhsSingleValueObj.Value, out var lhsDoubleValue) && double.TryParse(rhsSingleValueObj.Value, out var rhsDoubleValue))
        {
            return binaryExpression.Operator switch
            {
                TokenType.Plus => new SingleValueObj((lhsDoubleValue + rhsDoubleValue).ToString()),
                TokenType.Minus => new SingleValueObj((lhsDoubleValue - rhsDoubleValue).ToString()),
                TokenType.Star => new SingleValueObj((lhsDoubleValue * rhsDoubleValue).ToString()),
                TokenType.Slash => new SingleValueObj((lhsDoubleValue / rhsDoubleValue).ToString()),
                TokenType.Less => new SingleValueObj((lhsDoubleValue < rhsDoubleValue).ToString()),
                TokenType.LessEqual => new SingleValueObj((lhsDoubleValue <= rhsDoubleValue).ToString()),
                TokenType.Greater => new SingleValueObj((lhsDoubleValue > rhsDoubleValue).ToString()),
                TokenType.GreaterEqual => new SingleValueObj((lhsDoubleValue >= rhsDoubleValue).ToString()),
                TokenType.EqualEqual => new SingleValueObj((lhsDoubleValue == rhsDoubleValue).ToString()),
                TokenType.BangEqual => new SingleValueObj((lhsDoubleValue != rhsDoubleValue).ToString()),
                TokenType.PipePipe => throw new Exception("Cannot OR two numbers"),
                TokenType.AmpersandAmpersand => throw new Exception("Cannot AND two numbers"),
                _ => throw new NotImplementedException()
            };
        }
        
        if (bool.TryParse(lhsSingleValueObj.Value, out var lhsBoolValue) && bool.TryParse(rhsSingleValueObj.Value, out var rhsBoolValue))
        {
            return binaryExpression.Operator switch
            {
                TokenType.Plus => throw new Exception("Cannot add two booleans"),
                TokenType.Minus => throw new Exception("Cannot subtract two booleans"),
                TokenType.Star => throw new Exception("Cannot multiply two booleans"),
                TokenType.Slash => throw new Exception("Cannot divide two booleans"),
                TokenType.Less => throw new Exception("Cannot compare two booleans"),
                TokenType.LessEqual => throw new Exception("Cannot compare two booleans"),
                TokenType.Greater => throw new Exception("Cannot compare two booleans"),
                TokenType.GreaterEqual => throw new Exception("Cannot compare two booleans"),
                TokenType.EqualEqual => new SingleValueObj((lhsBoolValue == rhsBoolValue).ToString()),
                TokenType.BangEqual => new SingleValueObj((lhsBoolValue != rhsBoolValue).ToString()),
                TokenType.PipePipe => new SingleValueObj((lhsBoolValue || rhsBoolValue).ToString()),
                TokenType.AmpersandAmpersand => new SingleValueObj((lhsBoolValue && rhsBoolValue).ToString()),
                _ => throw new NotImplementedException()
            };
        }
        
        throw new Exception("Expected number or boolean");
    }
    
    private string ProcessString(string value)
    {
        var sb = new StringBuilder();
        var i = 0;
        while (i < value.Length)
        {
            if (i + 1 < value.Length && value[i] == '{' && value[i + 1] != '{')
            {
                var closeIndex = value.IndexOf('}', i + 1);
                if (closeIndex == -1)
                {
                    throw new Exception("Mismatched '{' in string interpolation");
                }

                var varName = value.Substring(i + 1, closeIndex - i - 1).Trim();
                Obj variableValue = CurrentEnvironment.GetVariable(varName);
                if (variableValue is SingleValueObj singleValueObj)
                {
                    sb.Append(singleValueObj.Value);
                }
                else
                {
                    throw new Exception($"Variable {varName} not found");
                }
                i = closeIndex + 1;
            }
            else if (i + 1 < value.Length && value[i] == '{' && value[i + 1] == '{')
            {
                sb.Append('{');
                i += 2;
            }
            else if (value[i] == '}' && i + 1 < value.Length && value[i + 1] == '}')
            {
                sb.Append('}');
                i += 2;
            }
            else if (value[i] == '}')
            {
                throw new Exception("Mismatched '}' in string interpolation");
            }
            else
            {
                sb.Append(value[i]);
                i++;
            }
        }
        return sb.ToString();
    }
    
    private void PushEnvironment()
    {
        if (_environments.Count == 0)
            _environments.Push(new Environment());
        else
            _environments.Push(new Environment(_environments.Peek()));
    }
    
    private void PopEnvironment()
    {
        _environments.Pop();
    }
}