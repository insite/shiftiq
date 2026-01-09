using System;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Constant;

using DocumentTypeConst = Shift.Sdk.UI.DocumentType;

namespace InSite.UI.Portal.Standards.Documents.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<StandardDocumentFilter>
    {
        private string DefaultDocumentType => Request.QueryString["type"];

        public override StandardDocumentFilter Filter
        {
            get
            {
                var filter = new StandardDocumentFilter
                {
                    OrganizationIdentifier = Organization.Key,
                    DocumentType = DefaultDocumentType,
                    Title = Title.Text,
                    Level = Level.Value,
                    Keyword = Keyword.Text,
                    Posted =
                    {
                        Since = UtcPostedSince.Value,
                        Before = UtcPostedBefore.Value
                    },
                    IsTemplate = IsTemplate.ValueAsBoolean,

                    IsPortal = CurrentSessionState.Identity.IsGranted(PermissionIdentifiers.Admin_Standards_Documents, PermissionOperation.Configure),
                    CreatedBy = User.UserIdentifier
                };
                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                Title.Text = value.Title;
                Level.Value = value.Level;
                Keyword.Text = value.Keyword;
                IsTemplate.ValueAsBoolean = !string.Equals(DefaultDocumentType, DocumentTypeConst.NationalOccupationStandard, StringComparison.OrdinalIgnoreCase) ? false : (bool?)null;

                UtcPostedSince.Value = value.Posted.Since;
                UtcPostedBefore.Value = value.Posted.Before;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                Translate(IsTemplate);
                Translate(Level);
            }
        }

        public override void Clear()
        {
            Title.Text = null;
            Level.ClearSelection();
            Keyword.Text = null;

            UtcPostedSince.Value = null;
            UtcPostedBefore.Value = null;

            IsTemplate.ClearSelection();
        }
    }
}