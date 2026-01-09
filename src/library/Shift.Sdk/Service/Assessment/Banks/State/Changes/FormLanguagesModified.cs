using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class FormLanguagesModified : Change
    {
        public Guid Form { get; set; }
        public string[] Languages { get; set; }

        public FormLanguagesModified(Guid form, string[] languages)
        {
            Form = form;
            Languages = languages;
        }
    }
}
