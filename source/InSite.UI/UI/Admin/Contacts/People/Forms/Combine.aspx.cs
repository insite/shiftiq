using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Application.Contacts.Read;
using InSite.Application.Credentials.Write;
using InSite.Application.Gradebooks.Write;
using InSite.Application.People.Write;
using InSite.Application.Progresses.Write;
using InSite.Application.Records.Read;
using InSite.Application.Registrations.Write;
using InSite.Application.Responses.Write;
using InSite.Application.Users.Write;
using InSite.Common;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Contacts;
using InSite.Domain.Records;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.Web.Data;
using InSite.Web.Security;

using Shift.Common;
using Shift.Constant;


namespace InSite.Admin.Contacts.People.Forms
{
    public partial class Combine : AdminBasePage
    {
        #region Constants

        private const string SearchUrl = "/ui/admin/contacts/people/search";
        private const string EditUrl = "/ui/admin/contacts/people/edit";

        #endregion

        #region Classes

        private enum MergeItemType { First, Second };

        private class ValueItem
        {
            public string Code { get; }
            public string Name { get; }
            public string Value1 { get; }
            public string Value2 { get; }
            public bool IsSame { get; }

            public ValueItem(string code, string name, string value1, string value2)
            {
                Code = code;
                Name = name;
                Value1 = value1;
                Value2 = value2;
                IsSame = string.Equals(Value1, Value2);
            }
        }

        private class MergeItem
        {
            public string Code { get; set; }
            public MergeItemType Type { get; set; }
        }

        private static class ValueCodes
        {
            public const string FirstName = "FirstName";
            public const string MiddleName = "MiddleName";
            public const string LastName = "LastName";
            public const string Email = "Email";
            public const string Honorific = "Honorific";
            public const string Gender = "Gender";
            public const string Birthdate = "Birthdate";
            public const string Employer = "Employer";
            public const string JobTitle = "JobTitle";
            public const string MembershipStatus = "MembershipStatus";
            public const string EmergencyContactName = "EmergencyContactName";
            public const string EmergencyContactPhoneNumber = "EmergencyContactPhoneNumber";
            public const string EmergencyContactRelationship = "EmergencyContactRelationship";
            public const string PhonePreferred = "PhonePreferred";
            public const string PhoneHome = "PhoneHome";
            public const string PhoneWork = "PhoneWork";
            public const string PhoneMobile = "PhoneMobile";
            public const string Pager = "Pager";
            public const string PhoneOther = "PhoneOther";
            public const string MembershipStartDate = "MembershipStartDate";
            public const string MembershipEndDate = "MembershipEndDate";
            public const string ArchiveStatus = "ArchiveStatus";
            public const string Referrer = "Referrer";
            public const string ReferrerOther = "ReferrerOther";
            public const string ShippingPreference = "ShippingPreference";
            public const string Region = "Region";
            public const string WebsiteURL = "WebsiteURL";
            public const string TradeworkerNumber = "TradeworkerNumber";
            public const string PersonCode = "PersonCode";
            public const string EnglishAsSecondLanguage = "EnglishAsSecondLanguage";
            public const string HomeAddress = "HomeAddress";
            public const string ShippingAddress = "ShippingAddress";
            public const string BillingAddress = "BillingAddress";
            public const string WorkAddress = "WorkAddress";
        }

        [Serializable]
        private class RowDataItem
        {
            public string Code { get; }
            public bool IsSame { get; }

            public RowDataItem(ValueItem item)
            {
                Code = item.Code;
                IsSame = item.IsSame;
            }
        }

        #endregion

        #region Properties

        private Guid? UserIdentifier1
            => Guid.TryParse(Request["user1"], out var result) ? result : (Guid?)null;

        private Guid? UserIdentifier2
            => Guid.TryParse(Request["user2"], out var result) ? result : (Guid?)null;

        private RowDataItem[] RowData
        {
            get => (RowDataItem[])ViewState[nameof(RowData)];
            set => ViewState[nameof(RowData)] = value;
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CombineButton.Click += CombineButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanEdit || !CanDelete)
                HttpResponseHelper.Redirect(SearchUrl);

            var items = GetItems();
            if (items == null)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(Page);

            ValueRepeater1.DataSource = items;
            ValueRepeater1.DataBind();

            ValueRepeater2.DataSource = items;
            ValueRepeater2.DataBind();

