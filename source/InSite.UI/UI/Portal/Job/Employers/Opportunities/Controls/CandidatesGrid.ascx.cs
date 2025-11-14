using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common.Linq;

namespace InSite.UI.Portal.Jobs.Employers.Opportunities.Controls
{
    public partial class CandidatesGrid : SearchResultsGridViewController<PersonFilter>
    {
        private class PersonItem
        {
            public Guid UserIdentifier { get; set; }
            public Guid? OccupationStandardIdentifier { get; set; }
            public DateTimeOffset Created { get; set; }
            public DateTimeOffset Modified { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Country { get; set; }
            public string City { get; set; }
            public string Street1 { get; set; }
            public string Phone { get; set; }
            public bool Approved { get; set; }
            public bool Archived { get; set; }
            public string OccupationList { get; set; }
            public bool IsActivelySeeking { get; set; }
            public string JobTitle { get; set; }

            public string NameString { get; set; }
            public int? CompletionProfilePercent { get; set; }
            public int? CompletionResumePercent { get; set; }
        }

        private class AttachmentItem
        {
            public int Length { get; set; }
            public string Name { get; set; }
            public Guid User { get; set; }
            public Guid File { get; set; }
            public string FileName { get; set; }
        }

        protected override bool IsFinder => false;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDataBound += Grid_RowDataBound;
        }

        private void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var row = (PersonItem)e.Row.DataItem;
            if (row == null || row.UserIdentifier == null || row.UserIdentifier == Guid.Empty)
                return;

            var extensions = new[] { ".doc", ".docx", ".pdf", ".zip", ".rtf" };

            var files = ServiceLocator.StorageService.GetGrantedFiles(Identity, row.UserIdentifier);

            var items = files
                .Where(x => extensions.Any(y => x.FileName.EndsWith(y, StringComparison.OrdinalIgnoreCase)))
                .Select(x => new AttachmentItem
                {
                    Length = x.FileSize,
                    Name = x.Properties.DocumentName,
                    User = row.UserIdentifier,
                    File = x.FileIdentifier,
                    FileName = x.FileName
                })
                .ToList();

            var attachmentsRepeter = (Repeater)e.Row.FindControl("DocumentRepeater");
            attachmentsRepeter.DataSource = items;
            attachmentsRepeter.DataBind();
        }

        public static string GetFileRelativePath(object o)
        {
            var item = (AttachmentItem)o;
            return ServiceLocator.StorageService.GetFileUrl(item.File, item.FileName);
        }

        public void LoadData(TOpportunity opportunity)
        {
            if (opportunity.OccupationStandardIdentifier.HasValue)
            {
                var filter = new PersonFilter
                {
                    OrganizationIdentifier = CurrentSessionState.Identity.Organization.OrganizationIdentifier,
                    IsApproved = true,
                    IsJobsApproved = true,
                    OccupationInterest = opportunity.OccupationStandardIdentifier,
                };

                Search(filter);
            }

            Count.Text = $"{RowCount:n0}";
            NoMatches.Visible = RowCount == 0;
        }

        protected override int SelectCount(PersonFilter filter)
            => PersonCriteria.Count(filter);

        protected override IListSource SelectData(PersonFilter filter)
        {
            var persons = PersonCriteria.Select(filter, x => x.User, x => x.HomeAddress);

            return persons
                .Select(x => new PersonItem
                {
                    UserIdentifier = x.UserIdentifier,
                    Created = x.Created,
                    Modified = x.Modified,
                    FirstName = x.User.FirstName,
                    LastName = x.User.LastName,
                    FullName = ($"{x.User.FirstName} {x.User.LastName}"),
                    Email = x.User.Email,
                    Country = x.HomeAddress?.Country,
                    City = x.HomeAddress?.City,
                    Street1 = x.HomeAddress?.Street1,
                    Phone = x.Phone,
                    Approved = x.JobsApproved.HasValue,
                    Archived = x.User.UtcArchived.HasValue,
                    OccupationList = x.CandidateOccupationList,
                    JobTitle = x.JobTitle,
                    OccupationStandardIdentifier = x.OccupationStandardIdentifier,
                    IsActivelySeeking = x.CandidateIsActivelySeeking ?? false
                })
               .ToList()
               .ToSearchResult();
        }

        protected string GetFileSize(long bytes)
        {
            return bytes.Bytes().Humanize("#");
        }

        protected string GetStatusHtml(object o)
        {
            var item = (PersonItem)o;
            var html = string.Empty;
            if (item.OccupationStandardIdentifier.HasValue)
                html += StandardSearch.BindFirst(x => x.ContentTitle, x => x.StandardIdentifier == item.OccupationStandardIdentifier);
            if (!string.IsNullOrWhiteSpace(item.JobTitle))
                html += $"<div class='fs-sm text-body-secondary'>{item.JobTitle}</div>";
            if (item.IsActivelySeeking)
                html += "<div><span class='badge bg-primary'>Actively Seeking</span></div>";
            return html;
        }
    }
}
