using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using InSite.Application.Gateways.Write;
using InSite.Application.Invoices.Read;
using InSite.Application.Invoices.Write;
using InSite.Domain.Invoices;
using InSite.Domain.Payments;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

using static InSite.UI.Portal.Billing.Models.PriceSelectionModel;

namespace InSite.UI.Portal.Billing.Utilities
{
    public static class InvoiceHelper
    {
        private static Guid OrganizationId => CurrentSessionState.Identity.Organization.Identifier;

        public static Guid ProcessOrderPayment(
            IList<TOrder> orders,
            Guid customerUserId,
            PriceSelectionMode mode,
            decimal taxRate,
            UnmaskedCreditCard card
            )
        {
            var updatedOrders = DraftAndSubmitInvoiceForOrders(
                orders,
                customerUserId,
                mode,
                taxRate,
                now: null
            );

            var invoiceId = updatedOrders.FirstOrDefault().InvoiceIdentifier;

            var invoice = ServiceLocator.InvoiceSearch.GetInvoice(invoiceId)
                ?? throw new InvalidOperationException($"Invoice {invoiceId} is not found after submission.");

            var amount = invoice.InvoiceAmount ?? 0m;
            if (amount <= 0m)
                throw new InvalidOperationException($"Invoice {invoiceId} has non-positive amount.");

            var paymentId = UniqueIdentifier.Create();
            var startPaymentCommand = new StartPayment(
                PaymentIdentifiers.BamboraGateway,
                OrganizationId,
                invoiceId,
                paymentId,
                new PaymentInput(
                    InSite.Domain.Invoices.Invoice.FormatInvoiceNumber(invoice.InvoiceNumber.Value),
                    amount,
                    card,
                    null,
                    HttpContext.Current.Request.UserHostAddress
                )
            );

            ServiceLocator.SendCommand(startPaymentCommand);

            return paymentId;
        }

        private static IList<TOrder> DraftAndSubmitInvoiceForOrders(
            IList<TOrder> orders,
            Guid customerUserId,
            PriceSelectionMode mode,
            decimal taxRate,
            DateTimeOffset? now = null)
        {
            if (orders == null || orders.Count == 0)
                return orders ?? Array.Empty<TOrder>();

            var stamp = now ?? DateTimeOffset.UtcNow;

            if (mode == PriceSelectionMode.ALaCarte || mode == PriceSelectionMode.Subscribe)
                return DraftALCorSubscribtionInvoice(orders, customerUserId, mode, taxRate, stamp);

            return DraftPackageInvoice(orders, customerUserId, taxRate, stamp);
        }

        private static IList<TOrder> DraftPackageInvoice(IList<TOrder> orders, Guid customerUserId, decimal taxRate, DateTimeOffset stamp)
        {
            foreach (var order in orders)
            {
                Guid rootItemId;
                var invoiceItems = BuildInvoiceItemsForOrder(order, taxRate, out rootItemId);
                var invoiceId = UniqueIdentifier.Create();
                var invoiceNumber = Sequence.Increment(OrganizationId, SequenceType.Invoice);

                ServiceLocator.SendCommand(new DraftInvoice(invoiceId, OrganizationId, invoiceNumber, customerUserId, invoiceItems));
                ServiceLocator.SendCommand(new SubmitInvoice(invoiceId));

                order.InvoiceIdentifier = invoiceId;
                order.InvoiceItemIdentifier = rootItemId;
                order.CustomerUserIdentifier = customerUserId;
                order.OrganizationIdentifier = OrganizationId;
                order.OrderCompleted = stamp;

                ServiceLocator.InvoiceStore.InsertOrder(order);
            }

            return orders;
        }

        private static IList<TOrder> DraftALCorSubscribtionInvoice(IList<TOrder> orders, Guid customerUserId, PriceSelectionMode mode, decimal taxRate, DateTimeOffset stamp)
        {
            var invoiceId = UniqueIdentifier.Create();
            var invoiceNumber = Sequence.Increment(OrganizationId, SequenceType.Invoice);

            var items = new List<InvoiceItem>();
            var perOrderItemId = new Dictionary<Guid, Guid>();

            foreach (var order in orders)
            {
                var item = BuildSingleItemForOrder(order, mode, taxRate, out Guid itemId);
                items.Add(item);
                perOrderItemId[order.OrderIdentifier] = itemId;
            }

            ServiceLocator.SendCommand(new DraftInvoice(invoiceId, OrganizationId, invoiceNumber, customerUserId, items.ToArray()));
            ServiceLocator.SendCommand(new SubmitInvoice(invoiceId));

            foreach (var order in orders)
            {
                order.InvoiceIdentifier = invoiceId;
                order.InvoiceItemIdentifier = perOrderItemId[order.OrderIdentifier];
                order.CustomerUserIdentifier = customerUserId;
                order.OrganizationIdentifier = OrganizationId;
                order.OrderCompleted = stamp;

                ServiceLocator.InvoiceStore.InsertOrder(order);
            }

            return orders;
        }

