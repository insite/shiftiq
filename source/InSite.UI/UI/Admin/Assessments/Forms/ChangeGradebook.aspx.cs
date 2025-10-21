using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

using InSite.Application.Banks.Write;
using InSite.Application.Records.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Assessments.Forms
{
    public partial class ChangeGradebook : AdminBasePage, IHasParentLinkParameters
    {
        private Guid BankId => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid FormId => Guid.Parse(Request.QueryString["form"]);

        private string DefaultParameters => $"bank={BankId}&form={FormId}";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;

            CreateGradebookMode.AutoPostBack = true;
            CreateGradebookMode.CheckedChanged += GradebookMode_CheckedChanged;

            SelectGradebookMode.AutoPostBack = true;
            SelectGradebookMode.CheckedChanged += GradebookMode_CheckedChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                RedirectToSearch();

            if (!IsPostBack)
                Open();
        }

        private void GradebookMode_CheckedChanged(object sender, EventArgs e)
        {
            Gradebook.Enabled = SelectGradebookMode.Checked;

            if (!CreateGradebookMode.Checked)
                return;

            var bank = ServiceLocator.BankSearch.GetBankState(BankId);
            var form = bank.FindForm(FormId);

            if (form.Gradebook.HasValue)
                Warning.AddMessage(AlertType.Warning, "A new gradebook will be created and assigned to the form. The gradebook which is currently assigned to the form will <b>not</b> be deleted.");
            else
                Warning.AddMessage(AlertType.Warning, "A new gradebook will be created and assigned to the form.");
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Save();

            RedirectToOutline();
        }

        private void Open()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankId);
            if (bank == null)
                RedirectToSearch();

            var form = bank.FindForm(FormId);
            if (form == null)
                RedirectToOutline();

            Gradebook.ListFilter.OrganizationIdentifier = Organization.Identifier;

            SetInputValues(form);
        }

        private void Save()
        {
            if (SelectGradebookMode.Checked)
            {
                ServiceLocator.SendCommand(new ChangeFormGradebook(BankId, FormId, Gradebook.ValueAsGuid));
                RedirectToOutline();
            }

            var bank = ServiceLocator.BankSearch.GetBankState(BankId);
            var form = bank.FindForm(FormId);

            var questionIds = form.GetQuestions().Select(x => x.Identifier).ToList();
            var rubrics = ServiceLocator.RubricSearch.GetQuestionRubrics(questionIds);

            var rubricScores = new List<RubricScore>();
            foreach (var rubricPair in rubrics)
                rubricScores.Add(new RubricScore { QuestionId = rubricPair.Key, MaxPoints = rubricPair.Value.RubricPoints });

            var (_, commands) = FormGradebookCreator.Create(FormId, bank, rubricScores);

            ServiceLocator.SendCommands(commands);

            RedirectToOutline();
        }

        private void SetInputValues(Form form)
        {
            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{(form.Content.Title?.Default).IfNullOrEmpty(form.Name)} <span class=\"form-text\">Asset #{form.Asset}</span>");

            FormDetails.BindForm(form, BankId, form.Specification.Bank.IsAdvanced, true, false);

            CreateGradebookMode.Checked = form.Gradebook == null;
            SelectGradebookMode.Checked = form.Gradebook.HasValue;

            Gradebook.ValueAsGuid = form.Gradebook;
            Gradebook.Enabled = SelectGradebookMode.Checked;

            CancelButton.NavigateUrl = GetParentUrl(DefaultParameters);
        }

        private static void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToOutline()
        {
            var url = GetParentUrl(DefaultParameters);

            HttpResponseHelper.Redirect(url, true);
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? DefaultParameters
                : null;
        }
    }
}