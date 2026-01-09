using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Admin.Records.Gradebooks.Controls;
using InSite.Application.Events.Read;
using InSite.Common.Web;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Records.Gradebooks.Forms
{
    public partial class Scores : AdminBasePage, IHasParentLinkParameters
    {
        private Guid GradebookIdentifier => Guid.TryParse(Request["gradebook"], out var value) ? value : Guid.Empty;

        private Guid? ItemKey => Guid.TryParse(Request["item"], out var value) ? value : (Guid?)null;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var gradebook = ServiceLocator.RecordSearch.GetGradebook(GradebookIdentifier, x => x.Event, x => x.Event.Attendees);
                if (gradebook == null
                    || gradebook.IsLocked
                    || gradebook.OrganizationIdentifier != Organization.OrganizationIdentifier
                    || gradebook.Event?.Attendees == null
                    || gradebook.Event.Attendees.FirstOrDefault(x => x.UserIdentifier == User.UserIdentifier && x.AttendeeRole == "Instructor") == null
                    )
                {
                    HttpResponseHelper.Redirect("/ui/admin/records/gradebooks/instructors/search");
                }

                var title = gradebook.GradebookTitle;

                if (gradebook.Event != null)
                    title += $" <span class='form-text'> for {gradebook.Event.EventTitle} ({GetLocalTime(gradebook.Event.EventScheduledStart)} - {GetLocalTime(gradebook.Event.EventScheduledEnd)})</span>";

                PageHelper.AutoBindHeader(this, null, title);

                var learners = GetRegisteredLearners(GetClassesWhereUserIsInstructor(User.Identifier));

                if (!ScoreControl.LoadData(GradebookIdentifier, ItemKey, $"/ui/admin/records/gradebooks/instructors/gradebook-outline?id={GradebookIdentifier}", learners))
                    HttpResponseHelper.Redirect("/ui/admin/records/gradebooks/instructors/search");
            }
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
            return item.FormatDateOnly(User.TimeZone, nullValue: string.Empty);
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/gradebook-outline")
                ? $"id={GradebookIdentifier}"
                : null;
        }
    }
}
