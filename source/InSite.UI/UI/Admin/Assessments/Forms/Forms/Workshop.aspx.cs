using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Forms.Controls;
using InSite.Admin.Assessments.Questions.Controls;
using InSite.Admin.Assessments.Questions.Utilities;
using InSite.Application.Banks.Read;
using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Assessments.Forms.Forms
{
    public partial class Workshop : AdminBasePage, IHasParentLinkParameters
    {
        #region Classes

        [Serializable]
        private class QuestionFilterInternal : QuestionFilter
        {
            public DateRangeShortcut? DateRangeShortcut { get; set; }

            public DateTimeRange DateRange { get; set; }

            #region Initialization

            static QuestionFilterInternal()
            {
                QuestionFilterSerializer.Register<QuestionFilterInternal>(63, ReadInternal, WriteInternal);
            }

            #endregion

            #region Methods (serialization)

            private static void WriteInternal(QuestionFilter filter, BinaryWriter writer)
            {
                var @internal = (QuestionFilterInternal)filter;

                Write(filter, writer);

                writer.Write(@internal.DateRangeShortcut.HasValue ? (byte)@internal.DateRangeShortcut.Value : byte.MaxValue);
                writer.WriteNullable(@internal.DateRange?.Since);
                writer.WriteNullable(@internal.DateRange?.Before);
            }

            private static QuestionFilter ReadInternal(BinaryReader reader)
            {
                var filter = new QuestionFilterInternal();

                Read(filter, reader);

                var shortcut = reader.ReadByte();
                var since = reader.ReadDateTimeNullable();
                var before = reader.ReadDateTimeNullable();

                filter.DateRangeShortcut = shortcut == byte.MaxValue ? null : (DateRangeShortcut?)shortcut;
                filter.DateRange = since.HasValue || before.HasValue
                    ? new DateTimeRange { Since = since, Before = before }
                    : null;

                return filter;
            }

            #endregion
        }

        #endregion

        #region Constants

        private const long QuestionsChangesDatesPrecision = TimeSpan.TicksPerDay;

        #endregion

        #region Properties

        protected Guid BankID => ParseGuid(Request.QueryString["bank"], Guid.Empty).Value;

        private Guid FormID => ParseGuid(Request.QueryString["form"], Guid.Empty).Value;

        private Guid? SectionID => ParseGuid(Request.QueryString["section"]);

        private BankState _bank;
        private BankState Bank => _bank ?? (_bank = ServiceLocator.BankSearch.GetBankState(BankID));

        private Form _form;
        private Form BankForm => _form ?? (_form = Bank?.FindForm(FormID));

        private Dictionary<Guid, Guid[]> SectionStandardMapping
        {
            get => (Dictionary<Guid, Guid[]>)ViewState[nameof(SectionStandardMapping)];
            set => ViewState[nameof(SectionStandardMapping)] = value;
        }

        private QuestionFilter CurrentQuestionFilter
        {
            get => (QuestionFilter)ViewState[nameof(CurrentQuestionFilter)];
            set => ViewState[nameof(CurrentQuestionFilter)] = value;
        }

        private Tuple<DateTime, Guid[]>[] QuestionsChangesDates
        {
            get
            {
                var result = (Tuple<DateTime, Guid[]>[])ViewState[nameof(QuestionsChangesDates)];

                if (result == null)
                {
                    ViewState[nameof(QuestionsChangesDates)] = result = ServiceLocator.BankSearch
                        .GetQuestions(new QBankQuestionFilter { OrganizationIdentifier = Organization.Identifier, BankIdentifier = BankID })
                        .Where(q => q.LastChangeTime.HasValue)
                        .GroupBy(q =>
                        {
                            var userDate = TimeZoneInfo.ConvertTime(q.LastChangeTime.Value, User.TimeZone).DateTime;
                            return Clock.Trim(userDate, QuestionsChangesDatesPrecision);
                        })
                        .Select(
                            group => new Tuple<DateTime, Guid[]>(
                                group.Key,
                                group
                                    .Select(q => q.QuestionIdentifier)
                                    .Distinct()
                                    .ToArray()
                            )
                        )
                        .ToArray();
                }

                return result;
            }
        }

        #endregion

        #region Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SectionSelector.AutoPostBack = true;
            SectionSelector.ValueChanged += SectionSelector_ValueChanged;

            CompetencySelector.AutoPostBack = true;
            CompetencySelector.ValueChanged += CompetencySelector_ValueChanged;

            QuestionRepeater.NeedRefresh += QuestionRepeater_NeedRefresh;
            QuestionRepeater.QuestionReplace += QuestionRepeater_QuestionReplace;

            ApplyFilterButton.Click += ApplyFilterButton_Click;
            ClearFilterButton.Click += ClearFilterButton_Click;
            FormDetails.HideLink();
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!CanEdit)
                RedirectToSearch();

            HandleAjaxRequest();

            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (Bank == null || Bank.Tenant != Organization.OrganizationIdentifier)
                RedirectToSearch();

            if (BankForm == null || !FormDetails.SetInputValues(BankForm, new ReturnUrl()))
                RedirectToOutline();

            PageHelper.AutoBindHeader(
                this,
                qualifier: (BankForm.Content.Title?.Default).IfNullOrEmpty(BankForm.Name));

            CloseButton.NavigateUrl = GetOutlineUrl(FormID, SectionID);

            WorkshopScript.LoadData(Bank);

            ProblemRepeater.BindModelToControls(BankForm);

            if (Request.QueryString["panel"] == "questions")
                QuestionsTab.IsSelected = true;
            else if (Request.QueryString["panel"] == "comments")
                CommentsTab.IsSelected = true;

            FormDetails.AllowEdit = false;
            FormDetails.VisibleFields = new[]
            {
                DetailsField.Specification,
                DetailsField.Standard,
                DetailsField.Name,
                DetailsField.Asset,
                DetailsField.Code,
                DetailsField.Source,
                DetailsField.PublicationStatus
            };

            StatisticsPanel.LoadData(BankForm);

            {
                var returnUrl = new ReturnUrl("bank&form&panel=comments");
                var comments = CommentRepeater.LoadData(Bank, new[] { Bank.Identifier, BankForm.Identifier }, returnUrl);

                CommentsTab.SetTitle("Comments", comments);

                AddCommentLink.NavigateUrl = returnUrl.GetRedirectUrl($"/ui/admin/assessments/comments/author?bank={Bank.Identifier}&form={FormID}");
            }

            {
                var hasData = Bank.Attachments.Count > 0;
                var returnUrl = new ReturnUrl("bank&form&attachment&panel=attachments");

                FilterAttachmentsTextBox.Visible = hasData;

                AttachmentsNav.Visible = hasData;
                AttachmentsNav.LoadData(Bank.Identifier, Bank.Attachments, returnUrl, true);
                AttachmentsTab.SetTitle("Attachments", Bank.Attachments.Count);

                if (!IsPostBack)
                {
                    var isSelected = false;

                    if (Guid.TryParse(Request.QueryString["attachment"], out var attachmentId))
                    {
                        var attachment = Bank.FindAttachment(attachmentId);
                        if (attachment != null)
                        {
                            if (attachment.Type == AttachmentType.Image)
                                isSelected = AttachmentsNav.SelectTab(TabType.Image);
                            else if (attachment.Type == AttachmentType.Document)
                                isSelected = AttachmentsNav.SelectTab(TabType.Document);
                            else
                                isSelected = AttachmentsNav.SelectTab(TabType.Other);
                        }
                    }

                    if (!isSelected && Request.QueryString["panel"] == "attachments")
                    {
                        isSelected = true;

                        var tab = Request.QueryString["tab"];
                        if (!string.IsNullOrEmpty(tab))
                        {
                            if (tab == "images")
                                AttachmentsNav.SelectTab(TabType.Image);
                            else if (tab == "documents")
                                AttachmentsNav.SelectTab(TabType.Document);
                            else if (tab == "other")
                                AttachmentsNav.SelectTab(TabType.Other);
                        }
                    }

                    if (isSelected)
                        AttachmentsTab.IsSelected = true;
                }
            }

            var hasSections = BankForm.Sections.Count > 0;

            QuestionCompetencyRow.Visible = hasSections;
            NoQuestionsMessage.Visible = !hasSections;

            if (!hasSections)
                QuestionRepeater.Hide();

            QuestionsHeader.Visible = hasSections;

            Guid? competencyId = null;

            if (hasSections)
            {
                var questionCount = 0;

                {
                    SectionStandardMapping = new Dictionary<Guid, Guid[]>();

                    SectionSelector.Items.Clear();

                    foreach (var section in BankForm.Sections.OrderBy(x => x.Sequence))
                    {
                        questionCount += section.Fields.Count;

                        SectionSelector.Items.Add(new ComboBoxOption(section.Criterion.Title, section.Identifier.ToString()));
                        SectionStandardMapping.Add(section.Identifier, section.Criterion.Sets.Select(x => x.Standard).ToArray());
                    }
                }

                QuestionsTab.SetTitle("Questions", questionCount);

                var sectionId = ParseGuid(Request.QueryString["section"]);
                var questionId = ParseGuid(Request.QueryString["question"]);

                if (questionId.HasValue)
                {
                    var field = BankForm.Sections.SelectMany(x => x.Fields).FirstOrDefault(x => x.QuestionIdentifier == questionId.Value);

                    if (field != null)
                    {
                        sectionId = field.Section.Identifier;
                        competencyId = field.Question.Standard == Guid.Empty ? (Guid?)null : field.Question.Standard;

                        ScriptManager.RegisterStartupScript(
                            Page,
                            GetType(),
                            "scrollto_question",
                            $"$(document).ready(function() {{ workshopQuestionRepeater.scrollToQuestion('{field.Question.Identifier}'); }});",
                            true);
                    }
                }

                if (sectionId.HasValue)
                {
                    var sectionItem = SectionSelector.FindOptionByValue(sectionId.Value.ToString(), true);
                    if (sectionItem != null)
                        sectionItem.Selected = true;
                }
            }

            OnSectionSelected(competencyId);

            QuestionDateRangeSelector.LoadItems(
                DateRangeShortcut.Today,
                DateRangeShortcut.Yesterday,
                DateRangeShortcut.ThisWeek,
                DateRangeShortcut.LastWeek,
                DateRangeShortcut.ThisMonth,
                DateRangeShortcut.LastMonth,
                DateRangeShortcut.ThisYear,
                DateRangeShortcut.LastYear);
            QuestionDateRangeSelector.Items.Add(new ComboBoxOption("Custom Dates", "Custom"));

            var filter = QuestionsTab.IsSelected
                ? QuestionFilterSerializer.Deserialize(Request.QueryString["filter"])
                : null;

            if (filter != null)
            {
                if (competencyId.HasValue)
                    filter.StandardIdentifier = competencyId.Value;

                SetQuestionFilter(filter);
            }

            LoadQuestionRepeater();
        }

        #endregion

        #region Event handlers

        private void ApplyFilterButton_Click(object sender, EventArgs e) => LoadQuestionRepeater();

        private void ClearFilterButton_Click(object sender, EventArgs e)
        {
            ClearQuestionFilter();
            LoadQuestionRepeater();
        }

        private void QuestionRepeater_NeedRefresh(object sender, EventArgs e) => LoadQuestionRepeater();

        private void SectionSelector_ValueChanged(object sender, EventArgs e)
        {
            OnSectionSelected(null);
            LoadQuestionRepeater();
        }

        private void CompetencySelector_ValueChanged(object sender, EventArgs e) => LoadQuestionRepeater();

        private void OnSectionSelected(Guid? competency)
        {
            var sectionId = SectionSelector.ValueAsGuid;
            var hasStandards = sectionId.HasValue && SectionStandardMapping.ContainsKey(sectionId.Value);
            var standardsIds = hasStandards ? SectionStandardMapping[sectionId.Value] : null;

            CompetencySelector.ValueAsGuid = null;
            CompetencySelector.ListFilter.ParentStandardIdentifiers = hasStandards ? standardsIds : new[] { Guid.Empty };
            CompetencySelector.Enabled = hasStandards;

            if (hasStandards)
            {
                CompetencySelector.RefreshData();

                if (competency != null)
                    CompetencySelector.ValueAsGuid = competency;
                else if (CompetencySelector.Items.Count > 1)
                    ((IComboBoxOption)CompetencySelector.Items[1]).Selected = true;
            }
        }

        private void HandleAjaxRequest()
        {
            if (!HttpRequestHelper.IsAjaxRequest || !bool.TryParse(Page.Request.Form["isPageAjax"], out var isAjax) || !isAjax)
                return;

            var bankId = Guid.Parse(Page.Request.Form["bankId"]);
            var fieldId = Guid.Parse(Page.Request.Form["fieldId"]);
            var authorType = Page.Request.Form["authorType"];
            var flag = Page.Request.Form["flag"].ToEnum<FlagType>();
            var text = Page.Request.Form["text"];

            if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(authorType) && CommentAuthorTypeComboBox.IsValidValue(authorType))
            {
                ServiceLocator.SendCommand(new PostComment(
                    bankId,
                    UniqueIdentifier.Create(),
                    flag,
                    CommentType.Field,
                    fieldId,
                    User.UserIdentifier,
                    authorType,
                    null,
                    text,
                    null, null, null,
                    DateTimeOffset.UtcNow));
            }

            var bank = ServiceLocator.BankSearch.GetBankState(bankId);
            var field = bank.FindField(fieldId);

            var returnQuery = $"bank&form&panel=questions";
            if (CurrentQuestionFilter?.IsEmpty == false)
                returnQuery += "&filter=" + QuestionFilterSerializer.Serialize(CurrentQuestionFilter);

            var commentRepeater = (WorkshopQuestionCommentRepeater)LoadControl("~/UI/Admin/Assessments/Questions/Controls/WorkshopQuestionCommentRepeater.ascx");
            commentRepeater.LoadFormData(FormID, field.Question, new ReturnUrl(returnQuery));

            var result = new StringBuilder();

            using (var writer = new StringWriter(result))
            {
                var htmlWriter = new HtmlTextWriter(writer);
                commentRepeater.RenderControl(htmlWriter);
            }

            Response.Clear();
            Response.Write(result.ToString());
            Response.End();
        }

        private void QuestionRepeater_QuestionReplace(object sender, CommandEventArgs e)
        {
            ReplaceQuestion((Guid)e.CommandArgument, e.CommandName);
            LoadQuestionRepeater();
        }

        private void ReplaceQuestion(Guid questionId, string command)
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                return;

            var form = bank.FindForm(FormID);
            if (form == null)
                return;

            var sectionId = SectionSelector.ValueAsGuid;
            var section = form.Sections.FirstOrDefault(x => x.Identifier == sectionId);
            if (section == null)
                return;

            var field = section.Fields.FirstOrDefault(x => x.QuestionIdentifier == questionId);
            if (field == null)
                return;

            var question = field.Question;

            var newQuestionId = UniqueIdentifier.Create();
            var newFieldId = UniqueIdentifier.Create();
            var fieldIndex = section.Fields.IndexOf(field);

            string newCondition;
            var isNewQuestion = false;

            switch (command)
            {
                case "NewVersion":
                    if (question.PublicationStatus != PublicationStatus.Published || !question.IsLastVersion())
                        return;

                    ServiceLocator.SendCommand(new UpgradeQuestion(bank.Identifier, questionId, newQuestionId));
                    newCondition = null;
                    break;
                case "NewQuestionAndSurplus":
                    ServiceLocator.SendCommand(new DuplicateQuestion(bank.Identifier, questionId, newQuestionId, Sequence.Increment(bank.Tenant, SequenceType.Asset)));
                    newCondition = "Surplus";
                    isNewQuestion = true;
                    break;
                case "NewQuestionAndPurge":
                    ServiceLocator.SendCommand(new DuplicateQuestion(bank.Identifier, questionId, newQuestionId, Sequence.Increment(bank.Tenant, SequenceType.Asset)));
                    newCondition = "Purge";
                    isNewQuestion = true;
                    break;
                case "RollbackQuestion":
                    if (question.AssetVersion <= 0 || question.IsFirstVersion())
                        return;

                    newQuestionId = question.PreviousVersion.Identifier;
                    newCondition = "Surplus";
                    break;
                default:
                    throw ApplicationError.Create("Command is not supported: {0}", command);
            }

            ServiceLocator.SendCommand(new DeleteField(bank.Identifier, field.Identifier, form.Identifier, questionId));
            ServiceLocator.SendCommand(new AddField(bank.Identifier, newFieldId, field.Section.Identifier, newQuestionId, fieldIndex));

            if (newCondition != null)
                ServiceLocator.SendCommand(new ChangeQuestionCondition(bank.Identifier, questionId, newCondition));

            if (isNewQuestion)
            {
                ServiceLocator.SendCommand(new ChangeQuestionFlag(bank.Identifier, newQuestionId, FlagType.None));

                var classification = question.Classification.Clone();
                classification.LikeItemGroup = null;
                classification.Reference = null;
                classification.Code = null;
                classification.Tag = null;
                ServiceLocator.SendCommand(new ChangeQuestionClassification(BankID, newQuestionId, classification));
            }
        }

        #endregion

        #region Database operations

        private void LoadQuestionRepeater()
        {
            var hasData = false;
            var hasQuestions = false;
            var sectionId = SectionSelector.ValueAsGuid;
            var section = sectionId.HasValue
                ? BankForm.Sections.SingleOrDefault(x => x.Identifier == sectionId.Value)
                : null;

            if (section != null)
            {
                CurrentQuestionFilter = GetQuestionFilter();

                var filterFunc = CurrentQuestionFilter.BuildExpression().Compile();
                var dataItemsQuery = section.Fields.AsQueryable().Select(x => new WorkshopQuestionRepeater.DataItem(x)).Where(x => filterFunc(x.Entity));

                if (CurrentQuestionFilter is QuestionFilterInternal intFilter)
                {
                    var range = intFilter.DateRangeShortcut.HasValue
                        ? Shift.Common.Calendar.GetDateTimeRange(intFilter.DateRangeShortcut.Value)
                        : new DateTimeRange(intFilter.DateRange?.Since, intFilter.DateRange?.Before);

                    if (!range.IsEmpty)
                    {
                        var query = QuestionsChangesDates.AsQueryable();

                        if (range.Since.HasValue)
                        {
                            var sinceDate = Clock.Trim(range.Since.Value, QuestionsChangesDatesPrecision);
                            query = query.Where(x => x.Item1 >= sinceDate);
                        }

                        if (range.Before.HasValue)
                        {
                            var beforeDate = Clock.Trim(range.Before.Value, QuestionsChangesDatesPrecision);
                            query = query.Where(x => x.Item1 <= beforeDate);
                        }

                        var ids = query.SelectMany(x => x.Item2).ToHashSet();

                        dataItemsQuery = dataItemsQuery.Where(x => ids.Contains(x.Entity.Identifier));
                    }
                }

                var dataItems = dataItemsQuery.ToArray();

                hasData = section.Fields.Count > 0;
                hasQuestions = dataItems.Length > 0;

                QuestionCount.Text = dataItems.Length.ToString("n0");

                var returnQuery = "bank&form&panel=questions";

                if (CurrentQuestionFilter?.IsEmpty == false)
                    returnQuery += "&filter=" + QuestionFilterSerializer.Serialize(CurrentQuestionFilter);

                QuestionRepeater.LoadData(Bank, section, dataItems, new ReturnUrl(returnQuery));
            }
            else
            {
                CurrentQuestionFilter = null;
            }

            if (!hasQuestions)
                QuestionRepeater.Hide();

            QuestionsHeader.Visible = hasQuestions;
            QuestionFilter.Visible = hasData;
        }

        #endregion

        #region Methods (question filter)

        private void SetQuestionFilter(QuestionFilter filter)
        {
            CompetencySelector.ValueAsGuid = filter.StandardIdentifier == Guid.Empty ? null : filter.StandardIdentifier;
            QuestionTaxonomy.ValueAsInt = filter.Taxonomy;
            IsQuestionHasLig.ValueAsBoolean = filter.HasLig;
            IsQuestionHasReference.ValueAsBoolean = filter.HasReference;

            QuestionFlag.EnsureDataBound();
            QuestionFlag.EnumValues = filter.Flag;

            QuestionCondition.EnsureDataBound();
            QuestionCondition.Values = filter.Condition;

            if (filter is QuestionFilterInternal intFilter)
            {
                QuestionDateRangeSince.Value = null;
                QuestionDateRangeBefore.Value = null;

                if (intFilter.DateRangeShortcut.HasValue)
                {
                    QuestionDateRangeSelector.Value = intFilter.DateRangeShortcut.Value.GetName();
                }
                else if (intFilter.DateRange?.IsEmpty == false)
                {
                    QuestionDateRangeSelector.Value = "Custom";
                    QuestionDateRangeSince.Value = intFilter.DateRange.Since;
                    QuestionDateRangeBefore.Value = intFilter.DateRange.Before;
                }
            }
        }

        private QuestionFilter GetQuestionFilter()
        {
            var flag = QuestionFlag.EnumValues.ToHashSet();
            var condition = QuestionCondition.Values.ToHashSet();

            var result = new QuestionFilterInternal
            {
                StandardIdentifier = CompetencySelector.ValueAsGuid ?? Guid.Empty,
                Flag = flag.Count > 0 ? flag : null,
                Condition = condition.Count > 0 ? condition : null,
                Taxonomy = QuestionTaxonomy.ValueAsInt,
                HasLig = IsQuestionHasLig.ValueAsBoolean,
                HasReference = IsQuestionHasReference.ValueAsBoolean
            };

            var questionDateRange = QuestionDateRangeSelector.Value;

            if (questionDateRange.IsNotEmpty())
            {
                if (questionDateRange == "Custom")
                    result.DateRange = new DateTimeRange
                    {
                        Since = QuestionDateRangeSince.Value,
                        Before = QuestionDateRangeBefore.Value,
                    };
                else
                    result.DateRangeShortcut = questionDateRange.ToEnum<DateRangeShortcut>();
            }

            return result;
        }

        private void ClearQuestionFilter()
        {
            QuestionFlag.ClearSelection();
            QuestionCondition.ClearSelection();
            QuestionTaxonomy.ClearSelection();
            IsQuestionHasLig.ClearSelection();
            IsQuestionHasReference.ClearSelection();
            QuestionDateRangeSelector.ClearSelection();
            QuestionDateRangeSince.Value = null;
            QuestionDateRangeBefore.Value = null;
        }

        #endregion

        #region Methods (redirect)

        private static void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToOutline(Guid? formId = null, Guid? sectionId = null)
        {
            var url = GetOutlineUrl(formId, sectionId);

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetOutlineUrl(Guid? formId = null, Guid? sectionId = null)
        {
            return $"/ui/admin/assessments/banks/outline?" + GetOutlineParams(formId, sectionId);
        }

        private string GetOutlineParams(Guid? formId = null, Guid? sectionId = null)
        {
            var parameters = "bank=" + BankID;

            if (formId.HasValue)
                parameters += $"&form={formId.Value}";

            if (sectionId.HasValue)
                parameters += $"&section={sectionId.Value}";

            return parameters;
        }

        #endregion

        #region IHasParentLinkParameters

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? GetOutlineParams(FormID, SectionID)
                : null;
        }

        #endregion

        #region Methods (helpers)

        private Guid? ParseGuid(string value, Guid? defaultValue = null) =>
            Guid.TryParse(value, out var result) ? result : defaultValue;

        #endregion
    }
}