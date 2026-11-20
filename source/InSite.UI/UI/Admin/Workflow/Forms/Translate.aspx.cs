using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Admin.Assets.Contents.Controls;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Surveys.Forms;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Common.Events;
using Shift.Common.Timeline.Commands;
using Shift.Constant;

namespace InSite.Admin.Workflow.Forms
{
    public partial class Translate : AdminBasePage, IHasParentLinkParameters
    {
        #region Classes

        private enum ContainerType
        {
            Survey, Question, OptionList, Option, LikertScale, LikertScaleItem
        }

        [Serializable]
        private class LikertScaleDataItem
        {
            public Guid QuestionId { get; }
            public string ScaleCategory { get; }
            public int ItemIndex { get; }
            public ContentContainer Content { get; }

            public LikertScaleDataItem(Guid questionId, string scaleCategory, int itemIndex, ContentContainer content)
            {
                QuestionId = questionId;
                ScaleCategory = scaleCategory;
                ItemIndex = itemIndex;
                Content = content ?? new ContentContainer();
            }
        }

        [Serializable]
        private class FieldKey
        {
            public Guid EntityID { get; private set; }
            public ContainerType Container { get; private set; }
            public string Field { get; private set; }

            public static string Serialize(Guid id, ContainerType container, string field)
            {
                return StringHelper.EncodeBase64Url(stream =>
                {
                    using (var writer = new BinaryWriter(stream))
                    {
                        writer.Write(id);
                        writer.Write((byte)container);
                        writer.Write(field);
                    }
                });
            }

