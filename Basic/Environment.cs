namespace Basic;

public class Environment
{
    private readonly Environment? _parent;
    private readonly Dictionary<string, Obj> _variables = new();
    
    public Environment(Environment? parent = null)
    {
        _parent = parent;
    }
    
    public Obj GetVariable(string name)
    {
        if (_variables.TryGetValue(name, out var value))
            return value;
        
        if (_parent is not null)
            return _parent.GetVariable(name);
        
        throw new Exception($"Variable '{name}' not found.");
    }
    
    public bool TryGetVariable(string name, out Obj value)
    {
        if (_variables.TryGetValue(name, out value))
            return true;
        
        if (_parent is not null)
            return _parent.TryGetVariable(name, out value);
        
        value = null!;
        return false;
    }
    
    public void SetVariable(string name, Obj value)
    {
        _variables[name] = value;
    }
}