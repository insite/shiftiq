using System;

using InSite.Web.Routing;

namespace InSite.Custom.CMDS.Portal.Courses
{
    public class CmdsCourseLink
    {
        public static string GetCourseLink(Guid course)
        {
            return RoutingConfiguration.PortalCourseUrl(course);
        }

        public static string GetOrientationLink(Guid course)
        {
            var pages = ServiceLocator.PageSearch.Select(x => x.ObjectType == "Course" && x.ObjectIdentifier == course);
            if (pages.Length > 0)
            {
                var path = ServiceLocator.PageSearch.GetPagePath(pages[0].PageIdentifier, false);
                if (path != null)
                    return path;
            }

            return $"#";
        }
    }
}