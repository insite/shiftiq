using System;

using Shift.Common;

namespace InSite.Domain.Banks
{
    /// <summary>
    /// Forms can be classified (optionally) by instrument and/or theme.
    /// </summary>
    [Serializable]
    public class FormClassification
    {
        public string Instrument { get; set; }
        public string Theme { get; set; }

        public FormClassification Clone()
        {
            var clone = new FormClassification();

            this.ShallowCopyTo(clone);

            return clone;
        }
    }
}
