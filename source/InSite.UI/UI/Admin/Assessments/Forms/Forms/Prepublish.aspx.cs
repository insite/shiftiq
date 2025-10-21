using System;
using System.Linq;
using System.Web;

using Humanizer;

using InSite.Application.Pages.Write;
using InSite.Application.Sites.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Portal.Assessments.Attempts.Utilities;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Forms.Forms
{
    public partial class Prepublish : AdminBasePage, IHasParentLinkParameters
    {
        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid FormID => Guid.Parse(Request["form"]);

        private bool _allowPreview;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                RedirectToSearch();

            var form = bank.FindForm(FormID);
            if (form == null)
                RedirectToOutline(null);

            _allowPreview = AllowPreview(form);

            var title = $"{(form.Content.Title?.Default).IfNullOrEmpty(form.Name)} <span class=\"form-text\">Asset #{form.Asset}</span>";

            PageHelper.AutoBindHeader(this, null, title);

            Name.Text = form.Name;
            FormTitle.Text = (form.Content.Title?.Default).IfNullOrEmpty("None");
            Version.Text = $"{form.Asset}.{form.AssetVersion}";
            FormStandard.AssetID = form.Specification.Bank.Standard;

            if (_allowPreview)
            {
                var startUrl = GetAssessmentAttemptStartUrl(form);
                AssessmentAttemptPanel.Visible = startUrl.HasValue();
                AssessmentAttemptLink.Text = AssessmentAttemptLink.NavigateUrl = startUrl;
            }
            else
            {
                AssessmentAttemptPanel.Visible = false;
            }

            CancelButton.NavigateUrl = GetOutlineUrl(form.Identifier);
        }

        private string GetAssessmentAttemptStartUrl(Form form)
        {
            var page = ServiceLocator.PageSearch.BindFirst(x => x, x => x.ObjectType == "Assessment" && x.ObjectIdentifier == FormID);

            if (page == null)
                page = CreateNewPage(form);

            var relativeUrl = AttemptUrlResource.GetStartUrl(page.PageIdentifier);

            if (!relativeUrl.HasValue())
                return null;

            var currentUrl = HttpContext.Current.Request.Url;

            return $"{currentUrl.Scheme}://{currentUrl.Host}{relativeUrl}";
        }

        private QPage CreateNewPage(Form form)
        {
            var formTitle = form.Content?.Title?.Default ?? form.Name;

            var id = UniqueIdentifier.Create();

            var a = new CreatePage(id, null, null, Organization.Identifier, User.Identifier, formTitle, "Page", 0, true, false);
            ServiceLocator.SendCommand(a);

            var b = new ChangePageContentControl(id, "Assessment");
            ServiceLocator.SendCommand(b);

            var c = new ChangePageAssessment(id, FormID);
            ServiceLocator.SendCommand(c);

            return ServiceLocator.PageSearch.BindFirst(x => x, x => x.ObjectType == "Assessment" && x.ObjectIdentifier == FormID);
        }

        private void RedirectToSearch()
            => HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToOutline(Guid? form)
            => HttpResponseHelper.Redirect(GetOutlineUrl(form), true);

        private bool AllowPreview(Form form)
        {
            var composedQuestions = form.GetQuestions()
                .Where(x => x.Type.IsComposed())
                .Select(x => x.Identifier)
                .ToArray();

            if (composedQuestions.Length == 0)
                return true;

            var composedQuestionsWithoutRubrics = ServiceLocator.BankSearch.GetQuestionsNotConnectedToRubrics(composedQuestions);

            if (composedQuestionsWithoutRubrics.Count == 0)
                return true;

            var count = composedQuestionsWithoutRubrics.Count;

            EditorStatus.AddMessage(
                AlertType.Warning,
                $" This assessment form cannot be prepublished because it contains {count} composed response {"question".ToQuantity(count)} without a rubric attached.");

            return false;
        }

        private string GetOutlineUrl(Guid? form)
        {
            var url = $"/ui/admin/assessments/banks/outline?bank={BankID}";

            if (form.HasValue)
                url += $"&form={form.Value}";

            return url;
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline") ? $"bank={BankID}" : null;
        }
    }
}