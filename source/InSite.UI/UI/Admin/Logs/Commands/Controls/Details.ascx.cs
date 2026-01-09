using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Admin.Logs.Commands.Utilities;
using InSite.Common.Web.UI;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Admin.Logs.Commands.Controls
{
    public partial class Details : BaseUserControl
    {
        #region Properties

        private Guid OrganizationIdentifier
        {
            get => (Guid)(ViewState[nameof(OrganizationIdentifier)] ?? Guid.Empty);
            set => ViewState[nameof(OrganizationIdentifier)] = value;
        }

        private bool IsAggregateTypeInited
        {
            get => (bool)(ViewState[nameof(IsAggregateTypeInited)] ?? false);
            set => ViewState[nameof(IsAggregateTypeInited)] = value;
        }

        public bool IsEditor { get; set; }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AggregateType.AutoPostBack = true;
            AggregateType.ValueChanged += AggregateType_ValueChanged;

            CustomCommandTypeValidator.ServerValidate += CustomCommandTypeValidator_ServerValidate;

            AggregateIdentifierValidator.ServerValidate += AggregateIdentifierValidator_ServerValidate;

            DataJsonValidator.ServerValidate += DataJsonValidator_ServerValidate;
            DataJsonValidatorEnabled.AutoPostBack = true;
            DataJsonValidatorEnabled.CheckedChanged += DataJsonValidatorEnabled_CheckedChanged;
        }

        private void DataJsonValidatorEnabled_CheckedChanged(object sender, EventArgs e)
            => DataJsonValidator.Enabled = DataJsonValidatorEnabled.Checked;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                AggregateType.Enabled = !IsEditor;
                AggregateIdentifier.Enabled = !IsEditor;
                CommandTypeSelector.Enabled = !IsEditor;
                CommandTypeSelectorNextView.Enabled = !IsEditor;
                CommandTypeText.Enabled = !IsEditor;
                CommandTypeTextNextView.Enabled = !IsEditor;

                BindAggregateType();
            }
        }

        #endregion

        #region Event handlers

        internal void SetOrganizationIdentifier(Guid identifier)
        {
            OrganizationIdentifier = identifier;
        }

        private void AggregateType_ValueChanged(object sender, EventArgs e) =>
            OnAggregateTypeChanged();

        private void OnAggregateTypeChanged()
        {
            BindChangeType();
        }

        private void CustomCommandTypeValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = CommandTypeSelectorView.IsActive
                ? CommandTypeSelector.ValueAsGuid.HasValue
                : CommandTypeText.Text.IsNotEmpty() && CommandHelper.GetCommandTypeInfo(CommandTypeText.Text) != null;
        }

        private void AggregateIdentifierValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;

            if (!Guid.TryParse(AggregateIdentifier.Text, out var aggId))
                return;

            var aggregate = ServiceLocator.AggregateSearch.Get(aggId);
            if (aggregate == null || aggregate.OriginOrganization != OrganizationIdentifier)
                return;

            var aggTypeId = AggregateType.ValueAsGuid;
            if (!aggTypeId.HasValue)
                return;

            var aggTypeInfo = CommandHelper.GetAggregateTypeInfo(aggTypeId.Value);
            if (aggTypeInfo == null || aggregate.AggregateClass != ServiceLocator.Serializer.GetClassName(aggTypeInfo.Type))
                return;

            args.IsValid = true;
        }

        private void DataJsonValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            IList<string> errors = null;

            args.IsValid = false;

            if (CommandTypeSelectorView.IsActive)
            {
                var commandTypeId = CommandTypeSelector.ValueAsGuid;
                var aggTypeId = AggregateType.ValueAsGuid;

                args.IsValid = commandTypeId.HasValue && aggTypeId.HasValue
                    && CommandHelper.IsCommandJsonValid(aggTypeId.Value, commandTypeId.Value, CommandData.Text, out errors);
            }
            else if (CommandTypeTextView.IsActive)
            {
                var cmd = CommandHelper.GetCommandTypeInfo(CommandTypeText.Text);

                args.IsValid = cmd != null
                    && CommandHelper.IsCommandJsonValid(cmd.ID, CommandData.Text, out errors);
            }

            if (!args.IsValid)
                DataJsonValidator.ErrorMessage = errors.IsNotEmpty()
                    ? $"Data JSON is invalid:<ul><li>{string.Join("</li><li>", errors)}</li></ul>"
                    : "Data JSON is invalid.";
        }

        #endregion

        #region Methods

        private void BindAggregateType()
        {
            if (IsAggregateTypeInited)
                return;

            AggregateType.LoadItems(CommandHelper.GetAggregates(), "ID", "Name");

            OnAggregateTypeChanged();

            IsAggregateTypeInited = true;
        }

        private void BindChangeType()
        {
            CommandTypeSelectorView.IsActive = true;
            CommandTypeText.Text = null;

            var aggTypeId = AggregateType.ValueAsGuid;
            var aggTypeInfo = aggTypeId.HasValue ? CommandHelper.GetAggregateTypeInfo(aggTypeId.Value) : null;
            if (aggTypeInfo != null)
                CommandTypeSelector.LoadItems(aggTypeInfo.Commands, "ID", "Type.Name");
            else
                CommandTypeSelector.Items.Clear();
        }

        private static string FormatJson(string json, Formatting formatting)
        {
            if (json.IsNotEmpty())
            {
                var jObj = JsonConvert.DeserializeObject(json);
                json = JsonConvert.SerializeObject(jObj, formatting);
            }

            return json;
        }

        #endregion

        #region Settings and getting input values

        public void SetInputValues(SerializedCommand command)
        {
            BindAggregateType();

            Guid? aggTypeId = null;

            {
                var aggregate = ServiceLocator.AggregateSearch.Get(command.AggregateIdentifier);
                if (aggregate != null)
                {
                    var aggregateType = Type.GetType(aggregate.AggregateClass);
                    var aggregateTypeInfo = CommandHelper.GetAggregateTypeInfo(aggregateType);

                    if (aggregateTypeInfo != null)
                        aggTypeId = aggregateTypeInfo.ID;
                }
            }

            var commandType = Type.GetType(command.CommandClass);
            var commandTypeInfo = CommandHelper.GetCommandTypeInfo(commandType);
            var hasCommandTypeInfo = commandTypeInfo != null;

            if (!aggTypeId.HasValue && hasCommandTypeInfo && commandTypeInfo.Aggregates.Length == 1)
                aggTypeId = commandTypeInfo.Aggregates[0].ID;

            if (aggTypeId.HasValue)
            {
                AggregateType.ValueAsGuid = aggTypeId;

                OnAggregateTypeChanged();
            }

            AggregateIdentifier.Text = command.AggregateIdentifier.ToString();

            if (commandTypeInfo != null)
            {
                var option = CommandTypeSelector.FindOptionByValue(commandTypeInfo.ID.ToString(), true);
                if (option != null)
                {
                    option.Selected = true;

                    CommandTypeSelectorView.IsActive = true;
                    CommandTypeText.Text = null;
                }
                else
                {
                    CommandTypeSelector.ClearSelection();
                    CommandTypeText.Text = commandTypeInfo.Type.Name;
                    CommandTypeTextView.IsActive = true;
                }
            }

            CommandData.Text = FormatJson(command.CommandData, Formatting.Indented);
            CommandDescription.Text = command.CommandDescription;
        }

        public void GetInputValues(SerializedCommand command)
        {
            command.AggregateIdentifier = Guid.Parse(AggregateIdentifier.Text);

            var commandInfo = CommandTypeSelectorView.IsActive
                ? CommandHelper.GetCommandTypeInfo(AggregateType.ValueAsGuid.Value, CommandTypeSelector.ValueAsGuid.Value)
                : CommandHelper.GetCommandTypeInfo(CommandTypeText.Text);

            command.CommandClass = ServiceLocator.Serializer.GetClassName(commandInfo.Type);
            command.CommandType = commandInfo.Type.Name;
            command.CommandData = FormatJson(CommandData.Text, Formatting.None);
            command.CommandDescription = CommandDescription.Text;
        }

        #endregion
    }
}