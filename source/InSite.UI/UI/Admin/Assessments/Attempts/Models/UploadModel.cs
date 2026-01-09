using System;

using InSite.Application.Events.Read;

namespace InSite.Admin.Assessments.Attempts.Forms
{
    [Serializable]
    public class UploadModel
    {
        #region Properties

        public Guid? EventIdentifier { get; set; }
        public string EventTitle { get; set; }
        public DateTimeOffset? EventScheduled { get; }

        public Guid? BankIdentifier { get; set; }
        public Guid? FormIdentifier { get; set; }
        public string FormTitle { get; set; }

        #endregion

        #region Construction

        public UploadModel()
        {
        }

        public UploadModel(QEvent entity)
            : this()
        {
            if (entity == null)
                return;

            EventIdentifier = entity.EventIdentifier;
            EventTitle = entity.EventTitle;
            EventScheduled = entity.EventScheduledStart;
        }

        #endregion

        #region Methods (helpers)

        public UploadModel Clone()
        {
            return (UploadModel)MemberwiseClone();
        }

        #endregion
    }

    public class UploadModelSummaryItem
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }
}