            RowData = items.Select(x => new RowDataItem(x)).ToArray();

            CancelButton.NavigateUrl = SearchUrl;
        }

        #endregion

        #region Event handlers

        private void CombineButton_Click(object sender, EventArgs e)
        {
            Merge();

            HttpResponseHelper.Redirect(EditUrl + $"?contact={UserIdentifier1}&status=combined");
        }

        #endregion

        #region Methods (get data)

        private List<ValueItem> GetItems()
        {
            if (!UserIdentifier1.HasValue || !UserIdentifier2.HasValue || UserIdentifier1.Value == UserIdentifier2.Value)
                return null;

            var person1 = ServiceLocator.PersonSearch.GetPerson(
                UserIdentifier1.Value, Organization.OrganizationIdentifier,
                x => x.User,
                x => x.EmployerGroup,
                x => x.HomeAddress,
                x => x.ShippingAddress,
                x => x.BillingAddress,
                x => x.WorkAddress);
            if (person1 == null)
                return null;

            var person2 = ServiceLocator.PersonSearch.GetPerson(
                UserIdentifier2.Value, Organization.OrganizationIdentifier,
                x => x.User,
                x => x.EmployerGroup,
                x => x.HomeAddress,
                x => x.ShippingAddress,
                x => x.BillingAddress,
                x => x.WorkAddress);
            if (person2 == null)
                return null;

            var user1 = person1.User;
            var user2 = person2.User;
            var items = new List<ValueItem>();

            var membershipStatus1 = TCollectionItemCache.GetName(person1.MembershipStatusItemIdentifier);
            var membershipStatus2 = TCollectionItemCache.GetName(person2.MembershipStatusItemIdentifier);
            var esl1 = string.Equals(person1.FirstLanguage, "Not English", StringComparison.OrdinalIgnoreCase) ? "Yes" : null;
            var esl2 = string.Equals(person2.FirstLanguage, "Not English", StringComparison.OrdinalIgnoreCase) ? "Yes" : null;

            AddValue(ValueCodes.FirstName, "First Name", user1.FirstName, user2.FirstName);
            AddValue(ValueCodes.MiddleName, "Middle Name", user1.MiddleName, user2.MiddleName);
            AddValue(ValueCodes.LastName, "Last Name", user1.LastName, user2.LastName);
            AddValue(ValueCodes.Email, "Email", user1.Email, user2.Email);
            AddValue(ValueCodes.Honorific, "Honorific", user1.Honorific, user2.Honorific);
            AddValue(ValueCodes.Gender, "Gender", person1.Gender, person2.Gender);
            AddValue(ValueCodes.Birthdate, "Birthdate", $"{person1.Birthdate:MMM d, yyyy}", $"{person2.Birthdate:MMM d, yyyy}");
            AddValue(ValueCodes.Employer, "Employer", person1.EmployerGroup?.GroupName, person2.EmployerGroup?.GroupName);
            AddValue(ValueCodes.JobTitle, "Job Title", person1.JobTitle, person2.JobTitle);
            AddValue(ValueCodes.MembershipStatus, "Membership Status", membershipStatus1, membershipStatus2);
            AddValue(ValueCodes.EmergencyContactName, "Emergency Contact Name", person1.EmergencyContactName, person2.EmergencyContactName);
            AddValue(ValueCodes.EmergencyContactPhoneNumber, "Emergency Contact Phone Number", person1.EmergencyContactPhone, person2.EmergencyContactPhone);
            AddValue(ValueCodes.EmergencyContactRelationship, "Emergency Contact Relationship", person1.EmergencyContactRelationship, person2.EmergencyContactRelationship);
            AddValue(ValueCodes.PhonePreferred, "Phone (Preferred)", person1.Phone, person2.Phone);
            AddValue(ValueCodes.PhoneHome, "Phone (Home)", person1.PhoneHome, person2.PhoneHome);
            AddValue(ValueCodes.PhoneWork, "Phone (Work)", person1.PhoneWork, person2.PhoneWork);
            AddValue(ValueCodes.PhoneMobile, "Phone (Mobile)", user1.PhoneMobile, user2.PhoneMobile);
            AddValue(ValueCodes.PhoneOther, "Phone (Other)", person1.PhoneOther, person2.PhoneOther);

            AddValue(ValueCodes.MembershipStartDate, "Membership Start Date", $"{person1.MemberStartDate:MMM d, yyyy}", $"{person2.MemberStartDate:MMM d, yyyy}");
            AddValue(ValueCodes.MembershipEndDate, "Membership End Date", $"{person1.MemberEndDate:MMM d, yyyy}", $"{person2.MemberEndDate:MMM d, yyyy}");
            AddValue(ValueCodes.ArchiveStatus, "Archive Status", user1.UtcArchived.HasValue ? "Archived" : "Not Archived", user2.UtcArchived.HasValue ? "Archived" : "Not Archived");
            AddValue(ValueCodes.Referrer, "Referrer", person1.Referrer, person2.Referrer);
            AddValue(ValueCodes.ReferrerOther, "Referrer (Other)", person1.ReferrerOther, person2.ReferrerOther);
            AddValue(ValueCodes.ShippingPreference, "Shipping Preference", person1.ShippingPreference, person2.ShippingPreference);
            AddValue(ValueCodes.Region, "Region", person1.Region, person2.Region);
            AddValue(ValueCodes.WebsiteURL, "Website URL", person1.WebSiteUrl, person2.WebSiteUrl);
            AddValue(ValueCodes.TradeworkerNumber, "Tradeworker Number", person1.TradeworkerNumber, person2.TradeworkerNumber);
            AddValue(ValueCodes.PersonCode, LabelHelper.GetLabelContentText("Person Code"), person1.PersonCode, person2.PersonCode);
            AddValue(ValueCodes.EnglishAsSecondLanguage, "English as Second Language", esl1, esl2);
            AddValue(ValueCodes.HomeAddress, "Home Address", GetAddress(person1.HomeAddress), GetAddress(person2.HomeAddress));
            AddValue(ValueCodes.ShippingAddress, "Shipping Address", GetAddress(person1.ShippingAddress), GetAddress(person2.ShippingAddress));
            AddValue(ValueCodes.BillingAddress, "Billing Address", GetAddress(person1.BillingAddress), GetAddress(person2.BillingAddress));
            AddValue(ValueCodes.WorkAddress, "Work Address", GetAddress(person1.WorkAddress), GetAddress(person2.WorkAddress));

            return items;

            void AddValue(string code, string name, string value1, string value2)
            {
                if (value1.IsNotEmpty() || value2.IsNotEmpty())
                    items.Add(new ValueItem(code, name, value1, value2));
            }

            string GetAddress(QPersonAddress address)
            {
                if (address == null)
                    return null;

                var html = LocationHelper.ToHtml(address.Street1, address.Street2, address.City, address.Province, address.PostalCode, address.Country, null, null);

                if (address.Description.IsNotEmpty())
                    html = address.Description + "<br/>" + html;

                return html;
            }
        }

