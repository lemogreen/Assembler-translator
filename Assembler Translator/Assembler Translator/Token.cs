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

    struct TokenPosition
    {
        int positionInTable;
        int positionInRow;

        public TokenPosition(int positionInTable, int positionInRow)
        {
            this.positionInTable = positionInTable;
            this.positionInRow = positionInRow;
        }
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
    }
}