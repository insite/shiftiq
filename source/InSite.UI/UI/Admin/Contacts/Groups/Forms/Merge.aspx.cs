using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Contacts.Read;
using InSite.Application.Groups.Write;
using InSite.Application.Messages.Read;
using InSite.Application.Messages.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Contacts;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.Web.Data;
using InSite.Web.Security;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Contacts.Groups.Forms
{
    public partial class Merge : AdminBasePage
    {
        #region Constants

        private const string SearchUrl = "/ui/admin/contacts/groups/search";
        private const string EditUrl = "/ui/admin/contacts/groups/edit";

        #endregion

        #region Classes

        private enum MergeItemType { First, Second };

        private class ValueItem
        {
            public string Code { get; }
            public string Name { get; }
            public string Value1 { get; }
            public string Value2 { get; }
            public bool IsSame { get; }

            public ValueItem(string code, string name, string value1, string value2)
            {
                Code = code;
                Name = name;
                Value1 = value1;
                Value2 = value2;
                IsSame = string.Equals(Value1, Value2);
            }
        }

        private class MergeItem
        {
            public string Code { get; set; }
            public MergeItemType Type { get; set; }
        }

        private static class ValueCodes
        {
            public const string GroupName = "GroupName";
            public const string GroupDescription = "GroupDescription";
            public const string GroupCode = "GroupCode";
            public const string GroupCategory = "GroupCategory";
            public const string GroupStatus = "GroupStatus";
            public const string Capacity = "Capacity";
            public const string Region = "Region";
            public const string Office = "Office";
            public const string Phone = "Phone";
            public const string ShippingAddress = "ShippingAddress";
            public const string BillingAddress = "BillingAddress";
            public const string PhysicalAddress = "PhysicalAddress";
        }

        [Serializable]
        private class RowDataItem
        {
            public string Code { get; }
            public bool IsSame { get; }

            public RowDataItem(ValueItem item)
            {
                Code = item.Code;
                IsSame = item.IsSame;
            }
        }

        #endregion

        #region Properties

        private Guid? GroupIdentifier1
            => Guid.TryParse(Request["group1"], out var result) ? result : Guid.Empty;

        private Guid? GroupIdentifier2
            => Guid.TryParse(Request["group2"], out var result) ? result : Guid.Empty;

        private RowDataItem[] RowData
        {
            get => (RowDataItem[])ViewState[nameof(RowData)];
            set => ViewState[nameof(RowData)] = value;
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            MergeButton.Click += MergeButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var items = GetItems();
            if (items == null)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(Page);

            ValueRepeater1.DataSource = items;
            ValueRepeater1.DataBind();

            ValueRepeater2.DataSource = items;
            ValueRepeater2.DataBind();

            RowData = items.Select(x => new RowDataItem(x)).ToArray();

            CancelButton.NavigateUrl = SearchUrl;
        }

        #endregion

        #region Event handlers

        private void MergeButton_Click(object sender, EventArgs e)
        {
            MergeGroups();

            HttpResponseHelper.Redirect(EditUrl + $"?contact={GroupIdentifier1}&status=merged");
        }

        #endregion

        #region Methods (get data)

        private List<ValueItem> GetItems()
        {
            if (!GroupIdentifier1.HasValue || !GroupIdentifier2.HasValue || GroupIdentifier1.Value == GroupIdentifier2.Value)
                return null;

            var group1 = ServiceLocator.GroupSearch.GetGroup(GroupIdentifier1.Value, x => x.Addresses);
            if (group1 == null || group1.OrganizationIdentifier != Organization.Identifier || group1.GroupType != "Employer")
                return null;

            var group2 = ServiceLocator.GroupSearch.GetGroup(GroupIdentifier2.Value, x => x.Addresses);
            if (group2 == null || group2.OrganizationIdentifier != Organization.Identifier || group2.GroupType != "Employer")
                return null;

            var groupStatus1 = TCollectionItemCache.GetName(group1.GroupStatusItemIdentifier);
            var groupStatus2 = TCollectionItemCache.GetName(group2.GroupStatusItemIdentifier);

            var items = new List<ValueItem>();

            AddValue(ValueCodes.GroupName, "Group Name", group1.GroupName, group2.GroupName);
            AddValue(ValueCodes.GroupDescription, "Description", group1.GroupDescription, group2.GroupDescription);
            AddValue(ValueCodes.GroupCode, "Group Code", group1.GroupCode, group2.GroupCode);
            AddValue(ValueCodes.GroupCategory, "Group Category", group1.GroupCategory, group2.GroupCategory);
            AddValue(ValueCodes.GroupStatus, "Group Status", groupStatus1, groupStatus2);
            AddValue(ValueCodes.Capacity, "Capacity", group1.GroupCapacity.ToString(), group2.GroupCapacity.ToString());
            AddValue(ValueCodes.Region, "Region", group1.GroupRegion, group2.GroupRegion);
            AddValue(ValueCodes.Office, "Office", group1.GroupOffice, group2.GroupOffice);
            AddValue(ValueCodes.Phone, "Phone", group1.GroupPhone, group2.GroupPhone);
            AddValue(ValueCodes.ShippingAddress, "Shipping Address", GetAddress(group1, AddressType.Shipping), GetAddress(group2, AddressType.Shipping));
            AddValue(ValueCodes.BillingAddress, "Billing Address", GetAddress(group1, AddressType.Billing), GetAddress(group2, AddressType.Billing));
            AddValue(ValueCodes.PhysicalAddress, "Physical Address", GetAddress(group1, AddressType.Physical), GetAddress(group2, AddressType.Physical));

            return items;

            void AddValue(string code, string name, string value1, string value2)
            {
                if (value1.IsNotEmpty() || value2.IsNotEmpty())
                    items.Add(new ValueItem(code, name, value1, value2));
            }

            string GetAddress(QGroup group, AddressType type)
            {
                var typeText = type.ToString();
                var address = group.Addresses.FirstOrDefault(x => string.Equals(x.AddressType, typeText, StringComparison.OrdinalIgnoreCase));

                if (address == null)
                    return null;

                var html = LocationHelper.ToHtml(address.Street1, address.Street2, address.City, address.Province, address.PostalCode, address.Country, null, null);

                if (address.Description.IsNotEmpty())
                    html = address.Description + "<br/>" + html;

                return html;
            }
        }

        #endregion

        #region Methods (merge)

        private void MergeGroups()
        {
            var g1 = GroupIdentifier1.HasValue
                ? ServiceLocator.GroupSearch.GetGroup(GroupIdentifier1.Value, x => x.Addresses, x => x.ConnectionChildren, x => x.ConnectionParents)
                : null;

            var g2 = GroupIdentifier2.HasValue
                ? ServiceLocator.GroupSearch.GetGroup(GroupIdentifier2.Value, x => x.Addresses, x => x.ConnectionChildren, x => x.ConnectionParents)
                : null;

            if (g1 == null || g1.OrganizationIdentifier != Organization.Identifier || g2 == null || g2.OrganizationIdentifier != Organization.Identifier)
            {
                HttpResponseHelper.Redirect(SearchUrl);
                return;
            }

            var mergeItems = GetMergeItems();
            var commands = new List<Command>();

            MergeSubscriberGroups(g1, g2, commands);
            MergeFields(g1, g2, mergeItems, commands);
            MergeConnections(g1, g2, commands);

            ServiceLocator.SendCommands(commands);

            MergeUsers(g1, g2);
            MergeEmployerGroup(g1, g2);
            MergeGroupToolkits(g1, g2);
            MergeGroupActions(g1, g2);
            MergePrivacyGroups(g1, g2);
            MergeWebPageGroups(g1, g2);

            GroupHelper.Delete(new Commander(), ServiceLocator.GroupSearch, ServiceLocator.RegistrationSearch, ServiceLocator.PersonSearch, g2.GroupIdentifier);
        }

        private void MergeFields(QGroup g1, QGroup g2, List<MergeItem> mergeItems, List<Command> commands)
        {
            if (mergeItems.Count == 0)
                return;

            var mergers = GetMergers();

            foreach (var mergeItem in mergeItems)
            {
                if (mergeItem.Type == MergeItemType.Second && mergers.TryGetValue(mergeItem.Code, out var merger))
                    merger.Invoke(g1, g2);
            }

            var id = g1.GroupIdentifier;

            commands.Add(new RenameGroup(id, g1.GroupType, g1.GroupName));
            commands.Add(new DescribeGroup(id, g1.GroupCategory, g1.GroupCode, g1.GroupDescription, g1.GroupLabel));
            commands.Add(new ModifyGroupStatus(id, g1.GroupStatusItemIdentifier));
            commands.Add(new ChangeGroupEmail(id, g1.GroupEmail));
            commands.Add(new ChangeGroupWebSiteUrl(id, g1.GroupWebSiteUrl));
            commands.Add(new ChangeGroupCapacity(id, g1.GroupCapacity));
            commands.Add(new ChangeGroupLocation(id, g1.GroupOffice, g1.GroupRegion, g1.ShippingPreference, g1.GroupWebSiteUrl));
            commands.Add(new ChangeGroupPhone(id, g1.GroupPhone));

            foreach (var source in g1.Addresses)
            {
                var address = new GroupAddress
                {
                    City = source.City,
                    Country = source.Country,
                    Description = source.Description,
                    PostalCode = source.PostalCode,
                    Province = source.Province,
                    Street1 = source.Street1,
                    Street2 = source.Street2
                };

                var type = source.AddressType.ToEnum<AddressType>();

                commands.Add(new ChangeGroupAddress(id, type, address));
            }
        }

        private void MergeUsers(QGroup g1, QGroup g2)
        {
            var canAddMembership = MembershipPermissionHelper.CanModifyMembership(g1);

            var memberships1 = MembershipSearch.Select(x => x.GroupIdentifier == g1.GroupIdentifier);
            var memberships2 = MembershipSearch.Select(x => x.GroupIdentifier == g2.GroupIdentifier);

            foreach (var membership2 in memberships2)
            {
                var membership1 = memberships1.FirstOrDefault(x => x.UserIdentifier == membership2.UserIdentifier);

                MembershipStore.Delete(membership2);

                if (canAddMembership &&
                    (membership1 == null || membership1.MembershipType.IsEmpty() && membership2.MembershipType.IsNotEmpty())
                    )
                {
                    membership2.GroupIdentifier = g1.GroupIdentifier;
                    MembershipHelper.Save(membership2, false, false);
                }
            }
        }

        private void MergeConnections(QGroup g1, QGroup g2, List<Command> commands)
        {
            var containments1 = g1.ConnectionParents
                .Union(g1.ConnectionChildren)
                .ToList();

            var containments2 = g2.ConnectionParents
                .Union(g2.ConnectionChildren)
                .ToList();

            foreach (var containment in containments2)
            {
                commands.Add(new DisconnectGroup(containment.ChildGroupIdentifier, containment.ParentGroupIdentifier));

                if (containment.ParentGroupIdentifier == g2.GroupIdentifier)
                {
                    if (!containments1.Any(x => x.ChildGroupIdentifier == containment.ChildGroupIdentifier))
                    {
                        containment.ParentGroupIdentifier = g1.GroupIdentifier;

                        commands.Add(new ConnectGroup(containment.ChildGroupIdentifier, containment.ParentGroupIdentifier));
                    }
                }
                else
                {
                    if (!containments1.Any(x => x.ParentGroupIdentifier == containment.ParentGroupIdentifier))
                    {
                        containment.ChildGroupIdentifier = g1.GroupIdentifier;

                        commands.Add(new ConnectGroup(containment.ChildGroupIdentifier, containment.ParentGroupIdentifier));
                    }
                }
            }
        }

        private void MergeEmployerGroup(QGroup g1, QGroup g2)
        {
            var persons2 = ServiceLocator.PersonSearch.GetPersonsByEmployer(g2.GroupIdentifier);
            foreach (var person in persons2)
            {
                person.EmployerGroupIdentifier = g1.GroupIdentifier;
                PersonStore.Update(person);
            }
        }

        private void MergeGroupToolkits(QGroup g1, QGroup g2)
        {
            TGroupPermissionStore.Copy(g2.GroupIdentifier, g1.GroupIdentifier);

            var items2 = TGroupPermissionSearch.SelectByGroup(g2.GroupIdentifier);
            foreach (var item in items2)
                TGroupPermissionStore.Delete(item.PermissionIdentifier);
        }

        private void MergeGroupActions(QGroup g1, QGroup g2)
        {
            var items1 = TGroupPermissionSearch.Bind(x => x, x => x.GroupIdentifier == g1.GroupIdentifier);
            var items2 = TGroupPermissionSearch.Bind(x => x, x => x.GroupIdentifier == g2.GroupIdentifier);
            foreach (var item in items2)
            {
                TGroupPermissionStore.Delete(item.GroupIdentifier, item.ObjectIdentifier);

                if (!items1.Any(x => x.ObjectIdentifier == item.ObjectIdentifier))
                {
                    item.GroupIdentifier = g1.GroupIdentifier;
                    TGroupPermissionStore.Insert(item);
                }
            }
        }

        private void MergePrivacyGroups(QGroup g1, QGroup g2)
        {
            var items1 = ServiceLocator.ContentSearch.SelectPrivacyGroup(x => x.GroupIdentifier == g1.GroupIdentifier);
            var items2 = ServiceLocator.ContentSearch.SelectPrivacyGroup(x => x.GroupIdentifier == g2.GroupIdentifier);
            foreach (var item in items2)
            {
                ServiceLocator.ContentStore.DeletePrivacyGroup(item.PermissionIdentifier);

                if (!items1.Any(x => x.ObjectIdentifier == item.ObjectIdentifier))
                    ServiceLocator.ContentStore.InsertPrivacyGroup(item.ObjectIdentifier, item.ObjectType, g1.GroupIdentifier);
            }
        }

        private void MergeWebPageGroups(QGroup g1, QGroup g2)
        {
            var items1 = TGroupPermissionSearch.Select(x => x.GroupIdentifier == g1.GroupIdentifier);
            var items2 = TGroupPermissionSearch.Select(x => x.GroupIdentifier == g2.GroupIdentifier);
            foreach (var item in items2)
            {
                TGroupPermissionStore.Delete(item.GroupIdentifier, item.ObjectIdentifier);

                if (!items1.Any(x => x.ObjectIdentifier == item.ObjectIdentifier))
                {
                    item.GroupIdentifier = g1.GroupIdentifier;
                    TGroupPermissionStore.Insert(item);
                }
            }
        }

        private void MergeSubscriberGroups(QGroup g1, QGroup g2, List<Command> commands)
        {
            var items1 = ServiceLocator.MessageSearch.GetSubscriberGroups(new QSubscriberGroupFilter { SubscriberIdentifier = g1.GroupIdentifier });
            var items2 = ServiceLocator.MessageSearch.GetSubscriberGroups(new QSubscriberGroupFilter { SubscriberIdentifier = g2.GroupIdentifier });

            foreach (var item in items1)
            {
                commands.Add(new RemoveMessageSubscriber(item.MessageIdentifier, item.GroupIdentifier, true));

                if (!items1.Any(x => x.MessageIdentifier == item.MessageIdentifier))
                    commands.Add(new AddSubscriber(item.MessageIdentifier, g1.GroupIdentifier, null, false, true));
            }
        }

        private List<MergeItem> GetMergeItems()
        {
            if (ValueRepeater1.Items.Count != ValueRepeater2.Items.Count || ValueRepeater1.Items.Count != RowData.Length)
                throw ApplicationError.Create("Invalid control state");

            var result = new List<MergeItem>();

            for (var i = 0; i < RowData.Length; i++)
            {
                var rowData = RowData[i];
                var selected1 = (IRadioButton)ValueRepeater1.Items[i].FindControl("Selected");
                var selected2 = (IRadioButton)ValueRepeater2.Items[i].FindControl("Selected");

                if (rowData.IsSame)
                    continue;

                var item = new MergeItem
                {
                    Code = rowData.Code,
                    Type = selected1.Checked
                        ? MergeItemType.First
                        : selected2.Checked
                            ? MergeItemType.Second
                            : throw ApplicationError.Create("Invalid selection value: " + i)
                };

                result.Add(item);
            }

            return result;
        }

        private Dictionary<string, Action<QGroup, QGroup>> GetMergers()
        {
            return new Dictionary<string, Action<QGroup, QGroup>>(StringComparer.OrdinalIgnoreCase)
            {
                { ValueCodes.GroupName, (g1, g2) => g1.GroupName = g2.GroupName },
                { ValueCodes.GroupDescription, (g1, g2) => g1.GroupDescription = g2.GroupDescription },
                { ValueCodes.GroupCode, (g1, g2) => g1.GroupCode = g2.GroupCode },
                { ValueCodes.GroupCategory, (g1, g2) => g1.GroupCategory = g2.GroupCategory },
                { ValueCodes.GroupStatus, (g1, g2) => g1.GroupStatusItemIdentifier = g2.GroupStatusItemIdentifier },
                { ValueCodes.Capacity, (g1, g2) => g1.GroupCapacity = g2.GroupCapacity },
                { ValueCodes.Region, (g1, g2) => g1.GroupRegion = g2.GroupRegion },
                { ValueCodes.Office, (g1, g2) => g1.GroupOffice = g2.GroupOffice },
                { ValueCodes.Phone, (g1, g2) => g1.GroupPhone = g2.GroupPhone },
                { ValueCodes.ShippingAddress, (g1, g2) => CopyAddress(g1, g2, AddressType.Shipping) },
                { ValueCodes.BillingAddress, (g1, g2) => CopyAddress(g1, g2, AddressType.Billing) },
                { ValueCodes.PhysicalAddress, (g1, g2) => CopyAddress(g1, g2, AddressType.Physical) }
            };

            void CopyAddress(QGroup g1, QGroup g2, AddressType type)
            {
                var typeText = type.ToString();
                var a1 = g1.Addresses.FirstOrDefault(x => string.Equals(x.AddressType, typeText, StringComparison.OrdinalIgnoreCase));

                if (a1 == null)
                    g1.Addresses.Add(a1 = new QGroupAddress { AddressIdentifier = UniqueIdentifier.Create(), AddressType = type.ToString() });

                var a2 = g2.Addresses.FirstOrDefault(x => string.Equals(x.AddressType, typeText, StringComparison.OrdinalIgnoreCase));

                a1.Description = a2?.Description;
                a1.Street1 = a2?.Street1;
                a1.Street2 = a2?.Street2;
                a1.City = a2?.City;
                a1.Province = a2?.Province;
                a1.PostalCode = a2?.PostalCode;
                a1.Country = a2?.Country;
            }
        }

        #endregion
    }
}