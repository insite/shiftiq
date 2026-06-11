using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using InSite.Application.Invoices.Read;
using InSite.Domain.Invoices;
using InSite.Domain.Sales.Invoices.Changes;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    public class InvoiceStore : IInvoiceStore
    {
        public const int DescriptionMaxLength = 400;

        internal InternalDbContext CreateContext() => new InternalDbContext(true);

        #region Invoices

        public void InsertInvoice(InvoiceDrafted e)
        {
            using (var db = CreateContext())
            {
                var invoice = new QInvoice
                {
                    InvoiceStatus = InvoiceStatus.Drafted.ToString(),
                    CustomerIdentifier = e.Customer,
                    OrganizationIdentifier = e.Tenant,
                    InvoiceDrafted = e.ChangeTime,
                    InvoiceIdentifier = e.AggregateIdentifier,
                    InvoiceNumber = e.Number
                };
                db.QInvoices.Add(invoice);

                for (var i = 0; i < e.Items.Length; i++)
                {
                    var item = new QInvoiceItem
                    {
                        InvoiceIdentifier = invoice.InvoiceIdentifier,
                        ItemIdentifier = e.Items[i].Identifier,
                        ProductIdentifier = e.Items[i].Product,
                        ItemSequence = i + 1,
                        ItemQuantity = e.Items[i].Quantity,
                        ItemPrice = e.Items[i].Price,
                        TaxRate = e.Items[i].TaxRate,
                        ItemDescription = StringHelper.Snip(e.Items[i].Description, DescriptionMaxLength),
                        OrganizationIdentifier = e.Tenant,
                    };
                    db.QInvoiceItems.Add(item);
                }

                db.SaveChanges();
            }
        }

        public void UpdateInvoice(InvoiceCustomerChanged e)
        {
            using (var db = CreateContext())
            {
                var invoice = db.QInvoices.Single(x => x.InvoiceIdentifier == e.AggregateIdentifier);
                invoice.CustomerIdentifier = e.Customer;
                db.SaveChanges();
            }
        }


        public void UpdateInvoice(InvoiceIssueChanged e)
        {
            using (var db = CreateContext())
            {
                var invoice = db.QInvoices.Single(x => x.InvoiceIdentifier == e.AggregateIdentifier);
                invoice.IssueIdentifier = e.Issue;
                db.SaveChanges();
            }
        }

        public void UpdateInvoice(InvoiceBusinessCustomerChanged e)
        {
            using (var db = CreateContext())
            {
                var invoice = db.QInvoices.Single(x => x.InvoiceIdentifier == e.AggregateIdentifier);
                invoice.BusinessCustomerGroupIdentifier = e.BusinessCustomer;
                db.SaveChanges();
            }
        }

        public void UpdateInvoice(InvoiceEmployeeChanged e)
        {
            using (var db = CreateContext())
            {
                var invoice = db.QInvoices.Single(x => x.InvoiceIdentifier == e.AggregateIdentifier);
                invoice.EmployeeUserIdentifier = e.Employee;
                db.SaveChanges();
            }
        }

        public void UpdateInvoice(InvoiceStatusChanged e)
        {
            using (var db = CreateContext())
            {
                var invoice = db.QInvoices.Single(x => x.InvoiceIdentifier == e.AggregateIdentifier);
                invoice.InvoiceStatus = e.InvoiceStatus;
                db.SaveChanges();
            }
        }

        public void UpdateInvoice(InvoicePaidDateChanged e)
        {
            using (var db = CreateContext())
            {
                var invoice = db.QInvoices.Single(x => x.InvoiceIdentifier == e.AggregateIdentifier);
                invoice.InvoicePaid = e.InvocePaidDate;
                db.SaveChanges();
            }
        }

        public void UpdateInvoice(InvoiceNumberChanged e)
        {
            using (var db = CreateContext())
            {
                var invoice = db.QInvoices.Single(x => x.InvoiceIdentifier == e.AggregateIdentifier);
                invoice.InvoiceNumber = e.Number;
                db.SaveChanges();
            }
        }

        public void UpdateInvoice(InvoicePaid e)
        {
            using (var db = CreateContext())
            {
                var invoice = db.QInvoices.Single(x => x.InvoiceIdentifier == e.AggregateIdentifier);
                invoice.InvoiceStatus = InvoiceStatus.Paid.ToString();
                invoice.InvoicePaid = e.Paid ?? e.ChangeTime;
                db.SaveChanges();
            }
        }

        public void UpdateInvoice(InvoicePaymentFailed e)
        {
            using (var db = CreateContext())
            {
                var invoice = db.QInvoices.Single(x => x.InvoiceIdentifier == e.AggregateIdentifier);
                invoice.InvoiceStatus = InvoiceStatus.PaymentFailed.ToString();
                db.SaveChanges();
            }
        }

        public void UpdateInvoice(InvoiceSubmitted e)
        {
            using (var db = CreateContext())
            {
                var invoice = db.QInvoices.Single(x => x.InvoiceIdentifier == e.AggregateIdentifier);
                invoice.InvoiceStatus = InvoiceStatus.Submitted.ToString();
                invoice.InvoiceSubmitted = e.ChangeTime;
                db.SaveChanges();
            }
        }

        public void UpdateInvoice(InvoiceReferenced e)
        {
            using (var db = CreateContext())
            {
                var invoice = db.QInvoices.Single(x => x.InvoiceIdentifier == e.AggregateIdentifier);
                invoice.ReferencedInvoiceIdentifier = e.ReferencedInvoice;
                db.SaveChanges();
            }
        }

        public void DeleteInvoice(InvoiceDeleted e)
        {
            using (var db = CreateContext())
            {
                var invoice = db.QInvoices
                    .Where(x => x.InvoiceIdentifier == e.AggregateIdentifier)
                    .Include(x => x.InvoiceItems)
                    .FirstOrDefault();

                if (invoice == null)
                    return;

                db.QInvoiceItems.RemoveRange(invoice.InvoiceItems);
                db.QInvoices.Remove(invoice);

                db.SaveChanges();
            }
        }

        #endregion

        #region Invoice Items

        public void InsertInvoiceItem(InvoiceItemAdded e)
        {
            using (var db = CreateContext())
            {
                var maxItemNumber = db.QInvoiceItems.Where(x => x.InvoiceIdentifier == e.AggregateIdentifier).Max(x => x.ItemSequence);

                var item = new QInvoiceItem
                {
                    InvoiceIdentifier = e.AggregateIdentifier,
                    ItemIdentifier = e.Item.Identifier,
                    ItemSequence = maxItemNumber + 1,
                    ProductIdentifier = e.Item.Product,
                    ItemPrice = e.Item.Price,
                    TaxRate = e.Item.TaxRate,
                    ItemQuantity = e.Item.Quantity,
                    ItemDescription = StringHelper.Snip(e.Item.Description, DescriptionMaxLength),
                    OrganizationIdentifier = e.OriginOrganization,
                };
                db.QInvoiceItems.Add(item);

                db.SaveChanges();
            }
        }

        public void UpdateInvoiceItem(InvoiceItemChanged e)
        {
            using (var db = CreateContext())
            {
                var invoiceItem = db.QInvoiceItems.Single(x => x.InvoiceIdentifier == e.AggregateIdentifier && x.ItemIdentifier == e.Item.Identifier);
                invoiceItem.ItemDescription = StringHelper.Snip(e.Item.Description, DescriptionMaxLength);
                invoiceItem.ItemPrice = e.Item.Price;
                invoiceItem.ItemQuantity = e.Item.Quantity;
                invoiceItem.ProductIdentifier = e.Item.Product;
                invoiceItem.TaxRate = e.Item.TaxRate;
                db.SaveChanges();
            }
        }

        public void DeleteInvoiceItem(InvoiceItemRemoved e)
        {
            using (var db = CreateContext())
            {
                var invoiceItem = db.QInvoiceItems.Single(x => x.InvoiceIdentifier == e.AggregateIdentifier && x.ItemIdentifier == e.ItemIdentifier);
                var itemNumber = invoiceItem.ItemSequence;

                db.QInvoiceItems.Remove(invoiceItem);
                db.SaveChanges();

                var items = db.QInvoiceItems.Where(x => x.InvoiceIdentifier == e.AggregateIdentifier && x.ItemSequence > itemNumber);
                foreach (var item in items)
                {
                    item.ItemSequence--;
                }
                db.SaveChanges();
            }
        }

        #endregion

        #region Products

        public void InsertProduct(TProduct product)
        {
            using (var db = CreateContext())
            {
                db.TProducts.Add(product);
                db.SaveChanges();
            }
        }

        public void UpdateProduct(TProduct product)
        {
            using (var db = CreateContext())
            {
                var entity = db.TProducts.Single(x => x.ProductIdentifier == product.ProductIdentifier);

                entity.ProductName = product.ProductName;
                entity.ProductDescription = product.ProductDescription;
                entity.ProductType = product.ProductType;
                entity.ProductPrice = product.ProductPrice;
                entity.ProductCurrency = product.ProductCurrency;
                entity.ProductImageUrl = product.ProductImageUrl;
                entity.ObjectIdentifier = product.ObjectIdentifier;
                entity.ObjectType = product.ObjectType;
                entity.ModifiedBy = product.ModifiedBy;
                entity.CreatedBy = product.CreatedBy;
                entity.ProductUrl = product.ProductUrl;
                entity.ProductSummary = product.ProductSummary;
                entity.IsFeatured = product.IsFeatured;
                entity.IsTaxable = product.IsTaxable;
                entity.IndustryItemIdentifier = product.IndustryItemIdentifier;
                entity.OccupationItemIdentifier = product.OccupationItemIdentifier;
                entity.LevelItemIdentifier = product.LevelItemIdentifier;
                entity.ProductQuantity = product.ProductQuantity;

                db.SaveChanges();
            }
        }

        public void UpdateProduct<TProduct>(Guid product, params (Expression<Func<TProduct, object>> Property, object Value)[] updates)
        {
            using (var db = CreateContext())
            {
                var entity = db.TProducts.SingleOrDefault(x => x.ProductIdentifier == product);

                if (entity == null)
                    return;

                foreach (var update in updates)
                {
                    MemberExpression memberExpression;

                    if (update.Property.Body is UnaryExpression unaryExpression)
                        memberExpression = unaryExpression.Operand as MemberExpression;
                    else
                        memberExpression = update.Property.Body as MemberExpression;

                    if (memberExpression == null)
                        throw new InvalidOperationException("Invalid property expression.");

                    var property = (PropertyInfo)memberExpression.Member;
                    property.SetValue(entity, update.Value);
                }

                db.SaveChanges();
            }
        }

        public void DeleteProduct(Guid product)
        {
            using (var db = CreateContext())
            {
                var entities = db.TProducts.Where(x => x.ProductIdentifier == product);

                db.TProducts.RemoveRange(entities);
                db.SaveChanges();
            }
        }

        #endregion

        #region Orders

        public void InsertOrder(TOrder order)
        {
            using (var db = CreateContext())
            {
                db.TOrders.Add(order);
                db.SaveChanges();
            }
        }

        public void UpdateOrder(TOrder order)
        {
            using (var db = CreateContext())
            {
                var entity = db.TOrders.Single(x => x.OrderIdentifier == order.OrderIdentifier);

                entity.OrderCompleted = order.OrderCompleted;
                entity.ProductUrl = order.ProductUrl;
                entity.ProductName = order.ProductName;

                db.SaveChanges();
            }
        }

        public void DeleteOrder(Guid order)
        {
            using (var db = CreateContext())
            {
                var entities = db.TOrders.Where(x => x.OrderIdentifier == order);

                db.TOrders.RemoveRange(entities);
                db.SaveChanges();
            }
        }


        #endregion

        #region Taxes

        public void InsertTax(TTax tax)
        {
            using (var db = CreateContext())
            {
                db.TTaxes.Add(tax);
                db.SaveChanges();
            }
        }

        public void UpdateTax(TTax tax)
        {
            using (var db = CreateContext())
            {
                var existing = db.TTaxes.FirstOrDefault(x => x.TaxIdentifier == tax.TaxIdentifier);
                if (existing == null)
                    return;

                existing.CountryCode = tax.CountryCode;
                existing.RegionCode = tax.RegionCode;
                existing.TaxName = tax.TaxName;
                existing.TaxRate = tax.TaxRate;

                db.SaveChanges();
            }
        }

        public void DeleteTax(Guid taxId)
        {
            using (var db = CreateContext())
            {
                var tax = db.TTaxes
                    .Where(x => x.TaxIdentifier == taxId)
                    .FirstOrDefault();

                if (tax != null)
                {
                    db.TTaxes.Remove(tax);
                    db.SaveChanges();
                }
            }
        }

        #endregion
    }
}
