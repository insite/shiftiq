using System;
using System.Web.UI;

using InSite.Admin.Contacts.Reports.Models;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Contacts.Reports.Forms
{
    public partial class TaxFormT2202 : AdminBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SearchButton.Click += SearchButton_Click;
            DownloadButton.Click += DownloadButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                if (!Identity.IsGranted(ActionName.Admin_Contacts_People_Edit_SocialInsuranceNumber))
                {
                    SearchCriteriaSection.Visible = false;
                    ScreenStatus.AddMessage(AlertType.Error, "You do not have permission to view this directory or page.");
                    return;
                }

                SetupYearSelector();

                PageHelper.AutoBindHeader(this);
            }
        }

        private void SetupYearSelector()
        {
            var year = DateTime.UtcNow.Year;

            YearSelector.Items.Clear();
            YearSelector.Items.Add(new ComboBoxOption());

            for (var i = 0; i < 10; i++)
            {
                var value = (year - i).ToString();
                YearSelector.Items.Add(new ComboBoxOption(value, value));
            }
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            SearchResultsSection.Visible = true;
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            ExportTaxFormInfoToXLSX();
        }

        private void ExportTaxFormInfoToXLSX()
        {
            var year = YearSelector.ValueAsInt;
            if (year.HasValue)
            {
                var bytes = TaxFormHelper.GetTaxFormInfoXlsx(year.Value);
                if (bytes == null)
                {
                    ScreenStatus.AddMessage(AlertType.Warning, "No data for the report");
                }
                else
                {
                    var filename = string.Format(
                        "{0}-{1:yyyyMMdd}-{1:HHmmss}",
                        StringHelper.Sanitize("Tax Form T2202 Info", '-'),
                        DateTime.UtcNow);

                    Page.Response.SendFile(filename, "xlsx", bytes);
                }
            }
            else
            {
                ScreenStatus.AddMessage(AlertType.Error, "Please select Year for build report");
            }
        }
    }
}
