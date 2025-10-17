using System;
using System.Collections.Generic;

namespace InSite.Application.Banks.Read
{
    public class QBank
    {
        public Guid BankIdentifier { get; set; }
        public Guid? DepartmentIdentifier { get; set; }
        public Guid? FrameworkIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public string BankEdition { get; set; }
        public string BankLevel { get; set; }
        public string BankName { get; set; }
        public string BankStatus { get; set; }
        public string BankTitle { get; set; }
        public string BankType { get; set; }

        public bool IsActive { get; set; }

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
        public string LastChangeType { get; set; }
        public string LastChangeUser { get; set; }

        public virtual ICollection<QBankForm> Forms { get; set; }
        public virtual ICollection<QBankQuestion> Questions { get; set; }
        public virtual ICollection<QBankSpecification> Specifications { get; set; }

        public QBank()
        {
            Forms = new HashSet<QBankForm>();
            Questions = new HashSet<QBankQuestion>();
            Specifications = new HashSet<QBankSpecification>();
        }
    }
}
