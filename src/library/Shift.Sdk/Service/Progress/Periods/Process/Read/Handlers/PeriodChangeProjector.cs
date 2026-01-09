using Shift.Common.Timeline.Changes;

using InSite.Domain.Records;

namespace InSite.Application.Records.Read
{
    /// <summary>
    /// Implements the projector for Program changes.
    /// </summary>
    /// <remarks>
    /// A projector is responsible for creating projections (i.e. records in query tables) based on changes. Changes can (and often should) be 
    /// replayed by a projector, and there should be no side effects (aside from changes to the projection tables). A processor, in contrast, should 
    /// *never* replay changes.
    /// </remarks>
    public class PeriodChangeProjector
    {
        private readonly IPeriodStore _store;

        public PeriodChangeProjector(IChangeQueue publisher, IPeriodStore store)
        {
            _store = store;

            publisher.Subscribe<PeriodCreated>(Handle);
            publisher.Subscribe<PeriodDeleted>(Handle);
            publisher.Subscribe<PeriodRenamed>(Handle);
            publisher.Subscribe<PeriodRescheduled>(Handle);
        }

        public void Handle(PeriodCreated e)
            => _store.InsertPeriod(e);

        public void Handle(PeriodDeleted e)
            => _store.DeletePeriod(e);

        public void Handle(PeriodRenamed e)
            => _store.UpdatePeriod(e, x => x.PeriodName = e.Name);

        public void Handle(PeriodRescheduled e)
        {
            _store.UpdatePeriod(e, x =>
            {
                x.PeriodStart = e.Start;
                x.PeriodEnd = e.End;
            });
        }
    }
}