using System;
using System.Collections.Specialized;
using System.Web.Routing;

using InSite.Application.Sites.Read;
using InSite.Persistence.Content;
using InSite.Web.Routing;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Records.Programs
{
    public class ProgramState
    {
        public ProgramModel Model { get; set; }
        public PortalPageModel PortalPage { get; set; }

        private string _programSlug;
        private Guid? _taskId;

        public ProgramState(RouteData routes, NameValueCollection parameters)
        {
            _programSlug = GetString("slug", routes, parameters);
            _taskId = GetIdentifier("task", routes, parameters);
        }

        private string GetString(string name, RouteData routes, NameValueCollection parameters) =>
            (routes.Values[name] as string).IfNullOrEmpty(parameters[name]).EmptyIfNull();

        private Guid? GetIdentifier(string name, RouteData routes, NameValueCollection parameters) =>
            (routes.Values[name] as string).IfNullOrEmpty(parameters[name]).ToGuidNullable();

        public Guid? GetTaskId() => _taskId;

        public void LoadModel(PortalPageModel model)
        {
            PortalPage = model;
            Load();
        }

        public bool LoadModel()
        {
            var identity = CurrentSessionState.Identity;

            var hasWriteAccess = identity.IsGranted(PermissionIdentifiers.Admin_Sites, PermissionOperation.Write);

            var appUrl = ServiceLocator.Urls.GetApplicationUrl(identity.Organization.Code);

            PortalPage = new PortalPageModel()
            {
                Page = new QPage()
            };

            if (_programSlug.HasNoValue())
                return false;

            PortalPage.Page.PageSlug = _programSlug;

            var page = ServiceLocator
                .PageSearch
                .BindFirst(x => x, x => x.PageSlug == _programSlug
                        && x.ObjectType == "Program" && x.ObjectIdentifier != null
                        && x.OrganizationIdentifier == CurrentSessionState.Identity.Organization.Identifier);

            if (page != null)
            {
                PortalPage.Page.ObjectType = "Program";
                PortalPage.Page.ObjectIdentifier = page.ObjectIdentifier;
            }

            PortalPage.Path = GetPreviewUrl(_programSlug);

            if (string.IsNullOrEmpty(PortalPage.Path))
                throw new ArgumentNullException("PortalPage.Path");

            Load();

            PortalPage.SetEditLinkUrl(identity.IsAuthenticated, identity.IsOperator, hasWriteAccess, appUrl);

            return true;
        }

        public static string GetPreviewUrl(string programSlug)
            => RoutingConfiguration.PortalProgramUrl(programSlug);

        public static string GetPreviewUrl(string programSlug, Guid taskId)
            => RoutingConfiguration.PortalProgramUrl(programSlug, taskId);

        private void Load()
        {
            var identity = CurrentSessionState.Identity;
            var user = identity.User.UserIdentifier;

            var program = PortalPage.Page.ObjectType == "Program" && PortalPage.Page.ObjectIdentifier.HasValue
                ? Persistence.ProgramSearch1.SelectFirst(x => x.ProgramIdentifier == PortalPage.Page.ObjectIdentifier.Value, y => y.Tasks)
                : null;

            Model = new ProgramModel(program, user, _programSlug);
        }
    }
}