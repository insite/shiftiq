using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Changes;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyAggregate : AggregateRoot
    {
        public override AggregateState CreateState() => new SurveyState();

        public SurveyState Data => (SurveyState)State;

        public void AddSurveyBranch(Guid fromOption, Guid? toQuestion)
        {
            var o = Data.Form.FindOptionItem(fromOption);
            if (o == null)
                return;

            if (toQuestion.HasValue && Data.Form.FindQuestion(toQuestion.Value) == null)
                toQuestion = null;

            if (o.BranchToQuestionIdentifier == toQuestion)
                return;

            Apply(new SurveyBranchAdded(fromOption, toQuestion));
        }

        public void AddSurveyCondition(Guid maskingItem, Guid[] maskedQuestions)
        {
            var o = Data.Form.FindOptionItem(maskingItem);
            if (o == null)
                return;

            maskedQuestions = maskedQuestions
                .Except(o.MaskedQuestionIdentifiers)
                .Where(x => Data.Form.FindQuestion(x) != null)
                .ToArray();

            if (maskedQuestions.Length == 0)
                return;

            Apply(new SurveyConditionAdded(maskingItem, maskedQuestions));
        }

        public void AddSurveyFormMessage(SurveyMessage message)
        {
            if (Data.Form.Messages.Any(x => x.Identifier == message.Identifier))
                return;

            Apply(new SurveyFormMessageAdded(message));
        }

        public void AddSurveyOptionItem(Guid list, Guid item)
        {
            if (Data.Form.FindOptionList(list) == null)
                return;

            if (Data.Form.FindOptionItem(item) != null)
                return;

            Apply(new SurveyOptionItemAdded(list, item));
        }

        public void AddSurveyOptionList(Guid question, Guid list)
        {
            if (Data.Form.FindQuestion(question) == null || Data.Form.FindOptionList(list) != null)
                return;

            Apply(new SurveyOptionListAdded(question, list));
        }

        public void AddSurveyQuestion(Guid question, SurveyQuestionType type, string code, string indicator, string source)
        {
            if (Data.Form.FindQuestion(question) != null)
                return;

            Apply(new SurveyQuestionAdded(question, type, code, indicator, source));
        }

        public void AddSurveyRespondents(Guid[] respondents)
        {
            respondents = respondents.Except(Data.Form.Respondents).ToArray();
            if (respondents.Length == 0)
                return;

            Apply(new SurveyRespondentsAdded(respondents));
        }

        public void AttributeSurveyQuestion(Guid question, string attribute)
        {
            var q = Data.Form.FindQuestion(question);
            if (q == null || q.Attribute == attribute)
                return;

            Apply(new SurveyQuestionAttributed(question, attribute));
        }

        public void ChangeSurveyDisplaySummaryChart(bool displaySummaryChart)
        {
            if (Data.Form.DisplaySummaryChart == displaySummaryChart)
                return;

            Apply(new SurveyDisplaySummaryChartChanged(displaySummaryChart));
        }

        public void ChangeSurveyFormAsset(int asset)
        {
            if (Data.Form.Asset == asset)
                return;

            Apply(new SurveyFormAssetChanged(asset));
        }

        public void ChangeSurveyFormContent(ContentContainer content)
        {
            if (ContentContainer.IsEqual(Data.Form.Content, content))
                return;

            Apply(new SurveyFormContentChanged(content));
        }

        public void ChangeSurveyFormLanguages(string language, string[] translations)
        {
            if (Data.Form.Language == language
                && (
                        Data.Form.LanguageTranslations == null && translations.Length == 0
                        ||
                        Data.Form.LanguageTranslations != null
                        && Data.Form.LanguageTranslations.Length == translations.Length
                        && Data.Form.LanguageTranslations.All(x => translations.Contains(x))
                    )
                )
            {
                return;
            }

            Apply(new SurveyFormLanguagesChanged(language, translations));
        }

        public void ChangeSurveyFormMessages(SurveyMessage[] messages)
        {
            if (messages == null)
                messages = new SurveyMessage[0];

            if (Data.Form.Messages.Count == messages.Length
                && Data.Form.Messages.All(x => messages.Any(y => y.Identifier == x.Identifier && y.Type == x.Type)))
                return;

            Apply(new SurveyFormMessagesChanged(messages));
        }

        public void ChangeSurveyFormSchedule(DateTimeOffset? opened, DateTimeOffset? closed)
        {
            if (Data.Form.Opened == opened && Data.Form.Closed == closed)
                return;

            Apply(new SurveyFormScheduleChanged(opened, closed));
        }

        public void ChangeSurveyFormSettings(UserFeedbackType userFeedback, bool requireUserIdentification, bool requireUserAuthentication, int? responseLimitPerUser, int? durationMinutes, bool enableUserConfidentiality)
        {
            if (Data.Form.UserFeedback == userFeedback
                && Data.Form.RequireUserIdentification == requireUserIdentification
                && Data.Form.RequireUserAuthentication == requireUserAuthentication
                && Data.Form.ResponseLimitPerUser == responseLimitPerUser
                && Data.Form.DurationMinutes == durationMinutes
                && Data.Form.EnableUserConfidentiality == enableUserConfidentiality)
                return;

            Apply(new SurveyFormSettingsChanged(
                userFeedback,
                requireUserIdentification,
                requireUserAuthentication,
                responseLimitPerUser,
                durationMinutes,
                enableUserConfidentiality));
        }

        public void ChangeSurveyFormStatus(SurveyFormStatus status)
        {
            if (Data.Form.Status == status)
                return;

            Apply(new SurveyFormStatusChanged(status));
        }

        public void ChangeSurveyHook(string hook)
        {
            if (Data.Form.Hook == hook)
                return;

            Apply(new SurveyHookChanged(hook));
        }

        public void ChangeSurveyOptionItemContent(Guid item, ContentContainer content)
        {
            var o = Data.Form.FindOptionItem(item);
            if (o == null || ContentContainer.IsEqual(o.Content, content))
                return;

            content.CreateSnips();

            Apply(new SurveyOptionItemContentChanged(item, content));
        }

        public void ChangeSurveyOptionItemSettings(Guid item, string category, decimal points)
        {
            var o = Data.Form.FindOptionItem(item);
            if (o == null)
                return;

            category = category.NullIfWhiteSpace();

            if (o.Category == category && o.Points == points)
                return;

            Apply(new SurveyOptionItemSettingsChanged(item, category, points));
        }

        public void ChangeSurveyOptionListContent(Guid list, ContentContainer content, string category)
        {
            var l = Data.Form.FindOptionList(list);
            if (l == null || l.Category == category && ContentContainer.IsEqual(l.Content, content))
                return;

            Apply(new SurveyOptionListContentChanged(list, content, category));
        }

        public void ChangeSurveyQuestionContent(Guid question, ContentContainer content)
        {
            var q = Data.Form.FindQuestion(question);
            if (q == null || ContentContainer.IsEqual(q.Content, content))
                return;

            content.CreateSnips();

            Apply(new SurveyQuestionContentChanged(question, content));
        }

        public void ChangeSurveyQuestionSettings(Guid question, bool isHidden, bool isRequired, bool isNested, string likertAnalysis, bool listEnableRandomization, bool listEnableOtherText, bool listEnableBranch, bool listEnableGroupMembership, bool disableColumnHeadingWrap, int? textLineCount, int? textCharacterLimit, bool numberEnableStatistics, bool numberEnableAutoCalc, Guid[] numberAutoCalcQuestions, bool numberEnableNotApplicable, SurveyQuestionListSelectionRange listSelectionRange, bool enableCreateCase)
        {
            var q = Data.Form.FindQuestion(question);

            if (numberEnableAutoCalc && numberEnableNotApplicable)
                numberEnableNotApplicable = false;

            if (q == null || q.IsHidden == isHidden
                && q.IsRequired == isRequired
                && q.IsNested == isNested
                && q.LikertAnalysis == likertAnalysis
                && q.ListDisableColumnHeadingWrap == disableColumnHeadingWrap
                && q.ListEnableRandomization == listEnableRandomization
                && q.ListEnableOtherText == listEnableOtherText
                && q.ListEnableBranch == listEnableBranch
                && q.ListEnableGroupMembership == listEnableGroupMembership
                && q.TextLineCount == textLineCount
                && q.TextCharacterLimit == textCharacterLimit
                && q.NumberEnableStatistics == numberEnableStatistics
                && q.NumberEnableAutoCalc == numberEnableAutoCalc
                && q.NumberAutoCalcQuestions.EmptyIfNull().Length == numberAutoCalcQuestions.EmptyIfNull().Length
                && (q.NumberAutoCalcQuestions.IsEmpty() && numberAutoCalcQuestions.IsEmpty() || q.NumberAutoCalcQuestions.All(x => numberAutoCalcQuestions.Contains(x)))
                && q.NumberEnableNotApplicable == numberEnableNotApplicable
                && q.ListSelectionRange.IsEqual(listSelectionRange)
                && q.EnableCreateCase == enableCreateCase)
                return;

            Apply(new SurveyQuestionSettingsChanged(
                question,
                isHidden,
                isRequired,
                isNested,
                likertAnalysis,
                listEnableRandomization,
                listEnableOtherText,
                listEnableBranch,
                listEnableGroupMembership,
                disableColumnHeadingWrap,
                textLineCount,
                textCharacterLimit,
                numberEnableStatistics,
                numberEnableAutoCalc,
                numberAutoCalcQuestions,
                numberEnableNotApplicable,
                listSelectionRange,
                enableCreateCase));
        }

        public void ChangeSurveyScale(Guid question, SurveyScale scale)
        {
            var q = Data.Form.FindQuestion(question);
            if (q == null)
                return;

            Apply(new SurveyScaleChanged(question, scale));
        }

        public void ConfigureSurveyWorkflow(SurveyWorkflowConfiguration configuration)
            => Apply(new SurveyWorkflowConfigured(configuration));

        public void CreateSurveyForm(string source, Guid organization, int asset, string name, SurveyFormStatus status, string language)
        {
            Apply(new SurveyFormCreated(source, organization, asset, name, status, language));
        }

        public void LockSurveyForm(DateTimeOffset locked)
        {
            if (Data.Form.Locked == locked)
                return;

            Apply(new SurveyFormLocked(locked));
        }

        public void ModifySurveyComment(Guid comment, string text, FlagType? flag, DateTimeOffset? resolved)
        {
            if (text.IsEmpty())
                return;

            var c = Data.Form.FindComment(comment);
            if (c == null)
                return;

            if (c.Text == text && c.Flag == flag && c.Resolved == resolved)
                return;

            Apply(new SurveyCommentModified(comment, text, flag, resolved));
        }

        public void PostSurveyComment(Guid comment, string text, FlagType? flag, DateTimeOffset? resolved)
        {
            if (text.IsEmpty())
                return;

            var c = Data.Form.FindComment(comment);
            if (c != null)
                return;

            Apply(new SurveyCommentPosted(comment, text, flag, resolved));
        }

        public void RecodeSurveyQuestion(Guid question, string code, string indicator)
        {
            var q = Data.Form.FindQuestion(question);
            if (q == null || q.Code == code && q.Indicator == indicator)
                return;

            Apply(new SurveyQuestionRecoded(question, code, indicator));
        }

        public void DeleteSurveyBranch(Guid fromItem, Guid? toQuestion)
        {
            var o = Data.Form.FindOptionItem(fromItem);
            if (o == null || !o.BranchToQuestionIdentifier.HasValue)
                return;

            Apply(new SurveyBranchDeleted(fromItem, toQuestion));
        }

        public void DeleteSurveyComment(Guid comment)
        {
            var c = Data.Form.FindComment(comment);
            if (c == null)
                return;

            Apply(new SurveyCommentDeleted(comment));
        }

        public void DeleteSurveyCondition(Guid maskingItem, Guid[] maskedQuestions)
        {
            var o = Data.Form.FindOptionItem(maskingItem);
            if (o == null)
                return;

            maskedQuestions = maskedQuestions
                .Intersect(o.MaskedQuestionIdentifiers)
                .Where(x => Data.Form.FindQuestion(x) != null)
                .ToArray();

            if (maskedQuestions.Length == 0)
                return;

            Apply(new SurveyConditionDeleted(maskingItem, maskedQuestions));
        }

        public void DeleteSurveyOptionItem(Guid item)
        {
            var o = Data.Form.FindOptionItem(item);
            if (o == null)
                return;

            Apply(new SurveyOptionItemDeleted(item));
        }

        public void DeleteSurveyOptionList(Guid list)
        {
            var l = Data.Form.FindOptionList(list);
            if (l == null)
                return;

            Apply(new SurveyOptionListDeleted(list));
        }

        public void DeleteSurveyQuestion(Guid question)
        {
            var q = Data.Form.FindQuestion(question);
            if (q == null)
                return;

            Apply(new SurveyQuestionDeleted(question));
        }

        public void DeleteSurveyRespondents(Guid[] respondents)
        {
            respondents = respondents.Intersect(Data.Form.Respondents).ToArray();
            if (respondents.Length == 0)
                return;

            Apply(new SurveyRespondentsDeleted(respondents));
        }

        public void RenameSurveyForm(string name)
        {
            if (Data.Form.Name == name)
                return;

            Apply(new SurveyFormRenamed(name));
        }

        public void ReorderSurveyOptionItems(Guid list, Dictionary<Guid, int> sequences)
        {
            if (sequences.Count == 0 || sequences.Count != sequences.Values.Distinct().Count())
                return;

            var l = Data.Form.FindOptionList(list);
            if (l == null || l.Items.Count != sequences.Count || !l.Items.All(x => sequences.ContainsKey(x.Identifier)))
                return;

            Apply(new SurveyOptionItemsReordered(list, sequences));
        }

        public void ReorderSurveyOptionLists(Guid question, Dictionary<Guid, int> sequences)
        {
            if (sequences.Count == 0 || sequences.Count != sequences.Values.Distinct().Count())
                return;

            var q = Data.Form.FindQuestion(question);
            if (q == null || q.Options.Lists.Count != sequences.Count || !q.Options.Lists.All(x => sequences.ContainsKey(x.Identifier)))
                return;

            Apply(new SurveyOptionListsReordered(question, sequences));
        }

        public void ReorderSurveyQuestions(Dictionary<Guid, int> sequences)
        {
            if (sequences.Count == 0 || sequences.Count != sequences.Values.Distinct().Count())
                return;

            if (Data.Form.Questions.Count != sequences.Count || !Data.Form.Questions.All(x => sequences.ContainsKey(x.Identifier)))
                return;

            Apply(new SurveyQuestionsReordered(sequences));
        }

        public void UnlockSurveyForm(DateTimeOffset unlocked)
        {
            Apply(new SurveyFormUnlocked(unlocked));
        }

        public void DeleteSurveyForm()
        {
            Apply(new SurveyFormDeleted());
        }
    }
}
