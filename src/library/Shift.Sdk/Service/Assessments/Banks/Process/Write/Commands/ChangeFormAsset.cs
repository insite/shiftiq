using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class ChangeFormAsset : Command
    {
        public Guid Form { get; set; }
        public int Asset { get; set; }

        public ChangeFormAsset(Guid bank, Guid form, int asset)
        {
            AggregateIdentifier = bank;
            Form = form;
            Asset = asset;
        }
    }
}