        #endregion

        #region Methods (merge)

        private void Merge()
        {
            var p1 = UserIdentifier1.HasValue
                ? ServiceLocator.PersonSearch.GetPerson(
                    UserIdentifier1.Value,
                    Organization.OrganizationIdentifier,
                    x => x.User,
                    x => x.HomeAddress,
                    x => x.ShippingAddress,
                    x => x.BillingAddress,
                    x => x.WorkAddress
                    )
                : null;

            var p2 = UserIdentifier2.HasValue
                ? ServiceLocator.PersonSearch.GetPerson(
                    UserIdentifier2.Value,
                    Organization.OrganizationIdentifier,
                    x => x.User,
                    x => x.HomeAddress,
                    x => x.ShippingAddress,
                    x => x.BillingAddress,
                    x => x.WorkAddress
                    )
                : null;

            if (p1 == null || p2 == null)
                HttpResponseHelper.Redirect(SearchUrl);

            var u1 = p1.User;
            var u2 = p2.User;

            var mergeItems = GetMergeItems();
            var commands = new List<Command>();

            MergeRegistrations(u1, u2, commands);
            MergeGrades(u1, u2, commands);
            MergeCredentials(u1, u2, commands);
            MergeSurveyResponses(u1, u2, commands);

            MergeFields(p1, p2, mergeItems);
            MergeGroups(u1, u2);
            MergeConnections(u1, u2);
            MergeComments(u1, u2);

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            PersonStore.Delete(u2.UserIdentifier, Organization.OrganizationIdentifier);

            MergeUniqueFields(u1, p1, u2, p2, mergeItems);
        }

