using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyFormCreated : Change
    {
        public SurveyFormCreated(string source, Guid tenant, int asset, string name, SurveyFormStatus status, string language)
        {
            Source = source;
            Tenant = tenant;

            Asset = asset;
            Name = name;
            Status = status;
            Language = language;
        }

        public string Source { get; set; }
        public Guid Tenant { get; set; }

        public int Asset { get; }
        public string Name { get; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public SurveyFormStatus Status { get; }

        public string Language { get; }
    }

    public class SurveyFormAssetChanged : Change
    {
        public SurveyFormAssetChanged(int asset)
        {
            Asset = asset;
        }

        public int Asset { get; }
    }
}