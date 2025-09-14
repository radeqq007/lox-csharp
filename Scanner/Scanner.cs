namespace lox
{
    public class Scanner(String src)
    {
        private readonly String src = src;
        private readonly List<Token> tokens = [];

        private int start = 0;
        private int current = 0;
        private int line = 1;

        List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                start = current;
                ScanToken();
            }

            tokens.Add(new Token(TokenType.EOF, "", null, line));
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
                    AddToken(TokenType.LEFT_PAREN);
                    break;

                case ')':
                    AddToken(TokenType.RIGHT_PAREN);
                    break;

                case '{':
                    AddToken(TokenType.LEFT_BRACE);
                    break;

                case '}':
                    AddToken(TokenType.RIGHT_BRACE);
                    break;

                case ',':
                    AddToken(TokenType.COMMA);
                    break;

                case '.':
                    AddToken(TokenType.DOT);
                    break;

                case '-':
                    AddToken(TokenType.MINUS);
                    break;

                case '+':
                    AddToken(TokenType.PLUS);
                    break;

                case ';':
                    AddToken(TokenType.SEMICOLON);
                    break;

                case '*':
                    AddToken(TokenType.STAR);
                    break;

                default:
                    Program.Error(line, "Unexpected character.");
                    break;
            }
        }

        private char Advance()
        {
            return src[current++];
        }

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object? literal)
        {
            String text = src[start..current];
            this.tokens.Add(new Token(type, text, literal, line));
        }

        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (src[current] != expected) return false;
            current++;
            return true;
        }
    }
}