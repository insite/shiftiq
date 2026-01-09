using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Surveys.Forms
{
    [Serializable]
    public class SurveyForm
    {
        public SurveyState State { get; internal set; }

        public SurveyFormStatus Status { get; set; }

        public Guid Identifier { get; set; }
        public Guid Tenant { get; set; }

        public string Hook { get; set; }
        public string Language { get; set; }
        public string[] LanguageTranslations { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Information about the source of this data (e.g. Authored|Uploaded|Upgraded|Copied).
        /// </summary>
        public string Source { get; set; }

        public bool EnableUserConfidentiality { get; set; }
        public UserFeedbackType UserFeedback { get; set; }
        public bool RequireUserAuthentication { get; set; }
        public bool RequireUserIdentification { get; set; }
        public bool DisplaySummaryChart { get; set; }

        public int Asset { get; set; }
        public int? ResponseLimitPerUser { get; set; }
        public int? DurationMinutes { get; set; }

        public DateTimeOffset? Opened { get; set; }
        public DateTimeOffset? Closed { get; set; }
        public DateTimeOffset? Locked { get; set; }

        public string GetTitle()
        {
            var title = Content?.Title.GetText(Language);

            return title.IfNullOrEmpty(Name).IfNullOrEmpty("None");
        }

        public int? GetPageNumber(Guid question)
        {
            var pages = GetPages();
            var page = pages.FirstOrDefault(x => x.Questions.Any(y => y.Identifier == question));
            return page?.Sequence;
        }

        /// <summary>
        /// The multilingual Text and/or HTML content for this option.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Shift.Common.ContentContainer Content { get; set; }

        public static SurveyContentLabel[] ContentLabels => new[]
        {
            new SurveyContentLabel("Title", "This is the survey title displayed to the survey respondent."),
            new SurveyContentLabel("Page Header", "Custom content displayed at the top of every page in the survey where respondents answer questions."),
            new SurveyContentLabel("Page Footer", "Custom content displayed at the bottom of every page in the survey where respondents answer questions."),
            new SurveyContentLabel("Starting Instructions", "These display when a respondent is launching or resuming a survey response. If left blank, their response will launch with no starting instructions."),
            new SurveyContentLabel("Ending Instructions", "These display on the last page of the survey, where the respondent is prompted to submit their response. If left blank, the default text of \"Thank you for participating! You can use the <b>Previous</b> button to review and edit your answers if desired. When you are satisfied, click <b>Submit My Response</b> below.\" will appear."),
            new SurveyContentLabel("Completed Instructions", "This displays at the top of the response review screen when feedback is enabled; if left blank, nothing is displayed."),
            new SurveyContentLabel("Closed Instructions", "These display after the survey is closed, if a respondent tries to start or resume a response. If left blank, the default text of \"This survey is currently not accepting responses.\" will appear.")
        };

        /// <summary>
        /// The questions contained by the form.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<SurveyQuestion> Questions { get; set; }

        /// <summary>
        /// The messages associated with the form.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<SurveyMessage> Messages { get; set; }

        public int CountPages() => GetPages().Count;

        public List<SurveyPage> GetPages()
        {
            var pages = new List<SurveyPage>();

            var i = 0;
            var current = new SurveyPage(++i);
            pages.Add(current);
            foreach (var question in Questions)
            {
                var isPageBreak = question.Type == SurveyQuestionType.BreakPage;

                if (!isPageBreak)
                    current.Questions.Add(question);

                if (isPageBreak)
                {
                    current.Content = question.Content;
                    current = new SurveyPage(++i);
                    pages.Add(current);
                }
            }

            return pages;
        }

        public SurveyPage GetPage(int page)
        {
            var pages = GetPages();
            if (0 < pages.Count && 0 < page && page <= pages.Count)
                return pages[page - 1];
            return null;
        }

        public SurveyPage GetPage(Guid question)
        {
            var pages = GetPages();
            var page = pages.FirstOrDefault(x => x.Questions.Any(y => y.Identifier == question));
            return page;
        }

        public List<SurveyBranch> GetBranches()
        {
            var branches = new List<SurveyBranch>();

            var items = FlattenOptionItems();
            foreach (var item in items)
            {
                if (item.BranchToQuestionIdentifier.HasValue)
                {
                    var branch = new SurveyBranch
                    {
                        FromOptionItem = item,
                        ToQuestion = FindQuestion(item.BranchToQuestionIdentifier.Value)
                    };
                    if (branch.ToQuestion != null)
                        branches.Add(branch);
                }
            }

            return branches;
        }

        public List<SurveyCondition> GetConditions()
        {
            var conditions = new List<SurveyCondition>();

            var items = FlattenOptionItems();
            foreach (var item in items)
            {
                if (item.MaskedQuestionIdentifiers != null)
                {
                    foreach (var masked in item.MaskedQuestionIdentifiers)
                    {
                        var condition = new SurveyCondition
                        {
                            MaskingOptionItem = item,
                            MaskedQuestion = FindQuestion(masked)
                        };
                        if (condition.MaskedQuestion != null)
                            conditions.Add(condition);
                    }
                }
            }

            return conditions;
        }

        public List<Guid> Respondents { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<SurveyComment> Comments { get; set; }

        public SurveyComment FindComment(Guid comment)
        {
            return Comments.Find(x => x.ID == comment);
        }

        public SurveyForm()
        {
            Questions = new List<SurveyQuestion>();
            Messages = new List<SurveyMessage>();
            Respondents = new List<Guid>();
            Content = new Shift.Common.ContentContainer();
            Comments = new List<SurveyComment>();
        }

        public SurveyForm(Guid identifier) : this()
        {
            Identifier = identifier;
        }

        internal void Initialize()
        {
            foreach (var question in Questions)
            {
                question.Form = this;
                question.Options.Question = question;
                foreach (var list in question.Options.Lists)
                {
                    list.Table = question.Options;
                    list.Question = question;
                    foreach (var item in list.Items)
                    {
                        item.List = list;
                        item.Question = question;
                    }
                }
            }
        }

        #region Serialization

        public bool ShouldSerializeContent() => Content != null && !Content.IsEmpty;
        public bool ShouldSerializeContentLabels() => false;
        public bool ShouldSerializeMessages() => Messages.IsNotEmpty();
        public bool ShouldSerializePageCount() => false;
        public bool ShouldSerializeComments() => Comments.IsNotEmpty();

        #endregion

        #region Navigation and Interrogation

        public SurveyOptionItem[] FlattenOptionItems()
            => Questions.SelectMany(x => x.Options.Lists.SelectMany(y => y.Items)).ToArray();

        public SurveyOptionList[] FlattenOptionLists()
            => Questions.SelectMany(x => x.Options.Lists).ToArray();

        public SurveyOptionItem FindOptionItem(Guid item)
            => FlattenOptionItems().FirstOrDefault(x => x.Identifier == item);

        public SurveyOptionList FindOptionList(Guid list)
            => FlattenOptionLists().FirstOrDefault(x => x.Identifier == list);

        public SurveyQuestion FindQuestion(Guid question)
            => Questions.FirstOrDefault(x => x.Identifier == question);

        public SurveyQuestion FindQuestionBySource(string source)
            => Questions.FirstOrDefault(x => x.Source == source);

        #endregion
    }
}
