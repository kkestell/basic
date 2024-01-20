namespace Basic;

public record Location(uint Row, uint Column, ReadOnlyMemory<char> LineContent, string? FileName = null);