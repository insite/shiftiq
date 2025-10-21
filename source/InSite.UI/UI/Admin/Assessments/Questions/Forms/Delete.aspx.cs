using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Questions.Forms
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters, IOverrideWebRouteParent
    {
        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid QuestionID => Guid.TryParse(Request.QueryString["question"], out var value) ? value : Guid.Empty;

        private Guid? FormID => Guid.TryParse(Request["form"], out var value) ? value : (Guid?)null;

        private string ReturnTo => Request["return"];

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write))
                RedirectToSearch();

            if (IsPostBack)
                return;

            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                RedirectToSearch();

            var question = bank.FindQuestion(QuestionID);
            if (question == null)
                HttpResponseHelper.Redirect(GetOutlineUrl());

            var attemptCount = ServiceLocator.AttemptSearch.CountAttempts(a => a.Questions.Any(q => q.QuestionIdentifier == QuestionID));
            var commentCount = question.Comments.Count;

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>");

            BankName.Text = $"<a href=\"/ui/admin/assessments/banks/outline?bank={bank.Identifier}\">{bank.Name}</a>";
            SetDetails.BindSet(question.Set);

            QuestionText.Text = question.Content.Title?.Default;
            QuestionAssetNumber.Text = question.Asset.ToString();

            QuestionCount.Text = FieldCount.Text = "0";
            OptionCount.Text = $"{question.Options.Count:n0}";
            AttemptQuestionCount.Text = $"{attemptCount:n0}";
            AttemptQuestionCommentCount.Text = $"{commentCount:n0}";

            CancelButton.NavigateUrl = GetOutlineUrl(QuestionID);

            BindContainers(bank);
        }

        private void BindContainers(BankState bank)
        {
            QuestionContainerList.Items.Add(new System.Web.UI.WebControls.ListItem($"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <small class='form-text'> &bull; Bank Asset #{bank.Asset}</small> - <b>Most Recent Version Only</b>", "removeRecentVersion"));

            if (!Organization.Toolkits.Assessments.LockPublishedQuestions)
                QuestionContainerList.Items.Add(new System.Web.UI.WebControls.ListItem($"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <small class='form-text'> &bull; Bank Asset #{bank.Asset}</small> - <b>All Versions</b>", "removeAllVersions"));

            var forms = bank.Specifications.SelectMany(x => x.EnumerateAllForms()).ToArray();
            foreach (var form in forms)
            {
                if (form.Sections.FirstOrDefault(x => x.Fields.FirstOrDefault(y => y.QuestionIdentifier == QuestionID) != null) != null)
                    QuestionContainerList.Items.Add(new System.Web.UI.WebControls.ListItem($"{form.Name} <small class='form-text'> &bull; Form Asset #{form.Asset}</small>", form.Identifier.ToString()));
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var removeAllVersions = false;
            var removeRecentVersion = false;
            var ids = new List<Guid>();

            foreach (System.Web.UI.WebControls.ListItem item in QuestionContainerList.Items)
            {
                if (!item.Selected)
                    continue;

                if (item.Value == "removeAllVersions")
                    removeAllVersions = true;
                else if (item.Value == "removeRecentVersion")
                    removeRecentVersion = true;
                else
                    ids.Add(Guid.Parse(item.Value));
            }

            if (removeAllVersions || removeRecentVersion)
            {
                ServiceLocator.SendCommand(new DeleteQuestion(BankID, QuestionID, removeAllVersions));
            }
            else
            {
                foreach (var id in ids)
                    ServiceLocator.SendCommand(new DeleteFields(BankID, id, QuestionID));
            }

            RedirectToOutline();
        }

        #region Methods (navigation)

        private void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToOutline(Guid? questionId = null)
        {
            var url = GetOutlineUrl(questionId);
            HttpResponseHelper.Redirect(url, true);
        }

        private string GetOutlineUrl(Guid? questionId = null)
        {
            var returnUrl = new ReturnUrl();
            var url = returnUrl.GetReturnUrl();

            if (url == null)
            {
                var query = string.Empty;

                if (questionId.HasValue)
                    query += $"&question={questionId.Value}";

                if (FormID.HasValue)
                    query += $"&form={FormID.Value}&tab=fields";

                if (!string.IsNullOrEmpty(ReturnTo))
                    query += $"&panel={ReturnTo}";
                else if (query.Length == 0)
                    query += "&panel=questions";

                return $"/ui/admin/assessments/banks/outline?bank={BankID}" + query;
            }

            return url;
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            if (parent.Name.EndsWith("/banks/outline"))
                return $"bank={BankID}";

            if (IsStandardEditorAction(parent.Name))
            {
                var webUrl = new WebUrl(GetReturnUrl());
                var removeKeys = webUrl.QueryString.AllKeys.Where(x => !x.Equals("id", StringComparison.OrdinalIgnoreCase)).ToArray();
                foreach (var key in removeKeys)
                    webUrl.QueryString.Remove(key);
                return webUrl.QueryString.ToString();
            }

            return null;
        }

        IWebRoute IOverrideWebRouteParent.GetParent()
        {
            var parent = GetParent();
            return IsStandardEditorAction(parent.Name) ? parent : null;
        }

        private static bool IsStandardEditorAction(string value) =>
            value != null && value.EndsWith("/standards/edit");

        #endregion
    }
}