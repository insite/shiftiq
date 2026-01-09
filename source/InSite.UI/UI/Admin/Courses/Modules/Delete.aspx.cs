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

namespace InSite.Admin.Courses.Outlines.Modules
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        private static readonly Regex SqlReferenceErrorRegex = new Regex(
            "The DELETE statement conflicted with the REFERENCE constraint \"(?<FkName>.+?)\". The conflict occurred in database \"(?<DbName>.+?)\", table \"(?<TableName>.+?)\", column '(?<ColumnName>.+?)'.",
            RegexOptions.Compiled);

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/manage") ? $"course={CourseIdentifier}" : null;

        private bool DeleteModule()
        {
            try
            {
                Course2Store.DeleteModule(CourseIdentifier, ModuleIdentifier);
            }
            catch (SqlException sqlex)
            {
                DeletePanel.Visible = false;
                ErrorPanel.Visible = true;

                if (sqlex.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                {
                    var errorMessage =
                        new StringBuilder("Database contains the records that reference this (or related) module");

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

                    errorMessage.Append("You must manually remove them before you can delete the module.");

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

        private string GetReturnUrlInternal() => StringHelper.FirstValue(
            GetReturnUrl,
            () => Request.QueryString["returnUrl"],
            () => Module == null ? null : $"/ui/admin/courses/manage?course={CourseIdentifier}",
            () => "/ui/admin/courses/search");

        private void GoBack()
        {
            var returnUrl = GetReturnUrlInternal();

            HttpResponseHelper.Redirect(returnUrl);
        }

        private QModule _module;
        private bool _moduleLoaded;

        private QModule Module
        {
            get
            {
                if (!_moduleLoaded)
                {
                    _module = CourseSearch.SelectModule(ModuleIdentifier, x => x.Unit.Course);
                    _moduleLoaded = true;
                }

                return _module;
            }
        }

        private Guid CourseIdentifier => Module.Unit.CourseIdentifier;

        private Guid ModuleIdentifier => Guid.Parse(Page.Request.QueryString["module"]);

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
            if (Module == null)
                GoBack();

            var title = Module.ModuleName;

            PageHelper.AutoBindHeader(this, null, title);

            var returnUrl = GetReturnUrlInternal();

            CancelButton.NavigateUrl = returnUrl;
            CloseButton.NavigateUrl = returnUrl;
        }

        protected void DeleteButton_Click(object sender, EventArgs e)
        {
            if (DeleteModule())
                GoBack();
        }
    }
}
