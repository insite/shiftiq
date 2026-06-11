using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Banks;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Banks.Write
{
    public class AddQuestion2 : Command
    {
        public Guid Question { get; set; }
        public Guid Set { get; set; }
        public QuestionItemType Type { get; set; }
        public string Condition { get; set; }
        public int Asset { get; set; }
        public Guid Standard { get; set; }
        public Guid? Source { get; set; }
        public decimal? Points { get; set; }
        public QuestionCalculationMethod CalculationMethod { get; set; }
        public ContentExamQuestion Content { get; set; }


        public AddQuestion2(Guid bank, Guid set, Question question)
        {
            AggregateIdentifier = bank;
            Question = question.Identifier;
            Set = set;
            Type = question.Type;
            Condition = question.Condition;
            Asset = question.Asset;
            Standard = question.Standard;
            Source = question.Source;
            Points = question.Points;
            CalculationMethod = question.CalculationMethod;
            Content = question.Content;
        }
    }
}
