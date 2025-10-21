using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Events.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Records.Gradebooks.Forms
{
    public partial class Outline : AdminBasePage
    {
        private Guid? GradebookID => Guid.TryParse(Request["id"], out var gradebookID) ? gradebookID : (Guid?)null;

        private Guid? ScoreItem => Guid.TryParse(Request["scoreItem"], out var value) ? value : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            if (GradebookID == null)
                HttpResponseHelper.Redirect("/ui/admin/records/gradebooks/search");

            var queryGradebook = ServiceLocator.RecordSearch.GetGradebook(GradebookID.Value, x => x.GradebookEvents);
            if (queryGradebook == null
                || queryGradebook.IsLocked
                || queryGradebook.OrganizationIdentifier != Organization.OrganizationIdentifier
                )
            {
                RedirectToSearch();
            }

            var classes = GetClassesWhereUserIsInstructor(User.Identifier);
            if (!classes.Any(x => queryGradebook.GradebookEvents.Any(y => y.EventIdentifier == x)))
                RedirectToSearch();

            var dataGradebook = ServiceLocator.RecordSearch.GetGradebookState(GradebookID.Value);

            var title = queryGradebook.GradebookTitle;

            if (queryGradebook.Event != null)
                title += $" <span class='form-text'> for {queryGradebook.Event.EventTitle} ({GetLocalTime(queryGradebook.Event.EventScheduledStart)} - {GetLocalTime(queryGradebook.Event.EventScheduledEnd)})</span>";

            PageHelper.AutoBindHeader(this, null, title);

            var learners = GetRegisteredLearners(classes);
            ScoreList.LoadData(dataGradebook, ScoreItem, learners);
            GradeItemsGrid.LoadData(GradebookID.Value, dataGradebook, true);
        }

        private void RedirectToSearch()
        {
            HttpResponseHelper.Redirect("/ui/admin/records/gradebooks/instructors/search");
        }

        private Guid[] GetRegisteredLearners(Guid[] classes)
        {
            var learners = new List<Guid>();
            foreach (var @class in classes)
            {
                var registrations = ServiceLocator.RegistrationSearch.GetRegistrationsByEvent(@class);
                learners.AddRange(registrations.Select(r => r.CandidateIdentifier));
            }
            return learners.ToArray();
        }

        private Guid[] GetClassesWhereUserIsInstructor(Guid user)
        {
            var filter = new QEventAttendeeFilter
            {
                ContactRole = "Instructor",
                ContactIdentifier = user
            };
            return ServiceLocator.EventSearch.GetAttendees(filter).Select(x => x.EventIdentifier).ToArray();
        }

        private static string GetLocalTime(DateTimeOffset? item)
        {
            return item.Format(User.TimeZone, nullValue: string.Empty);
        }
    }
}
