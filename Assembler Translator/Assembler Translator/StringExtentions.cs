using System;

namespace Assembler_Translator
{
    public static class StringExtentions
    {
        public static int CalculateHash(this string str, int upperBound)
        {
            int total = 0;
            char[] charArray;
            charArray = str.ToCharArray();


            foreach (char singleChar in charArray)
                total += 11 * total + (int)singleChar;

            total %= upperBound;
            return total;
        }
    }
}