        private void MergeUniqueFields(QUser u1, QPerson p1, QUser u2, QPerson p2, List<MergeItem> mergeItems)
        {
            var emailMergeItem = mergeItems.Find(x => x.Code.Equals(ValueCodes.Email, StringComparison.OrdinalIgnoreCase));
            if (emailMergeItem != null && emailMergeItem.Type == MergeItemType.Second)
            {
                if (UserSearch.SelectByEmail(u2.Email) == null)
                    u1.Email = u2.Email;
                else
                    u1.EmailAlternate = u2.Email;

                UserStore.Update(u1, OrganizationSearch.GetPersonFullNamePolicy(Organization.Identifier));
            }

            var personCodeMergeItem = mergeItems.Find(x => x.Code.Equals(ValueCodes.PersonCode, StringComparison.OrdinalIgnoreCase));
            if (personCodeMergeItem != null && personCodeMergeItem.Type == MergeItemType.Second)
                ServiceLocator.SendCommand(new ModifyPersonFieldText(p1.PersonIdentifier, PersonField.PersonCode, p2.PersonCode));
        }

        private void MergeFields(QPerson p1, QPerson p2, List<MergeItem> mergeItems)
        {
            if (mergeItems.Count == 0)
                return;

            var mergers = GetMergers();

            foreach (var mergeItem in mergeItems)
            {
                if (mergeItem.Type == MergeItemType.Second && mergers.TryGetValue(mergeItem.Code, out var merger))
                    merger.Invoke(p1, p2);
            }

            var u1 = p1.User;

            p1.User = null;
            u1.Persons = null;

            UserStore.Update(u1, OrganizationSearch.GetPersonFullNamePolicy(Organization.Identifier));
            PersonStore.Update(p1);
        }

        private void MergeGroups(QUser u1, QUser u2)
        {
            var roles1 = MembershipSearch.Select(x => x.UserIdentifier == u1.UserIdentifier && x.Group.OrganizationIdentifier == Organization.OrganizationIdentifier);
            var roles2 = MembershipSearch.Select(x => x.UserIdentifier == u2.UserIdentifier && x.Group.OrganizationIdentifier == Organization.OrganizationIdentifier);

            foreach (var role2 in roles2)
            {
                var role1 = roles1.FirstOrDefault(x => x.GroupIdentifier == role2.GroupIdentifier);

                MembershipStore.Delete(role2);

                if ((role1 == null || string.IsNullOrEmpty(role1.MembershipType) && !string.IsNullOrEmpty(role2.MembershipType))
                    && MembershipPermissionHelper.CanModifyMembership(role2.GroupIdentifier)
                    )
                {
                    role2.UserIdentifier = u1.UserIdentifier;
                    MembershipHelper.Save(role2, false, false);
                }
            }
        }

        private void MergeConnections(QUser u1, QUser u2)
        {
            var connections1 = UserConnectionSearch.Select(x =>
                x.FromUserIdentifier == u1.UserIdentifier && x.ToUser.Persons.Any(y => y.OrganizationIdentifier == Organization.OrganizationIdentifier)
                || x.ToUserIdentifier == u1.UserIdentifier && x.FromUser.Persons.Any(y => y.OrganizationIdentifier == Organization.OrganizationIdentifier)
                , "FromUserIdentifier"
                );

            var connections2 = UserConnectionSearch.Select(x =>
                x.FromUserIdentifier == u2.UserIdentifier && x.ToUser.Persons.Any(y => y.OrganizationIdentifier == Organization.OrganizationIdentifier)
                || x.ToUserIdentifier == u2.UserIdentifier && x.FromUser.Persons.Any(y => y.OrganizationIdentifier == Organization.OrganizationIdentifier)
                , "FromUserIdentifier"
                );

            foreach (var c in connections2)
            {
                UserConnectionStore.Delete(c);

                if (c.FromUserIdentifier == u2.UserIdentifier)
                {
                    if (!connections1.Any(x => x.ToUserIdentifier == c.ToUserIdentifier))
                    {
                        c.FromUserIdentifier = u1.UserIdentifier;
                        ServiceLocator.SendCommand(new ConnectUser(c.FromUserIdentifier, c.ToUserIdentifier, c.IsLeader, c.IsManager, c.IsSupervisor, c.IsValidator, c.Connected));
                    }
                }
                else
                {
                    if (!connections1.Any(x => x.FromUserIdentifier == c.FromUserIdentifier))
                    {
                        c.ToUserIdentifier = u1.UserIdentifier;
                        ServiceLocator.SendCommand(new ConnectUser(c.FromUserIdentifier, c.ToUserIdentifier, c.IsLeader, c.IsManager, c.IsSupervisor, c.IsValidator, c.Connected));
                    }
                }
            }
        }

