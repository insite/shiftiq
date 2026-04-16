using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Attachments.Controls;
using InSite.Admin.Assessments.Questions.Controls;
using InSite.Admin.Assessments.Questions.Utilities;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Forms.Forms
{
    public partial class Print : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid FormID => Guid.TryParse(Request.QueryString["form"], out var value) ? value : Guid.Empty;

        private List<Guid> CompetencyData
        {
            get => (List<Guid>)ViewState[nameof(CompetencyData)];
            set => ViewState[nameof(CompetencyData)] = value;
        }

        #endregion

        #region Fields

        private static readonly string _queueStoragePath;

        #endregion

        #region Construction

        static Print()
        {
            _queueStoragePath = PrintQueue.GetStoragePath("Assessments.Forms.Print");
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            HandleAjaxRequest();

            base.OnInit(e);

            BuildForm.Click += BuildForm_Click;
            BuildAddendum.Click += BuildAddendum_Click;
            BuildFormInternal.Click += BuildFormInternal_Click;
            BuildFormCompact.Click += BuildFormCompact_Click;

            DownloadButton.Click += DownloadButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                RedirectToSearch();

            if (!IsPostBack)
                Open();
        }

        protected override void OnPreRender(EventArgs e)
        {
            BuildingPdfPanel.Visible = PrintQueue.IsLocked(_queueStoragePath, User.UserIdentifier);

            base.OnPreRender(e);
        }

        private void HandleAjaxRequest()
        {
            if (!HttpRequestHelper.IsAjaxRequest || !bool.TryParse(Page.Request.Form["isPageAjax"], out var isAjax) || !isAjax)
                return;

            Response.Clear();
            Response.Write(PrintQueue.IsLocked(_queueStoragePath, User.UserIdentifier) ? "BUSY" : "DONE");
            Response.End();
        }

        #endregion

        #region Event handlers

        private void BuildForm_Click(object sender, EventArgs e)
        {
            PrintQueue.QueuePrint(
                _queueStoragePath,
                User.Identifier, FormID,
                new QuestionPrintExternal.FormOptions(Organization, FormID)
                {
                    IncludeImages = IncludeImages.Checked,
                    QuestionFilter = CreateQuestionFilter()
                },
                (user, options) => QuestionPrintExternal.RenderPdf(options));
        }

        private void BuildAddendum_Click(object sender, EventArgs e)
        {
            PrintQueue.QueuePrint(
                _queueStoragePath,
                User.Identifier, FormID,
                new ImagePrint.FormOptions(Organization, FormID, OutputType.Addendum),
                (user, options) => ImagePrint.RenderPdf(options));
        }

        private void BuildFormInternal_Click(object sender, EventArgs e)
        {
            PrintQueue.QueuePrint(
                _queueStoragePath,
                User.Identifier, FormID,
                new QuestionPrintInternal.FormOptions(Organization.Identifier, User.TimeZone, FormID)
                {
                    IncludeImages = IncludeImages.Checked,
                    IncludeAdminComments = IncludeAdminComments.Checked,
                    ExcludeHiddenComments = ExcludeHiddenComments.Checked,
                    QuestionFilter = CreateQuestionFilter()
                },
                (user, options) => QuestionPrintInternal.RenderPdf(options));
        }

        private void BuildFormCompact_Click(object sender, EventArgs e)
        {
            PrintQueue.QueuePrint(
                _queueStoragePath,
                User.Identifier, FormID,
                new QuestionPrintCompact.FormOptions(Organization, FormID)
                {
                    QuestionFilter = CreateQuestionFilter()
                },
                (user, options) => QuestionPrintCompact.RenderPdf(options));
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            var file = PrintQueue.GetPrintFile(_queueStoragePath, User.Identifier, FormID);
            if (file != null)
                Response.SendFile(file.Name, "pdf", file.Data);
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                RedirectToSearch();

            var form = bank.FindForm(FormID);
            if (form == null)
                RedirectToReader();

            SetInputValues(form);
        }

        #endregion

        #region Settings/getting input values

        private void SetInputValues(Form form)
        {
            var bank = form.Specification.Bank;

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{(form.Content.Title?.Default).IfNullOrEmpty(form.Name)} <span class=\"form-text\">Asset #{form.Asset}</span>");

            FormDetails.BindForm(form, BankID, bank.IsAdvanced);

            var hasQuestions = form.GetQuestions().Count > 0;
            BuildForm.Enabled = hasQuestions;
            BuildFormInternal.Enabled = hasQuestions;

            if (!form.Addendum.IsEmpty)
                BuildAddendum.Enabled = true;
            else
                BuildAddendum.Enabled = false;

            LoadCompetencies(bank.Standard);

            GoBackButton.NavigateUrl = GetReaderUrl(form.Identifier);
        }

        #endregion

        #region Data binding (Competency)

        private void LoadCompetencies(Guid frameworkId)
        {
            CompetencyField.Visible = false;

            if (frameworkId == Guid.Empty)
                return;

            var data = StandardSearch.Bind(
                a => new
                {
                    AreaId = a.StandardIdentifier,
                    Code = a.Code,
                    Title = a.ContentTitle,
                    Competencies = a.Children.OrderBy(c => c.Sequence).ThenBy(c => c.ContentTitle).Select(c => new
                    {
                        CompetencyId = c.StandardIdentifier,
                        Code = c.Code,
                        Title = c.ContentTitle,
                    })
                },
                a => a.ParentStandardIdentifier == frameworkId && a.Children.Any(),
                null, "Sequence,ContentTitle");

            if (data.IsEmpty())
                return;

            CompetencyField.Visible = true;

            AreaRepeater.DataBinding += AreaRepeater_DataBinding;
            AreaRepeater.ItemDataBound += AreaRepeater_ItemDataBound;
            AreaRepeater.DataSource = data;
            AreaRepeater.DataBind();
        }

        private void AreaRepeater_DataBinding(object sender, EventArgs e)
        {
            CompetencyData = new List<Guid>();
        }

        private void AreaRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var repeater = (Repeater)e.Item.FindControl("CompetencyRepeater");
            repeater.ItemDataBound += CompetencyRepeater_ItemDataBound;
            repeater.DataSource = DataBinder.Eval(e.Item.DataItem, "Competencies");
            repeater.DataBind();
        }

        private void CompetencyRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var competencyId = (Guid)DataBinder.Eval(e.Item.DataItem, "CompetencyId");
            CompetencyData.Add(competencyId);
        }

        protected string GetStandardTitle()
        {
            var dataItem = Page.GetDataItem();
            var code = (string)DataBinder.Eval(dataItem, "Code");
            var title = (string)DataBinder.Eval(dataItem, "Title");

            return (code.IsEmpty() ? string.Empty : code + ". ") + title;
        }

        private IEnumerable<Guid> GetSelectedCompetencies()
        {
            var index = 0;

            foreach (RepeaterItem areaItem in AreaRepeater.Items)
            {
                var competencyRepeater = (Repeater)areaItem.FindControl("CompetencyRepeater");
                foreach (RepeaterItem competencyItem in competencyRepeater.Items)
                {
                    var chk = (ICheckBox)competencyItem.FindControl("IsSelected");

                    if (chk.Checked)
                        yield return CompetencyData[index];

                    index++;
                }
            }
        }

        #endregion

        #region Methods (QuestionFilter)

        private QuestionPrintHelper.QuestionFilter CreateQuestionFilter()
        {
            return new QuestionPrintHelper.QuestionFilter
            {
                QuestionTaxonomy = QuestionTaxonomy.ValuesAsInt.ToHashSet(),
                QuestionCondition = QuestionCondition.Values.ToHashSet(),
                QuestionFlag = QuestionFlag.EnumValues.ToHashSet(),
                QuestionCompetency = GetSelectedCompetencies().ToHashSet(),
                IsQuestionHasLig = IsQuestionHasLig.ValueAsBoolean,
                IsQuestionHasReference = IsQuestionHasReference.ValueAsBoolean,
            };
        }

        #endregion

        #region Methods (redirect)

        private static void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToReader(Guid? formId = null)
        {
            var url = GetReaderUrl(formId);

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetReaderUrl(Guid? formId = null)
        {
            var url = $"/ui/admin/assessments/banks/outline?bank={BankID}";

            if (formId.HasValue)
                url += $"&form={formId.Value}";

            return url;
        }

        #endregion

        #region IHasParentLinkParameters

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"bank={BankID}"
                : null;
        }

        #endregion
    }
}