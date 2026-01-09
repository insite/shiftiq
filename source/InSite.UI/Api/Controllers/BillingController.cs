using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using InSite.Api.Settings;
using InSite.Application.Contacts.Read;
using InSite.Application.Groups.Write;
using InSite.Application.Invoices.Read;
using InSite.Application.Invoices.Write;
using InSite.Application.People.Write;
using InSite.Domain.Contacts;
using InSite.Persistence;
using InSite.Web.Data;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Api.Controllers
{
    [DisplayName("Billing")]
    [ApiAuthenticationRequirement(ApiAuthenticationType.None)]
    public partial class ProductsController : ApiBaseController
    {
        /// <summary>
        /// List products
        /// </summary>
        [Route("api/billing/products")]
        [HttpGet]
        public HttpResponseMessage ListProducts([FromUri] Guid organization)
        {
            try
            {
                var filter = new TProductFilter { OrganizationIdentifier = organization, IsPublished = true, IsAvailableForSale = true };
                var products = ServiceLocator.InvoiceSearch.GetProducts(filter);

                var list = new List<QueryProduct>();
                foreach (var product in products)
                    list.Add(MapProduct(product));

                return JsonSuccess(list);
            }
            catch (Exception ex)
            {
                return JsonError(ex.GetAllMessages());
            }
        }

        /// <summary>
        /// Get one product by product id
        /// </summary>
        [Route("api/billing/product")]
        [HttpGet]
        public HttpResponseMessage GetProduct([FromUri] Guid productId)
        {
            try
            {
                var filter = new TProductFilter { ProductIdentifier = productId, IsPublished = true, IsAvailableForSale = true };
                var product = ServiceLocator.InvoiceSearch.GetProducts(filter).FirstOrDefault();

                if (product == null)
                    return JsonError("PRODUCT NOT FOUND", HttpStatusCode.NotFound);

                return JsonSuccess(MapProduct(product));
            }
            catch (Exception ex)
            {
                return JsonError(ex.GetAllMessages());
            }
        }

        /// <summary>
        /// Returns persons checkout information using the person's unique globally unique identifier.
        /// </summary>
        [HttpGet]
        [ApiAuthenticationRequirement(ApiAuthenticationType.Jwt)]
        [Route("api/billing/checkout-info")]
        public HttpResponseMessage Get([FromUri] Guid user, [FromUri] Guid organization)
        {
            var item = GetCheckoutInfoItem(user, organization);

            return item != null
                ? JsonSuccess(item)
                : JsonError($"Checkout info Not Found: {user}", HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Creates Invoice based on order information object.
        /// </summary>
        [HttpPost]
        [ApiAuthenticationRequirement(ApiAuthenticationType.Jwt)]
        [Route("api/billing/create-order")]
        public HttpResponseMessage CreateOrder([FromBody] OrderInfo orderInfo)
        {
            try
            {
                var item = DraftOrderInvoice(orderInfo);

                SetCustomerOrderPrerequisites(orderInfo);

                return item != null
                    ? JsonSuccess(item)
                    : JsonError($"Order info Not Found", HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message);
            }
        }

        /// <summary>
        /// Returns invoice and invoice kids using invoice unique identifier.
        /// </summary>
        [HttpGet]
        [ApiAuthenticationRequirement(ApiAuthenticationType.Jwt)]
        [Route("api/billing/invoice")]
        public HttpResponseMessage GetInvoice([FromUri] Guid invoice)
        {
            var item = GetInvoiceData(invoice);

            return item != null
                ? JsonSuccess(item)
                : JsonError($"Invoice Not Found: {invoice}", HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Returns all orders made by a customer.
        /// </summary>
        [HttpGet]
        [ApiAuthenticationRequirement(ApiAuthenticationType.Jwt)]
        [Route("api/billing/orders")]
        public HttpResponseMessage GetOrders([FromUri] Guid user, [FromUri] Guid organization)
        {
            var items = GetHistoryData(user, organization);

            return items != null
                ? JsonSuccess(items)
                : JsonError($"Invoice for User: {user} Not Found", HttpStatusCode.BadRequest);
        }

        private List<OrderInfo> GetHistoryData(Guid user, Guid organization)
        {
            var orders = ServiceLocator.InvoiceSearch.GetOrders(
                new TOrderFilter() { CustomerIdentifier = user, OrganizationIdentifier = organization },
                x => x.Customer,
                x => x.Invoice,
                x => x.Invoice.InvoiceItems);

            if (orders == null || orders.Count == 0)
                return null;

            var groupedOrders = orders
                .GroupBy(order => order.InvoiceIdentifier)
                .Select(group => new
                {
                    InvoiceIdentifier = group.Key,
                    Orders = group.ToList()
                })
                .ToList();

            List<OrderInfo> results = new List<OrderInfo>();

            foreach (var group in groupedOrders)
            {
                results.Add(new OrderInfo()
                {
                    Organization = organization,
                    Invoice = group.InvoiceIdentifier,
                    Customer = user,
                    InvoiceNumber = group.Orders.First().Invoice.InvoiceNumber,
                    CreatedDate = group.Orders.First().Invoice.InvoiceDrafted,
                    OrderItems = MapOrderItems(group.Orders)
                });
            }

            return results;
        }

        private List<OrderItem> MapOrderItems(List<TOrder> orders)
        {
            var results = new List<OrderItem>();
            foreach (var order in orders)
            {
                results.Add(new OrderItem()
                {
                    Id = order.OrderIdentifier,
                    ProductId = GetProductIdentifier(order),
                    Name = order.ProductName,
                    ProductUrl = order.ProductUrl,
                    Quantity = GetItemQuantity(order),
                    Price = GetItemPrice(order),
                });
            }
            return results;
        }

        private Invoice GetInvoiceData(Guid invoiceId)
        {
            var invoice = ServiceLocator.InvoiceSearch.GetInvoice(invoiceId);
            var invoiceItems = ServiceLocator.InvoiceSearch.GetInvoiceItems(invoiceId);
            if (invoice == null || invoiceItems == null)
                return null;

            return new Invoice()
            {
                CustomerIdentifier = invoice.CustomerIdentifier,
                InvoiceDrafted = invoice.InvoiceDrafted,
                InvoiceIdentifier = invoice.InvoiceIdentifier,
                InvoiceNumber = invoice.InvoiceNumber,
                InvoicePaid = invoice.InvoicePaid,
                InvoiceStatus = invoice.InvoiceStatus,
                InvoiceSubmitted = invoice.InvoiceSubmitted,
                OrganizationIdentifier = invoice.OrganizationIdentifier,
                InvoiceItems = invoiceItems.Select(x => new InvoiceItem()
                {
                    Description = x.ItemDescription,
                    Identifier = x.ItemIdentifier,
                    Price = x.ItemPrice,
                    TaxRate = x.TaxRate,
                    Product = x.ProductIdentifier,
                    Quantity = x.ItemQuantity,
                }).ToList(),
            };
        }

        private OrderInfo DraftOrderInvoice(OrderInfo orderInfo)
        {
            if (orderInfo == null)
                return null;

            var invoiceItems = new List<Domain.Invoices.InvoiceItem>();

            foreach (var item in orderInfo.OrderItems)
            {
                invoiceItems.Add(new Domain.Invoices.InvoiceItem
                {
                    Identifier = UniqueIdentifier.Create(),
                    Product = item.Id,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Description = item.Description,
                });
            }

            var invoiceID = UniqueIdentifier.Create();
            var invoiceNumber = Sequence.Increment(orderInfo.Organization, SequenceType.Invoice);

            ServiceLocator.SendCommand(new DraftInvoice(invoiceID, orderInfo.Organization, invoiceNumber, orderInfo.Customer, invoiceItems.ToArray()));
            ServiceLocator.SendCommand(new SubmitInvoice(invoiceID));

            orderInfo.Invoice = invoiceID;
            orderInfo.InvoiceNumber = invoiceNumber;

            return orderInfo;
        }

        private void SetCustomerOrderPrerequisites(OrderInfo orderInfo)
        {
            var person = ServiceLocator.PersonSearch.GetPerson(orderInfo.Customer, orderInfo.Organization);
            if (person == null)
                return;

            if (orderInfo.CheckoutInfo.Company.HasValue())
                SetPersonWithEmployer(orderInfo, person);
            else
                SetPersonWithoutEmployer(orderInfo, person);
        }

        private void SetPersonWithEmployer(OrderInfo orderInfo, QPerson person)
        {
            var group = ServiceLocator.GroupSearch.GetGroups(
                new QGroupFilter()
                {
                    GroupName = orderInfo.CheckoutInfo.Company,
                    GroupType = "Employer"
                }).FirstOrDefault();

            if (group != null)
            {
                person.EmployerGroupIdentifier = group.GroupIdentifier;
            }
            else
            {
                var groupId = UniqueIdentifier.Create();
                ServiceLocator.SendCommand(new CreateGroup(groupId, orderInfo.Organization, "Employer", orderInfo.CheckoutInfo.Company));
                person.EmployerGroupIdentifier = groupId;
            }

            ServiceLocator.SendCommand(new ChangeGroupAddress(person.EmployerGroupIdentifier.Value, AddressType.Billing, MapGroupAddress(orderInfo.CheckoutInfo)));

            if (person.EmployerGroupIdentifier != null
                && group != null
                && person.EmployerGroupIdentifier == group.GroupIdentifier)
                return;

            AddEmployeeMembership(person.EmployerGroupIdentifier.Value, orderInfo.Customer);
            PersonStore.Update(person);
        }

        private void SetPersonWithoutEmployer(OrderInfo orderInfo, QPerson person)
        {
            var billingAddress = person.GetAddress(ContactAddressType.Billing);

            var billingAddressId = UniqueIdentifier.Create();

            if (billingAddress != null)
                billingAddressId = billingAddress.AddressIdentifier;

            var address = new PersonAddress
            {
                Identifier = billingAddressId,
                City = orderInfo.CheckoutInfo.City,
                Country = orderInfo.CheckoutInfo.Country,
                PostalCode = orderInfo.CheckoutInfo.PostalCode,
                Province = orderInfo.CheckoutInfo.State,
                Street1 = orderInfo.CheckoutInfo.Street1,
                Street2 = orderInfo.CheckoutInfo.Street2,
            };

            ServiceLocator.SendCommand(new ModifyPersonAddress(person.PersonIdentifier, AddressType.Billing, address));
        }

        private GroupAddress MapGroupAddress(CheckoutInfo checkoutInfo)
        {
            return new GroupAddress()
            {
                City = checkoutInfo.City,
                Country = checkoutInfo.Country,
                PostalCode = checkoutInfo.PostalCode,
                Province = checkoutInfo.State,
                Street1 = checkoutInfo.Street1,
                Street2 = checkoutInfo.Street2,
            };
        }

        private void AddEmployeeMembership(Guid groupId, Guid userId)
        {
            if (MembershipSearch.Exists(x => x.UserIdentifier == userId && x.GroupIdentifier == groupId))
                return;

            var newEmployment = new Membership
            {
                GroupIdentifier = groupId,
                UserIdentifier = userId,
                MembershipType = MembershipType.Employee,
                Assigned = DateTimeOffset.UtcNow
            };

            MembershipHelper.Save(newEmployment);

            var employer = ServiceLocator.GroupSearch.GetGroup(groupId);

            if (employer != null && employer.ParentGroupIdentifier.HasValue)
            {
                MembershipHelper.Save(new Membership
                {
                    GroupIdentifier = employer.ParentGroupIdentifier.Value,
                    UserIdentifier = userId,
                    Assigned = DateTimeOffset.UtcNow
                });
            }
        }

        private CheckoutInfo GetCheckoutInfoItem(Guid userId, Guid orgId)
        {
            var person = PersonSearch.Select(orgId, userId, x => x.BillingAddress, x => x.EmployerGroup);
            if (person == null)
                return null;

            var user = UserSearch.Select(person.UserIdentifier);
            if (user == null)
                return null;

            if (person.EmployerGroupIdentifier.HasValue)
                return person?.BillingAddressIdentifier != null
                ? MapToModel(user, person, ServiceLocator.GroupSearch.GetAddress(person.EmployerGroupIdentifier.Value, AddressType.Billing))
                : MapToModel(user, person);

            return person?.BillingAddressIdentifier != null
                ? MapToModel(user, person, AddressSearch.Select(person.BillingAddressIdentifier.Value))
                : MapToModel(user, person);
        }

        private CheckoutInfo MapToModel(User user, Person person)
        {
            return new CheckoutInfo()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
                Company = person.EmployerGroup?.GroupName,
                Phone = person.Phone,
                Country = "Canada",
            };
        }

        private CheckoutInfo MapToModel(User user, Person person, Address address)
        {
            var model = MapToModel(user, person);

            model.City = address.City;
            model.Country = address.Country;
            model.PostalCode = address.PostalCode;
            model.Street1 = address.Street1;
            model.Street2 = address.Street2;

            return model;
        }

        private CheckoutInfo MapToModel(User user, Person person, QGroupAddress address)
        {
            var model = MapToModel(user, person);

            model.City = address.City;
            model.Country = address.Country;
            model.PostalCode = address.PostalCode;
            model.Street1 = address.Street1;
            model.Street2 = address.Street2;

            return model;
        }

        private QueryProduct MapProduct(TProduct product)
        {
            return new QueryProduct
            {
                ProductIdentifier = product.ProductIdentifier,
                ProductCurrency = product.ProductCurrency,
                ProductDescription = product.ProductDescription,
                ProductName = product.ProductName,
                ProductType = product.ProductType,
                ProductPrice = product.ProductPrice,
                ProductImageUrl = product.ProductImageUrl,
                ObjectType = product.ObjectType,
                ObjectIdentifier = product.ObjectIdentifier,
                ObjectName = GetObjectName(product.ObjectIdentifier, product.ObjectType)
            };
        }

        private string GetObjectName(Guid? objectIdentifier, string objectType)
        {
            if (objectType == null || objectType.Length == 0)
                return null;

            if (!objectIdentifier.HasValue)
                return null;

            switch (objectType)
            {
                case "Course":
                    return GetObjectNameFromCourse(objectIdentifier.Value);
                case "Assessment":
                    return GetObjectNameFromAssessment(objectIdentifier.Value);
                default:
                    return null;
            }
        }

        private string GetObjectNameFromCourse(Guid objectIdentifier)
        {
            var course = ServiceLocator.CourseObjectSearch.GetCourse(objectIdentifier);

            if (course == null)
                return null;

            return course.CourseName;
        }

        private string GetObjectNameFromAssessment(Guid objectIdentifier)
        {
            var assessment = ServiceLocator.PageSearch.GetAssessmentPages(new Guid[] { objectIdentifier }).FirstOrDefault();

            if (assessment == null)
                return null;

            return assessment.FormName;
        }

        private static decimal GetItemPrice(TOrder order)
        {
            return order.Invoice.InvoiceItems.FirstOrDefault(x => x.ItemIdentifier == order.InvoiceItemIdentifier).ItemPrice;
        }

        private static int GetItemQuantity(TOrder order)
        {
            return order.Invoice.InvoiceItems.FirstOrDefault(x => x.ItemIdentifier == order.InvoiceItemIdentifier).ItemQuantity;
        }

        private static Guid GetProductIdentifier(TOrder order)
        {
            return order.Invoice.InvoiceItems.FirstOrDefault(x => x.ItemIdentifier == order.InvoiceItemIdentifier).ProductIdentifier;
        }
    }
}


