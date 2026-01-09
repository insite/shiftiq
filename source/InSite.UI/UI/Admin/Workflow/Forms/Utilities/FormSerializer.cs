using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using InSite.Application.Contents.Read;
using InSite.Domain.Surveys.Forms;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Shift.Common;
using Shift.Sdk.UI;

using SurveyQuestion = InSite.Domain.Surveys.Forms.SurveyQuestion;

namespace InSite.Admin.Workflow.Forms.Utilities
{
    public class FormSerializer
    {
        private SurveyForm _surveyForm;
        private List<SurveyQuestion> _questions;
        private List<TContent> _contents;
        private IReadOnlyDictionary<Guid, int> _questionIndexMapping;

        private FormSerializer()
        {
        }

        public static byte[] Serialize(Guid surveyIdentifier, IContractResolver resolver = null)
        {
            var serializer = new FormSerializer();
            serializer.LoadData(surveyIdentifier);

            var surveySerialized = serializer.GetSurveySerialized(surveyIdentifier);

            var json = JsonConvert.SerializeObject(surveySerialized, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = resolver
            });

            return Encoding.UTF8.GetBytes(json);
        }

        private void LoadData(Guid surveyIdentifier)
        {
            var survey = ServiceLocator.SurveySearch.GetSurveyState(surveyIdentifier);
            if (survey == null)
                return;

            _surveyForm = survey.Form;
            if (_surveyForm == null)
                return;

            var allIds = new List<Guid> { surveyIdentifier };
            var indexMapping = new Dictionary<Guid, int>();

            _questions = _surveyForm.Questions;

            if (_questions != null)
            {
                for (var i = 0; i < _questions.Count; i++)
                {
                    var q = _questions[i];
                    allIds.Add(q.Identifier);
                    indexMapping.Add(q.Identifier, i);
                }
            }

            _contents = ServiceLocator.ContentSearch.SelectContainers(x => allIds.Contains(x.ContainerIdentifier)).ToList();
            _questionIndexMapping = new ReadOnlyDictionary<Guid, int>(indexMapping);
        }

        private SurveySerialized GetSurveySerialized(Guid surveyIdentifier)
        {
            var surveySerialized = new SurveySerialized
            {
                DisplaySummaryChart = _surveyForm.DisplaySummaryChart,
                DurationMinutes = _surveyForm.DurationMinutes,
                EnableUserConfidentiality = _surveyForm.EnableUserConfidentiality,
                Hook = _surveyForm.Hook,
                Language = _surveyForm.Language,
                LanguageTranslations = _surveyForm.LanguageTranslations,
                Name = _surveyForm.Name,
                RequireUserAuthentication = _surveyForm.RequireUserAuthentication,
                RequireUserIdentification = _surveyForm.RequireUserIdentification,
                ResponseLimitPerUser = _surveyForm.ResponseLimitPerUser,
                Status = _surveyForm.Status,
                UserFeedback = _surveyForm.UserFeedback,
                Content = GetContent(surveyIdentifier),
                Questions = GetQuestions(_questions)
            };

            var titleContent = surveySerialized.Content.Find(x => StringHelper.Equals(x.Language, "en") && StringHelper.Equals(x.Label, "Title"));
            if (titleContent != null)
                titleContent.Text += " - Copy";

            return surveySerialized;
        }

