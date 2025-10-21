using System;

using InSite.Domain.Banks;

namespace InSite.Admin.Assessments.Forms.Controls
{
    public partial class FormInfo : System.Web.UI.UserControl
    {

        public void BindForm(Form form, Guid bankId, bool showSpec = true, bool showFormName = true, bool showFormCodes = true)
        {
            var bank = form.Specification.Bank;

            FormStandard.AssetID = bank.Standard;
            Name.Text = $"<a href=\"/ui/admin/assessments/banks/outline?bank={bankId}&form={form.Identifier}\">{form.Name}</a>";
            Version.Text = $"{form.Asset}.{form.AssetVersion}";
            Code.Text = form.Code ?? "None";
            Source.Text = form.Source ?? "None";
            Origin.Text = form.Origin ?? "None";
            Hook.Text = form.Hook ?? "None";

            if (showSpec)
                SpecificationDetails.BindSpec(form.Specification);
            else
                SpecDiv.Visible = false;

            FormNameField.Visible = showFormName;

            CodeField.Visible = SourceField.Visible = OriginField.Visible = HookField.Visible = showFormCodes;
        }
    }
}