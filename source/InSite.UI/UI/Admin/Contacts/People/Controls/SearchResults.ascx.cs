using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Admin.Contacts.People.Utilities;
using InSite.Application.Files.Read;
using InSite.Application.Issues.Read;
using InSite.Application.People.Write;
using InSite.Common;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Surveys.Forms;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Admin.Contacts.People.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<PersonFilter>
    {
        private Models.PersonSearchResultsModel _model;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDataBound += Grid_RowDataBound;

            SaveBulkButton.Click += SaveBulkButton_Click;
            MergeButton.Click += MergeButton_Click;
        }

        private void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            SetContentLabelHeaders(e);

            if (!IsContentItem(e))
                return;

            var dataItem = e.Row.DataItem;
            var userIdentifier = (Guid)DataBinder.Eval(dataItem, "UserIdentifier");

            var emailValue = DataBinder.Eval(dataItem, "Email") as string;
            var isEmailEnabled = (bool)DataBinder.Eval(dataItem, "EmailEnabled");

            var emailLink = (HyperLink)e.Row.FindControl("EmailLink");
            var emailText = (System.Web.UI.WebControls.Literal)e.Row.FindControl("EmailText");

            emailLink.Visible = isEmailEnabled;
            emailText.Visible = !isEmailEnabled;

            if (!string.IsNullOrEmpty(emailValue))
            {
                emailLink.Text = emailValue;
                emailLink.NavigateUrl = CreateMailToLink(emailValue);
                emailText.Text = emailValue;
            }

            if (Filter.ShowColumns.Count == 0 || Filter.ShowColumns.Any(c => StringHelper.Equals(c, "Home Address")))
            {
                var homeAddressLiteral = (ITextControl)e.Row.FindControl("HomeAddressLiteral");
                homeAddressLiteral.Text = LocationHelper.ToHtml(
                    DataBinder.Eval(dataItem, "HomeAddress.Street1") as string,
                    DataBinder.Eval(dataItem, "HomeAddress.Street2") as string,
                    DataBinder.Eval(dataItem, "HomeAddress.City") as string,
                    DataBinder.Eval(dataItem, "HomeAddress.Province") as string,
                    DataBinder.Eval(dataItem, "HomeAddress.PostalCode") as string,
                    null, null, null
                );
            }

            if (Filter.ShowColumns.Count == 0 || Filter.ShowColumns.Any(c => StringHelper.Equals(c, "Shipping Address")))
            {
                var shippingAddressLiteral = (ITextControl)e.Row.FindControl("ShippingAddressLiteral");
                shippingAddressLiteral.Text = LocationHelper.ToHtml(
                    DataBinder.Eval(dataItem, "ShippingAddress.Street1") as string,
                    DataBinder.Eval(dataItem, "ShippingAddress.Street2") as string,
                    DataBinder.Eval(dataItem, "ShippingAddress.City") as string,
                    DataBinder.Eval(dataItem, "ShippingAddress.Province") as string,
                    DataBinder.Eval(dataItem, "ShippingAddress.PostalCode") as string,
                    null, null, null
                );
            }

            var homeAddressPhone = DataBinder.Eval(dataItem, "PhoneHome") as string;
            var phone = DataBinder.Eval(dataItem, "Phone") as string;
            var phoneList = new StringBuilder();

            if (!string.IsNullOrEmpty(homeAddressPhone))
                phoneList.AppendFormat("<div>{0} <span class='form-text'>home</span></div>", homeAddressPhone);

            if (!string.IsNullOrEmpty(phone))
                phoneList.AppendFormat("<div>{0} <span class='form-text'>mobile</span></div>", phone);

            var phoneLiteral = (ITextControl)e.Row.FindControl("PhoneLiteral");
            phoneLiteral.Text = phoneList.ToString();

            var statusLiteral = (ITextControl)e.Row.FindControl("StatusLiteral");
            statusLiteral.Text = DataBinder.Eval(dataItem, "UserAccessGranted") is DateTimeOffset
                ? @"<span class='badge bg-success'>Access Granted</span>"
                : @"<span class='badge bg-danger'>Access Not Granted</span>";

            if (DataBinder.Eval(dataItem, "IsArchived") as bool? == true)
                statusLiteral.Text += @" <span class='badge bg-danger'>Archived</span>";

            BindIssueStatus(e, userIdentifier);
            BindDocuments(e, userIdentifier);
        }

        private static void SetContentLabelHeaders(GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (e.Row.Cells[i].Text == "Person Code")
                        e.Row.Cells[i].Text = LabelHelper.GetLabelContentText("Person Code");
                }
            }
        }

        private void BindIssueStatus(GridViewRowEventArgs e, Guid userIdentifier)
        {
            if (!Grid.Columns.FindByName("IssueStatus").Visible)
                return;

            var statuses = _model.GetStatuses(userIdentifier);

            var issueRepeater = (Repeater)e.Row.FindControl("IssueRepeater");
            issueRepeater.Visible = statuses.Count > 0;
            issueRepeater.DataSource = statuses;
            issueRepeater.DataBind();
        }

        private void BindDocuments(GridViewRowEventArgs e, Guid userIdentifier)
        {
            if (!Grid.Columns.FindByName("Documents").Visible)
                return;

            var link = (HtmlAnchor)e.Row.FindControl("DocumentsLink");
            link.Visible = _model.HasDocuments(userIdentifier);
        }

        private void SaveBulkButton_Click(object sender, EventArgs e)
        {
            PerformBulkUpdate();
            SearchWithCurrentPageIndex(Filter);
        }

        private void PerformBulkUpdate()
        {
            if (!GetBulkAction(out var grantAccess, out var sendWelcome))
            {
                BulkUpdateStatus.AddMessage(AlertType.Error, "No bulk action selected.");
                return;
            }

            if (!SetBulkProgress("Initialization", 0, 1))
                return;

            Server.ScriptTimeout = 60 * 5; // 5 minutes

            var persons = PersonCriteria.Select(Filter, x => x.User, x => x.BillingAddress, x => x.HomeAddress, x => x.ShippingAddress, x => x.WorkAddress);

            for (var i = 0; i < persons.Count; i++)
            {
                var person = persons[i];

                if (!SetBulkProgress("Updating " + person.User.FullName, i, persons.Count))
                    return;

                if (grantAccess && !person.UserAccessGranted.HasValue)
                {
                    ServiceLocator.SendCommand(new GrantPersonAccess(person.PersonIdentifier, DateTimeOffset.UtcNow, User.FullName));

                    if (sendWelcome && person.EmailEnabled)
                        PersonHelper.SendWelcomeMessage(Organization.OrganizationIdentifier, person.UserIdentifier);
                }
                else if (!grantAccess && person.UserAccessGranted.HasValue)
                    ServiceLocator.SendCommand(new RevokePersonAccess(person.PersonIdentifier, DateTimeOffset.UtcNow, User.FullName));
            }

            BulkAction.ClearSelection();
            BulkUpdateStatus.AddMessage(AlertType.Success, "Bulk update completed successfully!");
        }

        private bool GetBulkAction(out bool grantAccess, out bool sendWelcome)
        {
            grantAccess = false;
            sendWelcome = false;

            if (BulkAction.SelectedValue == "GrantAndWelcome")
            {
                grantAccess = true;
                sendWelcome = true;
            }
            else if (BulkAction.SelectedValue == "JustGrant")
            {
                grantAccess = true;
                sendWelcome = false;
            }
            else if (BulkAction.SelectedValue != "Revoke")
            {
                return false;
            }

            return true;
        }

        private bool SetBulkProgress(string status, int position, int total)
        {
            if (BulkProgress.IsRequestCancelled)
            {
                BulkUpdateStatus.AddMessage(AlertType.Warning, "Bulk update was canceled by the user.");
                return false;
            }

            BulkProgress.UpdateContext(context =>
            {
                var progressBar = (ProgressIndicator.ContextData)context.Items["Progress"];
                progressBar.Total = total;
                progressBar.Value = position;
                context.Variables["status"] = status;
            });

            return true;
        }

        private void MergeButton_Click(object sender, EventArgs e)
        {
            var users = new List<Guid>();

            foreach (GridViewRow row in Grid.Rows)
            {
                var mergeCheckBox = (ICheckBoxControl)row.FindControl("MergeCheckBox");
                if (!mergeCheckBox.Checked)
                    continue;

                users.Add(Grid.GetDataKey<Guid>(row));

                if (users.Count == 2)
                    break;
            }

            if (users.Count == 2)
            {
                var url = $"/ui/admin/contacts/people/combine?user1={users[0]}&user2={users[1]}";
                HttpResponseHelper.Redirect(url);
            }
        }

        public void SearchAndSetPageIndex(PersonFilter filter, int pageIndex, bool refreshLastSearched)
        {
            if (refreshLastSearched)
                Search(filter, refreshLastSearched);
            else
                Search(filter, pageIndex);
        }

        protected override int SelectCount(PersonFilter filter)
        {
            var count = PersonCriteria.Count(filter);

            ButtonPanel.Visible = count > 0;

            return count;
        }

        protected override IListSource SelectData(PersonFilter filter)
        {
            var result = PersonCriteria.SelectSearchResults(filter);

            if (filter.ShowColumns.FirstOrDefault(x => string.Equals(x, "Roles", StringComparison.OrdinalIgnoreCase)) != null)
            {
                var users = result.Select(x => x.UserIdentifier).ToArray();
                var accessibleOrgs = Identity.Organizations.Select(x => x.Identifier).ToArray();
                var memberships = MembershipSearch
                    .Bind(
                        x => new
                        {
                            x.UserIdentifier,
                            x.Group.GroupName
                        },
                        x => users.Any(user => user == x.UserIdentifier)
                          && x.Group.GroupType == GroupTypes.Role
                          && accessibleOrgs.Contains(x.Group.OrganizationIdentifier))
                    .GroupBy(x => x.UserIdentifier)
                    .ToDictionary(
                        x => x.Key,
                        x => string.Join(", ", x.OrderBy(y => y.GroupName).Select(y => y.GroupName)));

                foreach (var item in result)
                    item.PermissionLists = memberships.GetOrDefault(item.UserIdentifier);
            }

            _model = CreateModel(result.Select(x => x.UserIdentifier).ToArray());

            return result.ToSearchResult();
        }

        public static Models.PersonSearchResultsModel CreateModel(Guid[] users)
        {
            var model = new Models.PersonSearchResultsModel
            {
                Users = users,
                Documents = GetDocumentsForSearchResults(users),
                Statuses = GetIssueStatusesForSearchResults(users),
                Cases = GetIssueNamesSearchResults(users)
            };

            return model;
        }

        private static Dictionary<Guid, string[]> GetIssueStatusesForSearchResults(Guid[] users)
        {
            var filter = new QIssueFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                TopicUserIdentifiers = users
            };

            var issues = ServiceLocator.IssueSearch.GetIssues(filter);

            return issues
                .Where(x => x.TopicUserIdentifier.HasValue && !string.IsNullOrEmpty(x.IssueStatusName))
                .GroupBy(x => x.TopicUserIdentifier.Value)
                .ToDictionary(x => x.Key, x => x.Select(y => y.IssueStatusName).ToArray());
        }

        private static Dictionary<Guid, string[]> GetIssueNamesSearchResults(Guid[] users)
        {
            var filter = new QIssueFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                TopicUserIdentifiers = users
            };

            var issues = ServiceLocator.IssueSearch.GetIssues(filter);

            return issues
                .Where(x => x.TopicUserIdentifier.HasValue && x.IssueStatusCategory == "Open")
                .GroupBy(x => x.TopicUserIdentifier.Value)
                .ToDictionary(x => x.Key, x => x.Select(y => y.IssueType).ToArray());
        }

        private static Dictionary<Guid, bool> GetDocumentsForSearchResults(Guid[] users)
        {
            var dictionary = new Dictionary<Guid, bool>();

            var fileList = ServiceLocator.StorageService.GetGrantedFiles(Identity, users);

            var responseList = ServiceLocator.SurveySearch.GetResponseSurveyUploads(Organization.OrganizationIdentifier, users);

            var documentList = ServiceLocator.IssueSearch.GetAttachments(new QIssueAttachmentFilter
            {
                TopicUserIdentifiers = users,
                OrganizationIdentifier = Organization.OrganizationIdentifier
            });

            foreach (var user in users)
            {
                if (fileList.Any(file => file.ObjectIdentifier == user)
                    || HasGrantedResponseFiles(user, responseList)
                    || HasGrantedIssueAttachments(user, documentList)
                    )
                {
                    dictionary[user] = true;
                }

            }

            return dictionary;
        }

        private static bool HasGrantedResponseFiles(Guid user, List<ResponseSurveyUpload> responseList)
        {
            var responses = responseList.Where(x => x.RespondentUserIdentifier == user).ToList();
            foreach (var response in responses)
            {
                var responseFiles = ServiceLocator.StorageService.ParseSurveyResponseAnswer(response.ResponseAnswerText);
                foreach (var responseFile in responseFiles)
                {
                    if (ServiceLocator.StorageService.GetGrantStatus(Identity, responseFile.FileIdentifier) != FileGrantStatus.Denied)
                        return true;
                }
            }
            return false;
        }

        private static bool HasGrantedIssueAttachments(Guid user, List<VIssueAttachment> documentList)
        {
            var documents = documentList.Where(x => x.TopicUserIdentifier == user).ToList();
            foreach (var document in documents)
            {
                if (ServiceLocator.StorageService.GetGrantStatus(Identity, document.FileIdentifier) != FileGrantStatus.Denied)
                    return true;
            }
            return false;
        }

        #region Export

        public class ExportDataItem : User
        {
            public string CreatedByUser { get; set; }
            public string ModifiedByUser { get; set; }

            public string BillingAddressCity { get; set; }
            public string BillingAddressCountry { get; set; }
            public string BillingAddressDescription { get; set; }
            public string BillingAddressPostalCode { get; set; }
            public string BillingAddressProvince { get; set; }
            public string BillingAddressStreet1 { get; set; }
            public string BillingAddressStreet2 { get; set; }

            public string HomeAddressCity { get; set; }
            public string HomeAddressCountry { get; set; }
            public string HomeAddressDescription { get; set; }
            public string HomeAddressPostalCode { get; set; }
            public string HomeAddressProvince { get; set; }
            public string HomeAddressStreet1 { get; set; }
            public string HomeAddressStreet2 { get; set; }

            public string ShippingAddressCity { get; set; }
            public string ShippingAddressCountry { get; set; }
            public string ShippingAddressDescription { get; set; }
            public string ShippingAddressPostalCode { get; set; }
            public string ShippingAddressProvince { get; set; }
            public string ShippingAddressStreet1 { get; set; }
            public string ShippingAddressStreet2 { get; set; }

            public string WorkAddressCity { get; set; }
            public string WorkAddressCountry { get; set; }
            public string WorkAddressDescription { get; set; }
            public string WorkAddressPostalCode { get; set; }
            public string WorkAddressProvince { get; set; }
            public string WorkAddressStreet1 { get; set; }
            public string WorkAddressStreet2 { get; set; }

            public Guid? EmployerGroupIdentifier { get; set; }
            public string EmployerGroupName { get; set; }
            public string EmployerGroupDescription { get; set; }
            public string EmployerGroupType { get; set; }
            public string EmployerGroupLabel { get; set; }
            public string EmployerGroupCode { get; set; }
            public string EmployerGroupCategory { get; set; }
            public string EmployerGroupStatus { get; set; }
            public string EmployerGroupParent { get; set; }

            public string PersonCode { get; set; }

            public string PersonAccessRevokedBy { get; set; }
            public string PersonJobsApprovedBy { get; set; }
            public string PersonCitizenship { get; set; }
            public string PersonEducationLevel { get; set; }
            public string PersonEmergencyContactName { get; set; }
            public string PersonEmergencyContactPhone { get; set; }
            public string PersonEmergencyContactRelationship { get; set; }
            public string PersonEmployeeUnion { get; set; }
            public string PersonFirstLanguage { get; set; }
            public string PersonGender { get; set; }
            public string PersonJobTitle { get; set; }
            public string PersonLanguage { get; set; }
            public string PersonPhone { get; set; }
            public string PersonPhoneHome { get; set; }
            public string PersonPhoneOther { get; set; }
            public string PersonPhoneWork { get; set; }
            public string PersonRegion { get; set; }
            public string PersonCredentialingCountry { get; set; }
            public string PersonShippingPreference { get; set; }
            public string PersonSocialInsuranceNumber { get; set; }
            public string PersonUserAccessGrantedBy { get; set; }
            public string PersonUserApproveReason { get; set; }
            public string PersonWebSiteUrl { get; set; }
            public string PersonMembershipStatus { get; set; }

            public DateTimeOffset? PersonAccessRevoked { get; set; }
            public DateTimeOffset? PersonJobsApproved { get; set; }
            public DateTimeOffset? PersonBirthdate { get; set; }
            public DateTimeOffset PersonCreated { get; set; }
            public DateTimeOffset? PersonLastAuthenticated { get; set; }
            public DateTimeOffset? PersonMemberEndDate { get; set; }
            public DateTimeOffset? PersonMemberStartDate { get; set; }
            public DateTimeOffset PersonModified { get; set; }
            public DateTimeOffset? PersonUserAccessGranted { get; set; }
        }

        public override IListSource GetExportData(PersonFilter filter, bool empty)
        {
            filter.OrderBy = "User.FullName";

            var data = PersonCriteria.Select(filter,
                    x => x.User,
                    x => x.BillingAddress,
                    x => x.HomeAddress,
                    x => x.ShippingAddress,
                    x => x.WorkAddress,
                    x => x.EmployerGroup.Parent,
                    x => x.MembershipStatus
                )
                .ToList();

            var modifiersData = UserSearch
                .Bind(
                    x => new { x.UserIdentifier, x.FullName },
                    new UserFilter { IncludeUserIdentifiers = data.SelectMany(x => new[] { x.CreatedBy, x.ModifiedBy }).Distinct().ToArray() })
                .ToDictionary(x => x.UserIdentifier, x => x.FullName);

            var result = new List<ExportDataItem>(data.Count);

            foreach (var dataItem in data)
            {
                var exportItem = new ExportDataItem();

                dataItem.User.ShallowCopyTo(exportItem);

                exportItem.CreatedByUser = modifiersData.ContainsKey(dataItem.CreatedBy)
                    ? modifiersData[dataItem.CreatedBy]
                    : UserNames.Someone;
                exportItem.ModifiedByUser = modifiersData.ContainsKey(dataItem.ModifiedBy)
                    ? modifiersData[dataItem.ModifiedBy]
                    : UserNames.Someone;

                if (dataItem.BillingAddress != null)
                {
                    exportItem.BillingAddressCity = dataItem.BillingAddress.City;
                    exportItem.BillingAddressCountry = dataItem.BillingAddress.Country;
                    exportItem.BillingAddressDescription = dataItem.BillingAddress.Description;
                    exportItem.BillingAddressPostalCode = dataItem.BillingAddress.PostalCode;
                    exportItem.BillingAddressProvince = dataItem.BillingAddress.Province;
                    exportItem.BillingAddressStreet1 = dataItem.BillingAddress.Street1;
                    exportItem.BillingAddressStreet2 = dataItem.BillingAddress.Street2;
                }

                if (dataItem.HomeAddress != null)
                {
                    exportItem.HomeAddressCity = dataItem.HomeAddress.City;
                    exportItem.HomeAddressCountry = dataItem.HomeAddress.Country;
                    exportItem.HomeAddressDescription = dataItem.HomeAddress.Description;
                    exportItem.HomeAddressPostalCode = dataItem.HomeAddress.PostalCode;
                    exportItem.HomeAddressProvince = dataItem.HomeAddress.Province;
                    exportItem.HomeAddressStreet1 = dataItem.HomeAddress.Street1;
                    exportItem.HomeAddressStreet2 = dataItem.HomeAddress.Street2;
                }

                if (dataItem.ShippingAddress != null)
                {
                    exportItem.ShippingAddressCity = dataItem.ShippingAddress.City;
                    exportItem.ShippingAddressCountry = dataItem.ShippingAddress.Country;
                    exportItem.ShippingAddressDescription = dataItem.ShippingAddress.Description;
                    exportItem.ShippingAddressPostalCode = dataItem.ShippingAddress.PostalCode;
                    exportItem.ShippingAddressProvince = dataItem.ShippingAddress.Province;
                    exportItem.ShippingAddressStreet1 = dataItem.ShippingAddress.Street1;
                    exportItem.ShippingAddressStreet2 = dataItem.ShippingAddress.Street2;
                }

                if (dataItem.WorkAddress != null)
                {
                    exportItem.WorkAddressCity = dataItem.WorkAddress.City;
                    exportItem.WorkAddressCountry = dataItem.WorkAddress.Country;
                    exportItem.WorkAddressDescription = dataItem.WorkAddress.Description;
                    exportItem.WorkAddressPostalCode = dataItem.WorkAddress.PostalCode;
                    exportItem.WorkAddressProvince = dataItem.WorkAddress.Province;
                    exportItem.WorkAddressStreet1 = dataItem.WorkAddress.Street1;
                    exportItem.WorkAddressStreet2 = dataItem.WorkAddress.Street2;
                }

                if (dataItem.EmployerGroup != null)
                {
                    exportItem.EmployerGroupIdentifier = dataItem.EmployerGroup.GroupIdentifier;
                    exportItem.EmployerGroupName = dataItem.EmployerGroup.GroupName;
                    exportItem.EmployerGroupDescription = dataItem.EmployerGroup.GroupDescription;
                    exportItem.EmployerGroupType = dataItem.EmployerGroup.GroupType;
                    exportItem.EmployerGroupLabel = dataItem.EmployerGroup.GroupLabel;
                    exportItem.EmployerGroupCode = dataItem.EmployerGroup.GroupCode;
                    exportItem.EmployerGroupCategory = dataItem.EmployerGroup.GroupCategory;
                    exportItem.EmployerGroupStatus = dataItem.EmployerGroup.GroupStatus;
                    exportItem.EmployerGroupParent = dataItem.EmployerGroup.Parent?.GroupName;
                }

                exportItem.PersonCode = dataItem.PersonCode;

                exportItem.PersonAccessRevoked = dataItem.AccessRevoked;
                exportItem.PersonAccessRevokedBy = dataItem.AccessRevokedBy;
                exportItem.PersonJobsApproved = dataItem.JobsApproved;
                exportItem.PersonJobsApprovedBy = dataItem.JobsApprovedBy;
                exportItem.PersonBirthdate = dataItem.Birthdate == DateTime.MinValue ? null : dataItem.Birthdate;
                exportItem.PersonCitizenship = dataItem.Citizenship;
                exportItem.PersonCreated = dataItem.Created;
                exportItem.PersonEducationLevel = dataItem.EducationLevel;
                exportItem.PersonEmergencyContactName = dataItem.EmergencyContactName;
                exportItem.PersonEmergencyContactPhone = dataItem.EmergencyContactPhone;
                exportItem.PersonEmergencyContactRelationship = dataItem.EmergencyContactRelationship;
                exportItem.PersonEmployeeUnion = dataItem.EmployeeUnion;
                exportItem.PersonFirstLanguage = dataItem.FirstLanguage;
                exportItem.PersonGender = dataItem.Gender;
                exportItem.PersonJobTitle = dataItem.JobTitle;
                exportItem.PersonLanguage = dataItem.Language;
                exportItem.PersonLastAuthenticated = dataItem.LastAuthenticated;
                exportItem.PersonMemberEndDate = dataItem.MemberEndDate;
                exportItem.PersonMemberStartDate = dataItem.MemberStartDate;
                exportItem.PersonModified = dataItem.Modified;
                exportItem.PersonPhone = dataItem.Phone;
                exportItem.PersonPhoneHome = dataItem.PhoneHome;
                exportItem.PersonPhoneOther = dataItem.PhoneOther;
                exportItem.PersonPhoneWork = dataItem.PhoneWork;
                exportItem.PersonRegion = dataItem.Region;
                exportItem.PersonCredentialingCountry = dataItem.CredentialingCountry;
                exportItem.PersonShippingPreference = dataItem.ShippingPreference;
                exportItem.PersonSocialInsuranceNumber = dataItem.SocialInsuranceNumber;
                exportItem.PersonUserAccessGranted = dataItem.UserAccessGranted;
                exportItem.PersonUserAccessGrantedBy = dataItem.UserAccessGrantedBy;
                exportItem.PersonUserApproveReason = dataItem.UserApproveReason;
                exportItem.PersonWebSiteUrl = dataItem.WebSiteUrl;
                exportItem.PersonMembershipStatus = dataItem.MembershipStatus?.ItemName;

                result.Add(exportItem);
            }

            return result.ToSearchResult();
        }

        #endregion

        #region Helper methods

        public static string CreateMailToLink(object email)
            => !ValueConverter.IsNull(email) ? $"mailto:{email}" : null;

        protected string Localize(object date)
            => (date == null)
                ? string.Empty
                : TimeZones.Format((DateTimeOffset)date, User.TimeZone, true);

        protected string GetDateString(DateTime? date)
        {
            return date.HasValue ? date.Value.ToString("MMM d, yyyy") : string.Empty;
        }

        #endregion
    }
}

