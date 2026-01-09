using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Records
{
    public class ProgressAggregate : AggregateRoot
    {
        public override AggregateState CreateState() => new ProgressState();

        public ProgressState Data => (ProgressState)State;

        public void AddProgress(Guid record, Guid user, Guid item)
        {
            Apply(new ProgressAdded(record, user, item));
        }

        public void ChangeComment(string comment)
        {
            Apply(new ProgressCommentChanged(comment));
        }

        public void ChangeNumber(decimal? number, DateTimeOffset? graded)
        {
            Apply(new ProgressNumberChanged(number, graded));
        }

        public void ChangePercent(decimal? percent, DateTimeOffset? graded)
        {
            Apply(new ProgressPercentChanged(percent, graded));
        }

        public void ChangePoints(decimal? points, decimal? maxPoints, DateTimeOffset? graded)
        {
            Apply(new ProgressPointsChanged(points, maxPoints, graded));
        }

        public void ChangeText(string text, DateTimeOffset? graded)
        {
            Apply(new ProgressTextChanged(text, graded));
        }

        public void Complete(
            DateTimeOffset? completed,
            decimal? percent,
            bool? pass,
            int? elapsedMinutes,
            int? elapsedSeconds
            )
        {
            int? totalSeconds = null;

            if (elapsedMinutes.HasValue || elapsedSeconds.HasValue)
                totalSeconds = Number.CheckRange(elapsedMinutes ?? 0, 0) * 60 + Number.CheckRange(elapsedSeconds ?? 0, 0);

            Apply(new ProgressCompleted2(completed, percent, pass, totalSeconds));
        }

        public void Hide()
        {
            Apply(new ProgressHidden());
        }

        public void Lock()
        {
            Apply(new ProgressLocked());
        }

        public void Publish()
        {
            Apply(new ProgressPublished());
        }

        public void Delete()
        {
            Apply(new ProgressDeleted());
        }

        public void Show()
        {
            Apply(new ProgressShowed());
        }

        public void Start(DateTimeOffset started)
        {
            // There is no need to apply a ProgressStarted change if the the activity is already started or completed.
            if (Data.Status != ProgressStarted.Status && Data.Status != ProgressCompleted2.Status && Data.Status != ProgressIncompleted.Status)
                Apply(new ProgressStarted(started));
        }

        public void Unlock()
        {
            Apply(new ProgressUnlocked());
        }

        public void Incomplete()
        {
            if (Data.Status == ProgressStarted.Status)
                Apply(new ProgressIncompleted());
        }

        public void Ignore(bool isIgnored)
        {
            Apply(new ProgressIgnored(isIgnored));
        }
    }
}
