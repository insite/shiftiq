using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Assessments.Sections.Forms
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        private class ReferenceItem
        {
            public string Name { get; set; }
            public int Count { get; set; }

            public ReferenceItem(string name, int count)
            {
                Name = name;
                Count = count;
            }
        }

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid SectionID => Guid.Parse(Request["section"]);

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var bank = ServiceLocator.BankSearch.GetBankState(BankID);
                if (bank == null)
                    RedirectToSearch();

                var section = bank.FindSection(SectionID);
                if (section == null)
                    RedirectToReader();

                PageHelper.AutoBindHeader(
                    this, 
                    qualifier: $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>");

                FormStandard.AssetID = bank.Standard;
                FormName.Text = $"<a href=\"/ui/admin/assessments/banks/outline?bank={BankID}&form={section.Form.Identifier}\">{section.Form.Name}</a>";
                SpecificationName.Text = section.Form.Specification.Name;

                CriterionTitle.Text = section.Criterion.Title;
                SectionNumber.Text = $"{section.Sequence} of {section.Form.Sections.Count}";

                CancelButton.NavigateUrl = $"/ui/admin/assessments/banks/outline?bank={bank.Identifier}&form={section.Form.Identifier}&section={section.Identifier}&tab=section";

                {
                    var references = new List<ReferenceItem>();

                    if (!bank.IsAdvanced && section.Criterion.Sections.All(x => x.Identifier == section.Identifier))
                        references.Add(new ReferenceItem("Specification Criteria", 1));

                    references.Add(new ReferenceItem("Sections", 1));
                    references.Add(new ReferenceItem("Section Fields", section.Fields.Count));

                    ReferenceRepeater.DataSource = references;
                    ReferenceRepeater.DataBind();
                }
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                RedirectToSearch();

            var section = bank.FindSection(SectionID);
            if (section == null)
                RedirectToReader();

            if (!bank.IsAdvanced)
                ServiceLocator.SendCommand(new DeleteCriterion(BankID, section.CriterionIdentifier));
            else
                ServiceLocator.SendCommand(new DeleteSection(BankID, SectionID));

            RedirectToReader(section.Form.Identifier);
        }

        private void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToReader(Guid? form = null)
        {
            var url = $"/ui/admin/assessments/banks/outline?bank={BankID}";

            if (form.HasValue)
                url += $"&form={form.Value}";

            HttpResponseHelper.Redirect(url, true);
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline") ? $"bank={BankID}" : null;
        }
    }
}