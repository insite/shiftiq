using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Surveys.Forms;

using Shift.Common;

namespace InSite.Application.Surveys.Write
{
    public class SurveyCommandGenerator
    {
        /// <summary>
        /// Returns the list of commands to create an existing survey form.
        /// </summary>
        public ICommand[] GetCommands(string source, SurveyForm form, bool differentOrganizationUpload = false)
        {
            var id = form.Identifier;
            var script = new List<ICommand>
            {
                new CreateSurveyForm(id, source, form.Tenant, form.Asset, form.Name, form.Status, form.Language),
                //new ChangeSurveyFormLanguages(id, form.Language, form.LanguageTranslations),
                new ChangeSurveyFormSettings(id, form.UserFeedback, form.RequireUserIdentification, form.RequireUserAuthentication, form.ResponseLimitPerUser, form.DurationMinutes, form.EnableUserConfidentiality),
                new ChangeSurveyFormSchedule(id, form.Opened, form.Closed),
                new ChangeSurveyFormContent(id, form.Content)
            };

            if (form.LanguageTranslations != null)
                script.Add(new ChangeSurveyFormLanguages(id, form.Language, form.LanguageTranslations));

            if (!differentOrganizationUpload)
            {

                if (form.Hook != null)
                    script.Add(new ChangeSurveyHook(id, form.Hook));

                if (form.Messages.IsNotEmpty())
                    script.Add(new ChangeSurveyFormMessages(id, form.Messages.ToArray()));
            }
            else
            {
                if (form.Hook != null)
                    script.Add(new ChangeSurveyHook(id, id.ToString()));
            }

            foreach (var question in form.Questions)
                AddCommands(id, question, script);

            return EnsureCommandsOrder(script);
        }

        /// <summary>
        /// Returns the list of commands to create an existing survey question.
        /// </summary>
        public List<ICommand> GetCommands(Guid form, SurveyQuestion question)
        {
            var script = new List<ICommand>();

            AddCommands(form, question, script);

            return EnsureCommandsOrder(script).ToList();
        }

        private void AddCommands(Guid form, SurveyQuestion question, List<ICommand> script)
        {
            script.Add(new AddSurveyQuestion(form, question.Identifier, question.Type, question.Code, question.Indicator, question.Source));
            script.Add(new ChangeSurveyQuestionSettings(form, question.Identifier, question.IsHidden, question.IsRequired, question.IsNested, question.LikertAnalysis, question.ListEnableRandomization, question.ListEnableOtherText, question.ListEnableBranch, question.ListEnableGroupMembership, question.ListDisableColumnHeadingWrap, question.TextLineCount, question.TextCharacterLimit, question.NumberEnableStatistics, question.NumberEnableAutoCalc, question.NumberAutoCalcQuestions, question.NumberEnableNotApplicable, question.ListSelectionRange, question.EnableCreateCase));

            if (question.Content != null)
                script.Add(new ChangeSurveyQuestionContent(form, question.Identifier, question.Content));

            if (question.Attribute.IsNotEmpty())
                script.Add(new AttributeSurveyQuestion(form, question.Identifier, question.Attribute));

            if (question.Options != null && !question.Options.IsEmpty)
                AddCommands(form, question.Identifier, question.Options, script);

            if (question.Scales.IsNotEmpty())
                foreach (var scale in question.Scales)
                    script.Add(new ChangeSurveyScale(form, question.Identifier, scale));
        }

        /// <summary>
        /// Returns the list of commands to create an existing survey option table.
        /// </summary>
        public ICommand[] GetCommands(Guid form, Guid question, SurveyOptionTable table)
        {
            var script = new List<ICommand>();

            AddCommands(form, question, table, script);

            return EnsureCommandsOrder(script);
        }

        private void AddCommands(Guid form, Guid question, SurveyOptionTable table, List<ICommand> script)
        {
            foreach (var list in table.Lists)
            {
                script.Add(new AddSurveyOptionList(form, question, list.Identifier));

                if (list.Content != null)
                    script.Add(new ChangeSurveyOptionListContent(form, list.Identifier, list.Content, list.Category));

                if (list.Items != null && !list.IsEmpty)
                    AddCommands(form, list, script);
            }
        }

        /// <summary>
        /// Returns the list of commands to create an existing survey option list.
        /// </summary>
        public ICommand[] GetCommands(Guid form, SurveyOptionList list)
        {
            var script = new List<ICommand>();

            AddCommands(form, list, script);

            return script.ToArray();
        }

        private void AddCommands(Guid form, SurveyOptionList list, List<ICommand> script)
        {
            foreach (var item in list.Items)
            {
                script.Add(new AddSurveyOptionItem(form, list.Identifier, item.Identifier));
                script.Add(new ChangeSurveyOptionItemSettings(form, item.Identifier, item.Category, item.Points));

                if (item.Content != null)
                    script.Add(new ChangeSurveyOptionItemContent(form, item.Identifier, item.Content));

                if (item.BranchToQuestionIdentifier.HasValue)
                    script.Add(new AddSurveyBranch(form, item.Identifier, item.BranchToQuestionIdentifier.Value));

                if (item.MaskedQuestionIdentifiers.IsNotEmpty())
                    script.Add(new AddSurveyCondition(form, item.Identifier, item.MaskedQuestionIdentifiers.ToArray()));
            }
        }

