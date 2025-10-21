using System;

using InSite.Common;
using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.UI.Portal.Contacts.People.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<PersonFilter>
    {
        public override PersonFilter Filter
        {
            get
            {
                var filter = new PersonFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    EmailContains = Email.Text,
                    FullName = FullName.Text,
                    CodeContains = PersonCode.Text,
                    UtcCreatedSince = CreatedSince.Value?.UtcDateTime,
                    UtcCreatedBefore = CreatedBefore.Value?.UtcDateTime,
                    LastAuthenticatedSince = LastAuthenticatedSince.Value?.UtcDateTime,
                    LastAuthenticatedBefore = LastAuthenticatedBefore.Value?.UtcDateTime,
                    UpstreamUserIdentifiers = new Guid[] { User.Identifier }
                };
                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                Email.Text = value.EmailContains;
                FullName.Text = value.FullName;
                PersonCode.Text = value.CodeContains;
                CreatedSince.Value = value.UtcCreatedSince;
                CreatedBefore.Value = value.UtcCreatedBefore;
                LastAuthenticatedSince.Value = value.LastAuthenticatedSince;
                LastAuthenticatedBefore.Value = value.LastAuthenticatedBefore;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PersonCode.EmptyMessage = LabelHelper.GetLabelContentText("Person Code");
        }

        public override void Clear()
        {
            FullName.Text = null;
            Email.Text = null;
            PersonCode.Text = null;
            CreatedSince.Value = null;
            CreatedBefore.Value = null;
            LastAuthenticatedSince.Value = null;
            LastAuthenticatedBefore.Value = null;
        }
    }
}