using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Jobs.Candidates.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<PersonFilter>
    {
        private class PersonItem
        {
            public Guid UserIdentifier { get; set; }
            public DateTimeOffset Created { get; set; }
            public DateTimeOffset Modified { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public bool? Followup { get; set; }
            public string Email { get; set; }
            public string Country { get; set; }
            public string City { get; set; }
            public string Street1 { get; set; }
            public string Phone { get; set; }
            public bool Approved { get; set; }
            public bool Archived { get; set; }
            public string OccupationInterest { get; set; }
            public string OccupationList { get; set; }
            public int? StreamKey { get; set; }
            public string IsActivelySeeking { get; set; }

            public string NameString { get; set; }
            public int? CompletionProfilePercent { get; set; }
            public int? CompletionResumePercent { get; set; }
        }

        public List<Guid> SelectedItems => (List<Guid>)(ViewState[nameof(SelectedItems)]
            ?? (ViewState[nameof(SelectedItems)] = new List<Guid>()));

        protected override int SelectCount(PersonFilter filter)
        {
            return PersonCriteria.Count(filter);
        }

        protected override IListSource SelectData(PersonFilter filter)
        {
            filter.OrderBy = "Created DESC,User.FirstName,User.LastName,User.Email";

            var persons = PersonCriteria.Select(filter, x => x.User, x => x.HomeAddress);

            return persons
                .Select(x => new PersonItem
                {
                    UserIdentifier = x.UserIdentifier,
                    Created = x.Created,
                    Modified = x.Modified,
                    FirstName = x.User.FirstName,
                    LastName = x.User.LastName,
                    Email = x.User.Email,
                    City = x.HomeAddress?.City,
                    Approved = x.JobsApproved.HasValue,
                    OccupationInterest = GetOccupationArea(x.OccupationStandardIdentifier),
                    OccupationList = x.CandidateOccupationList,
                    IsActivelySeeking = GetYesNo(x.CandidateIsActivelySeeking)
                })
                .ToList()
                .ToSearchResult();
        }

        private string GetOccupationArea(Guid? standard)
        {
            if (standard == null)
                return null;

            return StandardSearch.BindFirst(x => x.ContentTitle, x => x.StandardIdentifier == standard);
        }

        private static string GetYesNo(bool? value)
        {
            if (value == null)
                return null;

            return value.Value ? "Yes" : "No";
        }

        public override IListSource GetExportData(PersonFilter filter, bool empty)
        {
            if (filter.GroupRoleIdentifiers.IsEmpty())
                return new List<PersonItem>().ToSearchResult();

            var list = base.GetExportData(filter, empty).GetList().Cast<PersonItem>().ToList();

            foreach (var item in list)
            {
                item.NameString = $"{item.FirstName} {item.LastName}";
                if (item.OccupationList != null)
                    item.OccupationList = item.OccupationList.Replace("<br />", "; ");

                if (item.CompletionProfilePercent == null || item.CompletionProfilePercent == 0)
                    item.CompletionProfilePercent = GetCompletionProfile(item.UserIdentifier);

                if (item.CompletionResumePercent == null || item.CompletionResumePercent == 0)
                    item.CompletionResumePercent = GetCompletionResume(item.UserIdentifier);
            }

            return list.ToSearchResult();
        }

        protected string CreateMailToLink(object email) =>
            !ValueConverter.IsNull(email) ? $"mailto:{email}" : null;

        protected string GetCompletionProfileHtml(Guid? id)
        {
            var percent = id.HasValue
                ? UserSearch.GetCompletionProfilePercent(Organization.Identifier, id.Value)
                : null;

            return percent.HasValue
                ? $"<span class='badge bg-{UserSearch.GetCompletionStatus(percent.Value)}'>Profile = {percent}%</span>"
                : string.Empty;
        }

        protected int GetCompletionProfile(Guid? id)
        {
            if (!id.HasValue)
                return 0;

            return (int)UserSearch.GetCompletionProfilePercent(Organization.Identifier, id.Value);
        }

        protected string GetCompletionResumeHtml(Guid? id)
        {
            var percent = id.HasValue
                ? UserSearch.GetCompletionResumePercent(Organization.Identifier, id.Value)
                : null;

            return percent.HasValue
                ? $"<span class='badge bg-{UserSearch.GetCompletionStatus(percent.Value)}'>Resume = {percent}%</span>"
                : null;
        }

        protected int GetCompletionResume(Guid? id)
        {
            if (!id.HasValue)
                return 0;

            return (int)UserSearch.GetCompletionResumePercent(Organization.Identifier, id.Value);
        }

        protected string[] GetCandidateUploads(object contactId)
        {
            if (contactId == null)
                return null;

            Guid.TryParse(contactId.ToString(), out var contactGUID);
            if (contactGUID != null)
            {
                return TCandidateUploadSearch.SelectByContact(contactGUID)
                    .Select((x, i) => $"{i + 1}.{x.UploadType}")
                    .ToArray();
            }

            return null;
        }

        protected string[] GetCommentsAboutCandidate(object contactId)
        {
            if (contactId == null)
                return null;

            Guid.TryParse(contactId.ToString(), out var contactGUID);
            if (contactGUID != null)
            {
                return TCandidateCommentSearch
                .SelectByCandidate(contactGUID)
                .Select(x => x.CommentText)
                .ToArray();
            }

            return null;
        }
    }
}