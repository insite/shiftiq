using System;
using System.Collections.Generic;

using InSite.Application.Invoices.Read;
using InSite.UI.Portal.Billing.Models;

using static InSite.UI.Portal.Billing.Models.PriceSelectionModel;

namespace InSite.UI.Portal.Billing.Utilities
{
    public static class OrderHelper
    {
        private static Guid OrganizationId => CurrentSessionState.Identity.Organization.Identifier;

        public static List<TOrder> BuildOrdersFromCart(
            CartState cart,
            Guid managerUserId,
            DateTimeOffset? now = null
            )
        {
            var stamp = now ?? DateTimeOffset.UtcNow;

            switch (cart.Mode)
            {
                case PriceSelectionMode.Package:
                    return GetPackageOrder(cart, managerUserId, stamp);
                case PriceSelectionMode.ALaCarte:
                    return GetOrdersFromItems(cart, managerUserId, stamp, "ALaCarte");
                case PriceSelectionMode.Subscribe:
                    return GetOrdersFromItems(cart, managerUserId, stamp, "Subscription");
                default:
                    throw new ArgumentException($"Invalid mode: {cart.Mode}");
            }
        }

        private static List<TOrder> GetPackageOrder(CartState cart, Guid managerUserId, DateTimeOffset stamp)
        {
            var result = new List<TOrder>();

            if (!cart.PackageProductId.HasValue)
                return result;

            var pkg = ServiceLocator.InvoiceSearch.GetProduct(cart.PackageProductId.Value);
            var pkgPrice = pkg?.ProductPrice ?? 0m;

            var pkgOrder = CreateBaseOrder(
                OrganizationId,
                managerUserId,
                stamp,
                rootProduct: pkg,
                totalAmount: pkgPrice);

            foreach (var kv in cart.Items)
            {
                var child = ServiceLocator.InvoiceSearch.GetProduct(kv.Key);
                pkgOrder.OrderItems.Add(GetOrderItem(child, "PackageItem", kv.Value, 0, managerUserId, stamp));
            }

            result.Add(pkgOrder);

            return result;
        }

        private static List<TOrder> GetOrdersFromItems(CartState cart, Guid managerUserId, DateTimeOffset stamp, string orderItemType)
        {
            var result = new List<TOrder>();

            foreach (var kv in cart.Items)
            {
                var productId = kv.Key;
                var qty = kv.Value;

                var p = ServiceLocator.InvoiceSearch.GetProduct(productId);
                var unit = p?.ProductPrice ?? 0m;
                var total = unit * qty;

                var order = CreateBaseOrder(
                    OrganizationId,
                    managerUserId,
                    stamp,
                    rootProduct: p,
                    totalAmount: total);

                order.OrderItems.Add(GetOrderItem(p, orderItemType, qty, unit, managerUserId, stamp));

                result.Add(order);
            }

            return result;
        }

        private static TOrderItem GetOrderItem(TProduct product, string orderItemType, int quantity, decimal unitPrice, Guid managerUserId, DateTimeOffset stamp)
        {
            return new TOrderItem
            {
                OrderItemIdentifier = Guid.NewGuid(),
                ProductIdentifier = product.ProductIdentifier,
                ProductName = product.ProductName,
                OrderItemType = orderItemType,
                OrderItemQuantity = quantity,
                UnitPrice = unitPrice,
                CreatedBy = managerUserId,
                ModifiedBy = managerUserId,
                Created = stamp,
                Modified = stamp
            };
        }

        private static TOrder CreateBaseOrder(
            Guid organizationId,
            Guid managerUserId,
            DateTimeOffset stamp,
            TProduct rootProduct,
            decimal totalAmount)
        {
            return new TOrder
            {
                OrderIdentifier = Guid.NewGuid(),
                CustomerUserIdentifier = managerUserId,
                OrganizationIdentifier = organizationId,
                ProductIdentifier = rootProduct?.ProductIdentifier ?? Guid.Empty,
                ManagerUserIdentifier = managerUserId,

                ProductUrl = rootProduct?.ProductUrl,
                ProductName = rootProduct?.ProductName,
                ProductType = rootProduct?.ProductType,

                DiscountAmount = 0m,
                TaxAmount = 0m,
                TaxRate = 0m,
                TotalAmount = totalAmount,

                OrderCompleted = stamp,

                CreatedBy = managerUserId,
                ModifiedBy = managerUserId,
                Created = stamp,
                Modified = stamp,

                OrderItems = new HashSet<TOrderItem>()
            };
        }
    }
}
