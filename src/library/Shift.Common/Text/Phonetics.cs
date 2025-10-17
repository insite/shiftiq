using System.Text.RegularExpressions;

using Shift.Constant;

namespace Shift.Common
{
    public class Phonetics
    {
        /*
	     * The algorithm for this function is:
	     * Copyright (c) 2006 Creativyst, Inc.
	     * 
	     * For more information go to:
	     * http://www.creativyst.com/Doc/Articles/SoundEx1/SoundEx1.htm
	     * 
	     * censusMode
	     *    0 = Disabled
	     *    1 = Properly calculated SoundEx codes found in all census years
	     *    2 = Improperly calculated SoundEx codes found in SOME of the censuses performed in 1880, 1900, and 1910
	     */

        public string Soundex(string word, int resultLength, int censusMode)
        {
            string temp;
            var soundexLength = 10;

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
                return string.Empty;

            word = Trim(word);

            if (string.IsNullOrEmpty(word))
                return string.Empty;

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

            word = word.ToUpper(Cultures.Default);

            var sound = word;

            // Replace non-chars with spaces; if there is nothing but 
            // whitespace left in the resulting string then abandon ship

            sound = Replace(sound, @"[^A-Z]", " ");
            sound = Trim(sound);

            if (string.IsNullOrEmpty(sound))
                return string.Empty;

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

            var firstLetter = sound[0];

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
                sound = Replace(sound, @"\.", string.Empty);

            // Remove extra equal adjacent digits

            var soundLength = sound.Length;
            var lastChar = '\0';
            temp = string.Empty;

            for (var i = 0; i < soundLength; i++)
            {
                var currentChar = sound[i];
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
            sound = Replace(sound, @"\s", string.Empty);

            // Remove zeros
            sound = Replace(sound, @"0", string.Empty);

            // Right-pad with zeros
            sound += "0000000000";

            // Add the first letter of the word
            sound = firstLetter + sound;

            // And, finally, size to taste
            sound = sound.Substring(0, soundexLength);

            return sound;
        }

        private string Replace(string text, string findWhat, string replaceWith)
        {
            var re = new Regex(findWhat, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return re.Replace(text, replaceWith);
        }

        private string Trim(string text)
        {
            if (text == null)
                return null;

            string result = text.Trim(' ', '\n', '\r', '\t');

            string[][] replacements =
            {
                new[] {"\xa0", " "},
                new[] {"\xa9", "(c)"},
                new[] {"\xad", "-"},
                new[] {"\xae", "(r)"},
                new[] {"\xb7", "*"},
                new[] {"\u2018", "'"},
                new[] {"\u2019", "'"},
                new[] {"\u201c", "\""},
                new[] {"\u201d", "\""},
                new[] {"\u2026", "..."},
                new[] {"\u2002", " "},
                new[] {"\u2003", " "},
                new[] {"\u2009", " "},
                new[] {"\u2013", "-"},
                new[] {"\u2014", "--"},
                new[] {"\u2122", "(tm)"}
            };

            foreach (var replacement in replacements)
            {
                result = result.Replace(replacement[0], replacement[1]);
            }

            return result;
        }
    }
}
