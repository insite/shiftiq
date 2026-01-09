using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Groups.Write;
using InSite.Common;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.Web.Security;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;

using BreadcrumbItem = Shift.Contract.BreadcrumbItem;

namespace InSite.UI.Admin.Contacts.People
{
    public partial class Search : SearchPage<PersonFilter>
    {
        public override string EntityName => "Person";

        private static readonly ICollection<string> Exclude = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "AccessGrantedToCmds",
            "AinNumber",
            "BillingAddressDescription",
            "DefaultPassword",
            "HomeAddressDescription",
            "Initials",
            "LoginOrganizationCode",
            "OldUserPasswordHash",
            "PersonEmployeeUnion",
            "PersonUserApproveReason",
            "PersonWebSiteUrl",
            "ShippingAddressDescription",
            "UserPasswordChangeRequested",
            "UserPasswordHash",
            "WorkAddressDescription"
        };

        private static readonly List<string> Sort = new List<string>
        {
            "UserIdentifier",
            "PersonCode",
            "Email",
            "EmailAlternate",
            "FullName",
            "FirstName",
            "MiddleName",
            "LastName",
            "PersonPhone",
            "PhoneMobile",
            "PersonPhoneOther",
            "PersonLastAuthenticated",
            "PersonCreated",
            "CreatedByUser",
            "PersonModified",
            "ModifiedByUser",
            "PersonUserAccessGranted",
            "PersonUserAccessGrantedby",
            "PersonBirthdate",
            "PersonGender",
            "PersonLanguage",
            "PersonFirstLanguage",
            "PersonJobTitle",
            "PersonJobsConsenttoShare",
            "PersonJobsApproved",
            "PersonJobsApprovedby",
            "EmployerGroupName",
            "EmployerGroupType",
            "EmployerGroupLabel",
            "EmployerGroupCode",
            "EmployerGroupCategory",
            "EmployerGroupStatus",
            "EmployerGroupDescription",
            "EmployerGroupIdentifier",
            "EmployerGroupParent",
            "PersonEmergencyContactName",
            "PersonEmergencyContactPhone",
            "PersonEmergencyContactRelationship",
            "PersonCitizenship",
            "PersonCredentialingCountry",
            "PersonEducationLevel",
            "PersonImmigrationApplicant",
            "PersonImmigrationCategory",
            "PersonImmigrationDestination",
            "PersonImmigrationDisability",
            "PersonImmigrationLandingDate",
            "PersonImmigrationNumber",
            "PersonConsentConsultation",
            "PersonMembershipStatus",
            "PersonMemberStartDate",
            "PersonMemberEndDate",
            "PersonReferrer",
            "PersonReferrerOther",
            "PersonRegion",
            "PersonSocialInsuranceNumber",
            "PersonShippingPreference",
            "HomeAddressStreet1",
            "HomeAddressStreet2",
            "HomeAddressCity",
            "HomeAddressProvince",
            "HomeAddressPostalCode",
            "HomeAddressCountry",
            "WorkAddressStreet1",
            "WorkAddressStreet2",
            "WorkAddressCity",
            "WorkAddressProvince",
            "WorkAddressPostalCode",
            "WorkAddressCountry",
            "ShippingAddressStreet1",
            "ShippingAddressStreet2",
            "ShippingAddressCity",
            "ShippingAddressProvince",
            "ShippingAddressPostalCode",
            "ShippingAddressCountry",
            "BillingAddressStreet1",
            "BillingAddressStreet2",
            "BillingAddressCity",
            "BillingAddressProvince",
            "BillingAddressPostalCode",
            "BillingAddressCountry",
            "UserLicenseAccepted",
            "EmailVerified",
            "UserPasswordChanged",
            "UserPasswordExpired",
            "DefaultPasswordExpired",
            "PersonAccessRevoked",
            "PersonAccessRevokedby",
            "IsArchived",
            "UtcArchived",
            "UtcUnarchived",
            "MultiFactorAuthentication",
            "MultiFactorAuthenticationCode",
            "OAuthProviderUserId",
            "PrimaryLoginMethod",
            "SecondaryLoginMethod",
            "Honorific",
            "ImageURL",
            "TimeZone",
            "PersonPhoneHome",
            "PersonPhoneWork",
            "SoundexFirstName",
            "SoundexLastName",
            "IsCloaked",
            "AccountCloaked"
        };

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            var rename = new Dictionary<string, string>(comparer)
            {
                { "IsCloaked", "Account Cloaked" },
                { "AccountCloaked", "Account Cloaked Date" },
                { "EmployerGroupLabel", "Employer Group Tag" },
                { "OAuthProviderUserId", "External Authorization Provider User ID" },
                { "PersonUserAccessGrantedBy", "Person Access Granted By" },
                { "PersonUserAccessGranted", "Person Access Granted Date" },
                { "IsArchived", "Person Archived" },
                { "UtcArchived", "Person Archived Date" },
                { "CreatedByUser", "Person Created By" },
                { "PersonCreated", "Person Created Date" },
                { "PersonFirstLanguage", "Person English Language Learner" },
                { "PersonCode", LabelHelper.GetLabelContentText("Person Code") },
                { "ModifiedByUser", "Person Modified By" },
                { "PersonModified", "Person Modified Date" },
                { "UtcUnarchived", "Person Unarchived Date" },
                { "PersonPhone", "Phone" },
                { "PersonPhoneOther", "Phone Other" },
                { "DefaultPasswordExpired", "User Temp Password Expired" },
                { "TimeZone", "User Time Zone" },
            };

