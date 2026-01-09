using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Banks;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Application.Banks.Write
{
    public class ChangeQuestionLikertRow : Command
    {
        public Guid Question { get; set; }
        public Guid Row { get; set; }
        public Guid Standard { get; set; }
        public Guid[] SubStandards { get; set; }
        public ContentTitle Content { get; set; }

        [JsonConstructor]
        private ChangeQuestionLikertRow()
        {

        }

        public ChangeQuestionLikertRow(Guid bank, Guid question, Guid row, Guid standard, Guid[] subStandards, ContentTitle content)
        {
            AggregateIdentifier = bank;
            Question = question;
            Row = row;
            Standard = standard;
            SubStandards = subStandards;
            Content = content;
        }

        public ChangeQuestionLikertRow(Guid bank, Guid question, LikertRow row)
            : this(bank, question, row.Identifier, row.Standard, row.SubStandards, row.Content)
        {

        }
    }
}
