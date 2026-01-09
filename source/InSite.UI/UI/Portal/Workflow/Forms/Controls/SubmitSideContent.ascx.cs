using System;

using InSite.Common.Web.UI;
using InSite.Domain.Surveys.Forms;
using InSite.Persistence;
using InSite.UI.Layout.Portal;
using InSite.Web.Routing;

namespace InSite.UI.Portal.Workflow.Forms.Controls
{
    public partial class SubmitSideContent : BaseUserControl
    {
        internal void BindControlsToModel(SurveyForm survey)
        {
            var hasCourse = false;

            if (survey != null)
            {
                MenuHeading.InnerText = survey.GetTitle();

                var courseId = CourseSearch.BindActivityFirst(
                    x => x.Module.Unit.CourseIdentifier,
                    x => x.SurveyFormIdentifier == survey.Identifier);

                hasCourse = courseId != Guid.Empty && TGroupPermissionSearch.IsAccessAllowed(courseId, CurrentSessionState.Identity);

                if (hasCourse)
                {
                    ReturnToCourseButton.NavigateUrl = RoutingConfiguration.PortalCourseUrl(courseId);
                    var course = CourseSearch.SelectCourse(courseId);
                    MenuHeading.InnerText = course.CourseName;
                }
            }

            ReturnToCourseButton.Visible = hasCourse;

            if (Page.Master is PortalMaster portalMaster)
            {
                portalMaster.HideBreadcrumbsAndTitle();

                if (!hasCourse)
                {
                    portalMaster.HideSideContent();
                    portalMaster.SidebarVisible(false);
                }
            }
            else if (Page.Master is PortalPrintMaster printMaster)
            {
                printMaster.HideSideContent();
            }
        }
    }
}