            public static FieldKey Deserialize(string data)
            {
                return StringHelper.DecodeBase64Url(data, stream =>
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        return new FieldKey
                        {
                            EntityID = reader.ReadGuid(),
                            Container = (ContainerType)reader.ReadByte(),
                            Field = reader.ReadString()
                        };
                    }
                });
            }
        }

        private class BaseTranslationItem
        {
            public string Title { get; set; }
            public string FromText { get; set; }
            public string ToText { get; set; }
            public string OnClick { get; set; }

            public BaseTranslationItem(string title)
            {
                Title = title;
            }
        }

        private class PageItem : BaseTranslationItem
        {
            public IEnumerable<QuestionItem> Questions { get; set; }

            public PageItem(string title)
                : base(title)
            {
                Title = title;
            }
        }

        private class QuestionItem : BaseTranslationItem
        {
            public IEnumerable<OptionListItem> OptionLists { get; set; }

            public IEnumerable<LikertScaleGroup> LikertScales { get; set; }

            public QuestionItem(SurveyQuestion question)
                : base(null)
            {
                var indicatorPreview = question.Code.HasValue() ? question.Code : question.Sequence.ToString();
                var indicatorStyle = question.GetIndicatorStyleName();

                Title = $"<span class=\"badge bg-{indicatorStyle}\">{indicatorPreview}</span>";
            }
        }

        private class OptionListItem : BaseTranslationItem
        {
            public bool FieldVisible { get; set; }

            public IEnumerable<BaseTranslationItem> Options { get; set; }

            public OptionListItem(string title)
                : base(title)
            {
                Title = title;
            }
        }

        private class LikertScaleGroup
        {
            public string Category { get; set; }

            public IEnumerable<LikertScaleItem> Items { get; set; }

            public LikertScaleGroup(SurveyScale scale)
            {
                Category = scale.Category;
            }
        }

        private class LikertScaleItem : BaseTranslationItem
        {
            public LikertScaleItem(string title)
                : base(title)
            {

            }
        }

        #endregion

        #region Properties

        protected Guid? SurveyID => Guid.TryParse(Request.QueryString["form"], out var value) ? value : (Guid?)null;

        protected SurveyForm Survey
        {
            get
            {
                if (!_isFormInited)
                {
                    _surveyForm = SurveyID.HasValue ? ServiceLocator.SurveySearch.GetSurveyState(SurveyID.Value)?.Form : null;

                    _isFormInited = true;
                }

                return _surveyForm;
            }
        }

        private FieldKey CurrentField
        {
            get => (FieldKey)ViewState[nameof(CurrentField)];
            set => ViewState[nameof(CurrentField)] = value;
        }

        private Dictionary<Guid, LikertScaleDataItem> LikertScaleItemMapping
        {
            get => (Dictionary<Guid, LikertScaleDataItem>)ViewState[nameof(LikertScaleItemMapping)];
            set => ViewState[nameof(LikertScaleItemMapping)] = value;
        }

        #endregion

        #region Fields

        private SurveyForm _surveyForm;
        private bool _isFormInited;

        protected bool _languageSelected;
        private static readonly HashSet<SurveyQuestionType> _excludeQuestions = new HashSet<SurveyQuestionType>
        {
            SurveyQuestionType.BreakPage,
            SurveyQuestionType.Terminate
        };

        #endregion

        #region Methods (initialization and loading)

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ToLanguage.AutoPostBack = true;
            ToLanguage.ValueChanged += ToLanguage_ValueChanged;

            PageRepeater.ItemCreated += PageRepeater_ItemCreated;
            PageRepeater.ItemDataBound += PageRepeater_ItemDataBound;

            TranslationWindow.LoadTranslation += TranslationWindow_LoadTranslation;
            TranslationWindow.SaveTranslation += TranslationWindow_SaveTranslation;

            UpdateButton.Click += UpdateButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (Survey == null
                || Survey.Tenant != Organization.OrganizationIdentifier
                || Survey.Locked.HasValue
                || !CurrentSessionState.Identity.IsGranted(Route.ToolkitName, DataAccess.Update)
                )
            {
                HttpResponseHelper.Redirect("/ui/admin/workflow/forms/search");
            }

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{Survey.Name} <span class='form-text'>Form #{Survey.Asset}</span>");

            if (!IsPostBack)
            {
                ToLanguage.Settings.ExcludeLanguage = new[] { Survey.Language };
                ToLanguage.RefreshData();

                CompareTranslations();
            }

            base.OnLoad(e);
        }

        #endregion

        #region Event handlers

        private void PageRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var questionRepeater = (Repeater)e.Item.FindControl("QuestionRepeater");
            questionRepeater.ItemCreated += QuestionRepeater_ItemCreated;
            questionRepeater.ItemDataBound += QuestionRepeater_ItemDataBound;
        }

        private void PageRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var item = (PageItem)e.Item.DataItem;

            var questionRepeater = (Repeater)e.Item.FindControl("QuestionRepeater");
            questionRepeater.DataSource = item.Questions;
            questionRepeater.DataBind();
        }

        private void QuestionRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var optionListRepeater = (Repeater)e.Item.FindControl("OptionListRepeater");
            optionListRepeater.ItemDataBound += OptionListRepeater_ItemDataBound;

            var likertScaleRepeater = (Repeater)e.Item.FindControl("LikertScaleRepeater");
            likertScaleRepeater.ItemDataBound += LikertScaleRepeater_ItemDataBound;
        }

        private void QuestionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var item = (QuestionItem)e.Item.DataItem;

            var optionListRepeater = (Repeater)e.Item.FindControl("OptionListRepeater");
            optionListRepeater.DataSource = item.OptionLists;
            optionListRepeater.DataBind();

            var likertScaleContainer = (Container)e.Item.FindControl("LikertScaleContainer");
            var likertScaleRepeater = (Repeater)e.Item.FindControl("LikertScaleRepeater");
            likertScaleRepeater.DataSource = item.LikertScales;
            likertScaleRepeater.DataBind();
            likertScaleContainer.Visible = likertScaleRepeater.Items.Count > 0;
        }

        private void OptionListRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var item = (OptionListItem)e.Item.DataItem;

            var optionRepeater = (Repeater)e.Item.FindControl("OptionRepeater");
            optionRepeater.DataSource = item.Options;
            optionRepeater.DataBind();
        }

        private void LikertScaleRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var info = (LikertScaleGroup)e.Item.DataItem;

            var itemRepeater = (Repeater)e.Item.FindControl("ItemRepeater");
            itemRepeater.DataSource = info.Items;
            itemRepeater.DataBind();
        }

        #endregion

        #region Methods (menus)

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline") ? $"form={SurveyID}" : null;
        }
        #endregion

        #region Methods (comparison)

        private void ToLanguage_ValueChanged(object sender, EventArgs e)
        {
            var fromLang = Survey.Language;
            var toLang = ToLanguage.Value;

            if (!string.IsNullOrEmpty(toLang) && SurveyID.HasValue)
            {
                var commands = new List<Command>();
                var data = new List<MultilingualString>();

                if (AddData(Survey.Content))
                    commands.Add(new Application.Surveys.Write.ChangeSurveyFormContent(Survey.Identifier, Survey.Content));

                foreach (var question in Survey.Questions)
                {
                    if (AddData(question.Content))
                        commands.Add(new Application.Surveys.Write.ChangeSurveyQuestionContent(Survey.Identifier, question.Identifier, question.Content));

                    foreach (var optionList in question.Options.Lists)
                    {
                        if (AddData(optionList.Content))
                            commands.Add(new Application.Surveys.Write.ChangeSurveyOptionListContent(Survey.Identifier, optionList.Identifier, optionList.Content, optionList.Category));

                        foreach (var optionItem in optionList.Items)
                        {
                            if (AddData(optionItem.Content))
                                commands.Add(new Application.Surveys.Write.ChangeSurveyOptionItemContent(Survey.Identifier, optionItem.Identifier, optionItem.Content));
                        }
                    }

                    foreach (var scale in question.Scales)
                    {
                        scale.SyncDefaultContent();

                        var hasTranslation = AddData(scale.Content);

                        foreach (var scaleItem in scale.Items)
                        {
                            if (AddData(scaleItem.Content))
                                hasTranslation = true;
                        }

                        if (hasTranslation)
                            commands.Add(new Application.Surveys.Write.ChangeSurveyScale(Survey.Identifier, question.Identifier, scale));
                    }
                }

                Translate(fromLang, toLang, data);

                foreach (var command in commands)
                    ServiceLocator.SendCommand(command);

                bool AddData(ContentContainer content)
                {
                    if (content == null)
                        return false;

                    var isUpdated = false;

                    foreach (var item in content.GetItems())
                    {
                        if (item.Text[fromLang].IsEmpty() || item.Text[toLang].IsNotEmpty())
                            continue;

                        data.Add(item.Text);

                        isUpdated = true;
                    }

                    return isUpdated;
                }

                if (commands.Count > 0)
                {
                    var fromName = Language.GetEnglishName(Survey.Language);
                    var toName = Language.GetEnglishName(ToLanguage.Value);

                    ComparisonStatus.AddMessage(AlertType.Success, $"Google Translate has been used to translate your from {fromName} to {toName}. Please review and revise the translation as needed.");

                    if (Survey.LanguageTranslations == null || !Survey.LanguageTranslations.Contains(ToLanguage.Value))
                    {
                        var translations = Survey.LanguageTranslations != null
                            ? Survey.LanguageTranslations.Append(ToLanguage.Value).Distinct().OrderBy(x => x).ToArray()
                            : new string[] { ToLanguage.Value };

                        var command = new Application.Surveys.Write.ChangeSurveyFormLanguages(Survey.Identifier, Survey.Language, translations);

                        ServiceLocator.SendCommand(command);
                    }
                }
            }

            CompareTranslations();
        }

        private void CompareTranslations()
        {
            var survey = Survey;
            if (survey == null)
                return;

            var fromLang = survey.Language.IfNullOrEmpty("en");
            var toLang = ToLanguage.Value;

            FromLanguage.Text = Language.GetEnglishName(fromLang);
            _languageSelected = !string.IsNullOrEmpty(toLang);

            SurveyFieldRepeater.DataSource = SurveyForm.ContentLabels.Select(x =>
            {
                var item = new BaseTranslationItem(x.Name);

                BindDataItem(item, x.Name, survey.Content, ContainerType.Survey, survey.Identifier);

                return item;
            });
            SurveyFieldRepeater.DataBind();

            LikertScaleItemMapping = new Dictionary<Guid, LikertScaleDataItem>();

            int pageCount = 0, questionCount = 0, optionCount = 0;

            PageRepeater.DataSource = survey.GetPages()
                .Select(p =>
                {
                    pageCount++;

                    return new PageItem(p.Sequence.ToString())
                    {
                        Questions = p.Questions.Where(x => !_excludeQuestions.Contains(x.Type)).Select(question =>
                        {
                            questionCount++;

                            var questionItem = new QuestionItem(question);

                            BindDataItem(questionItem, "Title", question.Content, ContainerType.Question, question.Identifier);

                            questionItem.OptionLists = question.Options.Lists.Select(list =>
                            {
                                var listItem = new OptionListItem(list.Sequence.ToString())
                                {
                                    FieldVisible = question.Type == SurveyQuestionType.Likert
                                };

                                if (listItem.FieldVisible)
                                    BindDataItem(listItem, "Title", list.Content, ContainerType.OptionList, list.Identifier);

                                listItem.Options = list.Items.Select(item =>
                                {
                                    optionCount++;

                                    var optionItem = new BaseTranslationItem(item.Sequence.ToString());

                                    BindDataItem(optionItem, "Title", item.Content, ContainerType.Option, item.Identifier);

                                    return optionItem;
                                });

                                return listItem;
                            });

                            questionItem.LikertScales = question.Scales.Where(x => x.Items.IsNotEmpty()).Select(scale => new LikertScaleGroup(scale)
                            {
                                Items = GetLikertScaleItems(question, scale).ToList()
                            });

                            return questionItem;
                        })
                    };
                })
                .Where(x => x.Questions.Any());
            PageRepeater.DataBind();

            PageCount.Visible = true;
            PageCount.Text = "Page".ToQuantity(pageCount);
            QuestionCount.Text = "Question".ToQuantity(questionCount);
            FieldCount.Text = "Option".ToQuantity(optionCount);

            IEnumerable<LikertScaleItem> GetLikertScaleItems(SurveyQuestion question, SurveyScale scale)
            {
                scale.SyncDefaultContent();

                var scaleId = Guid.NewGuid();
                var scaleItem = new LikertScaleItem(SurveyScale.CategoryLabel);

                BindDataItem(scaleItem, SurveyScale.CategoryLabel, scale.Content, ContainerType.LikertScale, scaleId);

                LikertScaleItemMapping.Add(
                    scaleId,
                    new LikertScaleDataItem(question.Identifier, scale.Category, -1, scale.Content));

                yield return scaleItem;

                for (var i = 0; i < scale.Items.Count; i++)
                {
                    var item = scale.Items[i];
                    var letter = Calculator.ToBase26(i + 1);
                    var itemId = Guid.NewGuid();

                    LikertScaleItemMapping.Add(
                        itemId,
                        new LikertScaleDataItem(question.Identifier, scale.Category, i, item.Content));

                    var gradeItem = new LikertScaleItem($"{letter}. Grade");
                    BindDataItem(gradeItem, SurveyScaleItem.GradeLabel, item.Content, ContainerType.LikertScaleItem, itemId);
                    yield return gradeItem;

                    var feedbackItem = new LikertScaleItem($"{letter}. Feedback");
                    BindDataItem(feedbackItem, "Description", item.Content, ContainerType.LikertScaleItem, itemId);
                    yield return feedbackItem;
                }
            }

            void BindDataItem(BaseTranslationItem item, string label, ContentContainer content, ContainerType container, Guid containerId)
            {
                item.FromText = content?.GetText(label, fromLang);

                if (_languageSelected)
                {
                    var fieldKey = FieldKey.Serialize(containerId, container, label);

                    item.ToText = content?.GetText(label, toLang);
                    item.OnClick = TranslationWindow.GetOpenScript(fieldKey) + " return false;";
                }
            }
        }

        #endregion

        #region Methods (editing)

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            _isFormInited = false;

            CurrentField = null;

            CompareTranslations();
        }

        private void TranslationWindow_LoadTranslation(object sender, TranslationWindow.LoadEventArgs e)
        {
            if (Survey == null)
                return;

            CurrentField = FieldKey.Deserialize(e.Key);

            var content = IsLikertContainer(CurrentField.Container)
                ? LikertScaleItemMapping[CurrentField.EntityID].Content
                : ServiceLocator.ContentSearch.GetBlock(CurrentField.EntityID, labels: new[] { CurrentField.Field });

            e.Translation = content[CurrentField.Field].Text.Serialize();
            e.Language = ToLanguage.Value;
            e.AllowHtml = !IsLikertPlainTextField(CurrentField);
        }

        private void TranslationWindow_SaveTranslation(object sender, StringValueArgs e)
        {
            if (SurveyID.HasValue && CurrentField != null)
            {
                var content = IsLikertContainer(CurrentField.Container)
                    ? LikertScaleItemMapping[CurrentField.EntityID].Content
                    : ServiceLocator.ContentSearch.GetBlock(CurrentField.EntityID);

                content[CurrentField.Field].Text = MultilingualString.Deserialize(e.Value);

                var cmd = CreateSaveCommand(content);
                if (cmd != null)
                    ServiceLocator.SendCommand(cmd);
            }

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(Translate),
                "update",
                TranslationWindow.GetCloseScript() + $" $('#{UpdateButton.ClientID}').click();",
                true);
        }

        private ICommand CreateSaveCommand(ContentContainer content)
        {
            switch (CurrentField.Container)
            {
                case ContainerType.Survey:
                    return new Application.Surveys.Write.ChangeSurveyFormContent(SurveyID.Value, content);

                case ContainerType.Question:
                    return new Application.Surveys.Write.ChangeSurveyQuestionContent(SurveyID.Value, CurrentField.EntityID, content);

                case ContainerType.OptionList:
                    var category = Survey.FindOptionList(CurrentField.EntityID)?.Category;
                    return new Application.Surveys.Write.ChangeSurveyOptionListContent(SurveyID.Value, CurrentField.EntityID, content, category);

                case ContainerType.Option:
                    return new Application.Surveys.Write.ChangeSurveyOptionItemContent(SurveyID.Value, CurrentField.EntityID, content);

                case ContainerType.LikertScaleItem:
                case ContainerType.LikertScale:
                    return CreateLikertScaleSaveCommand(content);

                default:
                    throw new NotImplementedException("Invalid container type: " + CurrentField.Container.GetName());
            }
        }

        private ICommand CreateLikertScaleSaveCommand(ContentContainer content)
        {
            var scaleItemData = LikertScaleItemMapping[CurrentField.EntityID];
            var question = Survey.FindQuestion(scaleItemData.QuestionId);

            if (question.Scales == null)
                return null;

            var scale = question.Scales.FirstOrDefault(x => x.Category == scaleItemData.ScaleCategory);
            if (scale == null)
                return null;

            if (CurrentField.Container == ContainerType.LikertScale)
                scale.Content = content;
            else if (scale.Items.Count != 0 && scaleItemData.ItemIndex < scale.Items.Count)
                scale.Items[scaleItemData.ItemIndex].Content = content;
            else
                return null;

            return new Application.Surveys.Write.ChangeSurveyScale(Survey.Identifier, question.Identifier, scale);
        }

        private static bool IsLikertContainer(ContainerType container) =>
            container == ContainerType.LikertScaleItem || container == ContainerType.LikertScale;

        private static bool IsLikertPlainTextField(FieldKey field) =>
               field.Container == ContainerType.LikertScale
            || field.Container == ContainerType.LikertScaleItem && field.Field == SurveyScaleItem.GradeLabel;

        #endregion
    }
}
