using System;

using InSite.Application.Files.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Assets.Files
{
    public partial class Edit : AdminBasePage, IHasParentLinkParameters, IOverrideWebRouteParent
    {
        private Guid FileIdentifier => Guid.TryParse(Request.QueryString["file"], out var id) ? id : Guid.Empty;

        private Guid? BackToCase => Guid.TryParse(Request.QueryString["case"], out var id) ? id : (Guid?)null;

        private FileObjectType ObjectType
        {
            get
            {
                var type = ViewState[nameof(ObjectType)];
                if (type != null)
                    return (FileObjectType)type;

                ViewState[nameof(ObjectType)] = Model.ObjectType;

                return Model.ObjectType;
            }
        }

        private Guid ObjectIdentifier
        {
            get
            {
                var id = ViewState[nameof(ObjectIdentifier)];
                if (id != null)
                    return (Guid)id;

                ViewState[nameof(ObjectIdentifier)] = Model.ObjectIdentifier;

                return Model.ObjectIdentifier;
            }
        }

        private bool _isLoaded;
        private FileStorageModel _model;
        private FileStorageModel Model
        {
            get
            {
                if (_isLoaded)
                    return _model;

                (_, _model) = ServiceLocator.StorageService.GetFileAndAuthorize(Identity, FileIdentifier);

                _isLoaded = true;

                return _model;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                if (Model == null)
                    HttpResponseHelper.Redirect("/");

                var (isValid, title) = Detail.BindModelToControls(Model);
                if (!isValid)
                    HttpResponseHelper.Redirect("/");

                HistoryList.BindModelToControls(Model);

                PageHelper.AutoBindHeader(this, null, title);

                CancelButton.NavigateUrl = GetReturnUrl();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

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

            Detail.UpdateFile(FileIdentifier);

            HttpResponseHelper.Redirect(GetReturnUrl());
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            if (Model == null)
                return null;

            if (BackToCase.HasValue)
                return parent.Name.EndsWith("/outline") ? $"case={BackToCase}&panel=attachments" : null;

            if (!parent.Name.EndsWith("/edit"))
                return null;
            
            switch (ObjectType)
            {
                case FileObjectType.User:
                    return $"contact={ObjectIdentifier}&panel=attachments";
                case FileObjectType.Response:
                    var response = ServiceLocator.SurveySearch.GetResponseSession(ObjectIdentifier);
                    return $"contact={response?.RespondentUserIdentifier}&panel=attachments#form-attachments";
                case FileObjectType.Issue:
                    var issue = ServiceLocator.IssueSearch.GetIssue(ObjectIdentifier);
                    return $"contact={issue?.TopicUserIdentifier}&panel=attachments#issue-attachments";
                default:
                    throw new NotImplementedException($"Support for {ObjectType} is not implemented");
            }
        }

        public new IWebRoute GetParent()
        {
            if (Model == null)
                return Route;

            if (BackToCase.HasValue && (
                    ObjectType == FileObjectType.Issue
                    || ObjectType == FileObjectType.Response
                    )
                )
            {
                return WebRoute.GetWebRoute("ui/admin/workflow/cases/outline");
            }

            switch (ObjectType)
            {
                case FileObjectType.User:
                case FileObjectType.Response:
                case FileObjectType.Issue:
                    return WebRoute.GetWebRoute("ui/admin/contacts/people/edit");
                default:
                    throw new NotImplementedException($"Support for {ObjectType} is not implemented");
            }
        }

        private new string GetReturnUrl()
        {
            var parent = GetParent();
            var parameters = GetParentLinkParameters(parent);

            return $"/{parent.Name}?{parameters}";
        }
    }
}