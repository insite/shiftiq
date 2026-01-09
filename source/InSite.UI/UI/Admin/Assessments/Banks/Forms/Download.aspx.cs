using System;
using System.Text;

using InSite.Application.Banks.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Assessments.Banks.Forms
{
    public partial class Download : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DownloadButton.Click += DownloadButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var query = BankID.HasValue ? ServiceLocator.BankSearch.GetBank(BankID.Value) : null;
                if (query == null)
                    HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

                PageHelper.AutoBindHeader(
                    this,
                    qualifier: (query.BankTitle ?? query.BankName) + $" <span class='form-text'>Asset #{query.AssetNumber}</span>");

                var bank = ServiceLocator.BankSearch.GetBankState(BankID.Value);
                BankDetails.BindBank(bank);

                SetupDownloadSection(query);

                CancelLink.NavigateUrl = $"/ui/admin/assessments/banks/outline?bank={BankID}";
            }
        }

        private void SetupDownloadSection(QBank bank)
        {
            FileName.Text = bank.BankName;
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var fileFormat = FileFormatSelector.SelectedValue;

            if (fileFormat == "JSON")
            {
                SendJson();
            }
        }

        private void SendJson()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID.Value);
            var json = JsonHelper.JsonExport(bank);
            var bytes = Encoding.UTF8.GetBytes(json);

            if (CompressionMode.Value == "ZIP")
                SendZipFile(bytes, FileName.Text, "json");
            else
                Response.SendFile(FileName.Text, "json", bytes);
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"bank={BankID}"
                : null;
        }
    }
}