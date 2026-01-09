using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.UI.Admin.Assessments.Forms.Utilities;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Forms.Forms
{
    public partial class ChangeAddendum : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid FormID => Guid.Parse(Request.QueryString["form"]);

        private Guid? SectionID => Guid.TryParse(Request.QueryString["section"], out var value) ? value : (Guid?)null;

        #endregion

        #region Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                RedirectToSearch();

            if (!IsPostBack)
                Open();
        }

        #endregion

        #region Event handlers

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Save();

            RedirectToReader(FormID, SectionID);
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                RedirectToSearch();

            var form = bank.FindForm(FormID);
            if (form == null)
                RedirectToReader();

            SetInputValues(form);
        }

        private void Save()
        {
            var acronyms = GetData(AcronymsValues.Value);
            var formulas = GetData(FormulasValues.Value);
            var figures = GetData(FiguresValues.Value);

            ServiceLocator.SendCommand(new ChangeFormAddendum(BankID, FormID, acronyms, formulas, figures));

            FormAddendumItem[] GetData(string value)
            {
                return value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x =>
                    {
                        var parts = x.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length != 2 || !int.TryParse(parts[0], out var asset) || !int.TryParse(parts[1], out var version))
                            return null;

                        return new FormAddendumItem
                        {
                            Asset = asset,
                            Version = version
                        };
                    })
                    .Where(x => x != null)
                    .ToArray();
            }
        }

        #endregion

        #region Settings/getting input values

        private void SetInputValues(Form form)
        {
            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{(form.Content.Title?.Default).IfNullOrEmpty(form.Name)} <span class=\"form-text\">Asset #{form.Asset}</span>");

            var attachments = AddendumHelper.GetRepeaterDataSource(form.Specification.Bank.EnumerateAllAttachments())[0];
            if (attachments.Length == 0)
            {
                EditorStatus.AddMessage(
                    AlertType.Error,
                    $"The bank contains no attachments." +
                    $"<div style='margin-top:15px;'><a class='btn btn-default' href='{GetReaderUrl(FormID, SectionID)}'><i class='fas fa-arrow-alt-left me-2'></i> Back</a></div>");

                FormContainer.Visible = false;

                return;
            }

            var addendum = form.Addendum;
            AcronymsValues.Value = GetAddendumValue(addendum.Acronyms);
            FormulasValues.Value = GetAddendumValue(addendum.Formulas);
            FiguresValues.Value = GetAddendumValue(addendum.Figures);

            AttachmentRepeater.DataSource = attachments;
            AttachmentRepeater.DataBind();

            CancelButton.NavigateUrl = GetReaderUrl(FormID, SectionID);

            string GetAddendumValue(List<FormAddendumItem> value) =>
                value.IsNotEmpty() ? string.Join(",", value.Select(x => $"{x.Asset}.{x.Version}")) : string.Empty;
        }

        #endregion

        #region Methods (redirect)

        private static void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToReader(Guid? formId = null, Guid? sectionId = null)
        {
            var url = GetReaderUrl(formId, sectionId);

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetReaderUrl(Guid? formId = null, Guid? sectionId = null)
        {
            var url = $"/ui/admin/assessments/banks/outline?bank={BankID}";

            if (formId.HasValue)
                url += $"&form={formId.Value}";

            if (sectionId.HasValue)
                url += $"&section={sectionId.Value}";

            url += "&tab=addendum";

            return url;
        }

        #endregion

        #region IHasParentLinkParameters

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"bank={BankID}"
                : null;
        }

        #endregion
    }
}