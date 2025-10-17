using System;

namespace Shift.Sdk.UI
{
    public class HistoryEntity
    {
        public DateTime Date { get; }
        public string UserName { get; }
        public string Description { get; set; }

        public HistoryEntity(DateTime date, string userName)
        {
            Date = date;
            UserName = userName;
        }

        public HistoryEntity(DateTime date, string userName, string description)
            : this(date, userName)
        {
            Description = description;
        }

        public virtual void MergeWith(HistoryEntity source)
        {
            Description = MergeDescription(Description, source.Description);
        }

        protected static string MergeDescription(string value1, string value2)
        {
            return value1.EndsWith(Environment.NewLine)
                ? value1 + value2
                : value1 + Environment.NewLine + value2;
        }

        public virtual string BuildDescription() 
        { 
            return Description;
        }
    }
}