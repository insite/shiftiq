using System;

namespace Shift.Contract
{
    public partial class BankModel
    {
        public Guid BankIdentifier { get; set; }
        public Guid? DepartmentIdentifier { get; set; }
        public Guid? FrameworkIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public bool IsActive { get; set; }

        public string BankEdition { get; set; }
        public string BankLevel { get; set; }
        public string BankName { get; set; }
        public string BankStatus { get; set; }
        public string BankTitle { get; set; }
        public string BankType { get; set; }
        public string LastChangeType { get; set; }
        public string LastChangeUser { get; set; }

        public int AssetNumber { get; set; }
        public int AttachmentCount { get; set; }
        public int BankSize { get; set; }
        public int CommentCount { get; set; }
        public int FormCount { get; set; }
        public int OptionCount { get; set; }
        public int QuestionCount { get; set; }
        public int SetCount { get; set; }
        public int SpecCount { get; set; }

        public DateTimeOffset LastChangeTime { get; set; }
    }
}