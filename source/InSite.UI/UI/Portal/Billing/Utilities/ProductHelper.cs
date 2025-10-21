﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;

using Common.Timeline.Changes;
using Common.Timeline.Commands;

using InSite.Admin.Invoices.Controls;
using InSite.Application.Contacts.Read;
using InSite.Application.Events.Read;
using InSite.Application.Events.Write;
using InSite.Application.Gradebooks.Write;
using InSite.Application.Invoices.Read;
using InSite.Domain.Events;
using InSite.Domain.Messages;
using InSite.Persistence;
using InSite.Web.Data;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Billing.Utilities
{
    public static class ProductHelper
    {
        public const string EventAttendeeRole = "Instructor";
        public const string EventStatus = "Active";
        public const string EventDurationUnit = "Day";
        public const string GroupContainerType = "Event";

        public static void ProcessOrder(Guid invoice, Guid? customerGroup, Guid? classEventVenueGroup)
        {
            var invoiceObj = ServiceLocator.InvoiceSearch.GetInvoice(invoice);

            if (invoiceObj == null)
                return;

            var person = ServiceLocator.PersonSearch.GetPerson(invoiceObj.CustomerIdentifier, invoiceObj.OrganizationIdentifier, x => x.EmployerGroup, x => x.User);

            if (person == null)
                return;

            SendPaidInvoiceInformation(invoiceObj, person);
            var classEventIdList = CreateClassEvent(invoiceObj, person, classEventVenueGroup);
            AssignPermissions(person.UserIdentifier, customerGroup);
            SendClassScheduledAlert(invoiceObj, person, classEventIdList);
        }

        public static void SendPaidInvoiceInformation(Guid invoiceId)
        {
            var invoice = ServiceLocator.InvoiceSearch.GetInvoice(invoiceId);

            if (invoice == null)
                return;

            var person = ServiceLocator.PersonSearch.GetPerson(invoice.CustomerIdentifier, invoice.OrganizationIdentifier, x => x.EmployerGroup, x => x.User);

            if (person == null)
                return;

            SendPaidInvoiceInformation(invoice, person);
        }

        private static void SendPaidInvoiceInformation(VInvoice invoice, QPerson person)
        {
            var invoiceFile = CreateInvoice(invoice.InvoiceIdentifier);
            var attachments = invoice != null ? new string[] { invoiceFile } : null;

            ServiceLocator.AlertMailer.Send(person.OrganizationIdentifier, person.UserIdentifier, new AlertInvoicePaid
            {
                InvoicePaidProperties = BuildVariableList()
            }, attachments);

            StringDictionary BuildVariableList()
            {
                var dict = new StringDictionary();

                var properties = typeof(VInvoice).GetProperties();
                foreach (var property in properties)
                {
                    var value = property.GetValue(invoice);
                    if (value == null) value = string.Empty;

                    if (value.GetType().Equals(typeof(DateTimeOffset)))
                        dict.Add(property.Name, TimeZones.Format((DateTimeOffset)value, person.User.TimeZone));
                    else
                        dict.Add(property.Name, value.ToString());
                }
                return dict;
            }
        }

        public static string GetInvoiceStatus(string status)
            => status.IsEmpty() ? string.Empty : status.ToEnum<InvoiceStatusType>().GetDescription();


        private static void SendClassScheduledAlert(VInvoice invoice, QPerson person, List<(Guid, string)> classEventIdList)
        {
            if (classEventIdList == null)
                return;

            foreach (var (id, value) in classEventIdList)
            {
                var alertVariables = BuildVariableList();
                alertVariables.Add("ClassEventUrl", GetClassEventUrl(id));
                alertVariables.Add("ClassEventName", value);

                ServiceLocator.AlertMailer.Send(invoice.OrganizationIdentifier, person.UserIdentifier, new Domain.Messages.AlertClassScheduled
                {
                    ClassScheduledProperties = alertVariables
                });
            }

            StringDictionary BuildVariableList()
            {
                var dict = new StringDictionary();

                var properties = typeof(VInvoice).GetProperties();

                foreach (var property in properties)
                {
                    var value = property.GetValue(invoice);
                    if (value == null) value = string.Empty;

                    if (value.GetType().Equals(typeof(DateTimeOffset)))
                        dict.Add(property.Name, TimeZones.Format((DateTimeOffset)value, person.User.TimeZone));
                    else
                        dict.Add(property.Name, value.ToString());
                }
                return dict;
            }
        }

        private static void AssignPermissions(Guid userIdentifier, Guid? customerGroup)
        {
            if (!customerGroup.HasValue)
                return;

            MembershipHelper.Save(new Membership
            {
                GroupIdentifier = customerGroup.Value,
                UserIdentifier = userIdentifier,
                Assigned = DateTimeOffset.UtcNow
            });
        }

        public static List<(Guid, Guid, Guid)> CreateClassEvent(Guid invoiceId, Guid? classEventVenueGroup)
        {
            var invoice = ServiceLocator.InvoiceSearch.GetInvoice(invoiceId);

            if (invoice == null)
                return null;

            var person = ServiceLocator.PersonSearch.GetPerson(invoice.CustomerIdentifier, invoice.OrganizationIdentifier, x => x.EmployerGroup, x => x.User);

            if (person == null)
                return null;

            var invoiceItems = ServiceLocator.InvoiceSearch.GetInvoiceItems(invoice.InvoiceIdentifier, x => x.Product);

            if (invoiceItems == null)
                return null;

            var results = new List<(Guid, Guid, Guid)>();

            foreach (var item in invoiceItems)
            {
                if (item.Product.ProductType != "Online Course" || !item.Product.ObjectIdentifier.HasValue)
                    continue;

                string className = GetClassEventName(person, item);
                var classMaxCapacity = item.ItemQuantity;

                var aggregate = new EventAggregate { AggregateIdentifier = UniqueIdentifier.Create() };
                var changes = CreateChanges(invoice.OrganizationIdentifier, invoice.CustomerIdentifier, className, classMaxCapacity, classEventVenueGroup);

                for (var i = 0; i < changes.Count; i++)
                    SetupChangeObject(invoice, aggregate, changes, i);

                ServiceLocator.ChangeStore.Save(aggregate, changes);

                for (int i = 0; i < changes.Count; i++)
                    ServiceLocator.ChangeQueue.Publish(changes[i]);

                var grants = new List<Guid>();
                if (person.EmployerGroupIdentifier.HasValue)
                {
                    grants.Add(person.EmployerGroupIdentifier.Value);
                    TGroupPermissionStore.Update(
                        DateTimeOffset.UtcNow,
                        invoice.CustomerIdentifier,
                        aggregate.AggregateIdentifier,
                        GroupContainerType,
                        grants,
                        new List<Guid>());
                }

                AttachCourseGradebookToClassEvent(item.Product.ObjectIdentifier.Value, aggregate.AggregateIdentifier);
                CreateClassSeats(aggregate.AggregateIdentifier, item.ItemDescription);
                AttachLearnerRegistrationGroupToClassEvent(aggregate.AggregateIdentifier, invoice.OrganizationIdentifier, item.ItemDescription);

                results.Add((item.ProductIdentifier, aggregate.AggregateIdentifier, item.Product.ObjectIdentifier.Value));
            }

            return results;
        }


        public static List<(Guid, string)> CreateClassEvent(VInvoice invoice, QPerson person, Guid? classEventVenueGroup)
        {
            var invoiceItems = ServiceLocator.InvoiceSearch.GetInvoiceItems(invoice.InvoiceIdentifier, x => x.Product);

            if (invoiceItems == null)
                return null;

            var results = new List<(Guid, string)>();

            foreach (var item in invoiceItems)
            {
                if (item.Product.ProductType != "Online Course" || !item.Product.ObjectIdentifier.HasValue)
                    continue;

                string className = GetClassEventName(person, item);
                var classMaxCapacity = item.ItemQuantity;

                var aggregate = new EventAggregate { AggregateIdentifier = UniqueIdentifier.Create() };
                var changes = CreateChanges(invoice.OrganizationIdentifier, invoice.CustomerIdentifier, className, classMaxCapacity, classEventVenueGroup);

                for (var i = 0; i < changes.Count; i++)
                    SetupChangeObject(invoice, aggregate, changes, i);

                ServiceLocator.ChangeStore.Save(aggregate, changes);

                for (int i = 0; i < changes.Count; i++)
                    ServiceLocator.ChangeQueue.Publish(changes[i]);

                var grants = new List<Guid>();
                if (person.EmployerGroupIdentifier.HasValue)
                {
                    grants.Add(person.EmployerGroupIdentifier.Value);
                    TGroupPermissionStore.Update(
                        DateTimeOffset.UtcNow,
                        invoice.CustomerIdentifier,
                        aggregate.AggregateIdentifier,
                        GroupContainerType,
                        grants,
                        new List<Guid>());
                }

                AttachCourseGradebookToClassEvent(item.Product.ObjectIdentifier.Value, aggregate.AggregateIdentifier);
                CreateClassSeats(aggregate.AggregateIdentifier, item.ItemDescription);
                CreateOrder(item, person.UserIdentifier, aggregate.AggregateIdentifier, invoice.OrganizationIdentifier, className);
                AttachLearnerRegistrationGroupToClassEvent(aggregate.AggregateIdentifier, invoice.OrganizationIdentifier, item.ItemDescription);

                results.Add((aggregate.AggregateIdentifier, className));
            }

            return results;
        }


        private static void CreateClassSeats(Guid eventIdentifier, string itemDescription)
        {
            var seat = new QSeat();

            var configuration = new SeatConfiguration();
            configuration.Prices = new List<SeatConfiguration.Price> { new SeatConfiguration.Price { Amount = 0 } };

            var content = new ContentSeat();
            content.Title.Default = itemDescription;
            content.Description.Default = itemDescription;
            content.AddOrGet("Agreement").Default = string.Empty;

            var seatId = UniqueIdentifier.Create();

            var command = new AddSeat(eventIdentifier, seatId, JsonConvert.SerializeObject(configuration), content.Serialize(), true, false, null, itemDescription);
            ServiceLocator.SendCommand(command);
        }

        private static void AttachLearnerRegistrationGroupToClassEvent(Guid eventId, Guid orgId, string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
                return;

            var group = ServiceLocator.GroupSearch
                .GetGroups(new QGroupFilter() { OrganizationIdentifier = orgId, GroupName = groupName })
                .FirstOrDefault();

            if (group == null)
                return;

            ServiceLocator.SendCommand(new ModifyLearnerRegistrationGroup(eventId, group.GroupIdentifier));
        }

        private static void CreateOrder(QInvoiceItem item, Guid userIdentifier, Guid classEventIdentifier, Guid organizationIdentifier, string className)
        {
            var now = DateTimeOffset.UtcNow;
            var currentUserId = CurrentSessionState.Identity.User.Identifier;

            var order = new TOrder()
            {
                CustomerUserIdentifier = userIdentifier,
                ProductUrl = GetClassEventUrl(classEventIdentifier),
                ProductIdentifier = classEventIdentifier,
                ProductName = className,
                ProductType = "Class",
                InvoiceItemIdentifier = item.ItemIdentifier,
                InvoiceIdentifier = item.InvoiceIdentifier,
                OrganizationIdentifier = organizationIdentifier,
                OrderCompleted = now,
                OrderIdentifier = UniqueIdentifier.Create(),
                Created = now,
                CreatedBy = currentUserId,
                Modified = now,
                ModifiedBy = currentUserId,
            };

            ServiceLocator.InvoiceStore.InsertOrder(order);
        }

        private static void AttachCourseGradebookToClassEvent(Guid courseId, Guid eventId)
        {
            var course = ServiceLocator.CourseObjectSearch.GetCourse(courseId);
            if (course == null || !course.GradebookIdentifier.HasValue)
                return;

            var gradebookId = course.GradebookIdentifier.Value;
            var gradebook = ServiceLocator.RecordSearch.GetGradebook(gradebookId);

            var commands = new List<Command>();

            if (gradebook.IsLocked)
                commands.Add(new UnlockGradebook(gradebookId));

            commands.Add(new AddGradebookEvent(gradebookId, eventId, false));

            if (gradebook.AchievementIdentifier.HasValue)
                commands.Add(new ChangeEventAchievement(eventId, gradebook.AchievementIdentifier.Value));

            if (gradebook.IsLocked)
                commands.Add(new LockGradebook(gradebookId));

            ServiceLocator.SendCommands(commands);
        }

        private static List<IChange> CreateChanges(Guid organizationIdentifier, Guid customerIdentifier, string eventTitle, int maxCapacity, Guid? classEventVenueGroup)
        {
            var number = Sequence.Increment(organizationIdentifier, SequenceType.Event);
            var start = DateTimeOffset.Now;
            var end = start.AddYears(1);
            double duration = CalculateDuration(ref start, ref end);
            var changes = new List<IChange>();

            changes.Add(new ClassScheduled2(organizationIdentifier, eventTitle, EventStatus, number, start, end, (int)duration, EventDurationUnit, null));

            if (classEventVenueGroup.HasValue)
            {
                var venue = ServiceLocator.GroupSearch.GetGroup(classEventVenueGroup.Value);

                if (venue != null)
                    changes.Add(new EventVenueChanged2(venue.GroupIdentifier, venue.GroupIdentifier, ""));
            }

            changes.Add(new EventRegistrationWithLinkAllowed());
            changes.Add(new CapacityAdjusted(1, maxCapacity, ToggleType.Disabled));
            changes.Add(new EventAttendeeAdded(customerIdentifier, EventAttendeeRole));
            changes.Add(new EventPublished(start, end));

            return changes;
        }

        #region Helper Methods

        private static string GetClassEventUrl(Guid id)
        {
            return $"{PathHelper.GetAbsoluteUrl("/ui/portal/events/classes/outline")}?event={id}";
        }

        private static double CalculateDuration(ref DateTimeOffset start, ref DateTimeOffset end)
        {
            return (new DateTime(end.UtcDateTime.Year, end.UtcDateTime.Month, end.UtcDateTime.Day) - new DateTime(start.UtcDateTime.Year, start.UtcDateTime.Month, start.UtcDateTime.Day)).TotalDays + 1;
        }

        private static string GetClassEventName(QPerson person, QInvoiceItem item)
        {
            return $"{item.ItemDescription} - {(person.EmployerGroupIdentifier.HasValue ? person.EmployerGroup.GroupName : person.FullName)}";
        }

        private static void SetupChangeObject(VInvoice invoice, EventAggregate aggregate, List<IChange> changes, int i)
        {
            var change = changes[i];
            change.AggregateIdentifier = aggregate.AggregateIdentifier;
            change.AggregateVersion = i + 1;
            change.OriginOrganization = invoice.OrganizationIdentifier;
            change.OriginUser = invoice.CustomerIdentifier;
            change.ChangeTime = DateTimeOffset.Now;
        }

        private static string CreateInvoice(Guid invoiceIdentifier)
        {
            var (data, fileName) = InvoiceEventReport.PrintByInvoice(invoiceIdentifier, InvoiceEventReportType.Invoice);

            string folder = ServiceLocator.FilePaths.GetPhysicalPathToShareFolder("Files", "Temp", "Invoices", Guid.NewGuid().ToString());

            var path = Path.Combine(folder, "Invoice.pdf");

            File.WriteAllBytes(path, data);

            return path;
        }

        #endregion
    }
}