using System;
using System.Net;

using InSite.UI.Layout.Admin;

using Shift.Constant;

namespace InSite.Admin.Assessments.Forms.Controls
{
    public partial class FormPageAlert : System.Web.UI.UserControl
    {
        private const string FormsSearchUrl = "/ui/admin/assessments/forms/search";
        private const string BanksSearchUrl = "/ui/admin/assessments/banks/search";

        public void ShowBankMissing()
        {
            Show(
                "No bank specified",
                $"No question bank specified in the URL. {Link(BanksSearchUrl, "Search all question banks")}.");
        }

        public void ShowBankNotFound(Guid bankId)
        {
            Show(
                "Bank not found",
                $"Question bank <code>{Encode(bankId.ToString())}</code> was not found. {Link(BanksSearchUrl, "Search all question banks")}.");
        }

        public void ShowFormMissing(Guid bankId, string bankName)
        {
            Show(
                "No form specified",
                $"No assessment form specified in the URL. {Link(FormsSearchUrl, "Search all assessment forms")}, or {Link(BankOutlineUrl(bankId), $"return to question bank <strong>{FormatName(bankName)}</strong>")}.");
        }

        public void ShowFormNotFound(Guid formId, Guid bankId, string bankName)
        {
            Show(
                "Form not found",
                $"Assessment form <code>{Encode(formId.ToString())}</code> was not found in question bank <strong>{FormatName(bankName)}</strong>. {Link(FormsSearchUrl, "Search all assessment forms")}, or {Link(BanksSearchUrl, "search all question banks")}.");
        }

        public void ShowPermissionDenied(string action)
        {
            Show(
                "Permission denied",
                $"You don't have permission to {Encode(action)}. {Link(FormsSearchUrl, "Search all assessment forms")}.");
        }

        private void Show(string qualifier, string message)
        {
            Visible = true;
            PageHelper.AutoBindHeader(Page, qualifier: qualifier);
            Alert.AddMessage(AlertType.Warning, message);
        }

        private static string BankOutlineUrl(Guid bankId) =>
            $"/ui/admin/assessments/banks/outline?bank={bankId}";

        private static string Link(string url, string text) =>
            $"<a href=\"{url}\">{text}</a>";

        private static string Encode(string text) =>
            WebUtility.HtmlEncode(text ?? string.Empty);

        private static string FormatName(string name) =>
            Encode((name ?? string.Empty).TrimEnd('.', ' '));
    }
}
