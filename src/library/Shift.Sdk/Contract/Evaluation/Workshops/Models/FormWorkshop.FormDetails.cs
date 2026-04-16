using System;

using Shift.Constant;

namespace Shift.Contract
{
    public partial class FormWorkshop
    {
        public class FormDetails
        {
            public string SpecificationName { get; set; }
            public SpecificationType SpecificationType { get; set; }
            public WorkshopStandard Standard { get; set; }
            public string FormName { get; set; }
            public int FormAssetNumber { get; set; }
            public int FormAssetVersion { get; set; }
            public string FormCode { get; set; }
            public string FormSource { get; set; }
            public string FormOrigin { get; set; }
            public string FormHook { get; set; }
            public string PublicationStatus { get; set; }
            public bool ThirdPartyAssessmentIsEnabled { get; set; }
            public DateTimeOffset? StaticQuestionOrderVerified { get; set; }
            public StaticQuestionOrder[] VerifiedQuestions { get; set; }
            public bool IsQuestionOrderMatch { get; set; }
        }
    }
}
