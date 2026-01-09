using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Admin.Records.Programs.Utilities;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Admin.Records.Programs.Controls
{
    public partial class PublicationSetup : System.Web.UI.UserControl
    {
        private Guid ProgramId
        {
            get => (Guid)ViewState[nameof(ProgramId)];
            set => ViewState[nameof(ProgramId)] = value;
        }

        public void LoadData(TProgram program)
        {
            ProgramId = program.ProgramIdentifier;

            ModifyPublicationLink1.NavigateUrl = ModifyPublication.GetNavigateUrl(program.ProgramIdentifier, tab: "publication");
            ModifyPublicationLink2.NavigateUrl = ModifyPublication.GetNavigateUrl(program.ProgramIdentifier, tab: "publication");
            ModifyPublicationLink3.NavigateUrl = ModifyPublication.GetNavigateUrl(program.ProgramIdentifier, tab: "tasks");

            BindModelToControls(program);
            BindRepeater();
        }

        public void Refresh()
        {
            BindRepeater();
        }

        public void BindModelToControls(TProgram program)
        {
            var page = ServiceLocator.PageSearch
                .Select(
                    x => x.ObjectType == "Program" && x.ObjectIdentifier == program.ProgramIdentifier,
                    x => x.Site, x => x.Parent)
                .FirstOrDefault();
            var hasPage = page != null;

            WebPageSection.Visible = hasPage;

            if (hasPage)
            {
                WebFolderName.Text = GetHtmlText(page.Parent?.PageSlug);
                WebSiteName.Text = GetHtmlText(page.Site?.SiteDomain);
                WebPageName.Text = GetHtmlText(page.PageSlug);

                PublicationStatus.Text = page.IsHidden ? "Not Published" : "Published";

                WebPageIdentifierEdit.HRef = $"/ui/admin/sites/pages/outline?id={page.PageIdentifier}";
                WebPageIdentifierView.HRef = ServiceLocator.PageSearch.GetPagePath(page.PageIdentifier, false);
                WebPageHelp.InnerHtml = $"{WebPageIdentifierView.HRef}";
            }

            ProgramSlug.Text = program.ProgramSlug ?? StringHelper.Sanitize(program.ProgramName, '-');
            ProgramIcon.Text = GetHtmlText(program.ProgramIcon);
            ProgramIconPreview.Visible = program.ProgramIcon.IsNotEmpty();
            ProgramIconPreview.InnerHtml = $"<i class='{program.ProgramIcon}'></i>";

            ProgramImage.ImageUrl = program.ProgramImage;
            ProgramImageField.Visible = program.ProgramImage.IsNotEmpty();

            string GetHtmlText(string text) => text.IsEmpty() ? "<i>None</i>" : HttpUtility.HtmlEncode(text);
        }

        private void BindRepeater()
        {
            var items = GetTaskItems();
            var hasItems = items.Count > 0;

            NoItemsMessage.Visible = !hasItems;

            DataRepeater.Visible = hasItems;

            DataRepeater.DataSource = items;
            DataRepeater.DataBind();
        }

        private List<TaskInfo> GetTaskItems()
        {
            var organization = CurrentSessionState.Identity.Organization;
            var taskObjectData = ProgramHelper.GetTaskObjectData(organization.OrganizationIdentifier, organization.ParentOrganizationIdentifier);

            var filter = new TTaskFilter
            {
                ProgramIdentifier = ProgramId,
                ExcludeObjectTypes = new string[] { "Assessment" }
            };

            filter.OrganizationIdentifiers.Add(organization.OrganizationIdentifier);

            var tasks = ProgramSearch1.GetProgramTasks(filter);

            var taskInfoContainer = ProgramHelper.BindTaskInfo(taskObjectData, tasks);

            return taskInfoContainer.OrderBy(x => x.Sequence).ToList();
        }

        protected string GetDisplayTextType(string type)
            => !string.IsNullOrEmpty(type) ? Shift.Common.Humanizer.TitleCase(type) : null;

        internal void SetTab(string tab)
        {
            SelectTab(PublicationTab, tab == "publication");
            SelectTab(TasksTab, tab == "tasks");

            void SelectTab(NavItem nav, bool selected)
            {
                if (selected)
                    nav.IsSelected = true;
            }
        }
    }
}