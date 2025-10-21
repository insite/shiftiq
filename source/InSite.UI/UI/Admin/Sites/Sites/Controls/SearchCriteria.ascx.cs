using InSite.Application.Sites.Read;
using InSite.Common.Web.UI;

namespace InSite.Admin.Sites.Sites.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QSiteFilter>
    {
        public override QSiteFilter Filter
        {
            get
            {
                var filter = new QSiteFilter
                {
                    OrganizationIdentifier = Organization.Key,
                    Domain = Name.Text,
                    Title = Title.Text,
                    LastModifiedSince = LastModifiedSince.Value,
                    LastModifiedBefore = LastModifiedBefore.Value
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                Name.Text = value.Domain;
                Title.Text = value.Title;

                LastModifiedSince.Value = value.LastModifiedSince;
                LastModifiedBefore.Value = value.LastModifiedBefore;
            }
        }

        public override void Clear()
        {
            Name.Text = null;
            Title.Text = null;
            LastModifiedSince.Value = null;
            LastModifiedBefore.Value = null;
        }
    }
}