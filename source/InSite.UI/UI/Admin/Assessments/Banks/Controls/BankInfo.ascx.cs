using System;

namespace InSite.Admin.Assessments.Banks.Controls
{
    public partial class BankInfo : System.Web.UI.UserControl
    {
        public void BindBank(Domain.Banks.BankState bank, bool showStandard = true, bool showName = true, bool showLevel = true)
        {
            AssetNumber.Text = bank.Asset.ToString();
            StandardDiv.Visible = showStandard;
            BankStandard.Text = bank.Standard != Guid.Empty
                ? $"<a href=\"/ui/admin/standards/edit?id={bank.Standard}\">{SnippetBuilder.GetHtml(bank.Standard)}</a>"
                : "None";
            if (!string.IsNullOrEmpty(bank.Level.ToString()))
                BankLevel.Text = bank.Level.ToString();
            else
                LevelDiv.Visible = false;
            NameDiv.Visible = showName;
            BankName.Text = $"<a href=\"/ui/admin/assessments/banks/outline?bank={bank.Identifier}\">{bank.Name}</a>";
            if (!string.IsNullOrEmpty(bank.Status))
                BankStatus.Text = bank.Status;
            else
                StatusDiv.Visible = false;
            BankEdition.Text = bank.Edition.ToString();

            EditionDiv.Visible = showLevel;
            if (LevelDiv.Visible)
                LevelDiv.Visible = showLevel;
        }
    }
}