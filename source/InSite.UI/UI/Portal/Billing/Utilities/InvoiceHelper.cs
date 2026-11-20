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
            ProvinceTax tax,
            UnmaskedCreditCard card
            )
        {
            var updatedOrders = DraftAndSubmitInvoiceForOrders(
                orders,
                customerUserId,
                mode,
                tax,
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
            ProvinceTax tax,
            DateTimeOffset? now = null)
        {
            if (orders == null || orders.Count == 0)
                return orders ?? Array.Empty<TOrder>();

            var stamp = now ?? DateTimeOffset.UtcNow;

            if (mode == PriceSelectionMode.ALaCarte || mode == PriceSelectionMode.Subscribe)
                return DraftALCorSubscribtionInvoice(orders, customerUserId, mode, tax, stamp);

            return DraftPackageInvoice(orders, customerUserId, tax, stamp);
        }

        private static IList<TOrder> DraftPackageInvoice(IList<TOrder> orders, Guid customerUserId, ProvinceTax tax, DateTimeOffset stamp)
        {
            foreach (var order in orders)
            {
                Guid rootItemId;
                var invoiceItems = BuildInvoiceItemsForOrder(order, tax, out rootItemId);
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

        private static IList<TOrder> DraftALCorSubscribtionInvoice(IList<TOrder> orders, Guid customerUserId, PriceSelectionMode mode, ProvinceTax tax, DateTimeOffset stamp)
        {
            var invoiceId = UniqueIdentifier.Create();
            var invoiceNumber = Sequence.Increment(OrganizationId, SequenceType.Invoice);

            var items = new List<InvoiceItem>();
            var perOrderItemId = new Dictionary<Guid, Guid>();

            foreach (var order in orders)
            {
                var item = BuildSingleItemForOrder(order, mode, tax, out Guid itemId);
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

        private static InvoiceItem BuildSingleItemForOrder(TOrder order, PriceSelectionMode mode, ProvinceTax tax, out Guid itemId)
        {
            itemId = UuidFactory.Create();

            var orderItem = order.OrderItems.FirstOrDefault();
            var productId = orderItem?.ProductIdentifier ?? order.ProductIdentifier;
            var product = ServiceLocator.InvoiceSearch.GetProduct(productId);
            var quantity = orderItem?.OrderItemQuantity ?? 1;

            var invoiceItem = new InvoiceItem
            {
                Identifier = itemId,
                Product = productId,
                Quantity = quantity
            };

            if (mode == PriceSelectionMode.Subscribe)
            {
                invoiceItem.Price = orderItem?.UnitPrice ?? (quantity > 0 ? Math.Round(order.TotalAmount / quantity, 2) : order.TotalAmount);
                invoiceItem.Description = "(Subscription) " + (orderItem?.ProductName ?? order.ProductName);
            }
            else
            {
                invoiceItem.Price = orderItem?.UnitPrice ?? order.TotalAmount;
                invoiceItem.Description = orderItem?.ProductName ?? order.ProductName;
            }

            SetTax(invoiceItem, tax, product?.IsTaxable == true);

            return invoiceItem;
        }

        private static InvoiceItem[] BuildInvoiceItemsForOrder(TOrder order, ProvinceTax tax, out Guid firstItemId)
        {
            var hasPackageChildren = order.OrderItems.Any(i =>
                string.Equals(i.OrderItemType, "PackageItem", StringComparison.OrdinalIgnoreCase));

            var isPackage = hasPackageChildren ||
                            string.Equals(order.ProductType, "Package", StringComparison.OrdinalIgnoreCase);

            if (!isPackage)
            {
                var product = ServiceLocator.InvoiceSearch.GetProduct(order.ProductIdentifier);
                var invoiceItem = new InvoiceItem
                {
                    Identifier = firstItemId = UuidFactory.Create(),
                    Product = order.ProductIdentifier,
                    Quantity = 1,
                    Price = order.TotalAmount,
                    Description = order.ProductName,
                };

                SetTax(invoiceItem, tax, product?.IsTaxable == true);

                return new[] { invoiceItem };
            }

            var package = ServiceLocator.InvoiceSearch.GetProduct(order.ProductIdentifier);
            var pkgTaxable = (package?.IsTaxable ?? false);

            if (hasPackageChildren)
            {
                var packQty = package?.ProductQuantity ?? 0;
                var perSlot = (packQty > 0) ? Math.Round(order.TotalAmount / packQty, 2) : order.TotalAmount;
                var pkgName = order.ProductName ?? package?.ProductName ?? "Package";

                var lines = new List<InvoiceItem>(order.OrderItems.Count);

                foreach (var child in order.OrderItems)
                {
                    var invoiceItem = new InvoiceItem
                    {
                        Identifier = UuidFactory.Create(),
                        Product = child.ProductIdentifier ?? order.ProductIdentifier,
                        Quantity = child.OrderItemQuantity,
                        Price = perSlot,
                        Description = $"({pkgName}) {child.ProductName ?? "(Item)"}"
                    };

                    SetTax(invoiceItem, tax, pkgTaxable);

                    lines.Add(invoiceItem);
                }

                firstItemId = lines.FirstOrDefault().Identifier;

                return lines.ToArray();
            }

            var rootItem = new InvoiceItem
            {
                Identifier = firstItemId = UuidFactory.Create(),
                Product = order.ProductIdentifier,
                Quantity = 1,
                Price = order.TotalAmount,
                Description = order.ProductName
            };

            SetTax(rootItem, tax, pkgTaxable);

            return new[] { rootItem };
        }

        private static void SetTax(InvoiceItem item, ProvinceTax tax, bool isTaxable)
        {
            if (!isTaxable)
                return;

            item.TaxRate = tax.Rate;
            item.TaxCountry = tax.Country;
            item.TaxRegion = tax.Region;
        }
    }
}