        private void MergeComments(QUser u1, QUser u2)
        {
            var comments2 = QCommentSearch.GetPersonComments(u2.UserIdentifier, Organization.Identifier);

            foreach (var comment in comments2)
            {
                comment.TopicUserIdentifier = u1.UserIdentifier;

                QCommentStore.Update(comment);
            }
        }

        private void MergeRegistrations(QUser u1, QUser u2, List<Command> commands)
        {
            var registrations1 = ServiceLocator.RegistrationSearch.GetRegistrationsByCandidate(u1.UserIdentifier);
            var registrations2 = ServiceLocator.RegistrationSearch.GetRegistrationsByCandidate(u2.UserIdentifier);

            foreach (var registration in registrations2)
            {
                if (registration.Event.OrganizationIdentifier == Organization.Identifier && registrations1.Find(x => x.EventIdentifier == registration.EventIdentifier) == null)
                    commands.Add(new ChangeCandidate(registration.RegistrationIdentifier, u1.UserIdentifier));
            }
        }

        private void MergeGrades(QUser u1, QUser u2, List<Command> commands)
        {
            var gradebooks1 = ServiceLocator.RecordSearch.GetGradebooks(new QGradebookFilter { OrganizationIdentifier = Organization.OrganizationIdentifier, StudentIdentifier = u1.UserIdentifier });
            var gradebooks2 = ServiceLocator.RecordSearch.GetGradebooks(new QGradebookFilter { OrganizationIdentifier = Organization.OrganizationIdentifier, StudentIdentifier = u2.UserIdentifier });

            foreach (var gradebook in gradebooks2)
            {
                var data = ServiceLocator.RecordSearch.GetGradebookState(gradebook.GradebookIdentifier);

                if (data.IsLocked)
                    commands.Add(new UnlockGradebook(gradebook.GradebookIdentifier));

                commands.Add(new DeleteEnrollment(gradebook.GradebookIdentifier, u2.UserIdentifier));

                if (gradebooks1.Find(x => x.GradebookIdentifier == gradebook.GradebookIdentifier) == null)
                {
                    var create = ServiceLocator.RecordSearch.CreateCommandToAddEnrollment(null, gradebook.GradebookIdentifier, u1.UserIdentifier, null, null, null);
                    commands.Add(create);

                    var scores = ServiceLocator.RecordSearch.GetGradebookScores(new QProgressFilter { GradebookIdentifier = gradebook.GradebookIdentifier, StudentUserIdentifier = u2.UserIdentifier });
                    foreach (var score in scores)
                    {
                        var command = ServiceLocator.RecordSearch.CreateCommandToAddProgress(null, gradebook.GradebookIdentifier, score.GradeItemIdentifier, u1.UserIdentifier);

                        commands.Add(command);

                        var newId = command.AggregateIdentifier;

                        if (score.ProgressPercent.HasValue)
                            commands.Add(new ChangeProgressPercent(newId, score.ProgressPercent, score.ProgressGraded));

                        if (score.ProgressPoints.HasValue || score.ProgressMaxPoints.HasValue)
                            commands.Add(new ChangeProgressPoints(newId, score.ProgressPoints, score.ProgressMaxPoints, score.ProgressGraded));

                        if (!string.IsNullOrEmpty(score.ProgressText))
                            commands.Add(new ChangeProgressText(newId, score.ProgressText, score.ProgressGraded));

                        if (score.ProgressNumber.HasValue)
                            commands.Add(new ChangeProgressNumber(newId, score.ProgressNumber, score.ProgressGraded));

                        if (!string.IsNullOrEmpty(score.ProgressComment))
                            commands.Add(new ChangeProgressComment(newId, score.ProgressComment));
                    }
                }

                if (data.IsLocked)
                    commands.Add(new LockGradebook(gradebook.GradebookIdentifier));
            }
        }