        public ICommand[] GetDifferenceCommands(SurveyQuestion original, SurveyQuestion changed, int originalSequence)
        {
            var form = changed.Form.Identifier;
            var script = new List<ICommand>();

            if (originalSequence != changed.Sequence)
            {
                var sequences = new Dictionary<Guid, int>();
                foreach (var question in changed.Form.Questions)
                    sequences.Add(question.Identifier, question.Sequence);

                script.Add(new ReorderSurveyQuestions(form, sequences));
            }

            if (original.Code != changed.Code || original.Indicator != changed.Indicator)
                script.Add(new RecodeSurveyQuestion(form, changed.Identifier, changed.Code, changed.Indicator));

            if (original.Attribute != changed.Attribute)
                script.Add(new AttributeSurveyQuestion(form, changed.Identifier, changed.Attribute));

            if (changed.NumberEnableAutoCalc && changed.NumberEnableNotApplicable)
                changed.NumberEnableNotApplicable = false;

            if (
                   original.IsRequired != changed.IsRequired
                || original.IsNested != changed.IsNested
                || original.LikertAnalysis != changed.LikertAnalysis
                || original.ListDisableColumnHeadingWrap != changed.ListDisableColumnHeadingWrap
                || original.ListEnableRandomization != changed.ListEnableRandomization
                || original.ListEnableOtherText != changed.ListEnableOtherText
                || original.ListEnableBranch != changed.ListEnableBranch
                || original.ListEnableGroupMembership != changed.ListEnableGroupMembership
                || original.TextLineCount != changed.TextLineCount
                || original.TextCharacterLimit != changed.TextCharacterLimit
                || original.NumberEnableStatistics != changed.NumberEnableStatistics
                || original.NumberEnableAutoCalc != changed.NumberEnableAutoCalc
                || original.NumberAutoCalcQuestions.EmptyIfNull().Length != changed.NumberAutoCalcQuestions.EmptyIfNull().Length
                || original.NumberAutoCalcQuestions.IsNotEmpty() && changed.NumberAutoCalcQuestions.IsNotEmpty() && original.NumberAutoCalcQuestions.Any(x => !changed.NumberAutoCalcQuestions.Contains(x))
                || original.NumberEnableNotApplicable != changed.NumberEnableNotApplicable
                || !original.ListSelectionRange.IsEqual(changed.ListSelectionRange)
                || original.EnableCreateCase != changed.EnableCreateCase
                )
            {
                script.Add(new ChangeSurveyQuestionSettings(form, changed.Identifier, changed.IsHidden, changed.IsRequired, changed.IsNested, changed.LikertAnalysis, changed.ListEnableRandomization, changed.ListEnableOtherText, changed.ListEnableBranch, changed.ListEnableGroupMembership, changed.ListDisableColumnHeadingWrap, changed.TextLineCount, changed.TextCharacterLimit, changed.NumberEnableStatistics, changed.NumberEnableAutoCalc, changed.NumberAutoCalcQuestions, changed.NumberEnableNotApplicable, changed.ListSelectionRange, changed.EnableCreateCase));
            }

            if (!ContentContainer.IsEqual(original.Content, changed.Content))
                script.Add(new ChangeSurveyQuestionContent(form, changed.Identifier, changed.Content));

            if (original.Options != null && !original.Options.IsEmpty && (changed.Options == null || changed.Options.IsEmpty))
            {
                foreach (var list in original.Options.Lists)
                    script.Add(new DeleteSurveyOptionList(form, list.Identifier));
            }
            else if ((original.Options == null || original.Options.IsEmpty) && changed.Options != null && !changed.Options.IsEmpty)
            {
                AddCommands(form, changed.Identifier, changed.Options, script);
            }
            else if (original.Options != null && !original.Options.IsEmpty)
            {
                GetDifferenceCommands(form, original.Identifier, original.Options.Lists, changed.Options.Lists, script);
            }

            return script.ToArray();
        }

        private void GetDifferenceCommands(Guid form, Guid question, List<SurveyOptionList> originalLists, List<SurveyOptionList> changedLists, List<ICommand> script)
        {
            foreach (var originalList in originalLists)
            {
                var changedList = changedLists.Find(x => x.Identifier == originalList.Identifier);
                if (changedList == null)
                    script.Add(new DeleteSurveyOptionList(form, originalList.Identifier));
                else
                    GetDifferenceCommands(form, originalList, changedList, script);
            }

            var reorder = false;

            foreach (var changedList in changedLists)
            {
                var originalList = originalLists.Find(x => x.Identifier == changedList.Identifier);
                if (originalList == null)
                {
                    script.Add(new AddSurveyOptionList(form, question, changedList.Identifier));
                    script.Add(new ChangeSurveyOptionListContent(form, changedList.Identifier, changedList.Content, changedList.Category));

                    if (changedList.Items != null && !changedList.IsEmpty)
                        AddCommands(form, changedList, script);
                }
                else if (originalList.Sequence != changedList.Sequence)
                {
                    reorder = true;
                }
            }

            if (reorder)
            {
                var sequences = new Dictionary<Guid, int>();
                foreach (var list in changedLists)
                    sequences.Add(list.Identifier, list.Sequence);

                script.Add(new ReorderSurveyOptionLists(form, question, sequences));
            }
        }

