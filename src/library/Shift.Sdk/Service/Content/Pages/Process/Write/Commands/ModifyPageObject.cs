using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Pages.Write
{
    public class ModifyPageObject : Command
    {
        public string Type { get; set; }
        public Guid? Object { get; set; }

        public ModifyPageObject(Guid page, string type, Guid? @object)
        {
            AggregateIdentifier = page;
            Type = type;
            Object = @object;
        }
    }
}
