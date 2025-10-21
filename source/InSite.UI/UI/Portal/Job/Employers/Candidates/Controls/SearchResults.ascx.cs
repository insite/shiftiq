using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.UI.Portal.Jobs.Employers.Candidates.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<JobPersonFilter>
    {
        #region Classes

        private class SearchDataItem
        {
            public Guid UserIdentifier { get; set; }
            public string FullName { get; set; }
            public string CurrentCity { get; set; }
            public string JobTitle { get; set; }
            public string Qualifications { get; set; }
            public string Occupations { get; set; }
            public DateTimeOffset? LastAuthenticated { get; set; }
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDataBound += Grid_RowDataBound;
        }

        #endregion

        #region Methods (event handling)

        private void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var row = (SearchDataItem)e.Row.DataItem;

            var requestLink = (Common.Web.UI.Button)e.Row.FindControl("RequestContactLink");
            requestLink.Attributes["OnClick"] = "candidateRequestContact.showRequestContact('" + row.UserIdentifier.ToString() + "'); return false;";
        }

        #endregion

        #region Methods (data binding)

        protected override int SelectCount(JobPersonFilter filter)
            => PersonSearch.CountByPersonFilter(filter);

        protected override IListSource SelectData(JobPersonFilter filter)
        {
            filter.OrderBy = "Modified DESC";

            var data = PersonSearch.SelectList(
                filter,
                x => x.User,
                x => x.HomeAddress);

            return data.Select(x => new SearchDataItem
            {
                UserIdentifier = x.UserIdentifier,
                FullName = x.User.FullName,
                Qualifications = GetQualificationsString(x.UserIdentifier),
                CurrentCity = x.HomeAddress != null
                    ? x.HomeAddress.City
                    : null,
                LastAuthenticated = x.LastAuthenticated,
                JobTitle = x.JobTitle,
                Occupations = GetSelectedOccupations(x.OccupationStandardIdentifier)
            }).ToList().ToSearchResult();

            string GetQualificationsString(Guid _userId)
            {
                var builder = new StringBuilder();

                builder.Append(GetAchievementsListString(_userId));
                builder.Append(GetLastExperienceListString(_userId));
                builder.Append(GetEducationListString(_userId));

                return builder.ToString();
            }
        }

        #endregion

        #region Helper methods

        private string GetAchievementsListString(Guid _userId)
        {
            var builder = new StringBuilder();
            var filter = new VCredentialFilter()
            {
                UserIdentifier = _userId,
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier
            };
            var achievements = ServiceLocator.AchievementSearch.GetCredentials(filter);
            achievements = achievements.OrderByDescending(x => x.CredentialGranted ?? DateTimeOffset.MaxValue).ToList();

            if (achievements != null && achievements.Count > 0)
            {
                builder.Append("<h6>Achievements</h6>");
                builder.Append("<div><ul>");
            }

            foreach (var achievement in achievements)
                builder.AppendLine($"<li class=\"indent-proper\"><small>{achievement.AchievementTitle} - Shift iQ</small></li>");

            if (achievements != null && achievements.Count > 0)
                builder.Append("</ul></div>");

            return builder.ToString();
        }

        private string GetLastExperienceListString(Guid _userId)
        {
            var builder = new StringBuilder();
            var experiences = TCandidateExperienceSearch.SelectByContact(_userId);
            experiences = experiences.OrderByDescending(x => x.ExperienceDateTo ?? DateTime.MaxValue).ToList();

            if (experiences != null && experiences.Count > 0)
            {
                builder.Append("<h6>Experience entries</h6>");
                builder.Append("<div><ul>");
            }

            foreach (var experience in experiences)
                builder.AppendLine($"<li class=\"indent-proper\"><small>{experience.ExperienceJobTitle}, {experience.EmployerName}</small></li>");

            if (experiences != null && experiences.Count > 0)
                builder.Append("</ul></div>");

            return builder.ToString();
        }

        private string GetEducationListString(Guid _userId)
        {
            var builder = new StringBuilder();
            var educations = TCandidateEducationSearch.SelectByContact(_userId);
            educations = educations.OrderByDescending(x => x.EducationDateTo ?? DateTime.MaxValue).ToList();

            if (educations != null && educations.Count > 0)
            {
                builder.Append("<h6>Education entries</h6>");
                builder.Append("<div><ul>");
            }

            foreach (var education in educations)
                builder.AppendLine($"<li class=\"indent-proper\"><small>{education.EducationName}, {education.EducationQualification}</small></li>");

            if (educations != null && educations.Count > 0)
                builder.Append("</ul></div>");

            return builder.ToString();
        }

        private string GetSelectedOccupations(Guid? userStandardId)
        {
            var builder = new StringBuilder();

            if (userStandardId.HasValue)
            {
                var occupation = StandardSearch.BindFirst(x => x.ContentTitle, x => x.StandardIdentifier == userStandardId);

                if (occupation.HasValue())
                {
                    builder.Append("<div class=\"mt-3\"><ul>");
                    builder.AppendLine($"<li class=\"indent-proper\"><small>{occupation}</small></li>");
                    builder.Append("</ul></div>");
                }
            }

            return builder.ToString();

        }

        protected string GetCandidateConnection(object o)
        {
            if (o == null || o == DBNull.Value)
                return "";

            var organization = CurrentSessionState.Identity.Organization.Identifier;
            var user = CurrentSessionState.Identity.User.UserIdentifier;

            var employer = PersonCriteria.BindFirst(
                x => x.EmployerGroupIdentifier,
                new PersonFilter
                {
                    OrganizationIdentifier = organization,
                    UserIdentifier = user
                });

            if (employer == null)
                return "";

            var assigned = MembershipSearch.BindFirst(x => x.Assigned, x => x.UserIdentifier == (Guid)o && x.Group.GroupCategory == "Candidate" && x.GroupIdentifier == employer.Value);
            if (assigned.Year == 1)
                return "";

            return "Contact Requested " + TimeZones.FormatDateOnly(assigned, CurrentSessionState.Identity.User.TimeZone);
        }

        #endregion

    }
}