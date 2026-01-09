using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class FormAssetChanged : Change
    {
        public Guid Form { get; set; }
        public int Asset { get; set; }

        public FormAssetChanged(Guid form, int asset)
        {
            Form = form;
            Asset = asset;
        }
    }
}