        private static InvoiceItem BuildSingleItemForOrder(TOrder order, PriceSelectionMode mode, decimal taxRate, out Guid itemId)
        {
            itemId = UuidFactory.Create();

            var li = order.OrderItems.FirstOrDefault();
            var qty = li?.OrderItemQuantity ?? 1;
            var unit = li?.UnitPrice ?? order.TotalAmount;

            if (mode == PriceSelectionMode.Subscribe)
            {
                var productId = li?.ProductIdentifier ?? order.ProductIdentifier;
                var product = ServiceLocator.InvoiceSearch.GetProduct(productId);
                bool taxable = (product?.IsTaxable ?? false);

                return new InvoiceItem
                {
                    Identifier = itemId,
                    Product = productId,
                    Quantity = qty,
                    Price = li?.UnitPrice ?? (qty > 0 ? Math.Round(order.TotalAmount / qty, 2) : order.TotalAmount),
                    Description = "(Subscription) " + (li?.ProductName ?? order.ProductName),
                    TaxRate = taxable ? taxRate : (decimal?)null
                };
            }

            var alcProductId = li?.ProductIdentifier ?? order.ProductIdentifier;
            var alc = ServiceLocator.InvoiceSearch.GetProduct(alcProductId);
            bool alcTaxable = (alc?.IsTaxable ?? false);

            return new InvoiceItem
            {
                Identifier = itemId,
                Product = alcProductId,
                Quantity = qty,
                Price = unit,
                Description = li?.ProductName ?? order.ProductName,
                TaxRate = alcTaxable ? taxRate : (decimal?)null
            };
        }

        private static InvoiceItem[] BuildInvoiceItemsForOrder(TOrder order, decimal taxRate, out Guid firstItemId)
        {
            firstItemId = Guid.Empty;

            var hasPackageChildren = order.OrderItems.Any(i =>
                string.Equals(i.OrderItemType, "PackageItem", StringComparison.OrdinalIgnoreCase));

            var isPackage = hasPackageChildren ||
                            string.Equals(order.ProductType, "Package", StringComparison.OrdinalIgnoreCase);

            if (!isPackage)
            {
                var product = ServiceLocator.InvoiceSearch.GetProduct(order.ProductIdentifier);
                bool taxable = (product?.IsTaxable ?? false);

                var singleId = UuidFactory.Create();
                firstItemId = singleId;
                return new[]
                {
                    new InvoiceItem
                    {
                        Identifier  = singleId,
                        Product     = order.ProductIdentifier,
                        Quantity    = 1,
                        Price       = order.TotalAmount,
                        Description = order.ProductName,
                        TaxRate     = taxable ? taxRate : (decimal?)null
                    }
                };
            }

            var package = ServiceLocator.InvoiceSearch.GetProduct(order.ProductIdentifier);
            var pkgTaxable = (package?.IsTaxable ?? false);

            if (hasPackageChildren)
            {
                var packQty = package?.ProductQuantity ?? 0;
                var perSlot = (packQty > 0) ? Math.Round(order.TotalAmount / packQty, 2) : order.TotalAmount;
                var pkgName = order.ProductName ?? package?.ProductName ?? "Package";

                var lines = new List<InvoiceItem>();
                foreach (var child in order.OrderItems)
                {
                    var id = UuidFactory.Create();
                    if (firstItemId == Guid.Empty) firstItemId = id;

                    lines.Add(new InvoiceItem
                    {
                        Identifier = id,
                        Product = child.ProductIdentifier ?? order.ProductIdentifier,
                        Quantity = child.OrderItemQuantity,
                        Price = perSlot,
                        Description = $"({pkgName}) {child.ProductName ?? "(Item)"}",
                        TaxRate = pkgTaxable ? taxRate : (decimal?)null
                    });
                }
                return lines.ToArray();
            }

            var rootId = UuidFactory.Create();
            firstItemId = rootId;
            return new[]
            {
                new InvoiceItem
                {
                    Identifier  = rootId,
                    Product     = order.ProductIdentifier,
                    Quantity    = 1,
                    Price       = order.TotalAmount,
                    Description = order.ProductName,
                    TaxRate     = pkgTaxable ? taxRate : (decimal?)null
                }
            };
        }
    }
}
