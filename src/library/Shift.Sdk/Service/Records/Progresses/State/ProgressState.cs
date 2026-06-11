using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class ProgressState : AggregateState
    {
        public Guid Identifier { get; set; }
        public Guid Record { get; set; }
        public Guid User { get; set; }
        public Guid Item { get; set; }
        public decimal? Percent { get; set; }
        public string Text { get; set; }
        public decimal? Number { get; set; }
        public decimal? Points { get; set; }
        public decimal? MaxPoints { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset? Graded { get; set; }
        public bool IsPublished { get; set; }
        public string Status { get; set; }

        public void When(ProgressAdded e)
        {
            Identifier = e.AggregateIdentifier;
            Record = e.Record;
            User = e.User;
            Item = e.Item;
        }

        public void When(ProgressCommentChanged e)
        {
            Comment = e.Comment;
        }

        public void When(ProgressNumberChanged e)
        {
            Number = e.Number;
            Graded = e.Graded;
        }

        public void When(ProgressPercentChanged e)
        {
            Percent = e.Percent;
            Graded = e.Graded;
        }

        public void When(ProgressPointsChanged e)
        {
            Points = e.Points;
            MaxPoints = e.MaxPoints;
            Graded = e.Graded;
        }

        public void When(ProgressTextChanged e)
        {
            Text = e.Text;
            Graded = e.Graded;
        }

        public void When(ProgressCompleted2 e)
        {
            Percent = e.Percent;
            Graded = e.Completed;
            Status = ProgressCompleted2.Status;
        }

        public void When(ProgressHidden _)
        {

        }

        public void When(ProgressLocked _)
        {

        }

        public void When(ProgressPublished _)
        {
            IsPublished = true;
        }

        public void When(ProgressDeleted _)
        {
            Status = ProgressDeleted.Status;
        }

        public void When(ProgressShowed _)
        {

        }

        public void When(ProgressStarted _)
        {
            Percent = null;
            Graded = null;
            Status = ProgressStarted.Status;
        }

        public void When(ProgressUnlocked _)
        {

        }

        public void When(ProgressIncompleted _)
        {
            Percent = null;
            Graded = null;
            Status = ProgressIncompleted.Status;
        }

        public void When(ProgressIgnored _)
        {

        }

        public void When(SerializedChange e)
        {
            // Obsolete changes go here

            if (e.ChangeType != ProgressCompleted2.ObsoleteChangeType)
                return;

            var v2 = ProgressCompleted2.Upgrade(e);

            When(v2);
        }
    }
}
