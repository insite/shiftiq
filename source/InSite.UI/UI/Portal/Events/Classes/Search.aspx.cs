using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Events.Read;
using InSite.Application.Registrations.Read;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Events.Classes
{
    public partial class Search : PortalBasePage
    {
        private class ClassItem
        {
            public Guid EventIdentifier { get; set; }
            public string EventTitle { get; set; }
            public Guid? AchievementIdentifier { get; set; }
            public string AchievementTitle { get; set; }
            public string AchievementType { get; set; }
            public DateTimeOffset EventScheduledStart { get; set; }
            public DateTimeOffset? EventScheduledEnd { get; set; }
            public string VenueLocationName { get; set; }
            public string VenueAddress { get; set; }
            public string Summary { get; set; }
            public bool IsFull { get; set; }
            public bool IsRegistered { get; set; }
            public bool IsWaitlisted { get; set; }
            public bool IsInvited { get; set; }
            public bool IsMoved { get; set; }
            public bool IsCancelled { get; set; }
            public bool IsWaitlistAvailable { get; set; }
            public bool IsLocked { get; set; }

            public string EventScheduledText
            {
                get
                {
                    return EventScheduledEnd == null || EventScheduledEnd.Value.Date == EventScheduledStart.Date
                        ? $"{EventScheduledStart.Format(User.TimeZone, true)}"
                        : $"{EventScheduledStart.Format(User.TimeZone, true)} to {EventScheduledEnd.Format(User.TimeZone, true)}";
                }
            }

            
        }

        private class SummaryItem
        {
            public string Summary { get; set; }

            public List<ClassItem> Classes { get; set; }
        }

        private class AchievementItem
        {
            public string Title { get; set; }

            public List<SummaryItem> Summaries { get; set; }
        }

        private class AchievementType
        {
            public string Title { get; set; }

            public List<AchievementItem> Achievements { get; set; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            TypeRepeater.ItemDataBound += TypeRepeater_ItemDataBound;

            SearchText.FilterClick += SearchText_FilterClick;
            SearchText.ClearClick += SearchText_FilterClick;

            HideFullWithNoWaitlist.AutoPostBack = true;
            HideFullWithNoWaitlist.CheckedChanged += SearchText_FilterClick;

            HideFullClasses.AutoPostBack = true;
            HideFullClasses.CheckedChanged += SearchText_FilterClick;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);
            PortalMaster.SidebarVisible(false);

            if (!LoadData())
            {
                SearchText.Visible = false;
                StatusAlert.AddMessage(AlertType.Warning, Translate("There are no published classes"));
            }
        }

        private void TypeRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var achievementType = (AchievementType)e.Item.DataItem;
            var resourceRepeater = (Repeater)e.Item.FindControl("ResourceRepeater");
            resourceRepeater.ItemDataBound += ResourceRepeater_ItemDataBound;
            resourceRepeater.DataSource = achievementType.Achievements;
            resourceRepeater.DataBind();
        }

        private void ResourceRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var achievement = (AchievementItem)e.Item.DataItem;
            var summaryRepeater = (Repeater)e.Item.FindControl("SummaryRepeater");
            summaryRepeater.ItemDataBound += SummaryRepeater_ItemDataBound;
            summaryRepeater.DataSource = achievement.Summaries;
            summaryRepeater.DataBind();
        }

        private void SummaryRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var summary = (SummaryItem)e.Item.DataItem;
            var classRepeater = (Repeater)e.Item.FindControl("ClassRepeater");
            classRepeater.DataSource = summary.Classes;
            classRepeater.DataBind();
        }

        private void SearchText_FilterClick(object sender, EventArgs e)
        {
            if (!LoadData())
                StatusAlert.AddMessage(AlertType.Warning, Translate("There are no published classes matching your criteria."));
        }

        private bool LoadData()
        {
            var classes = GetClasses(SearchText.Text, HideFullWithNoWaitlist.Checked, HideFullClasses.Checked);

            MainPanel.Visible = classes.Count > 0;

            if (classes.Count == 0)
                return false;

            var groups = GroupByResource(classes);

            TypeRepeater.DataSource = groups;
            TypeRepeater.DataBind();

            return true;
        }

        private static List<ClassItem> GetClasses(string eventTitle, bool hideFullWithNoWaitlist, bool hideFullClasses)
        {
            var events = GetAccessibleEvents(eventTitle);
            var registrations = ServiceLocator.RegistrationSearch.GetRegistrationsByCandidate(User.UserIdentifier);
            var classes = new List<ClassItem>();

            foreach (var ev in events)
            {
                var item = GetClassItem(registrations, ev);

                if (hideFullWithNoWaitlist && item.IsFull && !item.IsWaitlistAvailable)
                    continue;

                if (hideFullClasses && item.IsFull)
                    continue;

                classes.Add(item);
            }

            return classes;
        }

        private static ClassItem GetClassItem(List<QRegistration> registrations, QEvent ev)
        {
            var item = new ClassItem
            {
                EventIdentifier = ev.EventIdentifier,
                EventTitle = ev.EventTitle,
                AchievementIdentifier = ev.AchievementIdentifier,
                AchievementTitle = ev.Achievement?.AchievementTitle,
                AchievementType = ev.Achievement?.AchievementType,
                EventScheduledStart = ev.EventScheduledStart,
                EventScheduledEnd = ev.EventScheduledEnd,
                VenueLocationName = ev.VenueLocationName,
                VenueAddress = GetVenueAddress(ev),
                Summary = GetSummary(ev),
                IsFull = ev.Availability == EventAvailabilityType.Full || ev.Availability == EventAvailabilityType.Over,
                IsLocked = ev.RegistrationLocked.HasValue
            };

            var registration = registrations.Find(x => x.EventIdentifier == ev.EventIdentifier);
            if (registration != null)
            {
                item.IsCancelled = string.Equals(registration.AttendanceStatus, "Withdrawn/Cancelled", StringComparison.OrdinalIgnoreCase);
                item.IsRegistered = !item.IsCancelled && string.Equals(registration.ApprovalStatus, "Registered", StringComparison.OrdinalIgnoreCase);
                item.IsWaitlisted = !item.IsCancelled && string.Equals(registration.ApprovalStatus, "Waitlisted", StringComparison.OrdinalIgnoreCase);
                item.IsInvited = !item.IsCancelled && string.Equals(registration.ApprovalStatus, "Invitation Sent", StringComparison.OrdinalIgnoreCase);
                item.IsMoved = !item.IsCancelled && string.Equals(registration.ApprovalStatus, "Moved", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                item.IsWaitlistAvailable = item.IsFull && ev.WaitlistEnabled;
            }

            return item;
        }

        private static List<QEvent> GetAccessibleEvents(string eventTitle)
        {
            var events = GetAllEvents(eventTitle);
            var eventIds = events.Select(e => e.EventIdentifier).ToList();
            var accessibleIds = TGroupPermissionSearch.GetAccessAllowed(eventIds, Identity);

            var result = events
                .Where(e => accessibleIds.Contains(e.EventIdentifier))
                .ToList();

            return result;
        }

        private static List<QEvent> GetAllEvents(string eventTitle)
        {
            var filterByStart = new QEventFilter
            {
                OrganizationIdentifier = Identity.Organization.Identifier,
                EventType = "Class",
                EventPublicationStatus = PublicationStatus.Published.ToString(),
                EventScheduledSince = DateTimeOffset.UtcNow.AddDays(1),
                EventTitle = eventTitle
            };

            var eventsByStart = ServiceLocator.EventSearch
                .GetEvents(filterByStart, null, null, x => x.Achievement, x => x.Registrations, x => x.VenueLocation)
                .Where(x => x.EventSchedulingStatus != "Cancelled")
                .ToList();

            var filterByDeadline = new QEventFilter
            {
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
                EventType = "Class",
                EventPublicationStatus = PublicationStatus.Published.ToString(),
                RegistrationDeadlineSince = DateTimeOffset.UtcNow,
                EventTitle = eventTitle
            };

            var eventsByDeadline = ServiceLocator.EventSearch
                .GetEvents(filterByDeadline, null, null, x => x.Achievement, x => x.Registrations, x => x.VenueLocation)
                .Where(x => x.EventSchedulingStatus != "Cancelled" && !eventsByStart.Any(y => y.EventIdentifier == x.EventIdentifier))
                .ToList();

            var result = new List<QEvent>(eventsByStart);
            result.AddRange(eventsByDeadline);

            return result;
        }

        private List<AchievementType> GroupByResource(List<ClassItem> classes)
        {
            return classes
                .GroupBy(x => x.AchievementType ?? "")
                .Select(t => new AchievementType
                {
                    Title = t.Key,
                    Achievements = t
                        .GroupBy(x => x.AchievementTitle ?? "")
                        .Select(a => new AchievementItem
                        {
                            Title = a.Key,
                            Summaries = a
                                            .GroupBy(x => x.Summary)
                                            .Select(b => new SummaryItem
                                            {
                                                Summary = b.Key,
                                                Classes = b.OrderBy(x => x.EventScheduledStart).ToList()
                                            })
                                            .OrderBy(x => x.Summary)
                                            .ToList()
                        })
                        .OrderBy(x => x.Title)
                        .ToList()
                })
                .OrderBy(x => x.Title)
                .ToList();
        }

        private static string GetSummary(QEvent @event)
        {
            return Markdown.ToHtml(ContentEventClass.Deserialize(@event.Content).Summary.Default);
        }

        private static string GetVenueAddress(QEvent @event)
        {
            if (@event.VenueLocationIdentifier == null)
                return string.Empty;

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