using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Messages
{
    [Obsolete]
    public class LinkClicked : Change
    {
        public Guid LinkIdentifier { get; set; }
        public string UserBrowser { get; set; }
        public string UserHostAddress { get; set; }
        public Guid UserIdentifier { get; set; }
    }
}