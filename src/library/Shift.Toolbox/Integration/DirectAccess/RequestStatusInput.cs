using System.IO;
using System.Xml.Linq;

namespace Shift.Toolbox.Integration.DirectAccess
{
    public class RequestStatusInput
    {
        public string ReceiptId { get; set; }

        public string Raw { get; set; }

        public static RequestStatusInput Deserialize(string xml)
        {
            XElement root;

            using (var reader = new StringReader(xml))
                root = XElement.Load(reader);

            return new RequestStatusInput
            {
                ReceiptId = root.Attribute("receiptid")?.Value,
                Raw = xml
            };
        }

        public string Serialize()
        {
            return "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                new XElement("awis", new XAttribute("receiptid", ReceiptId)).ToString();
        }

        public bool Equals(string xml)
        {
            return string.Equals(ReceiptId, Deserialize(xml).ReceiptId);
        }
    }
}