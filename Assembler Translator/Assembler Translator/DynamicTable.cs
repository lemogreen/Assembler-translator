using System;
using System.Collections.Generic;

namespace Assembler_Translator
{
    class DynamicTable
    {
        private List<String>[] identificators;
        private List<int> constants;
        private const int dynamicTableSize = 100;


        public DynamicTable()
        {
            constants = new List<int>();
            identificators = new List<string>[dynamicTableSize];
            for (int i = 0; i < dynamicTableSize; i++)
                identificators[i] = new List<string>();
        }

        public Token addIdentificator(string name)
        {
            var indexInTable = name.CalculateHash(dynamicTableSize);
            var indexInRow = identificators[indexInTable].IndexOf(name);
            if (indexInRow != -1)
                return new Token(TokenType.identificator, indexInTable, indexInRow);
            else
            {
                identificators[indexInTable].Add(name);
                return new Token(TokenType.identificator, indexInTable, 0);
            }
        }

        public Token addConstant(int value)
        {
            var indexOfConstant = constants.IndexOf(value);
            if (indexOfConstant != -1)
                return new Token(TokenType.constant, indexOfConstant, -1);
            else
            {
                constants.Add(value);
                return new Token(TokenType.constant, constants.Count - 1, -1);
            }
        }
    }
}