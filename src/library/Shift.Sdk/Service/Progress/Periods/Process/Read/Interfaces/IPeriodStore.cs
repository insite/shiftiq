using System;

using Shift.Common.Timeline.Changes;

using InSite.Domain.Records;

namespace InSite.Application.Records.Read
{
    public interface IPeriodStore
    {
        void InsertPeriod(PeriodCreated e);
        void DeletePeriod(PeriodDeleted e);
        void UpdatePeriod(IChange e, Action<QPeriod> change);
    }
}
