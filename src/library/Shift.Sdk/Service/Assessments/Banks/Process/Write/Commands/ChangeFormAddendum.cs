using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Banks;

namespace InSite.Application.Banks.Write
{
    public class ChangeFormAddendum : Command
    {
        public Guid Form { get; set; }
        public FormAddendumItem[] Acronyms { get; set; }
        public FormAddendumItem[] Formulas { get; set; }
        public FormAddendumItem[] Figures { get; set; }
        public bool RemoveObsolete { get; set; }

        public ChangeFormAddendum(Guid bank, Guid form, FormAddendumItem[] acronyms, FormAddendumItem[] formulas, FormAddendumItem[] figures)
        {
            AggregateIdentifier = bank;
            Form = form;
            Acronyms = acronyms;
            Formulas = formulas;
            Figures = figures;
        }
    }
}