        private void GetDifferenceCommands(Guid form, SurveyOptionList originalList, SurveyOptionList changedList, List<ICommand> script)
        {
            if (!ContentContainer.IsEqual(originalList.Content, changedList.Content)
                || !StringHelper.Equals(originalList.Category, changedList.Category))
                script.Add(new ChangeSurveyOptionListContent(form, changedList.Identifier, changedList.Content, changedList.Category));

            foreach (var originalItem in originalList.Items)
            {
                var changedItem = changedList.Items.Find(x => x.Identifier == originalItem.Identifier);
                if (changedItem == null)
                    script.Add(new DeleteSurveyOptionItem(form, originalItem.Identifier));
                else
                    GetDifferenceCommands(form, originalItem, changedItem, script);
            }

            var reorder = false;

            foreach (var changedItem in changedList.Items)
            {
                var originalItem = originalList.Items.Find(x => x.Identifier == changedItem.Identifier);
                if (originalItem == null)
                {
                    script.Add(new AddSurveyOptionItem(form, changedList.Identifier, changedItem.Identifier));
                    script.Add(new ChangeSurveyOptionItemSettings(form, changedItem.Identifier, changedItem.Category, changedItem.Points));
                    script.Add(new ChangeSurveyOptionItemContent(form, changedItem.Identifier, changedItem.Content));

                    if (changedItem.BranchToQuestionIdentifier.HasValue)
                        script.Add(new AddSurveyBranch(form, changedItem.Identifier, changedItem.BranchToQuestionIdentifier.Value));

                    if (changedItem.MaskedQuestionIdentifiers.IsNotEmpty())
                        script.Add(new AddSurveyCondition(form, changedItem.Identifier, changedItem.MaskedQuestionIdentifiers.ToArray()));
                }
                else if (originalItem.Sequence != changedItem.Sequence)
                {
                    reorder = true;
                }
            }

            if (reorder)
            {
                var sequences = new Dictionary<Guid, int>();
                foreach (var item in changedList.Items)
                    sequences.Add(item.Identifier, item.Sequence);

                script.Add(new ReorderSurveyOptionItems(form, changedList.Identifier, sequences));
            }
        }

        private void GetDifferenceCommands(Guid form, SurveyOptionItem originalItem, SurveyOptionItem changedItem, List<ICommand> script)
        {
            if (originalItem.Category != changedItem.Category || originalItem.Points != changedItem.Points)
                script.Add(new ChangeSurveyOptionItemSettings(form, changedItem.Identifier, changedItem.Category, changedItem.Points));

            if (!ContentContainer.IsEqual(originalItem.Content, changedItem.Content))
                script.Add(new ChangeSurveyOptionItemContent(form, changedItem.Identifier, changedItem.Content));

            if (originalItem.BranchToQuestionIdentifier != changedItem.BranchToQuestionIdentifier)
            {
                if (originalItem.BranchToQuestionIdentifier.HasValue)
                    script.Add(new DeleteSurveyBranch(form, originalItem.Identifier, originalItem.BranchToQuestionIdentifier.Value));

                if (changedItem.BranchToQuestionIdentifier.HasValue)
                    script.Add(new AddSurveyBranch(form, changedItem.Identifier, changedItem.BranchToQuestionIdentifier.Value));
            }

            var originalMaskedQuestionIdentifiers = originalItem.MaskedQuestionIdentifiers != null ? originalItem.MaskedQuestionIdentifiers : new List<Guid>();
            var changedMaskedQuestionIdentifiers = changedItem.MaskedQuestionIdentifiers != null ? changedItem.MaskedQuestionIdentifiers : new List<Guid>();

            if (originalMaskedQuestionIdentifiers.Except(changedMaskedQuestionIdentifiers).Count() > 0)
            {
                if (originalMaskedQuestionIdentifiers.Count > 0)
                    script.Add(new DeleteSurveyCondition(form, originalItem.Identifier, originalMaskedQuestionIdentifiers.ToArray()));

                if (changedMaskedQuestionIdentifiers.Count > 0)
                    script.Add(new AddSurveyCondition(form, changedItem.Identifier, changedItem.MaskedQuestionIdentifiers.ToArray()));
            }
        }

        private ICommand[] EnsureCommandsOrder(List<ICommand> commands)
        {
            var commands1 = new List<ICommand>();
            var commands2 = new List<ICommand>();

            foreach (var command in commands)
            {
                var isPostCommand = command is Application.Surveys.Write.AddSurveyBranch
                    || command is Application.Surveys.Write.AddSurveyCondition;

                if (isPostCommand)
                    commands2.Add(command);
                else
                    commands1.Add(command);
            }

            return commands1.Concat(commands2).ToArray();
        }
    }
}
