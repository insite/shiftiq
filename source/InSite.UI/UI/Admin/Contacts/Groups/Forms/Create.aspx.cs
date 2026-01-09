using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Application.Groups.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Contacts.Groups.Forms
{
    public partial class Create : AdminBasePage
    {
        #region Constants

        private const string EditUrl = "/ui/admin/contacts/groups/edit";
        private const string SearchUrl = "/ui/admin/contacts/groups/search";

        #endregion

        #region Properties

        private Guid? ParentID => Guid.TryParse(Request["parent"], out var value) ? value : (Guid?)null;

        private string DefaultSubType =>
            Request.QueryString["type"].IfNullOrEmpty(GroupTypes.Employer);

        public string DefaultGroupLabel => Request.QueryString["label"];

        #endregion

        #region Inialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CreationType.AutoPostBack = true;
            CreationType.ValueChanged += CreationType_ValueChanged;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanCreate)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(Page);

            Open();
        }

        #endregion

        #region Event handlers

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (IsValid)
                Save();
        }

        private void CreationType_ValueChanged(object sender, EventArgs e)
        {
            OnCreationTypeSelected();
        }

        private void OnCreationTypeSelected()
        {
            var creationType = CreationType.ValueAsEnum;

            if (creationType == CreationTypeEnum.One)
                CreateMultiView.SetActiveView(ViewNewSection);
            else if (creationType == CreationTypeEnum.Bulk)
                CreateMultiView.SetActiveView(ViewMultipleSection);
            else
                throw ApplicationError.Create("Unsupported creation type: " + creationType.GetName());
        }

        #endregion

        #region Methods (save/load)

        private void Open()
        {
            SingleGroupType.Value = ParentID.HasValue ? GroupTypes.Employer : DefaultSubType;
            SingleGroupLabel.Text = DefaultGroupLabel;

            MultipleGroupType.Value = ParentID.HasValue ? GroupTypes.Employer : DefaultSubType;
            MultipleGroupLabel.Text = DefaultGroupLabel;

            CreationType.EnsureDataBound();
            CreationType.SetVisibleOptions(CreationTypeEnum.One, CreationTypeEnum.Bulk);

            OnCreationTypeSelected();

            CancelButton.NavigateUrl = SearchUrl;
        }

        private void Save()
        {
            Guid id;
            var commands = new List<Command>();
            var creationType = CreationType.ValueAsEnum;

            if (creationType == CreationTypeEnum.One)
            {
                if (!CreateCommandsForOne(commands, out id))
                    return;
            }
            else if (creationType == CreationTypeEnum.Bulk)
            {
                if (!CreateCommandsForBulk(commands, out id))
                    return;
            }
            else
                throw new ArgumentException($"Unsupported creation type: {CreationType.Value}");

            ServiceLocator.SendCommands(commands);

            HttpResponseHelper.Redirect($"{EditUrl}?contact={id}&status=saved");
        }

        private bool CreateCommandsForOne(List<Command> commands, out Guid groupId)
        {
            groupId = Guid.Empty;

            if (string.IsNullOrWhiteSpace(SingleGroupName.Text))
            {
                ScreenStatus.AddMessage(AlertType.Error, "Please input a group name.");
                return false;
            }

            groupId = UniqueIdentifier.Create();
            var label = GroupHelper.CleanGroupLabel(SingleGroupLabel.Text);

            commands.Add(new CreateGroup(groupId, Organization.Identifier, SingleGroupType.Value, SingleGroupName.Text));
            commands.Add(new DescribeGroup(groupId, null, null, null, label));

            if (ParentID.HasValue)
                commands.Add(new ChangeGroupParent(groupId, ParentID));

            return true;
        }

        private bool CreateCommandsForBulk(List<Command> commands, out Guid groupId)
        {
            groupId = Guid.Empty;

            var names = StringHelper.Split(MultipleGroupName.Text);
            if (names.Length == 0)
            {
                ScreenStatus.AddMessage(AlertType.Error, "Please input one or more group names.");
                return false;
            }

            var label = GroupHelper.CleanGroupLabel(MultipleGroupLabel.Text);
            var type = MultipleGroupType.Value;

            foreach (var name in names)
            {
                if (name.Length > 90)
                {
                    ScreenStatus.AddMessage(AlertType.Error, $"The group name exceeds the maximum allowed length of 90 characters: <strong>{name}</strong>");
                    return false;
                }

                groupId = UniqueIdentifier.Create();

                commands.Add(new CreateGroup(groupId, Organization.Identifier, type, name));
                commands.Add(new DescribeGroup(groupId, null, null, null, label));

                if (ParentID.HasValue)
                    commands.Add(new ChangeGroupParent(groupId, ParentID));
            }

            return true;
        }

        #endregion
    }
}