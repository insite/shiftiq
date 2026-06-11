using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class ArchiveForm : Command
    {
        /// <summary>
        /// Uniquely identifies the form to be archived.
        /// </summary>
        public Guid Form { get; set; }

        /// <summary>
        /// True if the questions contained in the form (in Fields in Sections) must be archived with the form.
        /// </summary>
        public bool Questions { get; set; }

        /// <summary>
        /// True if the attachments referenced by archived questions must be archived with the form.
        /// </summary>
        public bool Attachments { get; set; }

        public ArchiveForm(Guid bank, Guid form, bool questions, bool attachments)
        {
            AggregateIdentifier = bank;
            Form = form;
            Questions = questions;
            Attachments = attachments;
        }
    }
}