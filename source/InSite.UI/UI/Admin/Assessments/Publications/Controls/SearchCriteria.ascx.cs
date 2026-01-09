using InSite.Application.Sites.Read;
using InSite.Common.Web.UI;

namespace InSite.UI.Admin.Assessments.Publications.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<VAssessmentPageFilter>
    {
        public override VAssessmentPageFilter Filter
        {
            get
            {
                var filter = new VAssessmentPageFilter
                {
                    OrganizationIdentifier = Organization.Key,
                    FormName = FormName.Text,
                    PageTitle = PageTitle.Text,
                    FormAsset = FormAsset.ValueAsInt,
                    PageIsHidden = PageIsHidden.ValueAsBoolean,
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                FormName.Text = value.FormName;
                PageTitle.Text = value.PageTitle;
                FormAsset.ValueAsInt = value.FormAsset;
                PageIsHidden.ValueAsBoolean = value.PageIsHidden;
            }
        }

        public override void Clear()
        {
            FormName.Text = null;
            PageTitle.Text = null;
            FormAsset.ValueAsInt = null;
            PageIsHidden.Value = null;
        }
    }
}