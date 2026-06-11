using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class FormArchived : Change
    {
        /// <summary>
        /// Uniquely identifies the archived form.
        /// </summary>
        public Guid Form { get; set; }

        /// <summary>
        /// True if the questions contained in the form (in Fields in Sections) are archived with the form.
        /// </summary>
        public bool Questions { get; set; }

        /// <summary>
        /// True if the attachments referenced by archived questions are also archived with the form.
        /// </summary>
        public bool Attachments { get; set; }

        public FormArchived(Guid form, bool questions, bool attachments)
        {
            Form = form;
            Questions = questions;
            Attachments = attachments;
        }
    }
}
