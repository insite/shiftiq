using System;

using InSite.Admin.Standards.Documents.Utilities;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Standards.Documents.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<StandardDocumentFilter>
    {
        #region Properties

        public string DefaultDocumentType => Request.QueryString["type"];

        public override StandardDocumentFilter Filter
        {
            get
            {
                if (!IsPostBack)
                    TrySelectDocumentType(DefaultDocumentType);

                var filter = new StandardDocumentFilter
                {
                    OrganizationIdentifier = Organization.Key,
                    DocumentType = DocumentType.Value,
                    Title = Title.Text,
                    Level = Level.Value,
                    Keyword = Keyword.Text,
                    Posted =
                    {
                        Since = UtcPostedSince.Value?.UtcDateTime,
                        Before = UtcPostedBefore.Value?.UtcDateTime
                    },
                    IsTemplate = IsTemplate.ValueAsBoolean,
                    PrivacyScope = new StandardDocumentFilter.PrivacyScopeInfo(PrivacyScope.Value, User.UserIdentifier),
                    CreatedBy = CreatedBy.Value
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                DocumentType.Value = value.DocumentType;
                Title.Text = value.Title;
                Level.Value = value.Level;
                Keyword.Text = value.Keyword;
                IsTemplate.ValueAsBoolean = value.IsTemplate;
                PrivacyScope.Value = value.PrivacyScope?.Name;
                CreatedBy.Value = value.CreatedBy;

                UtcPostedSince.Value = value.Posted.Since;
                UtcPostedBefore.Value = value.Posted.Before;

                if (!IsPostBack)
                    TrySelectDocumentType(DefaultDocumentType);
            }
        }

        #endregion

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
                SectionInfo.BindComboBox(DocumentType);

            base.OnLoad(e);
        }

        #region Helper methods

        public override void Clear()
        {
            DocumentType.ClearSelection();
            TrySelectDocumentType(DefaultDocumentType);

            Title.Text = null;
            Level.ClearSelection();
            Keyword.Text = null;
            IsTemplate.ValueAsBoolean = null;
            PrivacyScope.ClearSelection();
            CreatedBy.Value = null;

            UtcPostedSince.Value = null;
            UtcPostedBefore.Value = null;
        }

        private void TrySelectDocumentType(string value)
        {
            if (value.IsEmpty())
                return;

            var option = DocumentType.FindOptionByValue(value, true);
            if (option != null)
                option.Selected = true;
        }

        #endregion
    }
}