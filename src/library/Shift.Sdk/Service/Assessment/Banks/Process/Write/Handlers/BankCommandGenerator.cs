using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Banks;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Banks.Write
{
    public class BankCommandGenerator
    {
        /// <summary>
        /// Returns the list of commands to create a Question.
        /// </summary>
        public List<ICommand> GetCommands(Guid bank, Guid set, Question question)
        {
            var script = new List<ICommand>
            {
                new AddQuestion2(bank, set, question)
            };

            if (question.Randomization.Enabled)
                script.Add(new ChangeQuestionRandomization(bank, question.Identifier, question.Randomization));

            if (question.Layout.Type == OptionLayoutType.Table && question.Layout.Columns.IsNotEmpty())
                script.Add(new ChangeQuestionLayout(bank, question.Identifier, question.Layout));

            if (!question.Classification.IsEmpty)
                script.Add(new ChangeQuestionClassification(bank, question.Identifier, question.Classification));

            if (question.CutScore.HasValue)
                script.Add(new ChangeQuestionScoring(bank, question.Identifier, question.Points, question.CutScore, question.CalculationMethod));

            if (question.Flag != default)
                script.Add(new ChangeQuestionFlag(bank, question.Identifier, question.Flag));

            if (question.Type == QuestionItemType.Matching)
            {
                if (!question.Matches.IsEmpty)
                    script.Add(new ChangeQuestionMatches(bank, question.Identifier, question.Matches));
            }
            else if (question.Options != null)
            {
                foreach (var option in question.Options)
                    script.Add(new AddOption(bank, question.Identifier, option.Content, option.Points, option.IsTrue, option.CutScore, option.Standard));
            }

            if (!question.ComposedVoice.IsEmpty)
                script.Add(new ChangeQuestionComposedVoice(bank, question.Identifier, question.ComposedVoice));

            return script;
        }

        /// <summary>
        /// Returns the list of commands to create a Set.
        /// </summary>
        public List<ICommand> GetCommands(Guid bank, Set set)
        {
            var script = new List<ICommand>
            {
                new AddSet(bank, set.Identifier, set.Name, set.Standard)
            };

            if (set.Randomization?.Enabled == true)
                script.Add(new ChangeSetRandomization(bank, set.Identifier, set.Randomization));

            if (set.Questions.Count > 0)
            {
                foreach (var question in set.Questions)
                {
                    script.AddRange(GetCommands(bank, set.Identifier, question));
                }
            }

            return script;
        }

        /// <summary>
        /// Returns the list of commands to create a Bank.
        /// </summary>
        public List<ICommand> GetCommands(BankState bank)
        {
            var newBank = new BankState
            {
                Type = bank.Type,
                Tenant = bank.Tenant,
                Identifier = bank.Identifier,
                Asset = bank.Asset,
                Standard = bank.Standard,
                Department = bank.Department,
                Name = bank.Name,
                Content = bank.Content.Clone(),
                Edition = bank.Edition.Clone(),
            };

            var script = new List<ICommand>
            {
                new OpenBank(newBank)
            };

            if (bank.Sets.IsNotEmpty())
            {
                foreach (var set in bank.Sets)
                {
                    script.AddRange(GetCommands(newBank.Identifier, set));
                }
            }

            return script;
        }
    }
}
