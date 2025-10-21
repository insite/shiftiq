using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Courses.Read;
using InSite.Application.Invoices.Read;

using Shift.Constant;

using static InSite.UI.Portal.Billing.Models.PriceSelectionModel;

namespace InSite.UI.Portal.Billing.Utilities
{

    public static class DistributionHelper
    {

        public static List<TCourseDistribution> BuildDistributionsFromOrders(
            IList<TOrder> orders,
            List<(Guid ProductId, Guid ClassEventId, Guid CourseId)> classEvents,
            PriceSelectionMode mode,
            Guid managerUserId,
            DateTimeOffset? now = null)
        {
            var result = new List<TCourseDistribution>();
            if (orders == null || orders.Count == 0)
                return result;

            var stamp = now ?? DateTimeOffset.UtcNow;

            Dictionary<Guid, (Guid EventId, Guid CourseId)> map = MapEvents(classEvents);

            switch (mode)
            {
                case PriceSelectionMode.ALaCarte:
                    {
                        ALaCarteDistribution(orders, map, managerUserId, result, stamp);
                        break;
                    }

                case PriceSelectionMode.Package:
                    {
                        PackageDistribution(orders, map, managerUserId, result, stamp);
                        break;
                    }

                case PriceSelectionMode.Subscribe:
                    {
                        SubscriptionDistribution(orders, managerUserId, result, stamp);
                        break;
                    }

                default:
                    break;
            }

            return result;
        }

        private static Dictionary<Guid, (Guid EventId, Guid CourseId)> MapEvents(List<(Guid ProductId, Guid ClassEventId, Guid CourseId)> classEvents)
        {
            var map = new Dictionary<Guid, (Guid EventId, Guid CourseId)>();
            if (classEvents != null)
            {
                foreach (var t in classEvents)
                    map[t.Item1] = (t.Item2, t.Item3);
            }

            return map;
        }

        private static void SubscriptionDistribution(IList<TOrder> orders, Guid managerUserId, List<TCourseDistribution> result, DateTimeOffset stamp)
        {
            foreach (var order in orders)
            {
                var li = order.OrderItems.FirstOrDefault();
                var packagePid = li?.ProductIdentifier ?? order.ProductIdentifier;

                var pkg = ServiceLocator.InvoiceSearch.GetProduct(packagePid);
                var perPackageQty = Math.Max(0, pkg?.ProductQuantity ?? 0);
                var packagesPurchased = Math.Max(1, li?.OrderItemQuantity ?? 1);
                var total = perPackageQty * packagesPurchased;

                for (int i = 0; i < total; i++)
                {
                    result.Add(new TCourseDistribution
                    {
                        CourseDistributionIdentifier = Guid.NewGuid(),
                        ProductIdentifier = packagePid,
                        ManagerUserIdentifier = managerUserId,
                        Created = stamp,
                        DistributionStatus = CourseDistributionStatus.NotAssigned.ToString()
                    });
                }
            }
        }

        private static void PackageDistribution(IList<TOrder> orders, IDictionary<Guid, (Guid EventId, Guid CourseId)> map, Guid managerUserId, List<TCourseDistribution> result, DateTimeOffset stamp)
        {
            var packageOrder = orders.First();
            foreach (var child in packageOrder.OrderItems.Where(oi =>
                         string.Equals(oi.OrderItemType, "PackageItem", StringComparison.OrdinalIgnoreCase)))
            {
                var childPid = child.ProductIdentifier ?? packageOrder.ProductIdentifier;
                var qty = Math.Max(1, child.OrderItemQuantity);

                Guid? eventId = null;
                Guid? courseId = null;
                if (map != null && map.TryGetValue(childPid, out var info))
                {
                    eventId = info.EventId;
                    courseId = info.CourseId;
                }

                for (int i = 0; i < qty; i++)
                {
                    result.Add(new TCourseDistribution
                    {
                        CourseDistributionIdentifier = Guid.NewGuid(),
                        ProductIdentifier = childPid,
                        ManagerUserIdentifier = managerUserId,
                        Created = stamp,
                        CourseIdentifier = courseId,
                        EventIdentifier = eventId,
                        DistributionStatus = CourseDistributionStatus.NotAssigned.ToString()
                    });
                }
            }
        }

        private static void ALaCarteDistribution(IList<TOrder> orders, IDictionary<Guid, (Guid EventId, Guid CourseId)> map, Guid managerUserId, List<TCourseDistribution> result, DateTimeOffset stamp)
        {
            foreach (var order in orders)
            {
                var li = order.OrderItems.FirstOrDefault();
                var productId = li?.ProductIdentifier ?? order.ProductIdentifier;
                var qty = Math.Max(1, li?.OrderItemQuantity ?? 1);
                Guid? eventId = null;
                Guid? courseId = null;

                if (map != null && map.TryGetValue(productId, out var info))
                {
                    eventId = info.EventId;
                    courseId = info.CourseId;
                }

                for (int i = 0; i < qty; i++)
                {
                    result.Add(new TCourseDistribution
                    {
                        CourseDistributionIdentifier = Guid.NewGuid(),
                        ProductIdentifier = productId,
                        ManagerUserIdentifier = managerUserId,
                        Created = stamp,
                        CourseIdentifier = courseId,
                        EventIdentifier = eventId,
                        DistributionStatus = CourseDistributionStatus.NotAssigned.ToString()
                    });
                }
            }
        }
    }
}