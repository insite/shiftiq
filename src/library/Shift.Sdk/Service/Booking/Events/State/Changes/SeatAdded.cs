using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class SeatAdded : Change
    {
        public Guid Seat { get; }
        public string Configuration { get; }
        public string Content { get; }
        public bool IsAvailable { get; set; }
        public bool IsTaxable { get; set; }
        public int? OrderSequence { get; }
        public string Title { get; }

        public SeatAdded(Guid seat, string configuration, string content, bool isAvailable, bool isTaxable, int? orderSequence, string title)
        {
            Seat = seat;
            Configuration = configuration;
            Content = content;
            IsAvailable = isAvailable;
            IsTaxable = isTaxable;
            OrderSequence = orderSequence;
            Title = title;
        }
    }
}
