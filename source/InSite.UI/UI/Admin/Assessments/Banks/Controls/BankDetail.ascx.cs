using System;

namespace InSite.Admin.Assessments.Banks.Controls
{
    public partial class BankDetail : System.Web.UI.UserControl
    {
        public void BindBank(Domain.Banks.BankState bank)
        {
            BankStandard.InnerHtml = bank.Standard != Guid.Empty
                ? $"<a href=\"/ui/admin/standards/edit?id={bank.Standard}\">{SnippetBuilder.GetHtml(bank.Standard)}</a>"
                : "None";

            if (!string.IsNullOrEmpty(bank.Level.ToString()))
                BankLevel.InnerText = bank.Level.ToString();

            BankName.InnerHtml = $"<a href=\"/ui/admin/assessments/banks/outline?bank={bank.Identifier}\">{bank.Name}</a>";

            BankEdition.InnerText = bank.Edition.ToString();

            if (!string.IsNullOrEmpty(bank.Status))
                BankStatus.InnerText = bank.Status;

            AssetNumber.InnerText = bank.Asset.ToString();
        }
    }
}