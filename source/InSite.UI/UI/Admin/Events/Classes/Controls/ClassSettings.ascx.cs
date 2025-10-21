using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Events.Read;
using InSite.Application.Events.Write;
using InSite.Common.Web.UI;
using InSite.Domain.Events;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Constant;

using CheckBox = System.Web.UI.WebControls.CheckBox;

namespace InSite.UI.Admin.Events.Classes.Controls
{
    public partial class ClassSettings : BaseUserControl
    {
        #region Classes

        private class DataItem
        {
            public string GroupName { get; }
            public RegistrationFieldName FieldName { get; }
            public string Title { get; }
            public bool IsVisible { get; }
            public bool IsRequired { get; }
            public bool IsEditable { get; }

            public bool CanChangeRequired { get; }
            public bool CanChangeEditable { get; }

            private DataItem(string groupName, string fieldName, string title, bool isVisible, bool isRequired, bool isEditable, bool canChangeRequired, bool canChangeMasked)
                : this(groupName, fieldName.ToEnum<RegistrationFieldName>(), title, isVisible, isRequired, isEditable, canChangeRequired, canChangeMasked)
            {
            }

            private DataItem(string groupName, RegistrationFieldName fieldName, string title, bool isVisible, bool isRequired, bool isEditable, bool canChangeRequired, bool canChangeMasked)
            {
                GroupName = groupName;
                FieldName = fieldName;
                Title = title;
                IsVisible = isVisible;
                IsRequired = isRequired;
                IsEditable = isEditable;
                CanChangeRequired = canChangeRequired;
                CanChangeEditable = canChangeMasked;
            }

            public static DataItem[] GetDataSource()
            {
                var data = PortalFieldInfo.ClassRegistration
                    .Select(x => new DataItem(x.GroupName, x.FieldName, x.Title, x.IsVisible, x.IsRequired, true, x.CanChangeRequired, true))
                    .ToList();

                data.Add(new DataItem("Registration Panels", RegistrationFieldName.EmployerPanel, "Employer Panel", true, true, true, false, false));

                return data.ToArray();
            }
        }

        #endregion

        #region Properties

        private Guid EventId
        {
            get => (Guid)ViewState[nameof(EventId)];
            set => ViewState[nameof(EventId)] = value;
        }

        private Dictionary<RegistrationFieldName, RegistrationField> RegistrationFields
        {
            get => (Dictionary<RegistrationFieldName, RegistrationField>)ViewState[nameof(RegistrationFields)];
            set => ViewState[nameof(RegistrationFields)] = value;
        }

        private List<RegistrationFieldName> Keys
        {
            get => (List<RegistrationFieldName>)ViewState[nameof(Keys)];
            set => ViewState[nameof(Keys)] = value;
        }

        #endregion

        #region Fields

        private string _currentGroupName;
        private Dictionary<RegistrationFieldName, RegistrationField> _updatedFields;

        #endregion

        #region Public methods

        public void LoadData(QEvent ev)
        {
            EventId = ev.EventIdentifier;
            RegistrationFields = ev.GetRegistrationFields().ToDictionary(x => x.FieldName, x => x);

            RowRepeater.DataBind();
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RowRepeater.DataBinding += RowRepeater_DataBinding;
            RowRepeater.ItemCreated += RowRepeater_ItemCreated;
            RowRepeater.ItemDataBound += RowRepeater_ItemDataBound;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (_updatedFields.IsNotEmpty())
            {
                foreach (var field in _updatedFields.Values)
                    ServiceLocator.SendCommand(new ModifyRegistrationField(EventId, field));
            }
        }

        #endregion

        #region Event Handlers

        private void RowRepeater_DataBinding(object sender, EventArgs e)
        {
            _currentGroupName = null;
            Keys = new List<RegistrationFieldName>();

            RowRepeater.DataSource = DataItem.GetDataSource()
                .OrderBy(x => x.GroupName).ThenBy(x => x.Title);
        }

        private void RowRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var isVisibleCheckBox = (CheckBox)e.Item.FindControl("IsVisible");
            isVisibleCheckBox.CheckedChanged += ItemCheckBox_CheckedChanged;

            var isRequiredCheckBox = (CheckBox)e.Item.FindControl("IsRequired");
            isRequiredCheckBox.CheckedChanged += ItemCheckBox_CheckedChanged;

            var isEditableCheckBox = (CheckBox)e.Item.FindControl("IsEditable");
            isEditableCheckBox.CheckedChanged += ItemCheckBox_CheckedChanged;
        }

        private void RowRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = (DataItem)e.Item.DataItem;
            var fieldName = dataItem.FieldName;

            Keys.Add(fieldName);

            var isVisibleCheckBox = (CheckBox)e.Item.FindControl("IsVisible");
            var isRequiredCheckBox = (CheckBox)e.Item.FindControl("IsRequired");
            var isEditableCheckBox = (CheckBox)e.Item.FindControl("IsEditable");

            if (RegistrationFields.TryGetValue(fieldName, out var regField))
            {
                isVisibleCheckBox.Checked = regField.IsVisible;
                isRequiredCheckBox.Checked = regField.IsRequired;
                isEditableCheckBox.Checked = regField.IsEditable;
            }
            else
            {
                var fieldNameStr = dataItem.FieldName.GetName();
                var orgField = Organization.Fields.ClassRegistration.FirstOrDefault(x => x.FieldName == fieldNameStr);

                if (orgField != null)
                {
                    isVisibleCheckBox.Checked = orgField.IsVisible;
                    isRequiredCheckBox.Checked = orgField.IsRequired;
                }
                else
                {
                    isVisibleCheckBox.Checked = dataItem.IsVisible;
                    isRequiredCheckBox.Checked = dataItem.IsRequired;
                }

                isEditableCheckBox.Checked = dataItem.IsEditable;
            }

            isRequiredCheckBox.Enabled = dataItem.CanChangeRequired;
            isEditableCheckBox.Enabled = dataItem.CanChangeEditable;
        }

        private void ItemCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            var repeaterItem = (RepeaterItem)((Control)sender).NamingContainer;
            var fieldName = Keys[repeaterItem.ItemIndex];
            var dataItem = DataItem.GetDataSource().FirstOrDefault(x => x.FieldName == fieldName);

            var field = RegistrationFields.GetOrAdd(fieldName, () => new RegistrationField
            {
                FieldName = dataItem.FieldName,
                IsVisible = dataItem.IsVisible,
                IsRequired = dataItem.IsRequired,
                IsEditable = dataItem.IsEditable
            });

            var isVisibleCheckBox = (CheckBox)repeaterItem.FindControl("IsVisible");
            var isRequiredCheckBox = (CheckBox)repeaterItem.FindControl("IsRequired");
            var isEditableCheckBox = (CheckBox)repeaterItem.FindControl("IsEditable");

            field.IsVisible = isVisibleCheckBox.Checked;
            field.IsRequired = isRequiredCheckBox.Checked;
            field.IsEditable = isEditableCheckBox.Checked;

            if (_updatedFields == null)
                _updatedFields = new Dictionary<RegistrationFieldName, RegistrationField>();

            _updatedFields[fieldName] = field;
        }

        #endregion

        #region Helper Methods

        protected bool IsGroupVisible()
        {
            var field = (DataItem)Page.GetDataItem();
            var rowGroup = field.GroupName.NullIfEmpty();
            var result = rowGroup != _currentGroupName;

            _currentGroupName = rowGroup;

            return result;
        }

        #endregion
    }
}