        private List<QuestionSerialized> GetQuestions(List<SurveyQuestion> questions)
        {
            var results = new List<QuestionSerialized>();
            foreach (var question in questions)
            {
                results.Add(new QuestionSerialized
                {
                    Index = _questionIndexMapping[question.Identifier],
                    Attribute = question.Attribute,
                    Code = question.Code,
                    Indicator = question.Indicator,
                    IsHidden = question.IsHidden,
                    IsNested = question.IsNested,
                    IsRequired = question.IsRequired,
                    LikertAnalysis = question.LikertAnalysis,
                    ListDisableColumnHeadingWrap = question.ListDisableColumnHeadingWrap,
                    ListEnableOtherText = question.ListEnableOtherText,
                    ListEnableRandomization = question.ListEnableRandomization,
                    ListEnableGroupMembership = question.ListEnableGroupMembership,
                    NumberEnableAutoCalc = question.NumberEnableAutoCalc,
                    NumberEnableNotApplicable = question.NumberEnableNotApplicable,
                    NumberEnableStatistics = question.NumberEnableStatistics,
                    TextCharacterLimit = question.TextCharacterLimit,
                    TextLineCount = question.TextLineCount,
                    ListEnableBranch = question.ListEnableBranch,
                    EnableCreateCase = question.EnableCreateCase,
                    Source = question.Source,
                    Type = question.Type,
                    Content = GetContent(question.Identifier),
                    Scales = GetScales(question.Scales),
                    Options = GetOptions(question.Options),

                });
            }
            return results;
        }

        private List<OptionSerialized> GetOptions(SurveyOptionTable options)
        {
            var results = new List<OptionSerialized>();
            foreach (var item in options.Lists)
            {
                results.Add(new OptionSerialized
                {
                    Category = item.Category,
                    Content = GetContent(item.Content),
                    Items = GetOptionItems(item.Items)
                });
            }
            return results;
        }

        private List<OptionItemSerialized> GetOptionItems(List<SurveyOptionItem> items)
        {
            var results = new List<OptionItemSerialized>();
            foreach (var item in items)
            {
                results.Add(new OptionItemSerialized
                {
                    Content = GetContent(item.Content),
                    Category = item.Category,
                    Points = item.Points,
                    BranchToQuestionIndex = item.BranchToQuestionIdentifier.HasValue && _questionIndexMapping.ContainsKey(item.BranchToQuestionIdentifier.Value)
                        ? _questionIndexMapping[item.BranchToQuestionIdentifier.Value]
                        : (int?)null,
                    MaskedQuestionIndexes = item.MaskedQuestionIdentifiers
                        .EmptyIfNull()
                        .Where(x => _questionIndexMapping.ContainsKey(x))
                        .Select(x => _questionIndexMapping[x])
                        .ToArray()
                        .NullIfEmpty()
                });
            }
            return results;
        }

        private List<ScaleSerialized> GetScales(List<SurveyScale> scales)
        {
            var results = new List<ScaleSerialized>();
            foreach (var scale in scales)
            {
                results.Add(new ScaleSerialized
                {
                    Category = scale.Category,
                    Items = GetScaleItems(scale.Items)
                });
            }
            return results;
        }

        private List<ScaleItemSerialized> GetScaleItems(List<SurveyScaleItem> items)
        {
            var results = new List<ScaleItemSerialized>();
            foreach (var item in items)
            {
                results.Add(new ScaleItemSerialized
                {
                    Calculation = item.Calculation,
                    Grade = item.Grade,
                    Maximum = item.Maximum,
                    Minimum = item.Minimum,
                    Content = GetContent(item.Content)
                });
            }
            return results;
        }

        private List<SurveyContentSerialized> GetContent(ContentContainer content)
        {
            var results = new List<SurveyContentSerialized>();
            if (content != null)
            {
                foreach (var lang in content.Languages)
                {
                    foreach (var label in content.GetLabels())
                    {
                        results.Add(new SurveyContentSerialized
                        {
                            Label = label,
                            Language = lang,
                            Html = content.GetHtml(label, lang),
                            Text = content.GetText(label, lang),
                        });
                    }
                }
            }
            return results;
        }

        private List<SurveyContentSerialized> GetContent(Guid containerIdentifier)
        {
            return _contents
                .Where(x => x.ContainerIdentifier == containerIdentifier)
                .Select(x => new SurveyContentSerialized
                {
                    Language = x.ContentLanguage,
                    Label = x.ContentLabel,
                    Text = x.ContentText,
                    Html = x.ContentHtml
                })
                .ToList();
        }

    }
}