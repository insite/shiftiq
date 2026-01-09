using System;

using Shift.Common.Timeline.Commands;

using Shift.Constant;

namespace InSite.Application.Groups.Write
{
    public class ChangeGroupSurvey : Command
    {
        public Guid? Survey { get; }
        public Necessity Necessity { get; }

        public ChangeGroupSurvey(Guid group, Guid? survey, Necessity necessity)
        {
            AggregateIdentifier = group;
            Survey = survey;
            Necessity = necessity;
        }
    }
}
