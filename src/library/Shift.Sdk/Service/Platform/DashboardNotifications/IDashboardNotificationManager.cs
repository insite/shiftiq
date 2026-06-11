using System;

namespace Shift.Sdk.Service.Platform.DashboardNotifications
{
    public interface IDashboardNotificationManager
    {
        ActiveNotification[] CollectActiveNotifications();
        DashboardNotificationList SearchNotifications(DashboardNotificationCriteria criteria);
        DashboardNotification RetrieveNotification(Guid notificationId);
        void ModifyNotification(DashboardNotification notification, Guid modifiedBy);
        void DeleteNotification(Guid notificationId);
    }
}