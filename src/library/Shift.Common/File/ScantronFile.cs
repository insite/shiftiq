using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Shift.Common.File
{
    internal class ScantronFileItem
    {
        #region Fields

        private static readonly Regex LinePattern = new Regex(
            @"^(?<Reference>.{40})(?<Code>[\w\d\s]{9})(?<Date>.{10})(?<Name>[\w\d\s]{23})(?<Responses>[\w\s]+)$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        #endregion

        #region Properties

        public string Reference { get; private set; }
        public string Code { get; private set; }
        public string Date { get; private set; }
        public string Name { get; private set; }
        public string[] Responses { get; private set; }

        #endregion

        public static IEnumerable<ScantronFileItem> Enumerate(Stream stream, Encoding encoding)
        {
            var lineNumber = 1;

            using (var reader = new StreamReader(stream, encoding))
            {
                while (true)
                {
                    var line = reader.ReadLine();
                    if (line == null)
                        break;

                    var match = LinePattern.Match(line);
                    if (!match.Success)
                        throw ApplicationError.Create($"Line {lineNumber + 1} doesn't match expected format: {line}");

                    yield return new ScantronFileItem
                    {
                        Reference = match.Groups["Reference"].Value.Trim(),
                        Code = match.Groups["Code"].Value.Trim(),
                        Date = match.Groups["Date"].Value.Trim(),
                        Name = match.Groups["Name"].Value.Trim(),
                        Responses = match.Groups["Responses"].Value.Trim().Select(x => x.ToString()).ToArray()
                    };

                    lineNumber++;
                }
            }
        }
    }
}