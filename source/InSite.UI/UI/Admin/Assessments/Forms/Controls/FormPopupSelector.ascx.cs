using System;
using System.Web.UI;

namespace InSite.Admin.Assessments.Forms.Controls
{
    public partial class FormPopupSelector : UserControl
    {
        public Guid? Value
        {
            get => string.IsNullOrEmpty(ValueInput.Value) ? (Guid?)null : Guid.Parse(ValueInput.Value);
            set
            {
                var isValid = value.HasValue;

                if (isValid)
                {
                    var form = ServiceLocator.BankSearch.GetForm(value.Value);

                    if (isValid = form != null)
                    {
                        TextInput.Value = $"{form.FormAsset}.{form.FormAssetVersion}: {form.FormTitle ?? form.FormName}";
                        ValueInput.Value = form.FormIdentifier.ToString();
                    }
                }

                if (!isValid)
                {
                    TextInput.Value = null;
                    ValueInput.Value = null;
                }
            }
        }

        public bool AllowEdit
        {
            get => OpenButton.Visible;
            set => OpenButton.Visible = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            StyleLiteral.ContentKey = GetType().FullName;
        }
    }
}