using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Shift.Toolbox.Integration.DirectAccess
{
    public class ExamScheduleRequestOutput
    {
        public CodeAndDate[] Exams { get; set; }

        public string Raw { get; set; }
        
        public static ExamScheduleRequestOutput Deserialize(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                return null;

            XElement root;

            try
            {
                using (var reader = new StringReader(xml))
                    root = XElement.Load(reader);
            }
            catch (XmlException)
            {
                return new ExamScheduleRequestOutput { Raw = xml, Exams = new CodeAndDate[0] };
            }

            return new ExamScheduleRequestOutput
            {
                Exams = CodeAndDate.DeserializeArray(root.Element("response").Element("exams")),
                Raw = xml
            };
        }
    }
}