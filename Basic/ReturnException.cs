namespace Basic;

public class ReturnException : Exception
{
    public Obj? Value { get; }

    public ReturnException(Obj? value)
    {
        Value = value;
    }
}