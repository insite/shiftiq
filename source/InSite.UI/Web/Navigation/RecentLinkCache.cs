using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;

using Shift.Common;

namespace InSite.Common.Web.UI
{

    public class RecentLinkCache : Control
    {
        private const int MaxItems = 15;

        static public void Add(Page page)
        {
            var session = page.Session;

            var uri = page.Request.Url;

            var observers = new Dictionary<string, (string Name, string IdParam)>
            {
                { "/ui/admin/assessments/banks/outline", ("Bank", "bank") },
                { "/ui/admin/contacts/groups/edit", ("Group", "contact") },
                { "/ui/admin/contacts/people/edit", ("Person", "contact") },
                { "/ui/admin/courses/manage", ("Course", "course") },
                { "/ui/admin/events/classes/outline", ("Class Event", "event") },
                { "/ui/admin/events/exams/outline", ("Exam Event", "event") },
                { "/ui/admin/messages/outline", ("Message", "message") },
                { "/ui/admin/records/achievements/outline", ("Achievement" , "id") },
                { "/ui/admin/records/credentials/outline", ("Credential", "id") },
                { "/ui/admin/records/gradebooks/outline", ("Gradebook", "id") },
                { "/ui/admin/records/logbooks/outline", ("Logbook", "journalsetup") },
                { InSite.Admin.Records.Programs.Outline.NavigateUrl, ("Program", "id") },
                { InSite.UI.Admin.Records.Rurbics.Outline.NavigateUrl, ("Rubric", "rubric") },
                { "/ui/admin/sales/invoices/outline", ("Invoice", "id") },
                { "/ui/admin/sales/payments/outline", ("Payment", "id") },
                { "/ui/admin/sites/outline", ("Site", "id") },
                { "/ui/admin/sites/pages/outline", ("Page", "id") },
                { "/ui/admin/standards/manage", ("Standard", "standard") },
                { "/ui/admin/workflow/forms/outline", ("Form", "form") },
                { "/ui/admin/workflow/forms/submissions/outline", ("Submission", "session") },
                { "/ui/admin/workflow/cases/outline", ("Case", "case") },
                { "/ui/admin/reports/dashboards", ("Dashboards", "") },
                { "/ui/admin/accounts/organizations/edit", ("Organization", "organization") },
                { "/ui/admin/accounts/senders/edit", ("Sender", "id") },
                { "/ui/admin/assessment/quizzes/edit", ("Quiz", "quiz") },
                { "/ui/admin/assets/collections/edit", ("Collection", "collection") },
                { "/ui/admin/assets/collections/edit-item", ("Collection", "item") },
                { "/ui/admin/assets/labels/edit", ("Label", "label") },
                { "/ui/admin/contacts/people/edit-membership", ("Person", "to") },
                { "/ui/admin/database/columns/outline", ("Column", "columnName") },
                { "/ui/admin/events/appointments/outline", ("Appointment", "event") },
                { "/ui/admin/integrations/api-requests/outline", ("API", "request") },
                { "/ui/admin/logs/aggregates/outline", ("Aggregate", "aggregate") },
                { "/ui/admin/records/gradebooks/instructors/gradebook-outline", ("Gradebook", "id") },
                { "/ui/admin/records/gradebooks/instructors/person-outline", ("Instructor", "contact") },
                { "/ui/admin/records/logbooks/validators/outline", ("Validator", "journalsetup") },
                { "/ui/admin/records/logbooks/validators/outline-journal", ("Logbook", "journalsetup") },
                { InSite.UI.Admin.Records.Rurbics.Edit.NavigateUrl, ("Rubric", "rubric") },
                { "/ui/admin/registrations/classes/edit", ("Class Event", "id") },
                { "/ui/admin/registrations/exams/edit", ("Exam Event", "registration") },
                { "/ui/admin/reports/edit", ("Report", "id") },
                { "/ui/admin/sales/products/edit", ("Product", "id") },
                { "/ui/admin/standards/documents/outline", ("Standard", "asset") },
                { "/ui/admin/standards/edit", ("Standard", "id") }
            };

            if (!observers.ContainsKey(uri.AbsolutePath))
                return;

            if (session[nameof(RecentLinkCache)] == null)
                session[nameof(RecentLinkCache)] = new List<RecentLinkItem>();

            var list = (List<RecentLinkItem>)session[nameof(RecentLinkCache)];

            var item = list.FirstOrDefault(i => i.PageUrl == uri.AbsoluteUri);

            if (item != null)
                list.Remove(item);

            var observer = observers[uri.AbsolutePath];

            item = new RecentLinkItem
            {
                PageUrl = uri.AbsoluteUri,
                ObserverUrl = observer.Name,
                Key = NormalizeUrlKey(uri.AbsoluteUri, observer.IdParam)
            };

            if (!string.IsNullOrEmpty(page.Title))
                item.PageTitle = StringHelper.Snip(page.Title, 100);

            list.RemoveAll(i => i.Key == item.Key);

            list.Insert(0, item);

            if (list.Count > MaxItems)
                list.RemoveAt(MaxItems - 1);

            session[nameof(RecentLinkCache)] = list;
        }

        private static string NormalizeUrlKey(string url, string preferredKey)
        {
            var uri = new Uri(url);
            string baseUrl = $"{uri.Scheme}://{uri.Host}{uri.AbsolutePath}";
            var queryParams = HttpUtility.ParseQueryString(uri.Query);

            if (string.IsNullOrWhiteSpace(preferredKey))
                return baseUrl;

            var preferredValue = queryParams[preferredKey];
            if (!string.IsNullOrEmpty(preferredValue))
                return $"{baseUrl}|{preferredValue}";

            return baseUrl;
        }

        static public bool IsVisible(HttpSessionState session)
        {
            try
            {
                if (session[nameof(RecentLinkCache)] == null)
                    session[nameof(RecentLinkCache)] = new List<RecentLinkItem>();

                var list = session[nameof(RecentLinkCache)] as List<RecentLinkItem>;

                return list?.Count > 0;
            }
            catch
            {
                session.Remove(nameof(RecentLinkCache));
                return false;
            }
        }

        static private List<RecentLinkItem> Get(HttpSessionState session)
        {
            if (session[nameof(RecentLinkCache)] == null)
                session[nameof(RecentLinkCache)] = new List<RecentLinkItem>();

            var list = (List<RecentLinkItem>)session[nameof(RecentLinkCache)];

            return list;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            var list = Get(Page.Session);

            if (list.Count() == 0)
                return;

            writer.WriteLine("<ul class='dropdown-menu'>");
            foreach (var item in list)
            {
                writer.WriteLine($"<li><a class='dropdown-item' href='{item.PageUrl}'>{item.PageTitle ?? item.ObserverUrl}</a></li>");
            }
            writer.WriteLine("</ul>");
        }
    }
}