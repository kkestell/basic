using System.Text;

namespace Basic;

public class Interpreter
{
    private readonly Stack<Environment> _environments = new();
    private Environment CurrentEnvironment => _environments.Peek();
    
    public Interpreter()
    {
        PushEnvironment();
    }
    
    public async Task ExecuteStatement(Statement statement, CancellationToken stoppingToken = default)
    {
        switch (statement)
        {
            case FunctionDefinition functionDefinition:
                ExecuteFunctionDefinition(functionDefinition, stoppingToken);
                break;
            case Block block:
                await ExecuteBlock(block, stoppingToken);
                break;
            case Assignment assignment:
                ExecuteAssignment(assignment, stoppingToken);
                break;
            case If @if:
                await ExecuteIf(@if, stoppingToken);
                break;
            case ForRange forRange:
                await ExecuteForRange(forRange, stoppingToken);
                break;
            case While @while:
                await ExecuteWhile(@while, stoppingToken);
                break;
            case ExpressionStatement call:
                ExecuteExpressionStatement(call, stoppingToken);
                break;
            case Return @return:
                ExecuteReturn(@return, stoppingToken);
                break;
            case Break _:
                throw new BreakException();
            default:
                throw new NotImplementedException();
        }
    }
    
    private void ExecuteFunctionDefinition(FunctionDefinition functionDefinition, CancellationToken _)
    {
        CurrentEnvironment.SetVariable(functionDefinition.Name, new FunctionObj(functionDefinition.Parameters, functionDefinition.Body, CurrentEnvironment));
    }
    
    private async Task ExecuteBlock(Block block, CancellationToken stoppingToken)
    {
        foreach (var statement in block.Statements)
        {
            await ExecuteStatement(statement, stoppingToken);
        }
    }
    
    private void ExecuteAssignment(Assignment assignment, CancellationToken stoppingToken)
    {
        var value = EvaluateExpression(assignment.Value, stoppingToken);
        CurrentEnvironment.SetVariable(assignment.Name, value);
    }

    private async Task ExecuteIf(If @if, CancellationToken stoppingToken)
    {
        var condition = EvaluateExpression(@if.Condition, stoppingToken);
        
        if (condition is not SingleValueObj singleValueObj)
            throw new Exception("Expected single value object");

        if (!bool.TryParse(singleValueObj.Value, out var boolValue))
            throw new Exception("Expected boolean");
        
        if (boolValue)
        {
            await ExecuteStatement(@if.Then, stoppingToken);
        }
        else if (@if.Else is not null)
        {
            await ExecuteStatement(@if.Else, stoppingToken);
        }
    }
    
    private async Task ExecuteForRange(ForRange forRange, CancellationToken stoppingToken)
    {
        var start = EvaluateExpression(forRange.Start, stoppingToken);
        var end = EvaluateExpression(forRange.End, stoppingToken);
        
        if (start is not SingleValueObj startSingleValueObj)
            throw new Exception("Expected single value object");

        if (end is not SingleValueObj endSingleValueObj)
            throw new Exception("Expected single value object");

        if (!double.TryParse(startSingleValueObj.Value, out var startDoubleValue) || !double.TryParse(endSingleValueObj.Value, out var endDoubleValue))
            throw new Exception("Expected number");
        
        for (var i = startDoubleValue; i <= endDoubleValue; i++)
        {
            CurrentEnvironment.SetVariable(forRange.Name, new SingleValueObj(i.ToString()));
            await ExecuteStatement(forRange.Body, stoppingToken);
        }
    }

    private async Task ExecuteWhile(While @while, CancellationToken stoppingToken)
    {
        try
        {
            while (true)
            {
                var condition = EvaluateExpression(@while.Condition, stoppingToken);

                if (condition is not SingleValueObj singleValueObj)
                    throw new Exception("Expected single value object");

                if (!bool.TryParse(singleValueObj.Value, out var boolValue))
                    throw new Exception("Expected boolean");
                
                if (!boolValue)
                    break;

                await ExecuteStatement(@while.Body, stoppingToken);
            }
        }
        catch (BreakException)
        {
        }
    }
    
    private void ExecuteExpressionStatement(ExpressionStatement expressionStatement, CancellationToken stoppingToken)
    {
        EvaluateExpression(expressionStatement.Expression, stoppingToken);
    }
    
    private void ExecuteReturn(Return @return, CancellationToken stoppingToken)
    {
        if (@return.Value is null)
            throw new NotImplementedException();
        
        var value = EvaluateExpression(@return.Value, stoppingToken);
        
        throw new ReturnException(value);
    }

    private Obj CallFunction(string name, List<Expression> arguments, CancellationToken stoppingToken)
    {
        var functionObj = CurrentEnvironment.GetVariable(name);
        
        if (functionObj is not FunctionObj function)
            throw new Exception("Expected function object");
        
        PushEnvironment();
        
        if (arguments.Count != function.Parameters.Count)
            throw new Exception("Argument count mismatch");
        
        for (var i = 0; i < arguments.Count; i++)
        {
            var value = EvaluateExpression(arguments[i], stoppingToken);
            CurrentEnvironment.SetVariable(function.Parameters[i], value);
        }
        
        try
        {
            ExecuteStatement(function.Body, stoppingToken).Wait(stoppingToken);
        }
        catch (AggregateException ae)
        {
            if (ae.InnerException is ReturnException e)
            {
                PopEnvironment();
                return e.Value ?? new NullObj();
            }
        }
        
        PopEnvironment();
        return new NullObj();
    }
    
    // FIXME: Make async
    private Obj EvaluateCallExpression(CallExpression call, CancellationToken stoppingToken)
    {
        if (call.Name == "PRINT")
        {
            foreach (var argument in call.Arguments)
            {
                var value = EvaluateExpression(argument, stoppingToken);
                Console.Write(value);
            }
            
            Console.WriteLine();
            return new NullObj();
        }
        
        return CallFunction(call.Name, call.Arguments, stoppingToken);
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