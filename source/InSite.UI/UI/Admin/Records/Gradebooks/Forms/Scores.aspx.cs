using System;

using InSite.Admin.Records.Gradebooks.Controls;
using InSite.Common.Web;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Records.Gradebooks.Forms
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
                var gradebook = ServiceLocator.RecordSearch.GetGradebook(GradebookIdentifier, x => x.Event);
                if (gradebook == null || gradebook.OrganizationIdentifier != Organization.OrganizationIdentifier)
                    HttpResponseHelper.Redirect("/ui/admin/records/gradebooks/search");

                var title = gradebook.GradebookTitle;

                if (gradebook.Event != null)
                    title += $" <span class='form-text'> for {gradebook.Event.EventTitle} ({GetLocalTime(gradebook.Event.EventScheduledStart)} - {GetLocalTime(gradebook.Event.EventScheduledEnd)})</span>";

                PageHelper.AutoBindHeader(this, null, title);

                if (!ScoreControl.LoadData(GradebookIdentifier, ItemKey, $"/ui/admin/records/gradebooks/outline?id={GradebookIdentifier}&panel=scores"))
                    HttpResponseHelper.Redirect("/ui/admin/records/gradebooks/search");
            }
        }

        private static string GetLocalTime(DateTimeOffset? item)
        {
            return item.FormatDateOnly(User.TimeZone, nullValue: string.Empty);
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={GradebookIdentifier}&panel=scores"
                : null;
        }
    }
}
