using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace Shift.Toolbox.Integration.DirectAccess
{
    public class CodeAndDate
    {
        public CodeAndDate()
        {

        }

        public CodeAndDate(string code, DateTime date)
        {
            Code = code;
            Date = date;
        }

        public string Code { get; set; }

        public DateTime Date { get; set; }

        public static CodeAndDate[] DeserializeArray(XElement collectionRoot) =>
            collectionRoot?.Elements("exam").Select(x => DeserializeExamCodeAndDate(x)).ToArray();

        public static CodeAndDate DeserializeExamCodeAndDate(XElement modelRoot) => new CodeAndDate
        {
            Code = modelRoot.Attribute("examid")?.Value,
            Date = DateTime.TryParseExact(modelRoot.Attribute("examdate")?.Value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var examdate) ? examdate : examdate,
        };
    }
}