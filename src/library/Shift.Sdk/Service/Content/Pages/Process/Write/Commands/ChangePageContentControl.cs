using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Pages.Write
{
    public class ChangePageContentControl : Command
    {
        public string ContentControl { get; set; }
        public ChangePageContentControl(Guid page, string contentControl)
        {
            AggregateIdentifier = page;
            ContentControl = contentControl;
        }
    }
}