        private void MergeCredentials(QUser u1, QUser u2, List<Command> commands)
        {
            var credentials1 = ServiceLocator.AchievementSearch.GetCredentials(new VCredentialFilter { OrganizationIdentifier = Organization.OrganizationIdentifier, UserIdentifier = u1.UserIdentifier });
            var credentials2 = ServiceLocator.AchievementSearch.GetCredentials(new VCredentialFilter { OrganizationIdentifier = Organization.OrganizationIdentifier, UserIdentifier = u2.UserIdentifier });

            foreach (var credential2 in credentials2)
            {
                commands.Add(new DeleteCredential(credential2.CredentialIdentifier));

                var credential1 = credentials1.Find(x => x.AchievementIdentifier == credential2.AchievementIdentifier);

                if (credential1 == null)
                {
                    CreateCredential(u1, credential2, commands);
                }
                else if (credential1.CredentialGranted == null && credential2.CredentialGranted.HasValue
                        || credential1.CredentialGranted.HasValue && credential2.CredentialGranted.HasValue && credential1.CredentialGranted < credential2.CredentialGranted
                        )
                {
                    commands.Add(new DeleteCredential(credential1.CredentialIdentifier));

                    CreateCredential(u1, credential2, commands);
                }
            }
        }

        private void CreateCredential(QUser u1, VCredential credential, List<Command> commands)
        {
            var expiration = new Expiration
            {
                Type = credential.CredentialExpirationType.ToEnum(ExpirationType.None),
                Date = credential.CredentialExpirationFixedDate
            };

            if (expiration.Type == ExpirationType.Relative && credential.CredentialExpirationLifetimeQuantity.HasValue)
            {
                expiration.Lifetime = new Lifetime
                {
                    Unit = credential.CredentialExpirationLifetimeUnit,
                    Quantity = credential.CredentialExpirationLifetimeQuantity.Value
                };
            }

            var id = ServiceLocator.AchievementSearch.GetCredentialIdentifier(credential?.CredentialIdentifier, credential.AchievementIdentifier, u1.UserIdentifier);

            commands.Add(new CreateCredential(id, Organization.OrganizationIdentifier, credential.AchievementIdentifier, u1.UserIdentifier, credential.CredentialAssigned));
            commands.Add(new ChangeCredentialExpiration(id, expiration));
            commands.Add(new TagCredential(id, credential.CredentialNecessity, credential.CredentialPriority));

            if (credential.CredentialGranted.HasValue)
                commands.Add(new GrantCredential(id, credential.CredentialGranted.Value, null, null, credential.EmployerGroupIdentifier, credential.EmployerGroupStatus));

            if (credential.CredentialRevoked.HasValue)
                commands.Add(new RevokeCredential(id, credential.CredentialRevoked.Value, "Merged", null));

            if (credential.CredentialExpired.HasValue)
                commands.Add(new ExpireCredential(id, credential.CredentialExpired.Value));
        }

        private void MergeSurveyResponses(QUser u1, QUser u2, List<Command> commands)
        {
            var responses = ServiceLocator.SurveySearch.GetResponseSessions(u2.UserIdentifier);
            foreach (var response in responses)
                commands.Add(new ChangeResponseUser(response.ResponseSessionIdentifier, u1.UserIdentifier));
        }

        private List<MergeItem> GetMergeItems()
        {
            if (ValueRepeater1.Items.Count != ValueRepeater2.Items.Count || ValueRepeater1.Items.Count != RowData.Length)
                throw ApplicationError.Create("Invalid control state");

            var result = new List<MergeItem>();

            for (var i = 0; i < RowData.Length; i++)
            {
                var rowData = RowData[i];
                var selected1 = (IRadioButton)ValueRepeater1.Items[i].FindControl("Selected");
                var selected2 = (IRadioButton)ValueRepeater2.Items[i].FindControl("Selected");

                if (rowData.IsSame)
                    continue;

                var item = new MergeItem
                {
                    Code = rowData.Code,
                    Type = selected1.Checked
                        ? MergeItemType.First
                        : selected2.Checked
                            ? MergeItemType.Second
                            : throw ApplicationError.Create("Invalid selection value: " + i)
                };

                result.Add(item);
            }

            return result;
        }

