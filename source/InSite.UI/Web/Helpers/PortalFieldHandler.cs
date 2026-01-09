using System;
using System.Web.UI;

using InSite.Common;
using InSite.Common.Web.UI;
using InSite.Domain.Events;
using InSite.Domain.Organizations;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Web.Helpers
{
    public class PortalFieldInfo
    {
        #region Properties

        public string GroupName { get; }
        public string FieldName { get; }
        public string Title => _titleIsLabel ? LabelHelper.GetLabelContentText(_title) : _title;
        public bool IsVisible { get; }
        public bool IsRequired { get; }
        public bool IsMasked { get; }
        public bool IsEditable { get; }
        public bool CanChangeRequired { get; }
        public bool CanChangeMasked { get; }
        public bool CanChangeEditable { get; }

        #endregion

        #region Fields

        private string _title;
        private bool _titleIsLabel;

        #endregion

        #region Construction

        private PortalFieldInfo(string groupName, string fieldName, string title, bool isVisible, bool isRequired, bool isMasked = false, bool canChangeRequired = true, bool canChangeMasked = true, bool titleIsLabel = false, bool isEditable = true, bool canChangeEditable = true)
        {
            GroupName = groupName;
            FieldName = fieldName;
            _title = title;
            _titleIsLabel = titleIsLabel;
            IsVisible = isVisible;
            IsRequired = isRequired;
            IsMasked = isMasked;
            IsEditable = isEditable;
            CanChangeRequired = canChangeRequired;
            CanChangeMasked = canChangeMasked;
            CanChangeEditable = canChangeEditable;
        }

        #endregion

        #region Lists

        public static readonly PortalFieldInfo[] UserProfile = new[]
        {
            new PortalFieldInfo("Personal", "FirstName", "First Name",                   isVisible: true, isRequired: true, isMasked: false, canChangeRequired: false),
            new PortalFieldInfo("Personal", "MiddleName", "Middle Name",                 isVisible: true, isRequired: false, isMasked: false),
            new PortalFieldInfo("Personal", "LastName", "Last Name",                     isVisible: true, isRequired: true, isMasked: false, canChangeRequired: false),
            new PortalFieldInfo("Personal", "Email", "Email",                            isVisible: true, isRequired: true, isMasked: false, canChangeRequired: false),
            new PortalFieldInfo("Personal", "Language", "Preferred Language",            isVisible: true, isRequired: false, isMasked: false, canChangeRequired: false, canChangeMasked: false),
            new PortalFieldInfo("Personal", "TimeZone", "Time Zone",                     isVisible: true, isRequired: true, isMasked: false, canChangeRequired: false, canChangeMasked: false),
            new PortalFieldInfo("Personal", "FirstLanguage", "English Language Learner", isVisible: true, isRequired: false, isMasked: false, canChangeRequired: false, canChangeMasked: false),
            new PortalFieldInfo("Employment", "EmployerGroupIdentifier", "Employer",     isVisible: true, isRequired: false, isMasked: false, canChangeMasked: false),
            new PortalFieldInfo("Employment", "JobTitle", "Job Title",                   isVisible: true, isRequired: false, isMasked: false),
            new PortalFieldInfo("Employment", "PersonCode", "Person Code",               isVisible: false, isRequired: false, isMasked: false, titleIsLabel: true),
            new PortalFieldInfo("Employment", "UnionInfo", "Union Info",                 isVisible: true, isRequired: false, isMasked: false),
            new PortalFieldInfo("Emergency Contact", "EmergencyContactName", "Name",     isVisible: true, isRequired: false, isMasked: false),
            new PortalFieldInfo("Emergency Contact", "EmergencyContactPhone", "Phone",   isVisible: true, isRequired: false, isMasked: false),
            new PortalFieldInfo("Emergency Contact", "EmergencyContactRelationship", "Relationship", isVisible: true, isRequired: false, isMasked: false),
            new PortalFieldInfo("Phone Numbers", "Phone", "Preferred",                   isVisible: true, isRequired: false, isMasked: false),
            new PortalFieldInfo("Phone Numbers", "PhoneHome", "Home",                    isVisible: true, isRequired: false, isMasked: false),
            new PortalFieldInfo("Phone Numbers", "PhoneWork", "Work",                    isVisible: true, isRequired: false, isMasked: false),
            new PortalFieldInfo("Phone Numbers", "PhoneMobile", "Mobile",                isVisible: true, isRequired: false, isMasked: false),
            new PortalFieldInfo("Phone Numbers", "PhoneOther", "Other",                  isVisible: true, isRequired: false, isMasked: false),
        };

        public static readonly PortalFieldInfo[] ClassRegistration = new[]
        {
            new PortalFieldInfo("Personal", "FirstName", "First Name",                     isVisible: true, isRequired: true,  isEditable: true, canChangeRequired: false),
            new PortalFieldInfo("Personal", "MiddleName", "Middle Name",                   isVisible: true, isRequired: false, isEditable: true),
            new PortalFieldInfo("Personal", "LastName", "Last Name",                       isVisible: true, isRequired: true,  isEditable: true, canChangeRequired: false),
            new PortalFieldInfo("Personal", "Birthdate", "Birthdate",                      isVisible: true, isRequired: true,  isEditable: true, canChangeMasked : false),
            new PortalFieldInfo("Personal", "PersonCode", "Person Code",                   isVisible: true, isRequired: true,  isEditable: true, titleIsLabel: true),
            new PortalFieldInfo("Personal", "WorkBasedHoursToDate", "Number of Work Based Hours to date", isVisible: true, isRequired: false, isEditable: true, canChangeMasked : false),
            new PortalFieldInfo("Personal", "FirstLanguage", "English Language Learner",   isVisible: true, isRequired: false, isEditable: true, canChangeRequired: false, canChangeMasked : false),
            new PortalFieldInfo("Address", "Street1", "Address 1",                         isVisible: true, isRequired: true,  isEditable: true),
            new PortalFieldInfo("Address", "City", "City",                                 isVisible: true, isRequired: true,  isEditable: true),
            new PortalFieldInfo("Address", "Country", "Country",                           isVisible: true, isRequired: true,  isEditable: true, canChangeMasked : false),
            new PortalFieldInfo("Address", "Province", "Province",                         isVisible: true, isRequired: true,  isEditable: true, canChangeMasked : false),
            new PortalFieldInfo("Address", "PostalCode", "Postal Code",                    isVisible: true, isRequired: true,  isEditable: true),
            new PortalFieldInfo("Emergency Contact", "EmergencyContactName", "Name",       isVisible: true, isRequired: true,  isEditable: true),
            new PortalFieldInfo("Emergency Contact", "EmergencyContactPhone", "Phone",     isVisible: true, isRequired: true,  isEditable: true),
            new PortalFieldInfo("Emergency Contact", "EmergencyContactRelationship", "Relationship", isVisible: true, isRequired: false, isEditable: true),
            new PortalFieldInfo("Phone Numbers", "Phone", "Preferred",                     isVisible: true, isRequired: false, isEditable: true),
            new PortalFieldInfo("Phone Numbers", "PhoneHome", "Home",                      isVisible: true, isRequired: false, isEditable: true),
            new PortalFieldInfo("Phone Numbers", "PhoneWork", "Work",                      isVisible: true, isRequired: false, isEditable: true),
            new PortalFieldInfo("Phone Numbers", "PhoneMobile", "Mobile",                  isVisible: true, isRequired: false, isEditable: true),
            new PortalFieldInfo("Phone Numbers", "PhoneOther", "Other",                    isVisible: true, isRequired: false, isEditable: true),
            new PortalFieldInfo("Registration Panels", "EmployerPanel", "Employer Panel",  isVisible: true, isRequired: true,  isEditable: true, canChangeRequired: false, canChangeEditable: false),
        };

        public static readonly PortalFieldInfo[] LearnerDashboard = new[]
        {
            new PortalFieldInfo("Panels", "achievements", "Achievements", true, false, false),
            new PortalFieldInfo("Panels", "assessments", "Assessments", true, false, false),
            new PortalFieldInfo("Panels", "chats", "Chats", false, false, false),
            new PortalFieldInfo("Panels", "competencies", "Competencies", true, false, false),
            new PortalFieldInfo("Panels", "courses", "Courses", true, false, false),
            new PortalFieldInfo("Panels", "programs", "Programs", true, false, false),
            new PortalFieldInfo("Panels", "events", "Events", true, false, false),
            new PortalFieldInfo("Panels", "grades", "Grades", true, false, false),
            new PortalFieldInfo("Panels", "logbooks", "Logbooks", true, false, false),
            new PortalFieldInfo("Panels", "messages", "Messages", true, false, false),
            new PortalFieldInfo("Panels", "reports", "Reports", true, false, false),
            new PortalFieldInfo("Panels", "surveys", "Forms", true, false, false)
        };

        public static readonly PortalFieldInfo[] InvoiceBillingAddress = new[]
        {
            new PortalFieldInfo(null, "ContactName", "Contact Name",    isVisible: true, isRequired: true, isMasked: false),
            new PortalFieldInfo(null, "ContactEmail", "Contact Email",  isVisible: true, isRequired: true, isMasked: false),
            new PortalFieldInfo(null, "ContactPhone", "Contact Phone",  isVisible: true, isRequired: true, isMasked: false),
            new PortalFieldInfo(null, "ContactAddress", "Address",             isVisible: true, isRequired: true, isMasked: false),
            new PortalFieldInfo(null, "ContactCity", "City",                   isVisible: true, isRequired: true, isMasked: false),
            new PortalFieldInfo(null, "ContactProvince", "Province",           isVisible: true, isRequired: true, isMasked: false),
            new PortalFieldInfo(null, "ContactPostalCode", "Postal Code",      isVisible: true, isRequired: true, isMasked: false),
            new PortalFieldInfo(null, "ContactCountry", "Country",             isVisible: true, isRequired: true, isMasked: false),
        };

        #endregion
    }

    public sealed class PortalFieldHandler<TContainer> where TContainer : Control
    {
        public interface IFieldData
        {
            TContainer Container { get; }
            string Name { get; }
            bool IsVisible { get; }
            bool IsRequired { get; }
            bool IsMasked { get; }
            bool IsEditable { get; }
        }

        private class FieldData : IFieldData
        {
            public TContainer Container { get; set; }
            public string Name { get; set; }
            public bool IsVisible { get; set; }
            public bool IsRequired { get; set; }
            public bool IsMasked { get; set; }
            public bool IsEditable { get; set; }

            public FieldData(TContainer container, string fieldName)
            {
                Container = container;
                Name = fieldName;
            }
        }

        public delegate void InitHandler(IFieldData field);

        private InitHandler _initHandler;
        private static readonly InitHandler _defaultInitHandler = f =>
        {
            var fieldId = $"{f.Name}Field";
            var fieldControl = f.Container.FindControl(fieldId);

            if (fieldControl == null)
                return;

            fieldControl.Visible = f.IsVisible;

            var inputId = f.Name;
            var inputControl = f.Container.FindControl(inputId)
                ?? throw ApplicationError.Create($"Input control not found: " + inputId);

            if (inputControl is TextBox textBox)
                textBox.TextMode = f.IsMasked ? TextBoxMode.Masked : TextBoxMode.SingleLine;
            else if (f.IsMasked)
                throw ApplicationError.Create($"Can't set IsMasked attribute: " + inputId);

            if (inputControl is BaseInputBox baseInputBox)
                baseInputBox.Enabled = f.IsEditable || !baseInputBox.HasValue;
            else if (inputControl is IBaseDateTimeSelector baseDateSelector)
                baseDateSelector.Enabled = f.IsEditable || !baseDateSelector.HasValue;
            else if (inputControl is BaseToggle baseToggle)
                baseToggle.Enabled = f.IsEditable;
            else if (inputControl is BaseComboBox baseComboBox)
                baseComboBox.Enabled = f.IsEditable || !baseComboBox.HasValue;
            else if (inputControl is IBaseFindEntity findEntity)
                findEntity.Enabled = f.IsEditable || findEntity.HasValue;
            else
                throw ApplicationError.Create($"Can't set IsEditable attribute: " + inputId);

            var requiredValidatorId = $"{f.Name}RequiredValidator";
            if (f.Container.FindControl(requiredValidatorId) is RequiredValidator requiredValidator)
                requiredValidator.Visible = f.IsRequired;
            else if (f.IsRequired)
                throw ApplicationError.Create($"Required validator not found: " + requiredValidatorId);
        };

        public PortalFieldHandler()
        {
            _initHandler = _defaultInitHandler;
        }

        public PortalFieldHandler(InitHandler init)
        {
            _initHandler = init ?? throw new ArgumentNullException(nameof(init));
        }

        public IFieldData Init(TContainer container, PortalFieldInfo defaultField, OrganizationField organizationField, RegistrationField registrationField)
        {
            var field = new FieldData(container, defaultField.FieldName)
            {
                IsVisible = defaultField.IsVisible,
                IsRequired = defaultField.IsRequired,
                IsMasked = defaultField.IsMasked,
                IsEditable = defaultField.IsEditable
            };

            if (organizationField != null)
            {
                field.IsVisible = organizationField.IsVisible;

                if (defaultField.CanChangeRequired)
                    field.IsRequired = organizationField.IsRequired;

                if (defaultField.CanChangeMasked)
                    field.IsMasked = organizationField.IsMasked;

                if (defaultField.CanChangeEditable)
                    field.IsEditable = organizationField.IsEditable;
            }

            if (registrationField != null)
            {
                field.IsVisible = registrationField.IsVisible;
                field.IsRequired = registrationField.IsRequired;
                field.IsEditable = registrationField.IsEditable;
            }

            _initHandler(field);

            return field;
        }
    }
}