using System;

using InSite.Application.Sites.Read;
using InSite.Application.Sites.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Sites.Pages;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Sites.Pages
{
    public partial class ChangeSettings : AdminBasePage, IHasParentLinkParameters
    {
        private Guid PageId => Guid.TryParse(Request["id"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ContentControl.AutoPostBack = true;
            ContentControl.ValueChanged += ContentControl_ValueChanged;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var page = ServiceLocator.PageSearch.GetPage(PageId);

                if (page == null || page.OrganizationIdentifier != Organization.Identifier)
                {
                    HttpResponseHelper.Redirect($"/ui/admin/sites/pages/search");
                    return;
                }

                PageHelper.AutoBindHeader(this, null, page.Title);

                PageDetails.BindPage(page);

                ContentLabels.Text = page.ContentLabels;
                Hook.Text = page.Hook;
                Icon.Text = page.PageIcon;
                ContentControl.Value = page.ContentControl;
                SetContentTemplate(page);

                CancelButton.NavigateUrl = $"/ui/admin/sites/pages/outline?id={PageId}&panel=setup&tab=settings";
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var entity = GetEntityValues();

            var commands = new PageCommandGenerator().
                GetDifferencePageSetupCommands(
                    entity,
                    GetInputValues(entity)
                );

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            HttpResponseHelper.Redirect($"/ui/admin/sites/pages/outline?id={PageId}&panel=setup&tab=settings");
        }

        private void ContentControl_ValueChanged(object sender, EventArgs e)
        {
            ContentTemplateSelected();
        }

        private void ContentTemplateSelected()
        {
            CatalogPanel.Visible = ContentControl.Value == "Catalog";
            if (!CatalogPanel.Visible)
                CatalogIdentifier.Value = null;

            CoursePanel.Visible = ContentControl.Value == "Course";
            if (!CoursePanel.Visible)
                CourseIdentifier.Value = null;

            ProgramPanel.Visible = ContentControl.Value == "Program";
            if (!ProgramPanel.Visible)
                ProgramIdentifier.Value = null;

            SurveyPanel.Visible = ContentControl.Value == "Survey";
            if (!SurveyPanel.Visible)
                SurveyIdentifier.Value = null;
        }

        private void SetContentTemplate(QPage page)
        {
            CatalogPanel.Visible = page.ContentControl == "Catalog";
            if (CatalogPanel.Visible && page.ObjectIdentifier.HasValue)
                CatalogIdentifier.ValueAsGuid = page.ObjectIdentifier.Value;

            CoursePanel.Visible = page.ContentControl == "Course";
            if (CoursePanel.Visible && page.ObjectIdentifier.HasValue)
                CourseIdentifier.Value = page.ObjectIdentifier.Value;

            ProgramPanel.Visible = page.ContentControl == "Program";
            if (ProgramPanel.Visible && page.ObjectIdentifier.HasValue)
                ProgramIdentifier.Value = page.ObjectIdentifier.Value;

            SurveyPanel.Visible = page.ContentControl == "Survey";
            if (SurveyPanel.Visible && page.ObjectIdentifier.HasValue)
                SurveyIdentifier.Value = page.ObjectIdentifier.Value;
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={PageId}&panel=setup&tab=settings"
                : null;
        }

        private PageState GetEntityValues()
        {
            var page = ServiceLocator.PageSearch.GetPage(PageId);

            return new PageState()
            {
                Identifier = page.PageIdentifier,
                Icon = page.PageIcon,
                Hook = page.Hook,
                ContentLabels = page.ContentLabels,
                ContentControl = page.ContentControl,

                Assessment = page.ObjectType == "Assessment" ? page.ObjectIdentifier : null,
                Catalog = page.ObjectType == "Catalog" ? page.ObjectIdentifier : null,
                Course = page.ObjectType == "Course" ? page.ObjectIdentifier : null,
                Program = page.ObjectType == "Program" ? page.ObjectIdentifier : null,
                Survey = page.ObjectType == "Survey" ? page.ObjectIdentifier : null
            };
        }

        private PageState GetInputValues(PageState entity)
        {
            // There is no assessment input on this screen, so carry the existing value forward. It
            // is only cleared when the layout is switched to something other than an assessment.

            return new PageState()
            {
                Hook = Hook.Text,
                Icon = Icon.Text,
                ContentControl = ContentControl.Value,
                ContentLabels = ContentLabels.Text,
                Assessment = ContentControl.Value == "Assessment" ? entity.Assessment : null,
                Catalog = CatalogIdentifier.ValueAsGuid,
                Course = CourseIdentifier.Value,
                Survey = SurveyIdentifier.Value,
                Program = ProgramIdentifier.Value
            };
        }
    }
}
