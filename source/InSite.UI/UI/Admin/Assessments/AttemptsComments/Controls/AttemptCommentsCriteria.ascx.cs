using InSite.Application.Attempts.Read;
using InSite.Common.Web.UI;


namespace InSite.Admin.Assessments.Reports.Controls
{
    public partial class SubmissionCommentaryCriteria : SearchCriteriaController<QAttemptCommentaryFilter>
    {
        public override QAttemptCommentaryFilter Filter
        {
            get
            {
                var filter = new QAttemptCommentaryFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    FormTitle = FormTitle.Text,
                    AssetNumber = AssetNumber.ValueAsInt,
                    CommentText = Comment.Text
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                FormTitle.Text = value?.FormTitle;
                AssetNumber.ValueAsInt = value?.AssetNumber;
                Comment.Text = value?.CommentText;
            }
        }

        public override void Clear()
        {
            FormTitle.Text = null;
            AssetNumber.ValueAsInt = null;
            Comment.Text = null;
        }
    }
}