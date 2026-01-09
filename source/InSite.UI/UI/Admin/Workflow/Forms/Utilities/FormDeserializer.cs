using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using InSite.Application.Contents.Read;
using InSite.Domain.Foundations;
using InSite.Domain.Surveys.Forms;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Admin.Workflow.Forms.Utilities
{
    public class FormDeserializer
    {
        public class Result
        {
            public SurveyForm Survey { get; set; }
            public List<SurveyQuestion> Questions { get; set; }
            public List<TContent> Contents { get; } = new List<TContent>();
        }

        private SurveySerialized _sourceSurvey;
        private ISecurityFramework _identity;
        private Result _result;
        private IReadOnlyDictionary<int, Guid> _questionIndexMapping;

        public Result Deserialize(string file)
        {
            _sourceSurvey = JsonConvert.DeserializeObject<SurveySerialized>(file);
            _identity = CurrentSessionState.Identity;
            _result = new Result();

            var indexMapping = new Dictionary<int, Guid>();
            foreach (var q in _sourceSurvey.Questions)
            {
                if (!q.Index.HasValue)
                    continue;

                if (indexMapping.ContainsKey(q.Index.Value))
                {
                    indexMapping.Clear();
                    break;
                }

                indexMapping.Add(q.Index.Value, UniqueIdentifier.Create());
            }

            _questionIndexMapping = new ReadOnlyDictionary<int, Guid>(indexMapping);

            CreateSurvey();
            CreateQuestions();

            return _result;
        }

        private void CreateSurvey()
        {
            _result.Survey = new SurveyForm
            {
                Identifier = UniqueIdentifier.Create(),
                Tenant = _identity.Organization.Identifier,
                DurationMinutes = _sourceSurvey.DurationMinutes,
                DisplaySummaryChart = _sourceSurvey.DisplaySummaryChart,
                EnableUserConfidentiality = _sourceSurvey.EnableUserConfidentiality,
                Hook = _sourceSurvey.Hook,
                Language = _sourceSurvey.Language,
                Name = _sourceSurvey.Name,
                UserFeedback = _sourceSurvey.UserFeedback,
                Status = _sourceSurvey.Status,
                ResponseLimitPerUser = _sourceSurvey.ResponseLimitPerUser,
                RequireUserIdentification = _sourceSurvey.RequireUserIdentification,
                RequireUserAuthentication = _sourceSurvey.RequireUserAuthentication,
                Content = AddContentContainer("Form", _sourceSurvey.Content)
            };

            AddContent(_result.Survey.Identifier, "Form", _sourceSurvey.Content);
        }

        private void CreateQuestions()
        {
            _result.Questions = new List<SurveyQuestion>();
            foreach (var question in _sourceSurvey.Questions)
            {
                var questionId = question.Index.HasValue && _questionIndexMapping.ContainsKey(question.Index.Value)
                    ? _questionIndexMapping[question.Index.Value]
                    : UniqueIdentifier.Create();

                _result.Questions.Add(new SurveyQuestion
                {
                    Attribute = question.Attribute,
                    Code = question.Code,
                    Indicator = question.Indicator,
                    IsHidden = question.IsHidden,
                    IsNested = question.IsNested,
                    IsRequired = question.IsRequired,
                    LikertAnalysis = question.LikertAnalysis,
                    ListDisableColumnHeadingWrap = question.ListDisableColumnHeadingWrap,
                    ListEnableBranch = question.ListEnableBranch,
                    ListEnableOtherText = question.ListEnableOtherText,
                    ListEnableRandomization = question.ListEnableRandomization,
                    ListEnableGroupMembership = question.ListEnableGroupMembership,
                    NumberEnableAutoCalc = question.NumberEnableAutoCalc,
                    NumberEnableNotApplicable = question.NumberEnableNotApplicable,
                    NumberEnableStatistics = question.NumberEnableStatistics,
                    EnableCreateCase = question.EnableCreateCase,
                    Source = question.Source,
                    TextCharacterLimit = question.TextCharacterLimit,
                    TextLineCount = question.TextLineCount,
                    Type = question.Type,
                    Identifier = questionId,
                    Scales = AddScales(question.Scales),
                    Options = AddOptions(question.Options),
                    Content = AddContentContainer("Form Question", question.Content)
                });

                AddContent(questionId, "Form Question", question.Content);
            }
        }

        private SurveyOptionTable AddOptions(List<OptionSerialized> options)
        {
            var surveyOptionsTable = new SurveyOptionTable();
            foreach (var option in options)
            {
                surveyOptionsTable.Add(AddSurveyOptionList(option));
            }
            return surveyOptionsTable;
        }

        private SurveyOptionList AddSurveyOptionList(OptionSerialized option)
        {
            var optionId = UniqueIdentifier.Create();

            return new SurveyOptionList
            {
                Category = option.Category,
                Identifier = optionId,
                Items = AddOptionItems(option.Items),
                Content = AddContentContainer("Form Option List", option.Content)
            };
        }

        private List<SurveyOptionItem> AddOptionItems(List<OptionItemSerialized> items)
        {
            var results = new List<SurveyOptionItem>();
            foreach (var item in items)
            {
                results.Add(new SurveyOptionItem
                {
                    Identifier = UniqueIdentifier.Create(),
                    Category = item.Category,
                    Points = item.Points,
                    Content = AddContentContainer("Form Option Item", item.Content),
                    BranchToQuestionIdentifier = item.BranchToQuestionIndex.HasValue && _questionIndexMapping.ContainsKey(item.BranchToQuestionIndex.Value)
                        ? _questionIndexMapping[item.BranchToQuestionIndex.Value]
                        : (Guid?)null,
                    MaskedQuestionIdentifiers = item.MaskedQuestionIndexes
                        .EmptyIfNull().Where(x => _questionIndexMapping.ContainsKey(x))
                        .Select(x => _questionIndexMapping[x])
                        .ToList()
                });
            }
            return results;
        }

        private List<SurveyScale> AddScales(List<ScaleSerialized> scales)
        {
            var results = new List<SurveyScale>();
            foreach (var scale in scales)
            {
                results.Add(new SurveyScale
                {
                    Category = scale.Category,
                    Items = AddScaleItems(scale.Items)
                });
            }
            return results;
        }

        private List<SurveyScaleItem> AddScaleItems(List<ScaleItemSerialized> items)
        {
            var results = new List<SurveyScaleItem>();
            foreach (var item in items)
            {
                results.Add(new SurveyScaleItem
                {
                    Calculation = item.Calculation,
                    Grade = item.Grade,
                    Maximum = item.Maximum,
                    Minimum = item.Minimum,
                    Content = AddContentContainer("", item.Content)
                });
            }
            return results;
        }

        #region Helper methods

        private void AddContent(Guid containerIdentifier, string containerType, List<SurveyContentSerialized> contentList)
        {
            foreach (var content in contentList)
                AddContent(content.Label, content.Language, content.Text, content.Html);

            void AddContent(string label, string lang, string text, string html)
            {
                if (string.IsNullOrWhiteSpace(text) && string.IsNullOrWhiteSpace(html))
                    return;

                _result.Contents.Add(new TContent
                {
                    OrganizationIdentifier = _identity.Organization.Identifier,
                    ContainerIdentifier = containerIdentifier,
                    ContentIdentifier = UniqueIdentifier.Create(),
                    ContentLabel = label,
                    ContentLanguage = lang,
                    ContainerType = containerType,
                    ContentText = text,
                    ContentHtml = html
                });
            }
        }

        private ContentContainer AddContentContainer(string containerType, List<SurveyContentSerialized> contentList)
        {
            var container = new List<TContent>();
            foreach (var contentItem in contentList)
            {
                container.Add(new TContent
                {
                    OrganizationIdentifier = _identity.Organization.Identifier,
                    ContentLabel = contentItem.Label,
                    ContentLanguage = contentItem.Language,
                    ContainerType = containerType,
                    ContentText = contentItem.Text,
                    ContentHtml = contentItem.Html
                });
            }
            return ServiceLocator.ContentSearch.GetBlock(container);
        }

        #endregion
    }
}