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

namespace InSite.Admin.Assessments.Sections.Forms
{
    public partial class Reorder : AdminBasePage, IHasParentLinkParameters
    {

        protected Guid? BankID => Guid.TryParse(Page.Request.QueryString["bank"], out var id) ? id : (Guid?)null;

        protected Guid? SectionID => Guid.TryParse(Page.Request.QueryString["section"], out var id) ? id : (Guid?)null;

        private string BankReadUrl
        {
            get
            {
                return (string)ViewState[nameof(BankReadUrl)];
            }
            set
            {
                ViewState[nameof(BankReadUrl)] = value;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!CanEdit)
                HttpResponseHelper.Redirect("/ui/admin/assessments/banks/search");

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
            var bank = BankID != null
                ? ServiceLocator.BankSearch.GetBankState(BankID.Value)
                : null;
            if (bank == null)
                HttpResponseHelper.Redirect("/ui/admin/assessments/banks/search");

            var section = SectionID != null
                ? bank.FindSection(SectionID.Value)
                : null;
            if (section == null)
                HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/outline?bank={bank.Identifier}");

            if (section.Form.Specification.Type != SpecificationType.Static)
                HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/outline?bank={bank.Identifier}&form={section.Form.Identifier}&tab=fields");

            FieldsPanel.IsSelected = true;

            PageHelper.AutoBindHeader(this, null, $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>");
            ReorderSectionsLink.NavigateUrl = $"/ui/admin/assessments/sections/reorder?bank={BankID}&form={section.Form.Identifier}";

            FormName.Text = section.Form.Name;
            SectionNumber.Text = $"{section.Sequence + 1} of {section.Form.Sections.Count}";

            SetName.Text = section.Criterion.ToString();

            FieldRepeater.DataSource = section.Fields.Select(x => new
            {
                BankSequence = x.Question.BankIndex + 1,
                Code = x.Question.Classification.Code,
                Title = Markdown.ToHtml(x.Question.Content.Title.Default),
            });

            FieldRepeater.DataBind();

            BankReadUrl = $"/ui/admin/assessments/banks/outline?bank={bank.Identifier}&form={section.Form.Identifier}&section={section.Identifier}&tab=fields";

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

            var cmd = new ReorderFields(BankID.Value, SectionID.Value, sequences);

            ServiceLocator.SendCommand(cmd);
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
                ? $"bank={BankID}&section={SectionID}&tab=fields"
                : null;
        }

    }
}
