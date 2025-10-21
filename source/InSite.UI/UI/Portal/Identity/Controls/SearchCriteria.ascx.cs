using System;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.UI.Desktops.Design.Users.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<PersonFilter>
    {
        public override PersonFilter Filter
        {
            get
            {
                var filter = new PersonFilter
                {
                    OrganizationIdentifier = Organization.Key,
                    IsApproved = true,
                    NameFilterType = MatchNamesWith.Value,
                    FullName = Name.Text,
                    EmailContains = Email.Text,
                    SessionCount = SessionCount.ValueAsInt,
                    LastAuthenticatedSince = LastAuthenticatedSince.Value,
                    LastAuthenticatedBefore = LastAuthenticatedBefore.Value
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                MatchNamesWith.Value = value.NameFilterType;
                Name.Text = value.FullName;
                Email.Text = value.EmailContains;
                SessionCount.ValueAsInt = value.SessionCount;
                LastAuthenticatedSince.Value = value.LastAuthenticatedSince?.DateTime;
                LastAuthenticatedBefore.Value = value.LastAuthenticatedBefore?.DateTime;
            }
        }

        public override void Clear()
        {
            MatchNamesWith.ClearSelection();
            Name.Text = null;
            Email.Text = null;
            SessionCount.ValueAsInt = null;
            LastAuthenticatedSince.Value = null;
            LastAuthenticatedBefore.Value = null;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Translate(MatchNamesWith);
        }
    }
}