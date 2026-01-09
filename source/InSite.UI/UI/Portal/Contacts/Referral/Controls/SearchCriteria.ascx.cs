using System;
using System.Linq;

using InSite.Common;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Constant;

namespace InSite.UI.Portal.Contacts.Referral.Controls
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
                    EmployerGroups = Identity.Groups.Where(g => g.Type == GroupType.Employer)
                                    .Select(g => g.Identifier)
                                    .ToArray(),
                    IssueStatusEffectiveSince = IssueStatusEffectiveSince.Value,
                    MustHaveCompletedCases = CompletedCasesOnly.Checked,
                    IssueType = IssueType.Value
                };
                GetCheckedShowColumns(filter);

                if (Organization.Toolkits.Contacts.PortalSearchActiveMembershipReasons)
                    filter.MembershipReasonExpirySince = DateTimeOffset.Now;

                return filter;
            }
            set
            {
                Email.Text = value.EmailContains;
                FullName.Text = value.FullName;
                PersonCode.Text = value.CodeContains;
                IssueStatusEffectiveSince.Value = value.IssueStatusEffectiveSince?.LocalDateTime;
                CompletedCasesOnly.Checked = value.MustHaveCompletedCases;
                IssueType.Value = value.IssueType;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PersonCode.EmptyMessage = LabelHelper.GetLabelContentText("Person Code");
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            IssueType.Settings.CollectionName = CollectionName.Cases_Classification_Type;
            IssueType.Settings.OrganizationIdentifier = Organization.OrganizationIdentifier;
        }

        public override void Clear()
        {
            FullName.Text = null;
            Email.Text = null;
            PersonCode.Text = null;
            IssueStatusEffectiveSince.Value = null;
            CompletedCasesOnly.Checked = false;
            IssueType.Value = null;
        }
    }
}