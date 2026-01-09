using System;
using System.Text;
using System.Web.UI;

using InSite.Application.Contacts.Read;
using InSite.Application.Events.Read;
using InSite.Application.Registrations.Read;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Contacts.Groups.Controls
{
    public partial class GroupInfo : UserControl
    {
        public void BindGroup(QGroup group)
        {
            BindGroup(group.GroupType, group.GroupName, group.GroupLabel, group.GroupCode, group.GroupIdentifier);
        }

        public void BindGroup(VGroupDetail group)
        {
            BindGroup(group.GroupType, group.GroupName, group.GroupLabel, group.GroupCode, group.GroupIdentifier);
        }

        private void BindGroup(string groupType, string groupName, string groupTag, string groupCode, Guid groupId)
        {
            GroupType.Text = groupType;

            GroupName.Text = groupName;

            if (groupTag.HasValue())
                GroupTag.Text = $"<span class='badge bg-primary fs-sm'>{groupTag}</span>";

            if (groupCode.HasValue())
                GroupCode.Text = $"<span class='badge bg-info fs-sm'>{groupCode}</span>";

            BindReferences(groupId);
        }

        protected int DownstreamRelationshipCount
        {
            get { return (ViewState[nameof(DownstreamRelationshipCount)] as int?) ?? 0; }
            set { ViewState[nameof(DownstreamRelationshipCount)] = value; }
        }

        protected int EventVenueCount
        {
            get { return (ViewState[nameof(EventVenueCount)] as int?) ?? 0; }
            set { ViewState[nameof(EventVenueCount)] = value; }
        }

        protected int UpstreamRelationshipCount
        {
            get { return (ViewState[nameof(UpstreamRelationshipCount)] as int?) ?? 0; }
            set { ViewState[nameof(UpstreamRelationshipCount)] = value; }
        }

        private void BindReferences(Guid groupId)
        {
            var sb = new StringBuilder();

            // Contained Contacts
            var contactsCount = ServiceLocator.PersonSearch.CountPersons(new QPersonFilter { EmployerGroupIdentifier = groupId });
            if (contactsCount > 0)
                sb.AppendLine("- " + Shift.Common.Humanizer.ToQuantity(contactsCount, "contact"));

            // Upstream Relationships
            UpstreamRelationshipCount = ServiceLocator.GroupSearch.CountParentConnections(groupId);
            if (UpstreamRelationshipCount > 0)
                sb.AppendLine("- " + Shift.Common.Humanizer.ToQuantity(contactsCount, "upstream relationship"));

            // Downstream Relationships
            DownstreamRelationshipCount = ServiceLocator.GroupSearch.CountChildConnections(groupId);
            if (DownstreamRelationshipCount > 0)
                sb.AppendLine("- " + Shift.Common.Humanizer.ToQuantity(DownstreamRelationshipCount, "downstream relationship"));

            // Permissions
            var permissionsCount = TGroupPermissionSearch.Count(new TGroupActionFilter { GroupIdentifier = groupId });
            if (permissionsCount > 0)
                sb.AppendLine("- " + Shift.Common.Humanizer.ToQuantity(permissionsCount, "permissions"));

            // Billing Customers
            var customersCount = ServiceLocator.RegistrationSearch.CountRegistrations(new QRegistrationFilter { RegistrationCustomerIdentifier = groupId });
            if (customersCount > 0)
                sb.AppendLine("- " + Shift.Common.Humanizer.ToQuantity(customersCount, "billing customer"));

            // Employers
            var employersCount = ServiceLocator.RegistrationSearch.CountRegistrations(new QRegistrationFilter { RegistrationEmployerIdentifier = groupId });
            if (employersCount > 0)
                sb.AppendLine("- " + Shift.Common.Humanizer.ToQuantity(employersCount, "employer"));

            // Events
            EventVenueCount = ServiceLocator.EventSearch.CountEvents(new QEventFilter { VenueLocationIdentifier = new[] { groupId } });
            if (EventVenueCount > 0)
                sb.AppendLine("- " + Shift.Common.Humanizer.ToQuantity(EventVenueCount, "event venue"));

            var markdown = sb.ToString();

            GroupReferencesDefn.InnerHtml = Markdown.ToHtml(markdown);

            GroupReferencesTerm.Visible = markdown.HasValue();

            GroupReferencesDefn.Visible = markdown.HasValue();
        }

        internal int CountDownstreamRelationships()
        {
            return DownstreamRelationshipCount;
        }

        internal int CountEventVenues()
        {
            return EventVenueCount;
        }

        internal int CountUpstreamRelationships()
        {
            return UpstreamRelationshipCount;
        }
    }
}