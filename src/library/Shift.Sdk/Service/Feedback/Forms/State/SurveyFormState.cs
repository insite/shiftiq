using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyState : AggregateState
    {
        public SurveyForm Form { get; set; }
        public SurveyWorkflowConfiguration WorkflowConfiguration { get; set; }

        public SurveyState()
        {
            Form = new SurveyForm();
            Form.State = this;
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            Form.Initialize();
            Form.State = this;
        }

        public void When(SurveyBranchAdded e)
        {
            var branch = Form.FindOptionItem(e.FromItem);
            if (branch != null)
                branch.BranchToQuestionIdentifier = e.ToQuestion;
        }

        public void When(SurveyBranchDeleted e)
        {
            var branch = Form.FindOptionItem(e.Item);
            if (branch != null)
                branch.BranchToQuestionIdentifier = null;
        }

        public void When(SurveyCommentDeleted e)
        {
            var comment = e.Comment;
            Form.Comments.RemoveAll(x => x.ID == comment);
        }

        public void When(SurveyCommentModified e)
        {
            var comment = Form.FindComment(e.Comment);
            comment.Text = e.Text;
            comment.Flag = e.Flag;
            comment.Resolved = e.Resolved;
        }

        public void When(SurveyCommentPosted e)
        {
            if (Form.FindComment(e.Comment) != null)
                throw ApplicationError.Create("A comment with this ID has already been added: " + e.Comment);

            Form.Comments.Add(new SurveyComment
            {
                ID = e.Comment,
                Text = e.Text,
                Flag = e.Flag,
                Resolved = e.Resolved,
            });
        }

        public void When(SurveyConditionAdded e)
        {
            var item = Form.FindOptionItem(e.Item);
            foreach (var question in e.MaskedQuestions)
                if (!item.MaskedQuestionIdentifiers.Any(x => x == question))
                    item.MaskedQuestionIdentifiers.Add(question);
        }

        public void When(SurveyConditionDeleted e)
        {
            var item = Form.FindOptionItem(e.MaskingItem);
            item.MaskedQuestionIdentifiers = item.MaskedQuestionIdentifiers.Except(e.MaskedQuestions).ToList();
        }

        public void When(SurveyDisplaySummaryChartChanged e)
        {
            Form.DisplaySummaryChart = e.DisplaySummaryChart;
        }

        public void When(SurveyFormAssetChanged e)
        {
            Form.Asset = e.Asset;
        }

        public void When(SurveyFormContentChanged e)
        {
            Form.Content = e.Content;
        }

        public void When(SurveyFormCreated e)
        {
            Form.Source = e.Source;
            Form.Tenant = e.Tenant;

            Form.Identifier = e.AggregateIdentifier;
            Form.Asset = e.Asset;

            Form.Name = e.Name;
            Form.Status = e.Status;
            Form.Language = e.Language;
        }

        public void When(SurveyFormLanguagesChanged e)
        {
            Form.Language = e.Language;
            Form.LanguageTranslations = e.Translations;
        }

        public void When(SurveyFormLocked e)
        {
            Form.Locked = e.Locked;
        }

        public void When(SurveyFormMessageAdded e)
        {
            if (Form.Messages == null)
                Form.Messages = new List<SurveyMessage>();

            if (Form.Messages.Find(x => x.Identifier == e.Message.Identifier) == null)
                Form.Messages.Add(e.Message);
        }

        public void When(SurveyFormMessagesChanged e)
        {
            Form.Messages = e.Messages.ToList();
        }

        public void When(SurveyFormRenamed e)
        {
            Form.Name = e.Name;
        }

        public void When(SurveyHookChanged e)
        {
            Form.Hook = e.Hook;
        }

        public void When(SurveyFormScheduleChanged e)
        {
            Form.Opened = e.Opened;
            Form.Closed = e.Closed;
        }

        public void When(SurveyFormSettingsChanged e)
        {
            Form.UserFeedback = e.UserFeedback;
            Form.RequireUserIdentification = e.RequireUserIdentification;
            Form.RequireUserAuthentication = e.RequireUserAuthentication;
            Form.ResponseLimitPerUser = e.ResponseLimitPerUser;
            Form.DurationMinutes = e.DurationMinutes;
            Form.EnableUserConfidentiality = e.EnableUserConfidentiality;
        }

        public void When(SurveyFormStatusChanged e)
        {
            Form.Status = e.Status;
        }

        public void When(SurveyFormUnlocked e)
        {
            Form.Locked = null;
        }

        public void When(SurveyFormDeleted _)
        {

        }

        public void When(SurveyOptionItemAdded e)
        {
            // Developer Note: We should rely on the Command Receiver to validate commands. The code here should never
            // execute if the list does not exist, or if the item is already added.

            var list = Form.FindOptionList(e.List);

            list.Items.Add(new SurveyOptionItem() { Identifier = e.Item, Question = list.Question, List = list });
        }

        public void When(SurveyOptionItemContentChanged e)
        {
            Form.FindOptionItem(e.Item).Content = e.Content;
        }

        public void When(SurveyOptionItemDeleted e)
        {
            var item = Form.FindOptionItem(e.Item);
            item.List.Items.Remove(item);
        }

        public void When(SurveyOptionItemSettingsChanged e)
        {
            var item = Form.FindOptionItem(e.Item);
            item.Category = e.Category.NullIfWhiteSpace().MaxLength(90);
            item.Points = e.Points;
        }

        public void When(SurveyOptionItemsReordered e)
        {
            var list = Form.FindOptionList(e.List);
            list.Items.Sort((a, b) =>
            {
                return e.Sequences[a.Identifier].CompareTo(e.Sequences[b.Identifier]);
            });
        }

        public void When(SurveyOptionListAdded e)
        {
            var question = Form.FindQuestion(e.Question);
            var list = new SurveyOptionList { Identifier = e.List, Question = question, Table = question.Options };

            question.Options.Add(list);
        }

        public void When(SurveyOptionListContentChanged e)
        {
            var list = Form.FindOptionList(e.List);
            list.Content = e.Content;
            list.Category = e.Category;

            var question = list.Question;
            if (e.Category.HasValue())
            {
                if (!question.Scales.Any(x => x.Category == e.Category))
                {
                    var scale = new SurveyScale
                    {
                        Question = question,
                        Category = e.Category
                    };
                    question.Scales.Add(scale);
                }
            }
            question.RemoveOrphanScales();
        }

        public void When(SurveyOptionListDeleted e)
        {
            var list = Form.FindOptionList(e.List);
            list.Question.Options.Remove(list);
            list.Question.RemoveOrphanScales();
        }

        public void When(SurveyOptionListsReordered e)
        {
            var question = Form.FindQuestion(e.Question);
            question.Options.Lists.Sort((a, b) =>
            {
                return e.Sequences[a.Identifier].CompareTo(e.Sequences[b.Identifier]);
            });
        }

        public void When(SurveyQuestionAdded e)
        {
            var question = new SurveyQuestion
            {
                Identifier = e.Question,
                Type = e.Type,
                Code = e.Code,
                Indicator = e.Indicator,
                Form = Form
            };
            Form.Questions.Add(question);
        }

        public void When(SurveyQuestionAttributed e)
        {
            var question = Form.FindQuestion(e.Question);
            question.Attribute = e.Attribute;
        }

        public void When(SurveyQuestionContentChanged e)
        {
            var q = Form.FindQuestion(e.Question);
            if (q != null)
                q.Content = e.Content;
        }

        public void When(SurveyQuestionRecoded e)
        {
            var question = Form.FindQuestion(e.Question);

            question.Code = e.Code;
            question.Indicator = e.Indicator;
        }

        public void When(SurveyQuestionDeleted e)
        {
            var question = Form.FindQuestion(e.Question);
            if (question == null)
                return;

            foreach (var option in Form.FlattenOptionItems())
            {
                if (option.BranchToQuestionIdentifier == question.Identifier)
                    option.BranchToQuestionIdentifier = null;

                if (option.MaskedQuestionIdentifiers != null)
                    option.MaskedQuestionIdentifiers.Remove(question.Identifier);
            }

            Form.Questions.Remove(question);
        }

        public void When(SurveyQuestionSettingsChanged e)
        {
            var question = Form.FindQuestion(e.Question);
            question.IsHidden = e.IsHidden;
            question.IsRequired = e.IsRequired;
            question.IsNested = e.IsNested;
            question.LikertAnalysis = e.LikertAnalysis;
            question.ListDisableColumnHeadingWrap = e.ListDisableColumnHeadingWrap;
            question.ListEnableRandomization = e.ListEnableRandomization;
            question.ListEnableOtherText = e.ListEnableOtherText;
            question.ListEnableBranch = e.ListEnableBranch;
            question.ListEnableGroupMembership = e.ListEnableGroupMembership;
            question.TextLineCount = e.TextLineCount;
            question.TextCharacterLimit = e.TextCharacterLimit;
            question.NumberEnableStatistics = e.NumberEnableStatistics;
            question.NumberEnableAutoCalc = e.NumberEnableAutoCalc;
            question.NumberAutoCalcQuestions = e.NumberAutoCalcQuestions;
            question.NumberEnableNotApplicable = !e.NumberEnableAutoCalc && e.NumberEnableNotApplicable;
            question.ListSelectionRange.Set(e.ListSelectionRange);
            question.EnableCreateCase = e.EnableCreateCase;
        }

        public void When(SurveyQuestionsReordered e)
        {
            Form.Questions.Sort((a, b) =>
            {
                return e.Sequences[a.Identifier].CompareTo(e.Sequences[b.Identifier]);
            });
        }

        public void When(SurveyRespondentsAdded e)
        {
            Form.Respondents = Form.Respondents.Union(e.Respondents).ToList();
        }

        public void When(SurveyRespondentsDeleted e)
        {
            Form.Respondents = Form.Respondents.Except(e.Respondents).ToList();
        }

        public void When(SurveyScaleChanged e)
        {
            var question = Form.FindQuestion(e.Question);

            var oldScale = question.Scales.FirstOrDefault(x => x.Category == e.Scale.Category);
            if (oldScale != null)
                question.Scales.Remove(oldScale);

            var newScale = e.Scale.Clone();
            newScale.Sort();
            newScale.Question = question;

            question.Scales.Add(newScale);
        }

        public void When(SurveyWorkflowConfigured e)
        {
            WorkflowConfiguration = e.Configuration;
        }
    }
}
