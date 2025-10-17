using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace Shift.Common.File
{
    public static class LxrMergeFile
    {
        #region Classes

        public class Options
        {
            public string[] RequiredColumns { get; set; }
            public string[] IncludeColumns { get; set; }
            public bool LastColumnIsArray { get; set; }
            public Encoding Encoding { get; set; }
        }

        #endregion

        public static DataTable ToDataTable(Stream stream, Options options)
        {
            if (options == null)
                options = new Options();

            string text;
            using (TextReader reader = new StreamReader(stream, options.Encoding))
                text = reader.ReadToEnd();

            var lines = text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length <= 1)
                throw new ApplicationError("Data stream contains no data.");

            var table = new DataTable();

            var columns = ParseLxrMergeLine(lines[0]);
            if (columns.Length == 0)
                throw new ApplicationError("Data stream contains no columns.");

            var requiredColumns = new HashSet<string>(options.RequiredColumns ?? new string[0]);
            var hasIncludeColumns = options.IncludeColumns.IsNotEmpty();
            var columnsMapping = hasIncludeColumns ? new List<Tuple<int, int>>() : null;
            var includeColumns = hasIncludeColumns ? new HashSet<string>(options.IncludeColumns) : null;

            for (var i = 0; i < columns.Length; i++)
            {
                var colName = columns[i];
                if (string.IsNullOrEmpty(colName))
                    throw new ApplicationError("Column name cannot be null.");

                if (requiredColumns.Contains(colName))
                    requiredColumns.Remove(colName);

                if (hasIncludeColumns && !includeColumns.Contains(colName))
                    continue;

                if (table.Columns.Contains(colName))
                    throw new ApplicationError($"A column named '{colName}' already exists in the table.");

                if (hasIncludeColumns)
                    columnsMapping.Add(new Tuple<int, int>(table.Columns.Count, i));

                table.Columns.Add(colName, typeof(string));
            }

            if (requiredColumns.Count > 0)
                throw new ApplicationError("Required column(s) not found: " + string.Join(", ", requiredColumns));

            var lastColumnArrayLength = -1;

            for (var i = 1; i < lines.Length; i++)
            {
                var values = ParseLxrMergeLine(lines[i]);

                if (options.LastColumnIsArray)
                {
                    var arrayLength = values.Length - columns.Length + 1;
                    if (arrayLength <= 1)
                        throw new ApplicationError("The last column isn't array: " + lines[i]);

                    if (lastColumnArrayLength < 0)
                        lastColumnArrayLength = arrayLength;
                    else if (lastColumnArrayLength != arrayLength)
                        throw new ApplicationError("Unexpected length of last column array: " + lines[i]);

                    var newValues = new string[columns.Length];

                    var lastColumnIndex = columns.Length - 1;
                    for (var x = 0; x < lastColumnIndex; x++)
                        newValues[x] = values[x];

                    var array = new string[arrayLength];
                    for (int x = 0, y = lastColumnIndex; y < values.Length; x++, y++)
                        array[x] = values[y];

                    newValues[columns.Length - 1] = JsonConvert.SerializeObject(array);

                    values = newValues;
                }

                if (values.Length != columns.Length)
                    throw new ApplicationError("Unexpected columns count: " + lines[i]);

                var row = table.NewRow();

                if (hasIncludeColumns)
                {
                    foreach (var map in columnsMapping)
                        row[map.Item1] = values[map.Item2];
                }
                else
                {
                    for (var j = 0; j < values.Length; j++)
                        row[j] = values[j];
                }

                table.Rows.Add(row);
            }

            return table;
        }

        private static string[] ParseLxrMergeLine(string line)
        {
            var result = new List<string>();
            var multilineState = 0;
            var buffer = new StringBuilder();

            for (var j = 0; j <= line.Length; j++)
            {
                if (j == line.Length)
                {
                    result.Add(buffer.ToString());
                    buffer.Clear();
                    break;
                }

                var ch = line[j];

                if (ch == '\t' && (multilineState == 0 || multilineState == 2))
                {
                    result.Add(buffer.ToString());
                    buffer.Clear();
                    multilineState = 0;
                }
                else if (ch == '`')
                {
                    multilineState++;

                    if (multilineState > 2)
                        throw new ApplicationError("Only one multiline closing symbol is allowed: " + line);
                }
                else
                {
                    if (multilineState == 2)
                        throw new ApplicationError("A text after multiline closing symbol is not allowed: " + line);

                    buffer.Append(ch);
                }
            }

            if (multilineState != 0)
                throw new ApplicationError("Invalid line: " + line);

            return result.ToArray();
        }
    }
}