        private Dictionary<string, Action<QPerson, QPerson>> GetMergers()
        {
            var list = new Dictionary<string, Action<QPerson, QPerson>>(StringComparer.OrdinalIgnoreCase)
            {
                { ValueCodes.FirstName, (p1, p2) => p1.User.FirstName = p2.User.FirstName },
                { ValueCodes.MiddleName, (p1, p2) => p1.User.MiddleName = p2.User.MiddleName },
                { ValueCodes.LastName, (p1, p2) => p1.User.LastName = p2.User.LastName },
                { ValueCodes.Honorific, (p1, p2) => p1.User.Honorific = p2.User.Honorific },
                { ValueCodes.Gender, (p1, p2) => p1.Gender = p2.Gender },
                { ValueCodes.Birthdate, (p1, p2) => p1.Birthdate = p2.Birthdate },
                { ValueCodes.Employer, (p1, p2) => p1.EmployerGroupIdentifier = p2.EmployerGroupIdentifier },
                { ValueCodes.JobTitle, (p1, p2) => p1.JobTitle = p2.JobTitle },
                { ValueCodes.MembershipStatus, (p1, p2) => p1.MembershipStatusItemIdentifier = p2.MembershipStatusItemIdentifier },
                { ValueCodes.EmergencyContactName, (p1, p2) => p1.EmergencyContactName = p2.EmergencyContactName },
                { ValueCodes.EmergencyContactPhoneNumber, (p1, p2) => p1.EmergencyContactPhone = p2.EmergencyContactPhone },
                { ValueCodes.EmergencyContactRelationship, (p1, p2) => p1.EmergencyContactRelationship = p2.EmergencyContactRelationship },
                { ValueCodes.PhonePreferred, (p1, p2) => p1.Phone = p2.Phone },
                { ValueCodes.PhoneHome, (p1, p2) => p1.PhoneHome = p2.PhoneHome },
                { ValueCodes.PhoneWork, (p1, p2) => p1.PhoneWork = p2.PhoneWork },
                { ValueCodes.PhoneMobile, (p1, p2) => p1.User.PhoneMobile = p2.User.PhoneMobile },
                { ValueCodes.PhoneOther, (p1, p2) => p1.PhoneOther = p2.PhoneOther },
                { ValueCodes.MembershipStartDate, (p1, p2) => p1.MemberStartDate = p2.MemberStartDate },
                { ValueCodes.MembershipEndDate, (p1, p2) => p1.MemberEndDate = p2.MemberEndDate },
                { ValueCodes.Referrer, (p1, p2) => p1.Referrer = p2.Referrer },
                { ValueCodes.ReferrerOther, (p1, p2) => p1.ReferrerOther = p2.ReferrerOther },
                { ValueCodes.ShippingPreference, (p1, p2) => p1.ShippingPreference = p2.ShippingPreference },
                { ValueCodes.Region, (p1, p2) => p1.Region = p2.Region },
                { ValueCodes.WebsiteURL, (p1, p2) => p1.WebSiteUrl = p2.WebSiteUrl },
                { ValueCodes.TradeworkerNumber, (p1, p2) => p1.TradeworkerNumber = p2.TradeworkerNumber },
                { ValueCodes.EnglishAsSecondLanguage, (p1, p2) => p1.FirstLanguage = p2.FirstLanguage },

                { ValueCodes.HomeAddress, (p1, p2) => CopyAddress(p1.GetAddress(ContactAddressType.Home), p2.HomeAddress) },
                { ValueCodes.ShippingAddress, (p1, p2) => CopyAddress(p1.GetAddress(ContactAddressType.Shipping), p2.ShippingAddress) },
                { ValueCodes.BillingAddress, (p1, p2) => CopyAddress(p1.GetAddress(ContactAddressType.Billing), p2.BillingAddress) },
                { ValueCodes.WorkAddress, (p1, p2) => CopyAddress(p1.GetAddress(ContactAddressType.Work), p2.WorkAddress) }
            };

            return list;

            void CopyAddress(QPersonAddress a1, QPersonAddress a2)
            {
                a1.Description = a2?.Description;
                a1.Street1 = a2?.Street1;
                a1.Street2 = a2?.Street2;
                a1.City = a2?.City;
                a1.Province = a2?.Province;
                a1.PostalCode = a2?.PostalCode;
                a1.Country = a2?.Country;
            }
        }

        #endregion
    }
}