using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Shift.Common;

using Newtonsoft.Json;

namespace Shift.Sdk.Service.Platform.DashboardNotifications
{
    public class DashboardNotificationManager : IDashboardNotificationManager
    {
        private static int ListLifeTimeInMinutes = 5;

        private static DateTimeOffset _listCreated;
        private static List<DashboardNotification> _list;

        private readonly FilePaths _paths;


        public DashboardNotificationManager(FilePaths paths)
        {
            _paths = paths;
        }

        public ActiveNotification[] CollectActiveNotifications()
        {
            var criteria = new DashboardNotificationCriteria
            {
                OnlyVisibleOnDashboard = true
            };

            var notifications = SearchNotifications(criteria)
                .Notifications
                .Select(x => new ActiveNotification
                {
                    NotificationId = x.NotificationId,
                    Type = x.Type,
                    Title= x.Title,
                    Details = x.Details,
                    LinkText = x.LinkText,
                    LinkUrl = x.LinkUrl,
                    Modified = x.Modified
                })
                .ToArray();

            return notifications;
        }

        public DashboardNotificationList SearchNotifications(DashboardNotificationCriteria criteria)
        {
            var list = ReadNotifications();
            var query = Filter(criteria, list);
            var filteredCount = query.Count();

            query = query
                .OrderByDescending(x => x.Modified)
                .ThenBy(x => x.Title);

            var filter = criteria.Filter;
            if (filter != null && filter.Page > 0 && filter.PageSize > 0)
            {
                query = query
                    .Skip((filter.Page - 1) * filter.PageSize)
                    .Take(filter.PageSize);
            }

            var notifications = query.ToArray();
            var now = DateTimeOffset.UtcNow;

            var visibleOnDashboard = notifications
                .Select(x => new
                {
                    x.NotificationId,
                    Visible = IsVisibleOnDashboard(x, now)
                })
                .ToDictionary(x => x.NotificationId, x => x.Visible);

            return new DashboardNotificationList
            {
                Notifications = notifications,
                VisibleOnDashboard = visibleOnDashboard,
                TotalCount = filteredCount
            };
        }

        public DashboardNotification RetrieveNotification(Guid notificationId)
        {
            var folderPath = _paths.NotificationsFolderPath;
            var filePath = Path.Combine(folderPath, $"{notificationId.ToString().ToLower()}.json");

            return File.Exists(filePath) ? ReadNotification(filePath) : null;
        }

        public void ModifyNotification(DashboardNotification notification, Guid modifiedBy)
        {
            ValidateNotification(notification);

            var now = DateTimeOffset.UtcNow;
            var folderPath = _paths.NotificationsFolderPath;
            var existingFilePath = Path.Combine(folderPath, $"{notification.NotificationId.ToString().ToLower()}.json");
            var isExist = File.Exists(existingFilePath);
            var existing = isExist ? ReadNotification(existingFilePath) : null;

            if (existing == null)
            {
                if (isExist)
                    throw new ApplicationError("Cannot save the notification");

                notification.NotificationId = UniqueIdentifier.Create();
                notification.Created = now;
                notification.CreatedBy = modifiedBy;
            }
            else
            {
                notification.Created = existing.Created;
                notification.CreatedBy = existing.CreatedBy;
            }

            notification.Modified = now;
            notification.ModifiedBy = modifiedBy;

            var filePath = Path.Combine(folderPath, $"{notification.NotificationId.ToString().ToLower()}.json");
            var serialized = JsonConvert.SerializeObject(notification);

            File.WriteAllText(filePath, serialized);

            _list = null;
        }

        public void DeleteNotification(Guid notificationId)
        {
            var folderPath = _paths.NotificationsFolderPath;
            var filePath = Path.Combine(folderPath, $"{notificationId.ToString().ToLower()}.json");

            if (File.Exists(filePath))
                File.Delete(filePath);

            _list = null;
        }

        private static void ValidateNotification(DashboardNotification notification)
        {
            if (notification.Type == DashboardNotificationType.None)
                throw new ArgumentException("norification.Type is none");

            if (string.IsNullOrEmpty(notification.Title))
                throw new ArgumentNullException("notification.Title");

            if (notification.Title.Length > 200)
                throw new ArgumentException("notification.Title exceeds 200 characters");

            if (!string.IsNullOrEmpty(notification.Details) && notification.Details.Length > 200)
                throw new ArgumentException("notification.Details exceeds 200 characters");

            if (!string.IsNullOrEmpty(notification.LinkText) && notification.LinkText.Length > 200)
                throw new ArgumentException("notification.LinkText exceeds 200 characters");

            if (!string.IsNullOrEmpty(notification.LinkUrl) && notification.LinkUrl.Length > 2000)
                throw new ArgumentException("notification.LinkUrl exceeds 2000 characters");
        }

        private static IEnumerable<DashboardNotification> Filter(DashboardNotificationCriteria criteria, IEnumerable<DashboardNotification> query)
        {
            if (!string.IsNullOrEmpty(criteria.Title))
                query = query.Where(x => x.Title.Contains(criteria.Title, StringComparison.OrdinalIgnoreCase));

            if (criteria.OnlyVisibleOnDashboard)
                query = query.Where(x => IsVisibleOnDashboard(x, DateTimeOffset.UtcNow));

            return query;
        }

        private static bool IsVisibleOnDashboard(DashboardNotification notification, DateTimeOffset now)
        {
            return notification.IsActive
                    && (notification.StartDate == null || notification.StartDate <= now)
                    && (notification.EndDate == null || notification.EndDate >= now);
        }

        private List<DashboardNotification> ReadNotifications()
        {
            // No need for thread synchronization

            if (_list != null && _listCreated.AddMinutes(ListLifeTimeInMinutes) >= DateTimeOffset.UtcNow)
                return _list;

            var folderPath = _paths.NotificationsFolderPath;
            var files = Directory.GetFiles(folderPath, "*.json");
            var result = new List<DashboardNotification>();

            foreach (var filePath in files)
            {
                var notification = ReadNotification(filePath);
                if (notification != null)
                    result.Add(notification);
            }

            _list = result;
            _listCreated = DateTimeOffset.UtcNow;

            return _list;
        }

        private static DashboardNotification ReadNotification(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            if (!Guid.TryParse(Path.GetFileNameWithoutExtension(fileName), out var notificationId))
                return null;

            DashboardNotification notification;

            try
            {
                var fileContent = File.ReadAllText(filePath);
                notification = JsonConvert.DeserializeObject<DashboardNotification>(fileContent);
            }
            catch
            {
                return null;
            }

            if (string.IsNullOrEmpty(notification.Title)
                || notification.Type == DashboardNotificationType.None
                || notification.Modified == DateTimeOffset.MinValue
            )
            {
                return null;
            }

            notification.NotificationId = notificationId;

            return notification;
        }
    }
}