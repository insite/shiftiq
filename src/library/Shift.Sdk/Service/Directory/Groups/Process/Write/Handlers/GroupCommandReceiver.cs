using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Contacts.Read;
using InSite.Application.Groups.Write;
using InSite.Domain.Contacts;

using Shift.Constant;

namespace InSite.Application.Contacts.Write
{
    public class GroupCommandReceiver
    {
        private readonly ICommandQueue _commander;
        private readonly IChangeQueue _publisher;
        private readonly IChangeRepository _repository;
        private readonly IGroupSearch _groupSearch;

        public GroupCommandReceiver(ICommandQueue commander, IChangeQueue publisher, IChangeRepository repository, IGroupSearch groupSearch)
        {
            _commander = commander;
            _publisher = publisher;
            _repository = repository;
            _groupSearch = groupSearch;

            commander.Subscribe<ChangeGroupAddress>(Handle);
            commander.Subscribe<ChangeGroupCapacity>(Handle);
            commander.Subscribe<ChangeGroupImage>(Handle);
            commander.Subscribe<ChangeGroupIndustry>(Handle);
            commander.Subscribe<ChangeGroupLocation>(Handle);
            commander.Subscribe<ChangeGroupParent>(Handle);
            commander.Subscribe<ChangeGroupPhone>(Handle);
            commander.Subscribe<ChangeGroupSettings>(Handle);
            commander.Subscribe<ModifyAllowJoinGroupUsingLink>(Handle);
            commander.Subscribe<ChangeGroupSize>(Handle);
            commander.Subscribe<ChangeGroupSurvey>(Handle);
            commander.Subscribe<ConfigureGroupNotifications>(Handle);
            commander.Subscribe<ConnectGroup>(Handle);
            commander.Subscribe<CreateGroup>(Handle);
            commander.Subscribe<DeleteGroup>(Handle);
            commander.Subscribe<DescribeGroup>(Handle);
            commander.Subscribe<DisconnectGroup>(Handle);
            commander.Subscribe<RenameGroup>(Handle);
            commander.Subscribe<ChangeGroupEmail>(Handle);
            commander.Subscribe<ChangeGroupWebSiteUrl>(Handle);
            commander.Subscribe<ChangeGroupSocialMediaUrl>(Handle);
            commander.Subscribe<AddGroupTag>(Handle);
            commander.Subscribe<RemoveGroupTag>(Handle);
            commander.Subscribe<ChangeGroupExpiry>(Handle);
            commander.Subscribe<ChangeGroupLifetime>(Handle);
            commander.Subscribe<ExpireGroup>(Handle);
            commander.Subscribe<ModifyGroupMembershipProduct>(Handle);
            commander.Subscribe<ModifyGroupStatus>(Handle);
            commander.Subscribe<ModifyGroupOnlyOperatorCanAddUser>(Handle);
        }

        private void Commit(GroupAggregate aggregate, ICommand c)
        {
            aggregate.Identify(c.OriginOrganization, c.OriginUser);
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
            {
                change.AggregateState = aggregate.State;
                _publisher.Publish(change);
            }
        }

