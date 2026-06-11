using System;

using InSite.Application.Banks.Read;

namespace InSite.Application.Events.Read
{
    public class QEventAssessmentForm
    {
        public Guid EventIdentifier { get; set; }
        public Guid BankIdentifier { get; set; }
        public Guid FormIdentifier { get; set; }

        public virtual QBankForm Form { get; set; }

        public int FormAsset => Form?.FormAsset ?? 0;
        public int FormAssetVersion => Form?.FormAssetVersion ?? 0;
        public string FormCode => Form?.FormCode;
        public string FormName => Form?.FormName;
        public string FormTitle => Form?.FormTitle;
        public string FormType => Form?.FormType;
    }
}
