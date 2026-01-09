using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class TSenderFilter : Filter
    {
        public string SenderType { get; set; }
        public string SenderNickname { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string SystemMailbox { get; set; }
        public bool? SenderEnabled { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyPostalCode { get; set; }
        public string CompanyCountry { get; set; }
    }
}
