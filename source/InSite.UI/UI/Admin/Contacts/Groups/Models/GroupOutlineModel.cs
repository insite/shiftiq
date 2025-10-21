using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Groups.Write;
using InSite.Domain.Organizations;

namespace InSite.Admin.Contacts.Groups.Models
{
    public class GroupOutlineModel
    {
        #region Construction

        private GroupOutlineModel()
        {
        }

        #endregion

        #region Properties

        public GroupModel Root { get; private set; }

        #endregion

        #region Initialization

        internal static GroupOutlineModel Create(OrganizationState organization, string keyword = null)
        {
            var data = ServiceLocator.GroupSearch
                .GetGroupOutlineItems(organization.Identifier, keyword)
                .Select(
                    x => new GroupModel
                    {
                        Identifier = x.GroupIdentifier,
                        Thumbprint = x.GroupIdentifier,
                        ParentIdentifier = x.ParentGroupIdentifier,
                        Name = x.GroupName,
                        Subtype = x.GroupType,
                        Abbreviation = x.GroupCode,
                        MemberCount = x.MemberCount,
                        PermissionCount = x.GroupActionCount
                    }
                ).ToDictionary(x => x.Identifier, x => x);

            var model = new GroupOutlineModel
            {
                Root = new GroupModel
                {
                    Identifier = organization.OrganizationIdentifier,
                    Name = organization.CompanyDescription.LegalName ?? organization.CompanyName ?? organization.OrganizationCode
                }
            };

            foreach (var group in data.Select(x => x.Value).OrderBy(x => x.Name))
                if (group.ParentIdentifier.HasValue && data.ContainsKey(group.ParentIdentifier.Value))
                    data[group.ParentIdentifier.Value].Add(group);
                else
                    model.Root.Add(group);

            return model;
        }

        #endregion

        internal static void Indent(Guid id, Guid previous)
        {
            ServiceLocator.SendCommand(new ChangeGroupParent(id, previous));
        }

        internal static void Outdent(Guid id)
        {
            var contact = ServiceLocator.GroupSearch.GetGroup(id);

            if (contact.ParentGroupIdentifier.HasValue)
            {
                var parentContact = ServiceLocator.GroupSearch.GetGroup(contact.ParentGroupIdentifier.Value);

                ServiceLocator.SendCommand(new ChangeGroupParent(id, parentContact.ParentGroupIdentifier));
            }
        }

        #region Classes

        public class GroupModel
        {
            #region Methods

            public void Add(GroupModel model)
            {
                model.Parent = this;
                if (_children.Count > 0) model.Previous = _children[_children.Count - 1];
                _children.Add(model);
            }

            #endregion

            #region Properties

            public Guid Identifier { get; set; }
            public Guid Thumbprint { get; set; }
            public Guid? ParentIdentifier { get; set; }
            public string Subtype { get; set; }
            public string Name { get; set; }
            public string Abbreviation { get; set; }
            public int MemberCount { get; set; }
            public int PermissionCount { get; set; }

            public GroupModel Parent { get; private set; }

            public IReadOnlyList<GroupModel> Children => _children;

            public GroupModel Previous { get; set; }

            #endregion

            #region Fields

            private readonly List<GroupModel> _children = new List<GroupModel>();

            #endregion
        }

        #endregion
    }
}