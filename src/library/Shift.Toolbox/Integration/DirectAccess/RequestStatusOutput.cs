using System.IO;
using System.Xml.Linq;

namespace Shift.Toolbox.Integration.DirectAccess
{
    public class RequestStatusOutput
    {
        public string ReceiptId { get; set; }

        public string Status { get; set; }

        public int ProcessingTime { get; set; }

        public string SessionId { get; set; }

        public string IndividualId { get; set; }

        public string ExamId { get; set; }


        public bool Error { get; set; }

        public string Raw { get; set; }

        public static RequestStatusOutput Deserialize(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                return null;

            XElement root;

            using (var reader = new StringReader(xml))
                root = XElement.Load(reader);

            return new RequestStatusOutput
            {
                ReceiptId = root.Attribute("receiptid")?.Value,
                Status = root.Attribute("status")?.Value,
                ProcessingTime = int.Parse(root.Attribute("processingtime")?.Value),
                SessionId = root.Element("examdata")?.Attribute("sessionid")?.Value,
                IndividualId = root.Element("examdata")?.Element("exam")?.Attribute("individualid")?.Value,
                ExamId = root.Element("examdata")?.Element("exam")?.Attribute("examid")?.Value,
                Error = root.Element("examdata")?.Element("exam")?.Attribute("error")?.Value == "Y",
                Raw = xml
            };
        }
    }
}