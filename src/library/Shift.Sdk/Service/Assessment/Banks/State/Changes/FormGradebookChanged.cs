using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class FormGradebookChanged : Change
    {
        public Guid Form { get; set; }
        public Guid? Gradebook { get; set; }

        public FormGradebookChanged(Guid form, Guid? gradebook)
        {
            Form = form;
            Gradebook = gradebook;
        }
    }
}
