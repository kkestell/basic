namespace Basic;

public class InputBuffer
{
    private readonly List<ReadOnlyMemory<char>> _lines;
    private readonly char[] _buffer;
    private uint _offset;
    private uint _line = 1;
    private uint _col = 1;
    
    public Location Location => new(_line, _col, GetLineContent(_line));
    

    public InputBuffer(string input)
    {
        _lines = input.Split("\n").Select(x => x.AsMemory()).ToList();
        _buffer = input.ToCharArray();
    }

    public char Peek(int next = 0)
    {
        if (_offset + next >= _buffer.Length)
        {
            return '\0';
        }
        
        return _buffer[_offset + next];
    }

    public void Pop()
    {
        if (_buffer[_offset] == '\n')
        {
            _line++;
            _col = 1;
        }
        else
        {
            _col++;
        }

        _offset++;
    }

    public bool IsEmpty()
    {
        return _offset >= _buffer.Length;
    }

    private ReadOnlyMemory<char> GetLineContent(uint line)
    {
        if (line > 0 && line <= _lines.Count)
        {
            return _lines[(int)line - 1];
        }

        throw new Exception();
    }
}