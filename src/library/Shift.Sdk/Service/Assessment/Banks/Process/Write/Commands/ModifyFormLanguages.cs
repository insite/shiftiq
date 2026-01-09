using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class ModifyFormLanguages : Command
    {
        public Guid Form { get; set; }
        public string[] Languages { get; set; }

        public ModifyFormLanguages(Guid bank, Guid form, string[] languages)
        {
            AggregateIdentifier = bank;
            Form = form;
            Languages = languages;
        }
    }
}
