using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

namespace Shift.Common.File
{
    public class AttemptUploadFileParser
    {
        public delegate AttemptUploadFileLine[] ParseFunc(Stream stream, int typeId, Encoding encoding);

        public static readonly AttemptUploadFileType CsvType_CodeNameQuestions = new AttemptUploadFileType(1, "Code, Name, Q1 ... QN");
        public static readonly AttemptUploadFileType CsvType_CodeDateNameQuestions = new AttemptUploadFileType(2, "Code, Date, Name, Q1 ... QN");

        public static readonly AttemptUploadFileFormat FileFormatCsv = new AttemptUploadFileFormat(
            "csv",
            "Shift iQ Comma Separated Values (*.csv)",
            "Upload Shift iQ CSV",
            "Shift iQ CSV Format Type",
            new[]
            {
                CsvType_CodeNameQuestions,
                CsvType_CodeDateNameQuestions
            },
            new[] { ".csv" },
            ParseCsv);

        public static readonly AttemptUploadFileFormat FileFormatScantron = new AttemptUploadFileFormat(
            "scantron",
            "Scantron Answer Sheet - Template 994 (*.txt)",
            "Upload Scantron Answer Sheet",
            null,
            null,
            new[] { ".txt" },
            ParseScantron);

        public static readonly AttemptUploadFileFormat FileFormatLxrMerge = new AttemptUploadFileFormat(
            "lxr",
            "LXR Merge (*.LXRMerge)",
            "Upload LXR Merge",
            null,
            null,
            new[] { ".lxrmerge" },
            ParseLxrMerge);

        public static readonly AttemptUploadFileFormat[] FileFormats = new[]
        {
            FileFormatCsv,
            FileFormatScantron,
            FileFormatLxrMerge,
        };

        public static AttemptUploadFileLine[] ParseCsv(Stream stream, int typeId, Encoding encoding)
        {
            var values = CsvImportHelper.GetValues(stream, null, false, encoding).Skip(1).Where(x => x.Length >= 2);

            if (typeId == CsvType_CodeDateNameQuestions.ID)
            {
                return values.Select((x, i) =>
                {
                    var dateValue = (x[1] ?? string.Empty).Trim();

                    return new AttemptUploadFileLine
                    {
                        LearnerCode = x[0],
                        AttemptDate = dateValue.Length == 0
                            ? (DateTime?)null
                            : DateTime.TryParseExact(dateValue, "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
                                ? date
                                : throw ApplicationError.Create("Line {0}, unexpected date format: {1}. Ensure the date format is DD/MM/YYYY.", i + 1, dateValue),
                        LearnerName = x[2],
                        AttemptAnswers = x.Skip(3).ToArray()
                    };
                }).ToArray();
            }
            else if (typeId == CsvType_CodeNameQuestions.ID)
            {
                return values.Select((x, i) => new AttemptUploadFileLine
                {
                    LearnerCode = x[0],
                    LearnerName = x[1],
                    AttemptAnswers = x.Skip(2).ToArray()
                }).ToArray();
            }
            else
            {
                throw new Exception($"Unexpected type ID: {typeId}");
            }
        }

        public static AttemptUploadFileLine[] ParseScantron(Stream stream, int typeId, Encoding encoding)
        {
            return ScantronFileItem.Enumerate(stream, encoding).Select((x, i) => new AttemptUploadFileLine
            {
                LearnerCode = x.Code,
                LearnerName = x.Name,
                AttemptAnswers = x.Responses
            }).ToArray();
        }

        public static AttemptUploadFileLine[] ParseLxrMerge(Stream stream, int typeId, Encoding encoding)
        {
            var includeColumns = new string[] { "SID", "SNAME", "C1", "C2", "C3", "RESPONSES" };

            var table = LxrMergeFile.ToDataTable(stream, new LxrMergeFile.Options
            {
                IncludeColumns = includeColumns,
                RequiredColumns = includeColumns,
                LastColumnIsArray = true,
                Encoding = encoding
            });

            return table.Rows.Cast<DataRow>().Select(row => new AttemptUploadFileLine
            {
                LearnerCode = (string)row["SID"],
                LearnerName = (string)row["SNAME"],
                AttemptDate = GetAttemptDate(row["C1"] as string, row["C2"] as string),
                Instructor = row["C3"] as string,
                AttemptAnswers = GetResponses((string)row["RESPONSES"])
            }).ToArray();

            DateTime? GetAttemptDate(string c1, string c2)
            {
                if (string.IsNullOrEmpty(c1) || string.IsNullOrEmpty(c2) || c2.Length != 3 && c2.Length != 4)
                    return null;

                if (!int.TryParse(c1, out var year) || year < DateTime.MinValue.Year || year > DateTime.MaxValue.Year)
                    return null;

                if (!int.TryParse(c2.Substring(0, c2.Length - 2), out var month) || month < 1 || month > 12)
                    return null;

                if (!int.TryParse(c2.Substring(c2.Length - 2, 2), out var day) || day < 1 || day > DateTime.DaysInMonth(year, month))
                    return null;

                return new DateTime(year, month, day);
            }

            string[] GetResponses(string json)
            {
                var responses = JsonConvert.DeserializeObject<string[]>(json);

                return responses.Take(responses.Length - 1).ToArray();
            }
        }
    }
}
