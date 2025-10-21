using System;
using System.Linq;

using InSite.Application.Contacts.Read;
using InSite.Application.Events.Read;

using Shift.Common;

namespace InSite.Persistence.Plugin.SkilledTradesBC
{
    class EventProcessorHelper
    {
        public enum ITA026DayCount { None, Days14, Days60 }

        private readonly IGroupSearch _groupSearch;

        public EventProcessorHelper(IGroupSearch groupSearch)
        {
            _groupSearch = groupSearch;
        }

        public bool IsVenueARC(Guid? venueLocationId)
        {
            if (venueLocationId == null)
                return false;

            var arcCount = _groupSearch
                .GetParentConnections(venueLocationId.Value, x => x.ParentGroup)
                .Where(x => x.ParentGroup.GroupName == "ARC")
                .Count();

            return arcCount > 0;
        }

        public ITA026DayCount GetITA026DayCount(QEvent @event)
        {
            var type = @event.ExamType;
            var isClass = type == EventExamType.Class.Value;
            var isIndividualA = type == EventExamType.IndividualA.Value;
            var isIndividualN = type == EventExamType.IndividualN.Value;
            var isArc = type == EventExamType.Arc.Value;
            var isSitting = type == EventExamType.Sitting.Value;

            if (isClass || isIndividualA && IsVenueARC(@event.VenueLocationIdentifier) || isArc || isSitting)
                return ITA026DayCount.Days14;
            else if (isIndividualA || isIndividualN)
                return ITA026DayCount.Days60;

            return ITA026DayCount.None;
        }
    }
}
