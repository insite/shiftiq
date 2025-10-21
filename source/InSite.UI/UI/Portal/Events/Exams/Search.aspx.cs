using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using InSite.Application.Banks.Read;
using InSite.Application.Events.Read;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Events.Exams
{
    public partial class Search : PortalBasePage
    {
        private class ExamItem
        {
            public Guid EventIdentifier { get; set; }
            public string EventTitle { get; set; }
            public string ExamFormat { get; set; }
            public string ExamType { get; set; }
            public Guid? AchievementIdentifier { get; set; }
            public string AchievementTitle { get; set; }
            public string AchievementType { get; set; }
            public DateTimeOffset EventScheduledStart { get; set; }
            public DateTimeOffset? EventScheduledEnd { get; set; }
            public string VenueLocationName { get; set; }
            public string VenueAddress { get; set; }
            public string Forms { get; set; }
            public bool IsFull { get; set; }
            public bool IsRegistered { get; set; }
            public bool IsWaitlisted { get; set; }
            public bool IsInvited { get; set; }
            public bool IsMoved { get; set; }
            public bool IsCancelled { get; set; }
            public bool IsWaitlistAvailable { get; set; }

            public string EventScheduledText
            {
                get
                {
                    var culture = CultureInfo.GetCultureInfo(Identity.Language);

                    return EventScheduledEnd == null || EventScheduledEnd.Value.Date == EventScheduledStart.Date
                        ? $"{EventScheduledStart.Format(User.TimeZone, true, false, false, culture)}"
                        : $"{EventScheduledStart.Format(User.TimeZone, true, false, false, culture)} to {EventScheduledEnd.Format(User.TimeZone, true, false, false, culture)}";
                }
            }
        }

        private class ExamFormatItem
        {
            public string Title { get; set; }

            public List<ExamItem> Exams { get; set; }
        }

        private class ExamTypeItem
        {
            public string Title { get; set; }

            public List<ExamFormatItem> ExamTypes { get; set; }
        }

        private class ExamTitleType
        {
            public string Title { get; set; }

            public List<ExamTypeItem> ExamTitleTypes { get; set; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DataRepeater.ItemDataBound += DataRepeater_ItemDataBound;
        }

        private void DataRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var item = (ExamTitleType)e.Item.DataItem;
            var examFormsRepeaterRepeater = (Repeater)e.Item.FindControl("ExamFormsRepeater");
            examFormsRepeaterRepeater.ItemDataBound += ExamFormsRepeater_ItemDataBound;
            examFormsRepeaterRepeater.DataSource = item.ExamTitleTypes;
            examFormsRepeaterRepeater.DataBind();
        }

        private void ExamFormsRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var item = (ExamTypeItem)e.Item.DataItem;
            var examTypeRepeater = (Repeater)e.Item.FindControl("ExamTypeRepeater");
            examTypeRepeater.ItemDataBound += ExamTypeRepeater_ItemDataBound;
            examTypeRepeater.DataSource = item.ExamTypes;
            examTypeRepeater.DataBind();
        }

        private void ExamTypeRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var item = (ExamFormatItem)e.Item.DataItem;
            var examRepeater = (Repeater)e.Item.FindControl("ExamRepeater");
            examRepeater.DataSource = item.Exams;
            examRepeater.DataBind();
        }

        private void LoadData()
        {
            PageHelper.AutoBindHeader(this, qualifier: Translate("Exams"));

            var exams = GetExams();

            MainPanel.Visible = exams.Count > 0;

            if (exams.Count > 0)
            {
                var groups = GroupByTitleLocation(exams);

                DataRepeater.DataSource = groups;
                DataRepeater.DataBind();
            }
            else
            {
                StatusAlert.AddMessage(AlertType.Warning, Translate("There are no published exams"));
            }
        }

        private List<ExamItem> GetExams()
        {
            var events = GetEvents();

            var classes = new List<ExamItem>();

            foreach (var ev in events)
            {
                var examForms = ServiceLocator.BankSearch.GetForms(ev.ExamForms.Select(x => x.FormIdentifier).ToList());

                var item = new ExamItem
                {
                    EventIdentifier = ev.EventIdentifier,
                    EventTitle = GetEventTitle(ev),
                    AchievementIdentifier = ev.AchievementIdentifier,
                    AchievementTitle = ev.Achievement?.AchievementTitle,
                    AchievementType = ev.Achievement?.AchievementType,
                    EventScheduledStart = ev.EventScheduledStart,
                    EventScheduledEnd = ev.EventScheduledEnd,
                    VenueLocationName = ev.VenueLocationName,
                    ExamFormat = ev.EventFormat,
                    ExamType = ev.ExamType,
                    VenueAddress = GetVenueAddress(ev),
                    Forms = GetForms(examForms)
                };

                classes.Add(item);
            }

            return classes;
        }

        private string GetEventTitle(QEvent @event)
        {
            var content = ContentEventClass.Deserialize(@event.Content);
            var title = content.Title?[Identity.Language];

            return !string.IsNullOrEmpty(title)
                ? title
                : @event.EventTitle;
        }

        private List<QEvent> GetEvents()
        {
            var filterByStart = new QEventFilter
            {
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
                EventType = "Exam",
                EventPublicationStatus = PublicationStatus.Published.ToString(),
                EventScheduledSince = DateTimeOffset.UtcNow.AddDays(1)
            };

            var eventsByStart = ServiceLocator.EventSearch
                .GetEvents(filterByStart, null, null, x => x.ExamForms, x => x.Registrations)
                .Where(x => x.EventSchedulingStatus != "Cancelled")
                .ToList();

            var filterByDeadline = new QEventFilter
            {
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
                EventType = "Exam",
                EventPublicationStatus = PublicationStatus.Published.ToString(),
                RegistrationDeadlineSince = DateTimeOffset.UtcNow
            };

            var eventsByDeadline = ServiceLocator.EventSearch
                .GetEvents(filterByDeadline, null, null, x => x.ExamForms, x => x.Registrations)
                .Where(x => x.EventSchedulingStatus != "Cancelled" && !eventsByStart.Any(y => y.EventIdentifier == x.EventIdentifier))
                .ToList();

            var result = new List<QEvent>(eventsByStart);
            result.AddRange(eventsByDeadline);

            return result;
        }

        private List<ExamTitleType> GroupByTitleLocation(List<ExamItem> exams)
        {
            return exams
                .GroupBy(x => x.EventTitle ?? "")
                .Select(t => new ExamTitleType
                {
                    Title = t.Key,
                    ExamTitleTypes = t
                        .GroupBy(x => x.ExamFormat ?? "")
                        .Select(a => new ExamTypeItem
                        {
                            Title = a.Key,
                            ExamTypes = a
                                            .GroupBy(x => x.ExamType)
                                            .Select(b => new ExamFormatItem
                                            {
                                                Title = b.Key,
                                                Exams = b.OrderBy(x => x.EventScheduledStart).ToList()
                                            })
                                            .OrderBy(x => x.Title)
                                            .ToList()
                        })
                        .OrderBy(x => x.Title)
                        .ToList()
                })
                .OrderBy(x => x.Title)
                .ToList();
        }

        private static string GetForms(List<QBankForm> forms)
        {
            if (forms == null || forms.Count == 0) return string.Empty;

            StringBuilder builder = new StringBuilder();
            builder.Append("<ul>");

            foreach (var form in forms)
            {
                builder.Append("<li>");
                builder.Append(form.FormName);
                builder.Append("</li>");
            }

            builder.Append("</ul>");
            return builder.ToString();
        }

        private static string GetVenueAddress(QEvent @event)
        {
            if (@event.VenueLocationIdentifier == null)
                return String.Empty;

            var address = ServiceLocator.GroupSearch.GetAddress(@event.VenueLocationIdentifier.Value, AddressType.Physical);

            if (address == null)
                return string.Empty;

            if (address.City.HasValue() && address.Province.HasValue())
                return $"{address.City}, {address.Province}";

            if (address.City.HasValue())
                return address.City;

            return address.Province;
        }
    }
}