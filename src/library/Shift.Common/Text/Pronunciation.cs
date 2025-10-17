using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Shift.Common
{
    /// <remarks>
    /// The algorithm for this function is Copyright(c) 2006 Creativyst, Inc. For more information 
    /// go to http://www.creativyst.com/Doc/Articles/SoundEx1/SoundEx1.htm
    /// 
    /// censusMode:
    ///   0 = Disabled
    ///   1 = Properly calculated SoundEx codes found in all census years
    ///   2 = Improperly calculated SoundEx codes found in SOME of the censuses performed in 1880, 1900, and 1910
    /// </remarks>
    public static class Pronunciation
    {
        public static string Soundex(string word, int resultLength, int censusMode)
        {
            String temp;
            Int32 soundexLength = 10;

            // Perform sanity checks 

            if (censusMode > 0)
                resultLength = 4;

            if (resultLength > 0)
                soundexLength = resultLength;

            if (soundexLength > 10)
                soundexLength = 10;

            if (soundexLength < 4)
                soundexLength = 4;

            // Remove leading and trailing whitespace

            if (word == null)
                return String.Empty;

            word = TrimAndClean(word);

            if (String.IsNullOrEmpty(word))
                return String.Empty;

            // Company names may include digits (e.g. 3M) - it is important to
            // preserve these digits in the resulting SoundEx code

            word = word.Replace("0", "Zero");
            word = word.Replace("1", "One");
            word = word.Replace("2", "Two");
            word = word.Replace("3", "Three");
            word = word.Replace("4", "Four");
            word = word.Replace("5", "Five");
            word = word.Replace("6", "Six");
            word = word.Replace("7", "Seven");
            word = word.Replace("8", "Eight");
            word = word.Replace("9", "Nine");

            // Clean and tidy

            word = word.ToUpper();

            String sound = word;

            // Replace non-chars with spaces; if there is nothing but 
            // whitespace left in the resulting string then abandon ship

            sound = Replace(sound, @"[^A-Z]", " ");
            sound = TrimAndClean(sound);

            if (String.IsNullOrEmpty(sound))
                return String.Empty;

            if (censusMode == 0)
            {
                sound = Replace(sound, @"DG", "G"); // Change DG to G
                sound = Replace(sound, @"GH", "H"); // Change GH to H
                sound = Replace(sound, @"GN", "N"); // Change GN to N
                sound = Replace(sound, @"KN", "N"); // Change KN to N
                sound = Replace(sound, @"PH", "F"); // Change PH to F
                sound = Replace(sound, @"MP([STZ])", "M$1"); // Change MPST|MPZ to MST|MZ
                sound = Replace(sound, @"^PS", "S"); // Change leading PS to S
                sound = Replace(sound, @"^PF", "F"); // Change leading PF to F
                sound = Replace(sound, @"MB", "M"); // Change MB to M
                sound = Replace(sound, @"TCH", "CH"); // Change TCH to CH
            }

            // The above improvements may change the first letter

            Char firstLetter = sound[0];

            // In properly done census, SoundEx for H and W will be squezeed out 
            // before performing the test for adjacent digits (this differs from 
            // how 'real' vowels are handled)

            if (firstLetter == 'H' || firstLetter == 'W')
            {
                temp = sound.Substring(1);
                sound = "-";
                sound += temp;
            }

            if (censusMode == 1)
                sound = Replace(sound, @"[HW]", ".");

            // Classic SoundEx

            sound = Replace(sound, @"[AEIOUYHW]", "0");
            sound = Replace(sound, @"[BPFV]", "1");
            sound = Replace(sound, @"[CSGJKQXZ]", "2");
            sound = Replace(sound, @"[DT]", "3");
            sound = Replace(sound, @"[L]", "4");
            sound = Replace(sound, @"[MN]", "5");
            sound = Replace(sound, @"[R]", "6");

            // Properly done census: squeeze out H and W before doing adjacent 
            // digit removal.

            if (censusMode == 1)
                sound = Replace(sound, @"\.", String.Empty);

            // Remove extra equal adjacent digits

            Int32 soundLength = sound.Length;
            Char lastChar = '\0';
            temp = String.Empty;

            for (Int32 i = 0; i < soundLength; i++)
            {
                Char currentChar = sound[i];
                if (currentChar == lastChar)
                    temp += " ";
                else
                {
                    temp += currentChar;
                    lastChar = currentChar;
                }
            }

            sound = temp;

            // Drop the first letter code
            sound = sound.Substring(1);

            // Remove spaces
            sound = Replace(sound, @"\s", String.Empty);

            // Remove zeros
            sound = Replace(sound, @"0", String.Empty);

            // Right-pad with zeros
            sound += "0000000000";

            // Add the first letter of the word
            sound = firstLetter + sound;

            // And, finally, size to taste
            sound = sound.Substring(0, soundexLength);

            return sound;
        }

        /// <remarks>
        /// Replaces every occurrence of a sub-String within the input text with the specified 
        /// replacement text. This function is not case-sensitive.
        /// </remarks>
        private static string Replace(string text, string findWhat, string replaceWith)
        {
            var re = new Regex(findWhat, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return re.Replace(text, replaceWith);
        }

        private static string TrimAndClean(string text)
        {
            var trimCharacters = new[] { ' ', '\n', '\r', '\t' };

            var cleanReplacements = new Dictionary<string, string>
            {
                {"\xa0", " "},
                {"\xa9", "(c)"},
                {"\xad", "-"},
                {"\xae", "(r)"},
                {"\xb7", "*"},
                {"\u2018", "'"},
                {"\u2019", "'"},
                {"\u201c", "\""},
                {"\u201d", "\""},
                {"\u2026", "..."},
                {"\u2002", " "},
                {"\u2003", " "},
                {"\u2009", " "},
                {"\u2013", "-"},
                {"\u2014", "--"},
                {"\u2122", "(tm)"}
            };
            
            var cleanRegex = new Regex("(" + string.Join("|", cleanReplacements.Keys) + ")", RegexOptions.Compiled);

            return (text == null || text.Length == 0)
                ? text
                : cleanRegex.Replace(text.Trim(trimCharacters), m => cleanReplacements[m.Value]);
        }
    }
}