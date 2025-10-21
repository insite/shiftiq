using System;
using System.Web.UI;

using InSite.Persistence;

using Shift.Constant;

namespace InSite.Admin.Standards.Standards.Controls
{
    public partial class StandardPopupSelector : UserControl
    {
        public Guid? Value
        {
            get => string.IsNullOrEmpty(ValueInput.Value) ? (Guid?)null : Guid.Parse(ValueInput.Value);
            set
            {
                var isValid = value.HasValue;

                if (isValid)
                {
                    var parent = StandardSearch.Bind(value.Value, x => new
                    {
                        x.StandardIdentifier,
                        x.AssetNumber,
                        Title = CoreFunctions.GetContentTextEn(x.StandardIdentifier, ContentLabel.Title),
                        x.StandardType
                    });

                    if (isValid = parent != null)
                    {
                        TextInput.Value = $"{parent.AssetNumber}: {parent.Title}";
                        ValueInput.Value = parent.StandardIdentifier.ToString();
                        SubTypeInput.Value = parent.StandardType;
                    }
                }

                if (!isValid)
                {
                    TextInput.Value = null;
                    ValueInput.Value = null;
                    SubTypeInput.Value = null;
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