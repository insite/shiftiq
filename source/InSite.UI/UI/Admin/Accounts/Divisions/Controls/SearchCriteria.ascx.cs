using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Accounts.Divisions.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<DivisionFilter>
    {
        public override DivisionFilter Filter
        {
            get
            {
                var filter = new DivisionFilter
                {
                    //OrganizationIdentifier = Organization.Key, //DEV-2008
                    DivisionName = DivisionName.Text,
                    DivisionCode = DivisionCode.Text,
                    Created = new DateTimeOffsetRange
                    {
                        Since = CreatedSince.Value,
                        Before = CreatedBefore.Value
                    },
                    CompanyName = CompanyName.Text
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                DivisionName.Text = value.DivisionName;
                DivisionCode.Text = value.DivisionCode;
                CreatedSince.Value = value.Created?.Since;
                CreatedBefore.Value = value.Created?.Before;
                CompanyName.Text = value.CompanyName;
            }
        }

        public override void Clear()
        {
            DivisionName.Text = null;
            DivisionCode.Text = null;
            CreatedSince.Value = null;
            CreatedBefore.Value = null;
            CompanyName.Text = null;
        }
    }
}