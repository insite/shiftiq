using System.IO;
using System.Xml.Linq;

namespace Shift.Toolbox.Integration.DirectAccess
{
    public class ExamScheduleRequestInput
    {
        public ExamScheduleRequestInput(string individual)
        {
            IndividualId = individual;
        }

        public string IndividualId { get; set; }

        public string Raw { get; set; }

        public static ExamScheduleRequestInput Deserialize(string xml)
        {
            XElement root;

            using (var reader = new StringReader(xml))
                root = XElement.Load(reader);

            return new ExamScheduleRequestInput(root.Element("query")?.Element("id")?.Value)
            {
                Raw = xml
            };
        }

        public string Serialize()
        {
            return "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                new XElement("awis",
                    new XElement("query",
                        new XElement("id", IndividualId)
                    )
                ).ToString();
        }

        public bool Equals(string xml)
        {
            return string.Equals(IndividualId, Deserialize(xml).IndividualId);
        }
    }
}