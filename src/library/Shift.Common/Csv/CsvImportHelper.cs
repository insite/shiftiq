using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Shift.Common
{
    public static class CsvImportHelper
    {
        #region Constants

        public const char CsvSeparator = ',';
        public const char QuoteChar = '"';

        #endregion

        #region Public methods

        public static string[][] GetValues(Stream csv, int? columnCount, bool autoDetermineSeparator, Encoding encoding)
        {
            using (var reader = new StreamReader(csv, encoding))
                return GetValues(reader, columnCount, autoDetermineSeparator);
        }

        public static string[][] GetValues(TextReader reader, int? columnCount, bool autoDetermineSeparator)
        {
            var list = new List<string[]>();
            var separator = autoDetermineSeparator ? (char?)null : CsvSeparator;

            string line;
            int lineNumber = 1;

            while ((line = reader.ReadLine()) != null)
            {
                if (separator == null)
                    separator = line.Contains("\t") ? '\t' : CsvSeparator;

                try
                {
                    var values = SplitCsvLine(line, columnCount, separator.Value);

                    // if (lineNumber > 1 || !firstRowContainsColumnNames)
                    list.Add(values);
                }
                catch (ApplicationError ex)
                {
                    throw ApplicationError.Create(ex, "Unexpected Error on Line {0}: {1}", lineNumber, ex.Message);
                }

                lineNumber++;
            }

            return list.ToArray();
        }

        #endregion

        #region Helper methods

        public static string[] SplitCsvLine(string line, int? columnCount, char separator)
        {
            var values = new List<string>();
            var startIndex = 0;
            var value = new StringBuilder();

            while (startIndex < line.Length)
            {
                while (startIndex < line.Length && char.IsWhiteSpace(line[startIndex]))
                    startIndex++;

                if (startIndex == line.Length)
                {
                    values.Add(string.Empty);
                    break;
                }

                char firstChar = line[startIndex];

                if (firstChar == separator)
                {
                    values.Add(string.Empty);

                    startIndex++;

                    if (startIndex == line.Length)
                        values.Add(string.Empty);

                    continue;
                }

                value.Clear();

                int index = startIndex + 1;
                Boolean isQuotedValue = firstChar == QuoteChar;
                Boolean isWaitingForQuote = false;
                Boolean isWaitingForComma = false;

                if (!isQuotedValue)
                    value.Append(firstChar);

                char c = '\0';

                while (index < line.Length)
                {
                    c = line[index];

                    if (isWaitingForQuote && char.IsWhiteSpace(c))
                    {
                        isWaitingForComma = true;
                    }
                    else if (isWaitingForComma && c != separator)
                    {
                        throw ApplicationError.Create("Parser error. Expected comma at col {0}, but found: {1}", index + 1, c);
                    }
                    else if (c == QuoteChar)
                    {
                        if (isWaitingForQuote)
                            value.Append(QuoteChar);

                        isWaitingForQuote = !isWaitingForQuote;
                    }
                    else if (c == separator && (!isQuotedValue || isWaitingForQuote))
                        break;
                    else
                        value.Append(c);

                    index++;
                }

                startIndex = index + 1;

                values.Add(StringHelper.TrimAndClean(value.ToString()));

                if (c == separator && startIndex == line.Length)
                    values.Add(string.Empty);
            }

            if (columnCount.HasValue && columnCount != values.Count)
                throw ApplicationError.Create("Expected {0} values but found {1}.", columnCount, values.Count);

            return values.ToArray();
        }

        #endregion
    }
}
