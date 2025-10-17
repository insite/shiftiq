using System;

using Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class AssessmentSelectionStarted : Change
    {
        public Guid Form { get; set; }

        public AssessmentSelectionStarted(Guid form)
        {
            Form = form;
        }
    }
}
