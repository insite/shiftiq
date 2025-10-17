using System.Collections.Generic;

namespace Shift.Sdk.UI
{
    public class MailingListLabel
    {
        public int RowNumber { get; set; }

        public string Col1_FirstName { get; set; }
        public string Col1_LastName { get; set; }
        public string Col1_Street1 { get; set; }
        public string Col1_Street2 { get; set; }
        public string Col1_City { get; set; }
        public string Col1_Province { get; set; }
        public string Col1_PostalCode { get; set; }

        public string Col2_FirstName { get; set; }
        public string Col2_LastName { get; set; }
        public string Col2_Street1 { get; set; }
        public string Col2_Street2 { get; set; }
        public string Col2_City { get; set; }
        public string Col2_Province { get; set; }
        public string Col2_PostalCode { get; set; }

        public string Col3_FirstName { get; set; }
        public string Col3_LastName { get; set; }
        public string Col3_Street1 { get; set; }
        public string Col3_Street2 { get; set; }
        public string Col3_City { get; set; }
        public string Col3_Province { get; set; }
        public string Col3_PostalCode { get; set; }

        public static List<MailingListLabel> GetLabels()
        {
            return null;
        }
    }
}