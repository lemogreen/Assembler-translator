using System;
using System.IO;
using System.Collections.Generic;

namespace Assembler_Translator
{
    class StaticTable
    {
        private const string keywordsTableFileName = "keywords.txt";
        private const string separatorsTableFileName = "separators.txt";
        private List<String> keywords;
        private List<String> separators;

        public StaticTable()
        {
            keywords = new List<string>();
            separators = new List<string>();

            string[] keywordLines = File.ReadAllLines(keywordsTableFileName);
            string[] separatorsLines = File.ReadAllLines(separatorsTableFileName);

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

            keywords.Sort();
            separators.Sort();
        }

        public Token? getIndexOf(string word)
        {
            int keywordIndex = keywords.FindIndex(keyword => keyword == word);
            if (keywordIndex != -1) return new Token(TokenType.keyword, keywordIndex, -1);

            int separatorIndex = separators.FindIndex(keyword => keyword == word);
            if (separatorIndex != -1) return new Token(TokenType.separator, keywordIndex, -1);

            return null;
        }
    }
}
