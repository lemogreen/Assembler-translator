using System;
using System.IO;
using System.Collections.Generic;

namespace Assembler_Translator
{
    class StaticTable
    {
        private const string keywordsTableFileName = "keywords.txt";
        private const string separatorsTableFileName = "separators.txt";
        private const string operationsTableFileName = "operations.txt";
        private const string operationsCharsTableFileName = "operationsChars.txt";
        private const string allowedAlphabethCharsTableFileName = "allowedAlphabeth.txt";
        private const string allowedNumbersCharsTableFileName = "allowedNumbers.txt";

        public List<String> keywords { get; private set; }
        public List<String> separators { get; private set; }
        public List<String> operations { get; private set; }
        public List<String> operationsChars { get; private set; }
        public HashSet<Char> allowedAlphabeth { get; private set; }
        public HashSet<Char> allowedNumbers { get; private set; }

        public StaticTable()
        {
            keywords = new List<string>();
            separators = new List<string>();
            operations = new List<string>();
            operationsChars = new List<string>();
            allowedAlphabeth = new HashSet<Char>();
            allowedNumbers = new HashSet<Char>();


            string[] keywordLines = File.ReadAllLines(keywordsTableFileName);
            string[] separatorsLines = File.ReadAllLines(separatorsTableFileName);
            string[] operationsLines = File.ReadAllLines(operationsTableFileName);
            string[] operationsCharsLines = File.ReadAllLines(operationsCharsTableFileName);
            string[] allowedAlphabethLines = File.ReadAllLines(allowedAlphabethCharsTableFileName);
            string[] allowedNumbersLines = File.ReadAllLines(allowedNumbersCharsTableFileName);

            foreach (string keyword in keywordLines)
            {
                if (keywords.Contains(keyword)) continue;
                keywords.Add(keyword);
            }

            foreach (string separator in separatorsLines)
            {
                if (separators.Contains(separator)) continue;
                separators.Add(separator);
            }

            foreach (var operation in operationsLines)
            {
                if (operations.Contains(operation)) continue;
                operations.Add(operation);
            }

            foreach (var operationsChar in operationsCharsLines)
            {
                if (operationsChars.Contains(operationsChar)) continue;
                operationsChars.Add(operationsChar);
            }

            foreach (string line in allowedAlphabethLines)
                foreach (var symbol in line)
                    allowedAlphabeth.Add(symbol);

            foreach (string line in allowedNumbersLines)
                foreach (var symbol in line)
                    allowedNumbers.Add(symbol);

            keywords.Sort();
            separators.Sort();
            operations.Sort();
        }

        public Token? getIndexOf(string word)
        {
            int keywordIndex = keywords.FindIndex(keyword => keyword == word);
            if (keywordIndex != -1) return new Token(TokenType.keyword, keywordIndex, -1);

            int separatorIndex = separators.FindIndex(keyword => keyword == word);
            if (separatorIndex != -1) return new Token(TokenType.separator, separatorIndex, -1);

            int operationIndex = operations.FindIndex(operation => operation == word);
            if (operationIndex != -1) return new Token(TokenType.operation, operationIndex, -1);

            return null;
        }

        public bool isOperation(char symbol) => operationsChars.Contains(symbol.ToString());
    }
}
