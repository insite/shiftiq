using System;
using System.Linq;

using Shift.Common;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class SurveySettings
    {
        public bool EnableUserConfidentiality { get; set; }
        public bool LockUserConfidentiality { get; set; }
        public Guid[] ResponseUploadClaimGroups { get; set; }

        public bool IsEqual(SurveySettings other)
        {
            var isEqual = EnableUserConfidentiality == other.EnableUserConfidentiality
                && LockUserConfidentiality == other.LockUserConfidentiality;
            if (!isEqual)
                return false;

            var groups1 = ResponseUploadClaimGroups.EmptyIfNull();
            var groups2 = other.ResponseUploadClaimGroups.EmptyIfNull();
            return groups1.Length == groups2.Length
                && groups1.Zip(groups2, (a, b) => a == b).All(x => x);
        }
    }
}
