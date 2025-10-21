using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Sets.Forms
{
    public partial class Reorder : AdminBasePage, IHasParentLinkParameters
    {
        protected Guid? BankID => Guid.TryParse(Page.Request.QueryString["bank"], out var id) ? id : (Guid?)null;

        protected Guid? SetID => Guid.TryParse(Page.Request.QueryString["set"], out var id) ? id : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write))
                RedirectToSearch();

            if (!IsPostBack)
                Open();

            base.OnLoad(e);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var args = Request.Form["__EVENTARGUMENT"];
            if (string.IsNullOrEmpty(args))
                return;

            var isSave = args.StartsWith("save&");
            if (!isSave)
                RedirectToOutline();

            var startIndex = args.IndexOf('&') + 1;
            var data = args.Substring(startIndex);

            var sequences = new Dictionary<int, int>();
            var oldSequences = data.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < oldSequences.Length; i++)
            {
                var oldSequence = int.Parse(oldSequences[i]);

                sequences.Add(oldSequence, i + 1);
            }

            if (MultiView.GetActiveView() == ViewQuestions)
                SaveQuestions(sequences);
            else
                SaveSets(sequences);

            RedirectToOutline();
        }

        private void Open()
        {
            var bank = BankID.HasValue ? ServiceLocator.BankSearch.GetBankState(BankID.Value) : null;
            if (bank == null)
                RedirectToSearch();

            if (SetID.HasValue)
            {
                var set = bank.Sets.FirstOrDefault(x => x.Identifier == SetID);
                if (set == null)
                    RedirectToSearch();

                ActionModel.ActionName = "Reorder Questions";

                MultiView.SetActiveView(ViewQuestions);

                QuestionRepeater.DataSource = set.Questions.Select(x => new
                {
                    BankSequence = x.BankIndex + 1,
                    Code = x.Classification.Code,
                    Title = Markdown.ToHtml(x.Content.Title.Default),
                });
                QuestionRepeater.DataBind();
            }
            else
            {
                MultiView.SetActiveView(ViewSets);

                SetRepeater.DataSource = bank.Sets.Select((x, i) => new
                {
                    Sequence = i + 1,
                    Title = x.Name
                });
                SetRepeater.DataBind();
            }

            PageHelper.AutoBindHeader(
                this, null,
                $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} " +
                $"<span class=\"form-text\">Asset #{bank.Asset}</span>");

            CancelButton.NavigateUrl = GetOutlineUrl();
        }

        private void SaveSets(Dictionary<int, int> sequences)
        {
            var cmd = new ReorderSets(BankID.Value, sequences);

            ServiceLocator.SendCommand(cmd);
        }

        private void SaveQuestions(Dictionary<int, int> sequences)
        {
            var cmd = new ReorderQuestions(BankID.Value, SetID.Value, sequences);

            ServiceLocator.SendCommand(cmd);
        }

        private void RedirectToSearch() =>
            HttpResponseHelper.Redirect("/ui/admin/assessments/banks/search");

        private void RedirectToOutline()
        {
            var url = GetOutlineUrl();

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetOutlineUrl()
        {
            var url = new ReturnUrl().GetReturnUrl();
            if (url != null)
                return url;

            url = $"/ui/admin/assessments/banks/outline?bank={BankID}";

            if (SetID.HasValue)
                url += $"&set={SetID}";

            return url;
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"bank={BankID}"
                : null;
        }
    }
}
