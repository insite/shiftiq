using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class UnarchiveForm : Command
    {
        /// <summary>
        /// Uniquely identifies the form to be unarchived.
        /// </summary>
        public Guid Form { get; set; }

        /// <summary>
        /// True if the questions contained in the form (in Fields in Sections) must be unarchived with the form.
        /// </summary>
        public bool Questions { get; set; }

        /// <summary>
        /// True if the attachments referenced by unarchived questions must be unarchived with the form.
        /// </summary>
        public bool Attachments { get; set; }

        public UnarchiveForm(Guid bank, Guid form, bool questions, bool attachments)
        {
            AggregateIdentifier = bank;
            Form = form;
            Questions = questions;
            Attachments = attachments;
        }
    }
}