            var columns = BaseSearchDownload
                .GetColumns(typeof(InSite.Admin.Contacts.People.Controls.SearchResults.ExportDataItem))
                .Where(x => !Exclude.Contains(x.Name))
                .OrderBy(x =>
                {
                    var n = Sort.FindIndex(y => y.ToLower().Equals(x.Name.ToLower()));
                    return n != -1 ? n : int.MaxValue;
                })
                .ThenBy(x => x.Title)
                .ToList();

            foreach (var column in columns)
            {
                if (rename.ContainsKey(column.Name))
                    column.Title = rename[column.Name];
            }

            return columns;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SearchResults.Searched += SearchResults_Searched;
            SearchResults.DataStateChanged += SearchResults_DataStateChanged;

            AppendToGroupButton.Click += AppendToGroupButton_Click;
            AddToNewGroupButton.Click += AddToNewGroupButton_Click;

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Person", "/ui/admin/contacts/people/create", null, null));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                BuildMailingListTab.Visible = false;

                GroupID.Filter.OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier;
            }
        }

        private void SearchResults_Searched(object sender, EventArgs e)
        {
            BuildMailingListTab.Visible = SearchResults.HasRows;
        }

        private void SearchResults_DataStateChanged(object sender, BooleanValueArgs args)
        {
            BuildMailingListTab.Visible = false;
        }

        private void AppendToGroupButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            AddToMailingList(GroupID.Value.Value);
        }

        private void AddToNewGroupButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var id = UniqueIdentifier.Create();

            ServiceLocator.SendCommands(new Command[]
            {
                new CreateGroup(id, Organization.Identifier, GroupTypes.List, GroupName.Text),
                new DescribeGroup(id, null, null, null, "Mailing List")
            });

            AddToMailingList(id);
        }

        private void AddToMailingList(Guid groupId)
        {
            if (!MembershipPermissionHelper.CanModifyMembership(groupId))
                return;

            var results = SearchResults.GetExportData().GetList();
            var users = results.Cast<object>().Select(x => (Guid)DataBinder.Eval(x, "UserIdentifier")).ToList();

            foreach (var user in users)
                MembershipStore.Save(MembershipFactory.Create(user, groupId, Organization.Identifier));

            HttpResponseHelper.Redirect($"/ui/admin/contacts/groups/edit?contact={groupId}");
        }
    }
}