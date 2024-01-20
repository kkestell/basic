namespace Basic;

public record Obj;

public record NullObj : Obj;

public record SingleValueObj(string Value) : Obj
{
    public override string ToString() => Value;
}

public record ArrayObj(List<Obj> Values) : Obj;