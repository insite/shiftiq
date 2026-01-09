using System;

using InSite.Application.Banks.Read;
using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.Admin.Assessments.Banks.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QBankFilter>
    {
        public override QBankFilter Filter
        {
            get
            {
                var filter = new QBankFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    AssetNumber = ValueConverter.ToInt32Nullable(Number.Text),
                    BankTitle = Title.Text,
                    BankName = Name.Text,
                    BankEnable = BankEnabled.ValueAsBoolean
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                Number.Text = value.AssetNumber?.ToString();
                Title.Text = value.BankTitle;
                Name.Text = value.BankName;
                BankEnabled.ValueAsBoolean = value.BankEnable;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;
        }

        public override void Clear()
        {
            Number.Text = null;
            Title.Text = null;
            Name.Text = null;
            BankEnabled.ClearSelection();
        }
    }
}