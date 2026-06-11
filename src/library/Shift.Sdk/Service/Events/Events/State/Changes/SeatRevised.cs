using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class SeatRevised : Change
    {
        public Guid Seat { get; set; }
        public string Configuration { get; }
        public string Content { get; }
        public bool IsAvailable { get; }
        public bool IsTaxable { get; }
        public int? OrderSequence { get; }
        public string Title { get; }

        public SeatRevised(Guid seat, string configuration, string content, bool isAvailable, bool isTaxable, int? orderSequence, string title)
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
