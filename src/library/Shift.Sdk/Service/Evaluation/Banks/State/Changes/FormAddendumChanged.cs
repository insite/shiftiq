using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Banks
{
    public class FormAddendumChanged : Change
    {
        public Guid Form { get; set; }
        public FormAddendumItem[] Acronyms { get; set; }
        public FormAddendumItem[] Formulas { get; set; }
        public FormAddendumItem[] Figures { get; set; }

        public bool RemoveObsolete { get; set; }

        [JsonProperty]
        public int[] Addendum { get; private set; }

        public FormAddendumChanged(Guid form, FormAddendumItem[] acronyms, FormAddendumItem[] formulas, FormAddendumItem[] figures, bool removeObsolete)
        {
            Form = form;
            Acronyms = acronyms;
            Formulas = formulas;
            Figures = figures;
            RemoveObsolete = removeObsolete;
        }
    }
}
