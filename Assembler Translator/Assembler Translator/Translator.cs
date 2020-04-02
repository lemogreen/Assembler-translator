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
    }
}
