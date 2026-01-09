using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Shift.Common
{
    public class CsvHelper
    {
        public DataTable Table { get; private set; }
        public List<string> Errors { get; set; }

        public CsvHelper()
        {
            Errors = new List<string>();
        }

        /// <summary>
        /// Takes DataTable table and String ignoreColumnName as arguments. 
        /// First, appends column names to a string builder, ignoring specified columns
        /// afterward, appends remaining rows and columns to the string builder
        /// </summary>

        public static string ConvertTableToCsv(DataTable table, string ignoreColumnName)
        {
            var csv = new StringBuilder();

            for (var j = 0; j < table.Columns.Count; j++)
            {
                var column = table.Columns[j];

                if (string.Compare(ignoreColumnName, column.ColumnName, StringComparison.InvariantCultureIgnoreCase) == 0)
                    continue;

                csv.Append(column.ColumnName);
                if (j < table.Columns.Count - 1)
                    csv.Append(",");
            }
            csv.AppendLine();

            for (var i = 0; i < table.Rows.Count; i++)
            {
                var row = table.Rows[i];
                for (var j = 0; j < table.Columns.Count; j++)
                {
                    var column = table.Columns[j];

                    if (string.Compare(ignoreColumnName, column.ColumnName, StringComparison.InvariantCultureIgnoreCase) == 0)
                        continue;

                    csv.Append(Delimit(GetValue(row, column.ColumnName)));
                    if (j < table.Columns.Count - 1)
                        csv.Append(",");
                }
                csv.AppendLine();
            }

            return csv.ToString();
        }

        public void Read(string path, Encoding encoding)
        {
            if (!ValidateFile(path))
                return;

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                Read(stream, encoding, path);
                stream.Close();
                stream.Dispose();
            }
        }

        private bool ValidateFile(string path)
        {
            Errors.Clear();

            if (!System.IO.File.Exists(path))
                Errors.Add($"File Not Found: {path}");

            return Errors.Count == 0;
        }

        public void Read(Stream stream, Encoding encoding, string path)
        {
            Read(stream, encoding, ',', 0, int.MaxValue, path);
        }

        public void Read(Stream stream, Encoding encoding, char chFieldSeparator, int fromLine, int thruLine, string path)
        {


            // Takes the path to a .csv file and appends entries to the public DataTable. Accounts for .csv formats


            var lineCount = 0;
            Table = new DataTable();
            using (TextReader reader = new StreamReader(stream, encoding))
            {
                // var csvLength = new FileInfo(Path.GetFileName(path)).Length;
                var csvLength = new FileInfo(path).Length;
                if (0 <= csvLength && 3 >= csvLength)
                {
                    Errors.Add($"File is Empty: {path}");
                    return;
                }

                Table.Columns.Add("Column001");

                string sLine;
                while ((sLine = reader.ReadLine()) != null)
                {
                    if (sLine.Length == 0)
                    {
                        if (lineCount == 0)
                        {
                            Errors.Add($"No Headings in File: {path}");
                        }

                        continue;
                    }

                    lineCount++;

                    if (lineCount > 1 && (lineCount < fromLine || lineCount > thruLine))
                        continue;

                    var row = Table.NewRow();
                    Table.Rows.Add(row);

                    var i = 0;
                    var nMode = 0;
                    var nField = 0;
                    var bContinueParsing = true;
                    while (bContinueParsing)
                    {
                        switch (nMode)
                        {
                            case 0: // Search for next entry. 
                                {
                                    if (chFieldSeparator == CsvControlChars.Tab)
                                    {
                                        // Don't skip the tab when it is used as a separator. 
                                        while (char.IsWhiteSpace(sLine[i]) && sLine[i] != CsvControlChars.Tab)
                                            i++;
                                    }
                                    else
                                    {
                                        while (char.IsWhiteSpace(sLine[i]))
                                            i++;
                                    }
                                    nMode = 1;
                                    break;
                                }
                            case 1: // Determine if field is quoted or unquoted. 
                                {
                                    // first check if field is empty. 
                                    var chPunctuation = sLine[i];
                                    if (chPunctuation == chFieldSeparator)
                                    {
                                        i++;
                                        nField++;
                                        if (nField > Table.Columns.Count)
                                            Table.Columns.Add("Column" + nField.ToString("000"));

                                        nMode = 0;

                                    }
                                    else if (chPunctuation == '\"')
                                    {
                                        i++;
                                        // Field is quoted, so start reading until next quote. 
                                        nMode = 3;
                                    }
                                    else
                                    {
                                        // Field is unquoted, so start reading until next separator or end-of-line.
                                        nMode = 2;
                                    }
                                    break;
                                }
                            case 2: // Extract unquoted field. 
                                {
                                    nField++;
                                    if (nField > Table.Columns.Count)
                                        Table.Columns.Add("Column" + nField.ToString("000"));

                                    var nFieldStart = i;
                                    // Field is unquoted, so start reading until next separator or end-of-line.
                                    while (i < sLine.Length && sLine[i] != chFieldSeparator)
                                        i++;
                                    var nFieldEnd = i;

                                    var sField = sLine.Substring(nFieldStart, nFieldEnd - nFieldStart);
                                    row[nField - 1] = sField;
                                    nMode = 0;
                                    i++;
                                    break;
                                }
                            case 3: // Extract quoted field. 
                                {
                                    nField++;
                                    if (nField > Table.Columns.Count)
                                        Table.Columns.Add("Column" + nField.ToString("000"));

                                    bool bMultiline;
                                    var sbField = new StringBuilder();
                                    do
                                    {
                                        var nFieldStart = i;
                                        // Field is quoted, so start reading until next quote.  Watch out for an escaped quote (two double quotes). 
                                        while ((i < sLine.Length && sLine[i] != '\"') || (i + 1 < sLine.Length && sLine[i] == '\"' && sLine[i + 1] == '\"'))
                                        {
                                            if (i + 1 < sLine.Length && sLine[i] == '\"' && sLine[i + 1] == '\"')
                                                i++;
                                            i++;
                                        }
                                        var nFieldEnd = i;
                                        if (sbField.Length > 0)
                                            sbField.Append(CsvControlChars.CrLf);
                                        sbField.Append(sLine.Substring(nFieldStart, nFieldEnd - nFieldStart));

                                        // 08/23/2006 Paul.  If we are at the end of the line, then it must be a multi-line string. 
                                        bMultiline = (i == sLine.Length);
                                        if (bMultiline)
                                        {
                                            sLine = reader.ReadLine();
                                            i = 0;
                                            if (sLine == null)
                                                break;
                                        }
                                    } while (bMultiline);

                                    if (sLine != null)
                                    {
                                        // Skip all characters until we reach the separator or end-of-line. 
                                        while (i < sLine.Length && sLine[i] != chFieldSeparator)
                                            i++;
                                    }

                                    var sField = sbField.ToString();
                                    sField = sField.Replace("\"\"", "\"");
                                    row[nField - 1] = sField;
                                    nMode = 0;
                                    i++;
                                    break;
                                }
                            default:
                                bContinueParsing = false;
                                break;
                        }
                        if (i >= sLine.Length)
                            break;
                    }
                }
            }
            AssignColumnNames();
            CheckTable(Table, path);
        }

        public void AssignColumnNames()
        {
            if (Table.Columns.Count <= 0 || Table.Rows.Count <= 1)
                return;

            var row = Table.Rows[0];
            for (var i = 0; i < Table.Columns.Count; i++)
            {
                var column = Table.Columns[i];
                var name = row[i];
                if (name != DBNull.Value)
                    column.ColumnName = (string)name;
            }
            row.Delete();
        }

        public void CheckTable(DataTable table, string path)
        {
            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn column in table.Columns)
                {
                    object value = row[column.ColumnName];
                    if (value == DBNull.Value)
                    {
                        Errors.Add($"File Contains Uneven Columns: {path}");
                        break;
                    }
                }
            }
        }

        internal static class CsvControlChars
        {
            internal const string DoubleQuote = "\"\"";
            internal const char Quote = '"';

            internal static string CrLf
            {
                get { return "\r\n"; }
            }

            internal static char Cr
            {
                get { return '\r'; }
            }

            internal static char Lf
            {
                get { return '\n'; }
            }

            internal static char Tab
            {
                get { return '\t'; }
            }
        }

        internal static string Delimit(String value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            if (value.Contains("\""))
                value = value.Replace("\"", "'");

            if (value.Contains(","))
                value = string.Format("\"{0}\"", value);

            return value;
        }

        internal static string GetValue(DataRow row, String name)
        {
            var o = row[name];
            if (o == DBNull.Value)
                return null;
            return o.ToString();
        }

        public static string SerializeCsv<T>(IEnumerable<T> items, string includes)
        {
            var csv = new CsvExportHelper(items);

            var properties = Reflector.GetSerializeProperties<T>(includes).Select(p => p.Name);

            foreach (var property in properties)
                csv.AddMapping(property, property);

            return csv.GetString();
        }
    }
}