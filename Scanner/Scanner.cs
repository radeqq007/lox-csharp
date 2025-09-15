namespace lox.Scanner
{
    public class Scanner(string src)
    {
        private readonly string src = src;
        private readonly List<Token.Token> tokens = [];

        private int start = 0;
        private int current = 0;
        private int line = 1;

        private static readonly Dictionary<string, Token.TokenType> keywords = new()
        {
            {"and", Token.TokenType.AND},
            {"class", Token.TokenType.CLASS},
            {"else", Token.TokenType.ELSE},
            {"false", Token.TokenType.FALSE},
            {"for", Token.TokenType.FOR},
            {"fun", Token.TokenType.FUN},
            {"if", Token.TokenType.IF},
            {"nil", Token.TokenType.NIL},
            {"or", Token.TokenType.OR},
            {"print", Token.TokenType.PRINT},
            {"return", Token.TokenType.RETURN},
            {"super", Token.TokenType.SUPER},
            {"this", Token.TokenType.THIS},
            {"true", Token.TokenType.TRUE},
            {"var", Token.TokenType.VAR},
            {"while", Token.TokenType.WHILE},
        };

        public List<Token.Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                start = current;
                ScanToken();
            }

            tokens.Add(new Token.Token(Token.TokenType.EOF, "", null, line));
            return tokens;
        }

        private bool IsAtEnd()
        {
            return current >= src.Length;
        }

        private void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                case '(':
                    AddToken(Token.TokenType.LEFT_PAREN);
                    break;

                case ')':
                    AddToken(Token.TokenType.RIGHT_PAREN);
                    break;

                case '{':
                    AddToken(Token.TokenType.LEFT_BRACE);
                    break;

                case '}':
                    AddToken(Token.TokenType.RIGHT_BRACE);
                    break;

                case ',':
                    AddToken(Token.TokenType.COMMA);
                    break;

                case '.':
                    AddToken(Token.TokenType.DOT);
                    break;

                case '-':
                    AddToken(Token.TokenType.MINUS);
                    break;

                case '+':
                    AddToken(Token.TokenType.PLUS);
                    break;

                case ';':
                    AddToken(Token.TokenType.SEMICOLON);
                    break;

                case '*':
                    AddToken(Token.TokenType.STAR);
                    break;


                case '/':
                    if (Match('/'))
                    {
                        while (Peek() != '\n' && !IsAtEnd()) Advance();
                    }
                    else
                    {
                        AddToken(Token.TokenType.SLASH);
                    }
                    break;

                case '!':
                    AddToken(Match('=') ? Token.TokenType.BANG_EQUAL : Token.TokenType.BANG);
                    break;

                case '=':
                    AddToken(Match('=') ? Token.TokenType.EQUAL_EQUAL : Token.TokenType.EQUAL);
                    break;

                case '<':
                    AddToken(Match('=') ? Token.TokenType.LESS_EQUAL : Token.TokenType.LESS);
                    break;

                case '>':
                    AddToken(Match('=') ? Token.TokenType.GREATER_EQUAL : Token.TokenType.GREATER);
                    break;

                case '"':
                    String();
                    break;

                case ' ':
                case '\r':
                case '\t':
                    break; // Ignore whitespace

                case '\n':
                    line++;
                    break;

                default:
                    if (IsDigit(c))
                        Number();
                    else if (IsAlpha(c))
                        Identifier();
                    else
                        Program.Error(line, "Unexpected character.");

                    break;
            }
        }

        private char Advance()
        {
            return src[current++];
        }

        private void AddToken(Token.TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(Token.TokenType type, object? literal)
        {
            string text = src[start..current];
            tokens.Add(new Token.Token(type, text, literal, line));
        }

        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (src[current] != expected) return false;
            current++;
            return true;
        }

        private char Peek()
        {
            if (IsAtEnd()) return '\0';
            return src[current];
        }

        private char PeekNext()
        {
            if (current + 1 >= src.Length) return '\0';
            return src[current + 1];
        }

        private void String()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') line++;
                Advance();
            }

            if (IsAtEnd())
            {
                Program.Error(line, "Unterminated string.");
                return;
            }

            Advance(); // The closing "

            string val = src[(start + 1)..(current - 1)];
            AddToken(Token.TokenType.STRING, val);
        }

        private void Number()
        {
            while (IsDigit(Peek())) Advance();

            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                Advance();

                while (IsDigit(Peek())) Advance();
            }

            AddToken(Token.TokenType.NUMBER, double.Parse(src[start..current]));
        }

        private void Identifier() {
            while (IsAlphaNumeric(Peek())) Advance();

            string text = src[start..current];

            AddToken(
                keywords.TryGetValue(text, out var type)
                    ? type
                    : Token.TokenType.IDENTIFIER
            );
        }

        private static bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private static bool IsAlpha(char c) {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
        }

        private static bool IsAlphaNumeric(char c) {
            return IsAlpha(c) || IsDigit(c);
        }
    }
}