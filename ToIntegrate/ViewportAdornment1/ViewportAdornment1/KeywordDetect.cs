using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewportAdornment1
{
    class KeywordDetect
    {
        public static string DetectKeyword(string text, int position)
        {
            string start = GetLeftPart(text, position);
            string end = GetRightPart(text, position + 1);

            return start + end;
        }

        private static string GetRightPart(string text, int position)
        {
            List<char> chars = new List<char>();

            while (position < text.Length && IsWordChar(text[position]))
            {
                chars.Add(text[position]);
                position++;
            }
            return new string(chars.ToArray());
        }

        private static string GetLeftPart(string text, int position)
        {
            List<char> begining = new List<char>();

            while (position >= 0 && IsWordChar(text[position]) )
            {
                begining.Add(text[position]);
                position--;
            }

            begining.Reverse();
            return new string(begining.ToArray());
        }

        private static bool IsWordChar(char p)
        {
            return Char.IsLetterOrDigit(p) || p == '.' || p == '_';
        }
    }
}
