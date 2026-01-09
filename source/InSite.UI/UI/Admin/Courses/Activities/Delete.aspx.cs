using System;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;

using InSite.Application.Courses.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Courses.Outlines.Activities
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        private static readonly Regex SqlReferenceErrorRegex = new Regex(
            "The DELETE statement conflicted with the REFERENCE constraint \"(?<FkName>.+?)\". The conflict occurred in database \"(?<DbName>.+?)\", table \"(?<TableName>.+?)\", column '(?<ColumnName>.+?)'.",
            RegexOptions.Compiled);

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/manage") ? $"course={CourseIdentifier}" : null;

        private bool DeleteActivity()
        {
            try
            {
                Course2Store.DeleteActivity(CourseIdentifier, ModuleIdentifier, ActivityIdentifier);
            }
            catch (SqlException sqlex)
            {
                DeletePanel.Visible = false;
                ErrorPanel.Visible = true;

                if (sqlex.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                {
                    var errorMessage =
                        new StringBuilder("Database contains the records that reference this (or related) activity");

                    var matches = SqlReferenceErrorRegex.Matches(sqlex.Message);
                    if (matches.Count > 0)
                    {
                        errorMessage.Append(": <ul>");

                        foreach (Match m in matches)
                            errorMessage.Append("<li>").Append(m.Groups["TableName"].Value).Append(" (")
                                .Append(m.Groups["ColumnName"].Value).Append(")</li>");

                        errorMessage.Append("</ul>");
                    }
                    else
                    {
                        errorMessage.Append(".");
                    }

                    errorMessage.Append("You must manually remove them before you can delete the activity.");

                    ErrorText.Text = errorMessage.ToString();
                }
                else
                {
                    ErrorText.Text = "An error occurred on the server side.";
                    AppSentry.SentryError(sqlex);
                }

                return false;
            }

            return true;
        }

        private void GoBack()
        {
            var returnUrl = $"/ui/admin/courses/manage?course={CourseIdentifier}";
            HttpResponseHelper.Redirect(returnUrl);
        }

        private QActivity _activity;
        private QActivity Activity
        {
            get
            {
                if (_activity == null)
                    _activity = CourseSearch.SelectActivity(ActivityIdentifier, x => x.Module.Unit.Course);

                return _activity;
            }
        }

        private Guid CourseIdentifier => Guid.TryParse(Page.Request.QueryString["course"], out var id) ? id : Guid.Empty;

        public Guid ModuleIdentifier
        {
            get => (Guid?)ViewState[nameof(ModuleIdentifier)] ?? Guid.Empty;
            set => ViewState[nameof(ModuleIdentifier)] = value;
        }

        private Guid ActivityIdentifier => Guid.TryParse(Page.Request.QueryString["activity"], out var id) ? id : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            if (Activity == null)
                GoBack();

            ModuleIdentifier = Activity.Module.ModuleIdentifier;
            PageHelper.AutoBindHeader(this, null, Activity.ActivityName);

            CancelButton.NavigateUrl = $"/ui/admin/courses/manage?course={CourseIdentifier}&activity={ActivityIdentifier}";
            CloseButton.NavigateUrl = CancelButton.NavigateUrl;
        }

        protected void DeleteButton_Click(object sender, EventArgs e)
        {
            if (DeleteActivity())
                GoBack();
        }
    }
}
