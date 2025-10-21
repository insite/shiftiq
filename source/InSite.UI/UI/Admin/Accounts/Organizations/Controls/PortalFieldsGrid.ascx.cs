using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Domain.Organizations;
using InSite.Persistence;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Common.Events;
using Shift.Common.Linq;

using CheckBox = System.Web.UI.WebControls.CheckBox;

namespace InSite.Admin.Accounts.Organizations.Controls
{
    public partial class PortalFieldsGrid : BaseUserControl
    {
        #region Classes

        [Serializable]
        private class ControlData
        {
            public Guid OrganizationIdentifier { get; set; }
            public string ListName { get; set; }
        }

        #endregion

        #region Properties

        private ControlData Data
        {
            get => (ControlData)ViewState[nameof(Data)];
            set => ViewState[nameof(Data)] = value;
        }

        private List<string> DataKeys
        {
            get => (List<string>)ViewState[nameof(DataKeys)];
            set => ViewState[nameof(DataKeys)] = value;
        }

        public bool HideMasked
        {
            get => (ViewState[nameof(HideMasked)] as bool?) ?? false;
            set => ViewState[nameof(HideMasked)] = value;
        }

        public bool HideRequired
        {
            get => (ViewState[nameof(HideRequired)] as bool?) ?? false;
            set => ViewState[nameof(HideRequired)] = value;
        }

        #endregion

        #region Fields

        private OrganizationState _organization;
        private List<OrganizationField> _organizationList;
        private PortalFieldInfo[] _defaultList;

        private string _currentGroupName;

        #endregion

        #region Public methods

        public void LoadData(Guid organizationIdentifier, string listName)
        {
            Data = new ControlData
            {
                OrganizationIdentifier = organizationIdentifier,
                ListName = listName
            };

            RowRepeater.DataBind();
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RowRepeater.DataBinding += RowRepeater_DataBinding;
            RowRepeater.ItemCreated += RowRepeater_ItemCreated;
            RowRepeater.ItemDataBound += RowRepeater_ItemDataBound;

            UpdatePanel.Request += UpdatePanel_Request;
        }

        #endregion

        #region Event Handlers

        private void UpdatePanel_Request(object sender, StringValueArgs e)
        {
            if (e.Value == "save")
            {
                EnsureDataLoaded();

                for (var i = 0; i < RowRepeater.Items.Count; i++)
                {
                    var item = RowRepeater.Items[i];

                    var isVisible = (CheckBox)item.FindControl("IsVisible");
                    var isRequired = (CheckBox)item.FindControl("IsRequired");
                    var isMasked = (CheckBox)item.FindControl("IsMasked");

                    var fieldName = DataKeys[i];
                    var field = _organizationList.FirstOrDefault(x => x.FieldName == fieldName);

                    if (field == null)
                        _organizationList.Add(field = new OrganizationField
                        {
                            FieldName = fieldName
                        });

                    field.IsVisible = isVisible.Checked;
                    field.IsRequired = isRequired.Checked;
                    field.IsMasked = isMasked.Checked;
                }

                OrganizationStore.Update(_organization);
            }
        }

        private void RowRepeater_DataBinding(object sender, EventArgs e)
        {
            EnsureDataLoaded();

            _currentGroupName = null;

            DataKeys = new List<string>();

            RowRepeater.DataSource = _defaultList
                .OrderBy(x => x.GroupName).ThenBy(x => x.Title);
        }

        private void RowRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Header)
                return;

            var th = (HtmlTableCell)e.Item.FindControl("RequiredHeader");
            th.Visible = !HideRequired;

            th = (HtmlTableCell)e.Item.FindControl("MaskedHeader");
            th.Visible = !HideMasked;
        }

        private void RowRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var defaultField = (PortalFieldInfo)e.Item.DataItem;
            var fieldName = defaultField.FieldName;
            var organizationField = _organizationList.FirstOrDefault(x => x.FieldName == fieldName);

            DataKeys.Add(fieldName);

            var isVisibleCheckBox = (CheckBox)e.Item.FindControl("IsVisible");
            var isRequiredCheckBox = (CheckBox)e.Item.FindControl("IsRequired");
            var isMaskedCheckBox = (CheckBox)e.Item.FindControl("IsMasked");

            isRequiredCheckBox.Enabled = defaultField.CanChangeRequired;
            isMaskedCheckBox.Enabled = defaultField.CanChangeMasked;

            if (organizationField != null)
            {
                isVisibleCheckBox.Checked = organizationField.IsVisible;
                isRequiredCheckBox.Checked = defaultField.CanChangeRequired ? organizationField.IsRequired : defaultField.IsRequired;
                isMaskedCheckBox.Checked = defaultField.CanChangeMasked ? organizationField.IsMasked : defaultField.IsMasked;
            }
            else
            {
                isVisibleCheckBox.Checked = defaultField.IsVisible;
                isRequiredCheckBox.Checked = defaultField.IsRequired;
                isMaskedCheckBox.Checked = defaultField.IsMasked;
            }

            var td = (HtmlTableCell)e.Item.FindControl("RequiredData");
            td.Visible = !HideRequired;

            td = (HtmlTableCell)e.Item.FindControl("MaskedData");
            td.Visible = !HideMasked;
        }

        #endregion

        #region Methods (data binding)

        private void EnsureDataLoaded()
        {
            if (_organization != null)
                return;

            _organization = OrganizationSearch.Select(Data.OrganizationIdentifier);

            switch (Data.ListName)
            {
                case nameof(PortalFieldInfo.UserProfile):
                    _defaultList = PortalFieldInfo.UserProfile;
                    _organizationList = _organization.Fields.User;
                    break;
                case nameof(PortalFieldInfo.ClassRegistration):
                    _defaultList = PortalFieldInfo.ClassRegistration;
                    _organizationList = _organization.Fields.ClassRegistration;
                    break;
                case nameof(PortalFieldInfo.LearnerDashboard):
                    _defaultList = PortalFieldInfo.LearnerDashboard;
                    _organizationList = _organization.Fields.LearnerDashboard;
                    break;
                case nameof(PortalFieldInfo.InvoiceBillingAddress):
                    _defaultList = PortalFieldInfo.InvoiceBillingAddress;
                    _organizationList = _organization.Fields.InvoiceBillingAddress;
                    break;
                default:
                    throw ApplicationError.Create("Unexpected list name: " + Data.ListName);
            }
        }

        protected bool IsGroupVisible()
        {
            var field = (PortalFieldInfo)Page.GetDataItem();
            var rowGroup = field.GroupName.NullIfEmpty();
            var result = rowGroup != _currentGroupName;

            _currentGroupName = rowGroup;

            return result;
        }

        #endregion
    }
}