using System;
using System.Text.RegularExpressions;
using System.Linq;

namespace Assembler_Translator
{
    class Program
    {
        static void Main(string[] args)
        {
            var translator = new Translator();
            translator.generateTokens("testProgram.txt");
            translator.tokenize("testProgram.txt");
        }
    }
}
