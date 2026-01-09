using System;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class CompanyDescription
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public CompanySize CompanySize { get; set; }

        public string LegalName { get; set; }
        public string CompanySummary { get; set; }

        public bool IsEqual(CompanyDescription other)
        {
            return CompanySize == other.CompanySize
                && LegalName.NullIfEmpty() == other.LegalName.NullIfEmpty()
                && CompanySummary.NullIfEmpty() == other.CompanySummary.NullIfEmpty();
        }

        public CompanyDescription Clone()
        {
            return new CompanyDescription
            {
                CompanySize = CompanySize,
                LegalName = LegalName.NullIfEmpty(),
                CompanySummary = CompanySummary.NullIfEmpty()
            };
        }
    }
}