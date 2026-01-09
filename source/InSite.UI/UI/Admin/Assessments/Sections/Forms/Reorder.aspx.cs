using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Forms.Forms
{
    public partial class Reorder : AdminBasePage, IHasParentLinkParameters
    {

        protected Guid? BankID => Guid.TryParse(Page.Request.QueryString["bank"], out var id) ? id : (Guid?)null;

        protected Guid? FormID => Guid.TryParse(Page.Request.QueryString["form"], out var id) ? id : (Guid?)null;

        private string BankReadUrl
        {
            get => (string)ViewState[nameof(BankReadUrl)];
            set => ViewState[nameof(BankReadUrl)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
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

            if (isSave)
            {
                var startIndex = args.IndexOf('&') + 1;

                Save(args.Substring(startIndex));
            }

            HttpResponseHelper.Redirect(BankReadUrl);
        }

        private void Open()
        {
            if (BankID == null || FormID == null)
                HttpResponseHelper.Redirect("/ui/admin/assessments/banks/search");

            BankReadUrl = $"/ui/admin/assessments/banks/outline?bank={BankID}&form={FormID}&tab=fields";

            var bank = ServiceLocator.BankSearch.GetBankState(BankID.Value);
            if (bank == null)
                HttpResponseHelper.Redirect(BankReadUrl);

            var form = bank.FindForm(FormID.Value);
            if (form == null || form.Specification.Type != SpecificationType.Static)
                HttpResponseHelper.Redirect(BankReadUrl);

            PageHelper.AutoBindHeader(this, null, $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>");

            SectionRepeater.DataSource = form.Sections.Select(x => new
            {
                SetName = x.Criterion.ToString()
            });

            SectionRepeater.DataBind();

            CancelButton.NavigateUrl = BankReadUrl;
        }

        private void Save(string args)
        {
            var sequences = new Dictionary<int, int>();

            var oldSequences = args.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < oldSequences.Length; i++)
            {
                var oldSequence = int.Parse(oldSequences[i]);

                sequences.Add(oldSequence, i + 1);
            }

            var reorder = new ReorderSections(BankID.Value, FormID.Value, sequences);
            ServiceLocator.SendCommand(reorder);
        }

        private void CloseWindow(string args = null)
        {
            Page.ClientScript.RegisterClientScriptBlock(
                GetType(),
                "Close",
                $"modalManager.closeModal({(string.IsNullOrEmpty(args) ? "null" : HttpUtility.JavaScriptStringEncode(args, true))});",
                true);
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"bank={BankID}&form={FormID}&tab=fields"
                : null;
        }

    }
}
