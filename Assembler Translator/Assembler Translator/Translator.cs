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
            var errorsList = new List<TokenizeError>();
            var finalList = new List<List<Token>>();
            string[] programLines = File.ReadAllLines(fileName);
            bool isOpenedMultilineComment = false;
            int lineIndex = 0;
            foreach (var programLine in programLines)
            {
                int columnIndex = -1;
                List<Token> lineTokens = new List<Token>();
                if (programLine.Length != 0)
                {
                    bool isStartOfWord = true;
                    bool isOpenedComment = false;
                    TypeOfChar typeOfCurrentWord = getTypeOfChar(programLine[0]);
                    string currentWord = "";
                    foreach (char symbol in programLine)
                    {
                        columnIndex++;
                        if (isOpenedComment) break; // Если обнаружено начало комментария "//" - то прекратить считывание текущей строки, тк в этом нет смысла

                        if (isOpenedMultilineComment)
                        {
                            if (symbol == '*')
                            {
                                currentWord = symbol.ToString();
                                continue;
                            }
                            if (symbol == '/' && currentWord.Length != 0)
                            {
                                isOpenedMultilineComment = false;
                                currentWord = "";
                            }
                            currentWord = "";
                            continue;
                        }

                        if (isStartOfWord)
                        {
                            typeOfCurrentWord = getTypeOfChar(symbol);
                            isStartOfWord = false;
                        }

                        var typeOfCurrentChar = getTypeOfChar(symbol);

                        switch (typeOfCurrentChar)
                        {
                            case TypeOfChar.separator:
                                isStartOfWord = true;
                                if (currentWord.Length == 0) continue;
                                lineTokens.Add(addWord(typeOfCurrentWord, currentWord).Value);
                                currentWord = "";
                                var separatorToken = addWord(TypeOfChar.separator, symbol.ToString());
                                if (separatorToken.HasValue) lineTokens.Add(separatorToken.Value);
                                break;
                            case TypeOfChar.constant:
                                if (typeOfCurrentWord == TypeOfChar.operations)
                                {
                                    lineTokens.Add(addWord(typeOfCurrentWord, currentWord).Value);
                                    currentWord = "";
                                    currentWord += symbol;
                                    typeOfCurrentWord = TypeOfChar.constant;
                                    continue;
                                }
                                currentWord += symbol;
                                continue;
                            case TypeOfChar.keywordOrIdentificator:
                                if (typeOfCurrentWord == TypeOfChar.constant)
                                {
                                    errorsList.Add(new TokenizeError(TokenizeErrorType.wrongFormatOfNumber, lineIndex, columnIndex));
                                    continue;
                                }
                                if (typeOfCurrentWord != typeOfCurrentChar)
                                {
                                    lineTokens.Add(addWord(typeOfCurrentWord, currentWord).Value);
                                    currentWord = "";
                                    currentWord += symbol;
                                    typeOfCurrentWord = typeOfCurrentChar;
                                    continue;
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
                                if (currentWord == "/")
                                {
                                    if (symbol == '/')
                                    {
                                        isOpenedComment = true;
                                        currentWord = "";
                                        break;
                                    }
                                    if (symbol == '*')
                                    {
                                        isOpenedMultilineComment = true;
                                        currentWord = "";
                                        break;
                                    }
                                }
                                currentWord += symbol;
                                break;
                            case TypeOfChar.wrongCharacter:
                                errorsList.Add(new TokenizeError(TokenizeErrorType.forbiddenСharacter, lineIndex, columnIndex));
                                break;
                        }
                    }
                }
                finalList.Add(lineTokens);
                lineIndex++;
            }
            if (isOpenedMultilineComment)
                errorsList.Add(new TokenizeError(TokenizeErrorType.notClosedComment, null, null));
            #if DEBUG
            using (StreamWriter file = new StreamWriter("tokens.txt"))
                foreach (var tokenLine in finalList)
                    file.WriteLine(String.Join(" ", tokenLine.ToArray()));
            using (StreamWriter file = new StreamWriter("errors.txt"))
                foreach (var error in errorsList)
                    file.WriteLine($"{error.localizedError()}");
            #endif
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
                    if (word == " " || word == "\t") return null;
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
            return TypeOfChar.wrongCharacter;
        }
    }
}
