using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class ReviseSeat : Command
    {
        public Guid Seat { get; }
        public string Configuration { get; }
        public string Content { get; }
        public bool IsAvailable { get; }
        public bool IsTaxable { get; }
        public int? OrderSequence { get; }
        public string Title { get; }

        public ReviseSeat(Guid @event, Guid seat, string configuration, string content, bool isAvailable, bool isTaxable, int? orderSequence, string title)
        {
            AggregateIdentifier = @event;
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
