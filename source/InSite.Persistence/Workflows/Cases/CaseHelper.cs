using System;
using System.Text;

using InSite.Application.Contacts.Read;
using InSite.Application.Issues.Read;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Persistence
{
    public static class CaseHelper
    {
        #region Classes

        private class VIssueJson
        {
            public string AdministratorName { get; set; }
            public string AssigneeEmployerGroupName { get; set; }
            public string AssigneeMembershipStatus { get; set; }
            public string AssigneeName { get; set; }
            public string AssigneeOrganizationGroupNames { get; set; }
            public string EmployerGroupName { get; set; }
            public string EmployerParentGroupName { get; set; }
            public string IssueDescription { get; set; }
            public string IssueSource { get; set; }
            public DateTimeOffset? IssueReported { get; set; }
            public string IssueStatusCategory { get; set; }
            public string IssueTitle { get; set; }
            public string IssueType { get; set; }
            public string LawyerName { get; set; }
            public DateTimeOffset? IssueClosed { get; set; }
            public DateTimeOffset IssueOpened { get; set; }

            public VUserJson Administrator { get; set; }
            public VUserJson Assignee { get; set; }
            public VUserJson Lawyer { get; set; }

            public VIssueJson()
            {

            }

            public VIssueJson(VIssue issue) : this()
            {
                AdministratorName = issue.AdministratorUserName;
                AssigneeEmployerGroupName = issue.TopicEmployerGroupName;
                AssigneeMembershipStatus = issue.TopicAccountStatus;
                AssigneeName = issue.TopicUserName;
                AssigneeOrganizationGroupNames = issue.TopicGroupNames;
                EmployerGroupName = issue.IssueEmployerGroupName;
                EmployerParentGroupName = issue.IssueEmployerGroupParentGroupName;
                IssueDescription = issue.IssueDescription;
                IssueSource = issue.IssueSource;
                IssueReported = issue.IssueReported;
                IssueStatusCategory = issue.IssueStatusCategory;
                IssueTitle = issue.IssueTitle;
                IssueType = issue.IssueType;
                LawyerName = issue.LawyerUserName;
                IssueClosed = issue.IssueClosed;
                IssueOpened = issue.IssueOpened;
                Administrator = issue.Administrator != null ? new VUserJson(issue.Administrator) : null;
                Assignee = issue.Topic != null ? new VUserJson(issue.Topic) : null;
                Lawyer = issue.Lawyer != null ? new VUserJson(issue.Lawyer) : null;
            }
        }

        private class VUserJson
        {
            public string UserEmail { get; set; }
            public string UserEmailAlternate { get; set; }
            public string UserFirstName { get; set; }
            public string UserFullName { get; set; }
            public string UserLastName { get; set; }

            public VUserJson()
            {

            }

            public VUserJson(VUser user) : this()
            {
                UserEmail = user.UserEmail;
                UserEmailAlternate = user.UserEmailAlternate;
                UserFirstName = user.UserFirstName;
                UserLastName = user.UserLastName;
                UserFullName = user.UserFullName;
            }
        }

        #endregion

        public static byte[] Serialize(VIssue issue)
        {
            var data = new VIssueJson(issue);
            var json = JsonHelper.JsonExport(data);

            return Encoding.UTF8.GetBytes(json);
        }

        public static VIssue Deserialize(string json)
        {
            VIssueJson data;

            try
            {
                data = JsonHelper.JsonImport<VIssueJson>(json);
            }
            catch (JsonReaderException)
            {
                return null;
            }
            catch (ApplicationError)
            {
                return null;
            }

            return new VIssue
            {
                AdministratorUserName = data.AdministratorName,
                TopicEmployerGroupName = data.AssigneeEmployerGroupName,
                TopicAccountStatus = data.AssigneeMembershipStatus,
                TopicUserName = data.AssigneeName,
                TopicGroupNames = data.AssigneeOrganizationGroupNames,
                IssueEmployerGroupName = data.EmployerGroupName,
                IssueEmployerGroupParentGroupName = data.EmployerParentGroupName,
                IssueDescription = data.IssueDescription,
                IssueSource = data.IssueSource,
                IssueReported = data.IssueReported,
                IssueStatusCategory = data.IssueStatusCategory,
                IssueTitle = data.IssueTitle,
                IssueType = data.IssueType,
                LawyerUserName = data.LawyerName,
                IssueClosed = data.IssueClosed,
                IssueOpened = data.IssueOpened,
                IssueIdentifier = UniqueIdentifier.Create(),
                AdministratorUserIdentifier = GetVUserID(data.Administrator?.UserEmail),
                LawyerUserIdentifier = GetVUserID(data.Lawyer?.UserEmail),
                TopicUserIdentifier = GetVUserID(data.Assignee?.UserEmail)
            };

            Guid? GetVUserID(string email)
            {
                if (email == null)
                    return null;

                var vUser = SurveySearch.GetUser(email);

                if (vUser == null)
                    return null;

                return vUser.UserIdentifier;
            }
        }
    }
}
