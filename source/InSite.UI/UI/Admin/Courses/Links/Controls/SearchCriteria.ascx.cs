using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Courses.Links.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<LtiLinkFilter>
    {
        #region Properties

        public override LtiLinkFilter Filter
        {
            get
            {
                var filter = new LtiLinkFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    Publisher = Publisher.Value,
                    Title = Title.Text,
                    Location = Location.Text,
                    Subtype = Subtype.Value,
                    Code = Code.Text
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                Publisher.Value = value.Publisher;
                Title.Text = value.Title;
                Location.Text = value.Location;
                Subtype.Value = value.Subtype;
                Code.Text = value.Code;
            }
        }

        #endregion

        #region Helper methods

        public override void Clear()
        {
            Publisher.ClearSelection();
            Title.Text = null;
            Location.Text = null;
            Subtype.ClearSelection();
            Code.Text = null;
        }

        #endregion
    }
}