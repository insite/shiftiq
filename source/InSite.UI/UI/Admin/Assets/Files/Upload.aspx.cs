using System;
using System.IO;

using InSite.Application.Cases.Write;
using InSite.Application.Files.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Assets.Files
{
    public partial class Upload : AdminBasePage, IHasParentLinkParameters, IOverrideWebRouteParent
    {
        private Guid? UserIdentifier => Guid.TryParse(Request.QueryString["user"], out var id) ? id : (Guid?)null;

        private Guid? CaseIdentifier => Guid.TryParse(Request.QueryString["case"], out var id) ? id : (Guid?)null;

        private FileObjectType ObjectType => UserIdentifier.HasValue
            ? FileObjectType.User
            : FileObjectType.Issue;

        private Guid ObjectIdentifier => UserIdentifier ?? CaseIdentifier ?? Guid.Empty;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var (isValid, title) = Detail.BindDefaultsToControls(ObjectType, ObjectIdentifier);
                if (!isValid)
                    HttpResponseHelper.Redirect("/");

                PageHelper.AutoBindHeader(this, null, title);

                CancelButton.NavigateUrl = GetReturnUrl();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Detail.UploadError += (s, a) => FileAlert.AddMessage(AlertType.Error, a.Value);

            SaveButton.Click += SaveButton_Click;
        }

        public override void ApplyAccessControl()
        {
            var parent = GetParent();

            if (!Identity.IsActionAuthorized(parent.Name)
                || !Identity.IsGranted(parent.ToolkitNumber, PermissionOperation.Write))
            {
                CreateAccessDeniedException();
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            var model = Detail.CreateFile(ObjectIdentifier, ObjectType);

            if (ObjectType == FileObjectType.Issue)
                AddIssueAttachment(model);

            HttpResponseHelper.Redirect(GetReturnUrl());
        }

        private void AddIssueAttachment(FileStorageModel model)
        {
            var command = new AddAttachment(
                ObjectIdentifier,
                model.Properties.DocumentName,
                Path.GetExtension(model.FileName),
                model.FileIdentifier,
                DateTimeOffset.UtcNow,
                User.UserIdentifier
            );

            ServiceLocator.SendCommand(command);
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            switch (ObjectType)
            {
                case FileObjectType.User:
                    return parent.Name.EndsWith("/edit") ? $"contact={ObjectIdentifier}&panel=attachments" : null;
                case FileObjectType.Issue:
                    return parent.Name.EndsWith("/outline") ? $"case={ObjectIdentifier}&panel=attachments#case-attachments" : null;
                default:
                    throw new NotImplementedException($"Support for {ObjectType} is not implemented");
            }
        }

        public new IWebRoute GetParent()
        {
            string actionName;

            switch (ObjectType)
            {
                case FileObjectType.User:
                    actionName = "ui/admin/contacts/people/edit";
                    break;
                case FileObjectType.Issue:
                    actionName = "ui/admin/workflow/cases/outline";
                    break;
                default:
                    throw new NotImplementedException($"Support for {ObjectType} is not implemented");
            }

            return WebRoute.GetWebRoute(actionName);
        }

        private new string GetReturnUrl()
        {
            var parent = GetParent();
            var parameters = GetParentLinkParameters(parent);

            return $"/{parent.Name}?{parameters}";
        }
    }
}