        public void Handle(ChangeGroupAddress c)
        {
            _repository.LockAndRun<GroupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                var address = aggregate.Data.FindAddress(c.Type);

                if (address == null && c.Address != null
                    || address != null && (
                        c.Address == null
                        || address.City != c.Address.City
                        || address.Country != c.Address.Country
                        || address.Description != c.Address.Description
                        || address.PostalCode != c.Address.PostalCode
                        || address.Province != c.Address.Province
                        || address.Street1 != c.Address.Street1
                        || address.Street2 != c.Address.Street2
                        )
                    )
                {
                    aggregate.ChangeGroupAddress(c.Type, c.Address);

                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(ChangeGroupCapacity c)
        {
            _repository.LockAndRun<GroupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.Capacity != c.Capacity)
                {
                    aggregate.ChangeGroupCapacity(c.Capacity);

                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(ChangeGroupImage c)
        {
            _repository.LockAndRun<GroupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.Image != c.Image)
                {
                    aggregate.ChangeGroupImage(c.Image);

                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(ChangeGroupIndustry c)
        {
            _repository.LockAndRun<GroupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.Industry != c.Industry
                    || aggregate.Data.IndustryComment != c.IndustryComment
                    )
                {
                    aggregate.ChangeGroupIndustry(c.Industry, c.IndustryComment);

                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(ChangeGroupLocation c)
        {
            _repository.LockAndRun<GroupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                var data = aggregate.Data;
                if (data.Office != c.Office
                    || data.Region != c.Region
                    || data.ShippingPreference != c.ShippingPreference
                    || data.WebSiteUrl != c.WebSiteUrl
                    )
                {
                    aggregate.ChangeGroupLocation(c.Office, c.Region, c.ShippingPreference, c.WebSiteUrl);

                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(ChangeGroupParent c)
        {
            _repository.LockAndRun<GroupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.Parent != c.Parent)
                {
                    aggregate.ChangeGroupParent(c.Parent);

                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(ChangeGroupPhone c)
        {
            _repository.LockAndRun<GroupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                var data = aggregate.Data;
                if (data.Phone != c.Phone)
                {
                    aggregate.ChangeGroupPhone(c.Phone);

                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(ChangeGroupSettings c)
        {
            _repository.LockAndRun<GroupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                var data = aggregate.Data;
                if (data.AddNewUsersAutomatically != c.AddNewUsersAutomatically
                    || data.AllowSelfSubscription != c.AllowSelfSubscription
                    )
                {
                    aggregate.ChangeGroupSettings(c.AddNewUsersAutomatically, c.AllowSelfSubscription);

                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(ModifyAllowJoinGroupUsingLink c)
        {
            _repository.LockAndRun<GroupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                var data = aggregate.Data;
                if (data.AllowJoinGroupUsingLink != c.AllowJoinGroupUsingLink)
                {
                    aggregate.ModifyAllowJoinGroupUsingLink(c.AllowJoinGroupUsingLink);

                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(ChangeGroupSize c)
        {
            _repository.LockAndRun<GroupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.Size != c.Size)
                {
                    aggregate.ChangeGroupSize(c.Size);

                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(AddGroupTag c)
        {
            _repository.LockAndRun<GroupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (!aggregate.Data.Tags.Contains(c.Tag))
                {
                    aggregate.AddGroupTag(c.Tag);
                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(RemoveGroupTag c)
        {
            _repository.LockAndRun<GroupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.Tags.Contains(c.Tag))
                {
                    aggregate.RemoveGroupTag(c.Tag);
                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(ChangeGroupEmail c)
        {
            _repository.LockAndRun<GroupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.Email != c.Email)
                {
                    aggregate.ChangeGroupEmail(c.Email);

                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(ChangeGroupWebSiteUrl c)
        {
            _repository.LockAndRun<GroupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.WebSiteUrl != c.Url)
                {
                    aggregate.ChangeGroupURL(c.Url);

                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(ChangeGroupSocialMediaUrl c)
        {
            _repository.LockAndRun<GroupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                aggregate.ChangeSocialMediaUrl(c.Type, c.Url);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeGroupSurvey c)
        {
            _repository.LockAndRun<GroupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.Survey != c.Survey || aggregate.Data.SurveyNecessity != c.Necessity)
                {
                    aggregate.ChangeGroupSurvey(c.Survey, c.Necessity);

                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(ConfigureGroupNotifications c)
        {
            _repository.LockAndRun<GroupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                var data = aggregate.Data;

                if (data.MessageToAdminWhenEventVenueChanged != c.MessageToAdminWhenEventVenueChanged
                    || data.MessageToAdminWhenMembershipEnded != c.MessageToAdminWhenMembershipEnded
                    || data.MessageToAdminWhenMembershipStarted != c.MessageToAdminWhenMembershipStarted
                    || data.MessageToUserWhenMembershipEnded != c.MessageToUserWhenMembershipEnded
                    || data.MessageToUserWhenMembershipStarted != c.MessageToUserWhenMembershipStarted
                    )
                {
                    aggregate.ConfigureGroupNotifications(
                        c.MessageToAdminWhenEventVenueChanged,
                        c.MessageToAdminWhenMembershipEnded,
                        c.MessageToAdminWhenMembershipStarted,
                        c.MessageToUserWhenMembershipEnded,
                        c.MessageToUserWhenMembershipStarted
                        );

                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(ConnectGroup c)
        {
            bool exists = false;

            _repository.LockAndRun<GroupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.Connections.TryGetValue(c.Parent, out var connectionType))
                {
                    if (connectionType == ConnectionType.Child)
                        throw new GroupException($"Group {aggregate.AggregateIdentifier} cannot be connected to {c.Parent} as a child because it is already parent for this group.");

                    exists = true;

                    return;
                }

                aggregate.ConnectGroup(c.Parent, ConnectionType.Parent);

                Commit(aggregate, c);
            });

            if (exists)
                return;

            _repository.LockAndRun<GroupAggregate>(c.Parent, (aggregate) =>
            {
                aggregate.ConnectGroup(c.AggregateIdentifier, ConnectionType.Child);

                Commit(aggregate, c);
            });
        }

        public void Handle(CreateGroup c)
        {
            var aggregate = new GroupAggregate { AggregateIdentifier = c.AggregateIdentifier };

            aggregate.CreateGroup(c.Tenant, c.Type, c.Name);

            Commit(aggregate, c);
        }

        public void Handle(DeleteGroup c)
        {
            _repository.LockAndRun<GroupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.Connections.Count > 0)
                    throw new GroupException($"Group {aggregate.AggregateIdentifier} has connections and therefore cannot be deleted");

                if (_groupSearch.GroupExists(new QGroupFilter { ParentGroupIdentifier = aggregate.AggregateIdentifier }))
                    throw new GroupException($"Group {aggregate.AggregateIdentifier} is the parent for one or more groups and therefore cannot be deleted");

                aggregate.DeleteGroup(c.Reason);

                Commit(aggregate, c);
            });
        }

        public void Handle(DescribeGroup c)
        {
            _repository.LockAndRun<GroupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                var data = aggregate.Data;
                if (data.Category != c.Category
                    || data.Code != c.Code
                    || data.Description != c.Description
                    || data.Label != c.Label
                    )
                {
                    aggregate.DescribeGroup(c.Category, c.Code, c.Description, c.Label);

                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(DisconnectGroup c)
        {
            var notExists = false;

            _repository.LockAndRun<GroupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (!aggregate.Data.Connections.ContainsKey(c.ConnectedGroup))
                {
                    notExists = true;
                    return;
                }

                aggregate.DisconnectGroup(c.ConnectedGroup);

                Commit(aggregate, c);
            });

            if (notExists)
                return;

            _repository.LockAndRun<GroupAggregate>(c.ConnectedGroup, (aggregate) =>
            {
                aggregate.DisconnectGroup(c.AggregateIdentifier);

                Commit(aggregate, c);
            });
        }

        public void Handle(RenameGroup c)
        {
            _repository.LockAndRun<GroupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                var data = aggregate.Data;
                if (data.Name != c.Name
                    || data.Type != c.Type
                    )
                {
                    aggregate.RenameGroup(c.Type, c.Name);

                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(ChangeGroupExpiry c)
        {
            _repository.LockAndRun<GroupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                var data = aggregate.Data;
                if (data.Expiry == c.Expiry)
                    return;

                aggregate.ChangeGroupExpiry(c.Expiry);
                Commit(aggregate, c);
            });
        }

        public void Handle(ExpireGroup c)
        {
            _repository.LockAndRun<GroupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                aggregate.ExpireGroup(c.Expiry);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeGroupLifetime c)
        {
            _repository.LockAndRun<GroupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                var data = aggregate.Data;
                if (data.LifetimeQuantity == c.Quantity && data.LifetimeUnit == c.Unit)
                    return;

                aggregate.ChangeGroupLifetime(c.Quantity, c.Unit);
                Commit(aggregate, c);
            });
        }

        public void Handle(ModifyGroupMembershipProduct c)
        {
            _repository.LockAndRun<GroupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                var data = aggregate.Data;
                if (data.MembershipProduct == c.MembershipProduct)
                    return;

                aggregate.ModifyGroupMembershipProduct(c.MembershipProduct);
                Commit(aggregate, c);
            });
        }

        public void Handle(ModifyGroupStatus c)
        {
            _repository.LockAndRun<GroupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.StatusId == c.StatusId)
                    return;

                aggregate.ModifyGroupStatus(c.StatusId);

                Commit(aggregate, c);
            });
        }

        public void Handle(ModifyGroupOnlyOperatorCanAddUser c)
        {
            _repository.LockAndRun<GroupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.OnlyOperatorCanAddUser == c.OnlyOperatorCanAddUser)
                    return;

                aggregate.ModifyGroupOnlyOperatorCanAddUser(c.OnlyOperatorCanAddUser);

                Commit(aggregate, c);
            });
        }
    }
}
