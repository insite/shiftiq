using System;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Workflow.Forms.Comments.Controls
{
    public partial class CommentRepeater : BaseUserControl
    {
        private Guid FormIdentifier
        {
            get => (Guid)ViewState[nameof(FormIdentifier)];
            set => ViewState[nameof(FormIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Repeater.DataBinding += Repeater_DataBinding;
        }

        public bool LoadData(Guid surveyFormId)
        {
            FormIdentifier = surveyFormId;

            Repeater.DataBind();

            AddCommentButton.NavigateUrl = Author.GetNavigateUrl(surveyFormId);

            return Repeater.Items.Count > 0;
        }

        private void Repeater_DataBinding(object sender, EventArgs e)
        {
            var data = PersonCommentSummarySearch.Select(
                x => x.ContainerType == SurveyStore.CommentContainerType
                  && x.ContainerIdentifier == FormIdentifier);
            var userIds = data.Select(x => x.AuthorUserIdentifier).Distinct().ToArray();
            var userNames = UserSearch
                .Bind(
                    x => new { x.UserIdentifier, x.FullName },
                    new UserFilter { IncludeUserIdentifiers = userIds })
                .ToDictionary(x => x.UserIdentifier, x => x.FullName);

            Repeater.DataSource = data.Select(x => new
            {
                AuthorName = userNames.GetOrDefault(x.AuthorUserIdentifier, UserNames.Someone),
                TimestampHtml = "<span title='" + LocalizeTime(x.CommentPosted, null, false) + "'>commented " + TimeZones.Format(x.CommentPosted, CurrentSessionState.Identity.User.TimeZone) + "</span>",
                CommentFlagHtml = x.CommentFlagHtml,
                CommentCategoryHtml = x.CommentCategoryHtml,
                CommentHtml = Markdown.ToHtml(x.CommentText),
                ReviseUrl = Revise.GetNavigateUrl(x.CommentIdentifier),
                DeleteUrl = Delete.GetNavigateUrl(x.CommentIdentifier)
            });
        }
    }
}