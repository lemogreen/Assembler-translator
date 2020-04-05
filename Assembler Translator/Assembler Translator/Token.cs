using System;

namespace Assembler_Translator
{
    enum TokenType: int
    {
        keyword = 10, 
        separator = 20, 
        identificator = 30, 
        constant = 40,
        operation = 50
    }

    public enum TokenizeErrorType
    {
        forbiddenСharacter, 
        wrongFormatOfNumber, 
        notClosedComment
    }

    struct TokenPosition
    {
        int positionInTable;
        int positionInRow;

        public TokenPosition(int positionInTable, int positionInRow)
        {
            this.positionInTable = positionInTable;
            this.positionInRow = positionInRow;
        }

        public override string ToString() => $"({positionInTable}, {positionInRow})";
    }

    struct Token
    {
        TokenType tokenType;
        TokenPosition tokenPosition;


        public Token(TokenType tokenType, TokenPosition tokenPosition)
        {
            this.tokenType = tokenType;
            this.tokenPosition = tokenPosition;
        }

        public Token(TokenType tokenType, int positionInTable, int positionInRow)
        {
            this.tokenType = tokenType;
            this.tokenPosition = new TokenPosition(positionInTable, positionInRow);
        }

        public override string ToString() => $"({(int)tokenType}, {tokenPosition})";
    }

    public struct TokenizeError
    {
        public TokenizeErrorType error;
        public int? line;
        public int? column;

        public TokenizeError(TokenizeErrorType error, int? line, int? column)
        {
            this.error = error;
            this.line = line;
            this.column = column;
        }
    }

    public static class TokenizeErrorExtantions
    {
        public static string localizedError(this TokenizeError error)
        {
            return $"{error.error.localizedError(error.line, error.column)}";
        }
    }


    public static class TokenizeErrorTypeExtensions
    {
        public static string localizedError(this TokenizeErrorType error, int? line, int? column)
        {
            switch (error)
            {
                case TokenizeErrorType.forbiddenСharacter:
                    return $"Forribden character at line {line}, column {column}";
                case TokenizeErrorType.notClosedComment:
                    return $"Tokenizer reaches end with not closed comment";
                case TokenizeErrorType.wrongFormatOfNumber:
                    return $"Using non-allowed characters in number at line {line}, column {column}";
                default:
                    return "";
            }
        }
    }
}

