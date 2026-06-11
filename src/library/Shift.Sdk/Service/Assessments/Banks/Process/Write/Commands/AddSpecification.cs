using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Banks;

using Shift.Constant;

namespace InSite.Application.Banks.Write
{
    public class AddSpecification : Command
    {
        public SpecificationType Type { get; set; }
        public ConsequenceType Consequence { get; set; }

        public Guid Identifier { get; set; }
        public string Name { get; set; }
        public int Asset { get; set; }
        public int FormLimit { get; set; }
        public int QuestionLimit { get; set; }

        public ScoreCalculation Calculation { get; set; }

        public AddSpecification(Guid bank, SpecificationType type, ConsequenceType consequence, Guid identifier, string name, int asset, int formLimit, int questionLimit, ScoreCalculation calculation)
        {
            AggregateIdentifier = bank;
            
            Type = type;
            Consequence = consequence;
            Identifier = identifier;
            Name = name;
            Asset = asset;
            FormLimit = formLimit;
            QuestionLimit = questionLimit;
            Calculation = calculation;
        }
    }
}