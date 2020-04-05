using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System;

namespace Assembler_Translator
{
    class Translator
    {
        private const string regexString = @"((?<=;)|(?=;)|(?<= )|(?= )|(?<=,)|(?=,)|(?=\w)(?<!\w)|(?<=\w)(?!\w))";
        private const string regexIdentificator = @"\b[a-z][a-z0-9]*";
        private const string regexConstant = @"^[0-9]*$";
        private DynamicTable dynamicTable = new DynamicTable();
        private StaticTable staticTable = new StaticTable();

        enum TypeOfChar
        {
            separator, keywordOrIdentificator, constant, operations, wrongCharacter
        }

        public List<List<Token>> generateTokens(string fileName)
        {
            var finalList = new List<List<Token>>();
            string[] programLines = File.ReadAllLines(fileName);
            foreach (var programLine in programLines)
            {
                var currentTokenLine = new List<Token>();
                string[] words = Regex.Split(programLine, regexString).Where(word => word != "" && word != " ").ToArray();
                foreach (var word in words)
                {
                    if (Regex.Matches(word, regexConstant).Count == 1)
                    {
                        try
                        {
                            var intWord = int.Parse(word);
                            currentTokenLine.Add(dynamicTable.addConstant(intWord));
                            continue;
                        }
                        catch (FormatException e)
                        {
                            Console.WriteLine($"Unable to parse '{e}'");
                        }
                    }

                    var tokenOfElementInStaticTable = staticTable.getIndexOf(word);
                    if (tokenOfElementInStaticTable.HasValue)
                    {
                        currentTokenLine.Add(tokenOfElementInStaticTable.Value);
                        continue;
                    }
                        

                    if (Regex.Matches(word, regexIdentificator).Count == 1)
                        currentTokenLine.Add(dynamicTable.addIdentificator(word));
                }
                finalList.Add(currentTokenLine);
            }
            return finalList;
        }


        public List<List<Token>> tokenize(string fileName)
        {
            var finalList = new List<List<Token>>();
            string[] programLines = File.ReadAllLines(fileName);
            foreach (var programLine in programLines)
            {
                List<Token> lineTokens = new List<Token>();
                if (programLine.Length != 0)
                {
                    bool isStartOfWord = true;
                    bool isOpenedComment = true;
                    TypeOfChar typeOfCurrentWord = getTypeOfChar(programLine[0]);
                    string currentWord = "";
                    foreach (char symbol in programLine)
                    {
                        if (isStartOfWord)
                        {
                            typeOfCurrentWord = getTypeOfChar(symbol);
                            isStartOfWord = false;
                        }

                        var typeOfCurrentChar = getTypeOfChar(symbol);
                        
                        switch (typeOfCurrentChar)
                        {
                            case TypeOfChar.separator:
                                if (symbol == ' ' || symbol == '\t')
                                {
                                    isStartOfWord = true;
                                    if (currentWord.Length == 0) continue;

                                    lineTokens.Add(addWord(typeOfCurrentWord, currentWord).Value);
                                    currentWord = "";
                                    continue;
                                }
                                lineTokens.Add(addWord(typeOfCurrentWord, currentWord).Value);
                                currentWord = "";
                                isStartOfWord = true;
                                lineTokens.Add(addWord(TypeOfChar.separator, symbol.ToString()).Value);
                                break;
                            case TypeOfChar.constant:
                                if (typeOfCurrentWord == TypeOfChar.operations)
                                {
                                    lineTokens.Add(addWord(typeOfCurrentWord, currentWord).Value);
                                    currentWord = "";
                                    currentWord += symbol;
                                    typeOfCurrentWord = TypeOfChar.constant;
                                }
                                currentWord += symbol;
                                continue;
                            case TypeOfChar.keywordOrIdentificator:
                                if (typeOfCurrentWord == TypeOfChar.constant)
                                {
                                    // todo
                                    continue;
                                }
                                if (typeOfCurrentWord != typeOfCurrentChar)
                                {
                                    lineTokens.Add(addWord(typeOfCurrentWord, currentWord).Value);
                                    currentWord = "";
                                    currentWord += symbol;
                                    typeOfCurrentWord = typeOfCurrentChar;
                                }
                                currentWord += symbol;
                                break;
                            case TypeOfChar.operations:
                                if (typeOfCurrentWord != typeOfCurrentChar)
                                {
                                    lineTokens.Add(addWord(typeOfCurrentWord, currentWord).Value);
                                    currentWord = "";
                                    currentWord += symbol;
                                    typeOfCurrentWord = TypeOfChar.operations;
                                    continue;
                                }
                                currentWord += symbol;
                                //lineTokens.Add(addWord(TypeOfChar.operations, symbol.ToString()).Value);
                                break;
                            case TypeOfChar.wrongCharacter:
                                // todo
                                break;
                        }
                    }
                }
                finalList.Add(lineTokens);
            }
            return finalList;
        }

        private Token? addWord(TypeOfChar typeOfWord, string word)
        {
            switch (typeOfWord)
            {
                case TypeOfChar.constant:
                    var intValue = int.Parse(word);
                    return dynamicTable.addConstant(intValue);
                case TypeOfChar.keywordOrIdentificator:
                    var keywordToken = staticTable.getIndexOf(word);
                    if (keywordToken.HasValue) return keywordToken;
                    return dynamicTable.addIdentificator(word);
                case TypeOfChar.operations:
                    return staticTable.getIndexOf(word);
                case TypeOfChar.separator:
                    return staticTable.getIndexOf(word);
                default:
                    return null;
            }
        }

        private TypeOfChar getTypeOfChar(char symbol)
        {
            if (staticTable.separators.Contains(symbol.ToString()) || symbol == '\t') return TypeOfChar.separator;
            if (staticTable.allowedAlphabeth.Contains(symbol)) return TypeOfChar.keywordOrIdentificator;
            if (staticTable.allowedNumbers.Contains(symbol)) return TypeOfChar.constant;
            if (staticTable.operationsChars.Contains(symbol.ToString())) return TypeOfChar.operations;
            return TypeOfChar.keywordOrIdentificator;
        }
    }
}
