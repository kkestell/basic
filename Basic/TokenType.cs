namespace Basic;

public enum TokenType
{
    Eof,
    // Single character tokens
    Plus,
    Minus,
    Star,
    Slash,
    Greater,
    Less,
    Equal,
    Bang,
    LeftParen,
    RightParen,
    LeftBracket,
    RightBracket,
    Comma,
    // Double character tokens
    GreaterEqual,
    LessEqual,
    EqualEqual,
    BangEqual,
    PipePipe,
    AmpersandAmpersand,
    // Keywords
    If,
    Do,
    Else,
    End,
    For,
    In,
    To,
    While,
    Def,
    Return,
    Break,
    Continue,
    // Literals
    Boolean,
    Number,
    String,
    Identifier,
}