using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Contacts
{
    public class GroupSurveyChanged : Change
    {
        public Guid? Survey { get; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public Necessity Necessity { get; }

        public GroupSurveyChanged(Guid? survey, Necessity necessity)
        {
            Survey = survey;
            Necessity = necessity;
        }
    }
}
