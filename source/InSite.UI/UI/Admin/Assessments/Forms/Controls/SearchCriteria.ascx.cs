using InSite.Application.Banks.Read;
using InSite.Common.Web.UI;

using Shift.Constant;

namespace InSite.Admin.Assessments.Forms.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QBankFormFilter>
    {
        public override QBankFormFilter Filter
        {
            get
            {
                var filter = new QBankFormFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    AssetNumber = int.TryParse(Number.Text, out var assetNum) ? assetNum : (int?)null,
                    FormTitle = Title.Text,
                    FormName = Name.Text,
                    IncludeFormStatus = Status.Value
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                Number.Text = value.AssetNumber.ToString();
                Title.Text = value.FormTitle;
                Name.Text = value.FormName;
                Status.Value = value.IncludeFormStatus;
            }
        }

        public override void Clear()
        {
            Number.Text = null;
            Title.Text = null;
            Name.Text = null;
            Status.ClearSelection();
        }

        protected override void OnLoad(System.EventArgs e)
        {
            if (!IsPostBack)
            {
                Status.LoadItems(
                    PublicationStatus.Drafted,
                    PublicationStatus.Published,
                    PublicationStatus.Archived,
                    PublicationStatus.Unpublished);
            }

            base.OnLoad(e);
        }
    }
}