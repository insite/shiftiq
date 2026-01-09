using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web.UI;

using Humanizer;

using InSite.Application.Contacts.Read;
using InSite.Application.Groups.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Organizations;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.Web.Data;
using InSite.Web.Security;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Contacts.People.Forms
{
    public partial class Upload : AdminBasePage
    {
        #region Constants

        private const string HomeUrl = "/ui/admin/contacts/home";
        private const string SearchUrl = "/ui/admin/contacts/people/search";

        private const int MaxErrorsCount = 25;

        #endregion

        #region Classes

        private class CsvColumnInfo
        {
            #region Delegates

            public delegate string ValidateValue(string value);

            #endregion

            #region Construction

            public CsvColumnInfo(string name, Expression<Func<PersonImportRow, string>> expr, int csvColumnIndex, bool required, ValidateValue validator = null)
            {
                _targetProperty = (PropertyInfo)((MemberExpression)expr.Body).Member;
                _validateMethod = validator;

                var memberExpression = ((MemberExpression)expr.Body).Expression as MemberExpression;
                _addressPropertyName = memberExpression?.Member?.Name;

                Name = name;
                CsvColumnIndex = csvColumnIndex;
                Required = required;
            }

            #endregion

            #region Properties

            public string Name { get; }
            public int CsvColumnIndex { get; }
            public bool Required { get; }

            #endregion

            #region Fields

            private readonly PropertyInfo _targetProperty;
            private readonly ValidateValue _validateMethod;
            private readonly string _addressPropertyName;

            #endregion

            #region Methods

            public string Validate(string value)
            {
                return _validateMethod?.Invoke(value);
            }

            public void SetValue(PersonImportRow obj, object value)
            {
                if (_addressPropertyName == null)
                    _targetProperty.SetValue(obj, value);
                else if (_addressPropertyName.Equals("HomeAddress"))
                    _targetProperty.SetValue(obj.HomeAddress, value);
                else
                    _targetProperty.SetValue(obj.WorkAddress, value);
            }

            public override string ToString()
            {
                return $"{typeof(PersonImportRow).Name}.{Name}";
            }

            #endregion
        }
        private class PersonImport
        {
            public PersonRowSettings Settings { get; } = new PersonRowSettings();
            public List<PersonImportRow> Rows { get; set; } = new List<PersonImportRow>();
        }

        [Serializable]
        private class PersonAddress
        {
            public string Description { get; set; }
            public string Street1 { get; set; }
            public string Street2 { get; set; }
            public string City { get; set; }
            public string Country { get; set; }
            public string Province { get; set; }
            public string PostalCode { get; set; }

            public Persistence.Address ToAddress()
            {
                return new Persistence.Address()
                {
                    Description = Description,
                    Street1 = Street1,
                    Street2 = Street2,
                    City = City,
                    Country = Country,
                    Province = Province,
                    PostalCode = PostalCode
                };
            }
        }

        [Serializable]
        private class SerializablePersonImportRow
        {
            public SerializablePersonImportRow()
            {

            }
            public SerializablePersonImportRow(PersonImportRow row)
            {
                UniqueIdentifier = row.UniqueIdentifier;
                FirstName = row.FirstName;
                LastName = row.LastName;
                Email = row.Email;
                AlternateEmail = row.AlternateEmail;
                JobTitle = row.JobTitle;
                Honorific = row.Honorific;
                Function = row.Function;
                PersonGender = row.PersonGender;
                PersonLanguage = row.PersonLanguage;
                EmployerGroupIdentifier = row.EmployerGroupIdentifier;

                PersonCode = row.PersonCode;
                SocialInsuranceNumber = row.SocialInsuranceNumber;
                StartDate = row.StartDate;
                Birthdate = row.Birthdate;
                ArchiveStatus = row.ArchiveStatus;
                Password = row.Password;
                TimeZone = row.TimeZone;

                HomeAddress = row.HomeAddress;
                WorkAddress = row.WorkAddress;

                TradeworkerNumber = row.TradeworkerNumber;
                ESL = row.ESL;
                PhonePreferred = row.PhonePreferred;
                PhoneHome = row.PhoneHome;
                PhoneMobile = row.PhoneMobile;
                PhoneOther = row.PhoneOther;
                PhoneWork = row.PhoneWork;
                EmergencyContactName = row.EmergencyContactName;
                EmergencyContactPhone = row.EmergencyContactPhone;
                EmergencyContactRelationship = row.EmergencyContactRelationship;
                EmployeeUnion = row.EmployeeUnion;

                DuplicateProcessChoice = row.DuplicateProcessChoice;

            }

            public string UniqueIdentifier { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string AlternateEmail { get; set; }
            public string JobTitle { get; set; }
            public string Honorific { get; set; }
            public string EmployerGroupIdentifier { get; set; }
            public string Function { get; set; }

            //Other
            public string PersonCode { get; set; }
            public string SocialInsuranceNumber { get; set; }
            public string StartDate { get; set; }
            public string Birthdate { get; set; }
            public string ArchiveStatus { get; set; }
            public string PersonLanguage { get; set; }
            public string PersonGender { get; set; }

            public string Password { get; set; }

            //Address
            public string TimeZone { get; set; }

            public PersonAddress HomeAddress { get; protected set; } = new PersonAddress();
            public PersonAddress WorkAddress { get; protected set; } = new PersonAddress();


            // Entities
            public string TradeworkerNumber { get; internal set; }
            public string ESL { get; internal set; }
            public string PhonePreferred { get; internal set; }
            public string PhoneHome { get; internal set; }
            public string PhoneMobile { get; internal set; }
            public string PhoneOther { get; internal set; }
            public string PhoneWork { get; internal set; }
            public string EmergencyContactName { get; internal set; }
            public string EmergencyContactPhone { get; internal set; }
            public string EmergencyContactRelationship { get; internal set; }
            public string EmployeeUnion { get; internal set; }


            //duplicates
            public int DuplicateProcessChoice { get; set; } = -1;
        }

        private class PersonImportRow : SerializablePersonImportRow
        {
            public PersonImportRow()
            {

            }

            public PersonImportRow(SerializablePersonImportRow row, QUser user, QPerson person)
            {
                UniqueIdentifier = row.UniqueIdentifier;
                FirstName = row.FirstName;
                LastName = row.LastName;
                Email = row.Email;
                AlternateEmail = row.AlternateEmail;
                JobTitle = row.JobTitle;
                Honorific = row.Honorific;
                Function = row.Function;
                PersonGender = row.PersonGender;
                PersonLanguage = row.PersonLanguage;
                EmployerGroupIdentifier = row.EmployerGroupIdentifier;

                PersonCode = row.PersonCode;
                SocialInsuranceNumber = row.SocialInsuranceNumber;
                StartDate = row.StartDate;
                Birthdate = row.Birthdate;
                ArchiveStatus = row.ArchiveStatus;
                Password = row.Password;
                TimeZone = row.TimeZone;

                HomeAddress = row.HomeAddress;
                WorkAddress = row.WorkAddress;

                TradeworkerNumber = row.TradeworkerNumber;
                ESL = row.ESL;
                PhonePreferred = row.PhonePreferred;
                PhoneHome = row.PhoneHome;
                PhoneMobile = row.PhoneMobile;
                PhoneOther = row.PhoneOther;
                PhoneWork = row.PhoneWork;
                EmergencyContactName = row.EmergencyContactName;
                EmergencyContactPhone = row.EmergencyContactPhone;
                EmergencyContactRelationship = row.EmergencyContactRelationship;
                EmployeeUnion = row.EmployeeUnion;

                DuplicateProcessChoice = row.DuplicateProcessChoice;

                User = user;
                Person = person;
            }

            // Entities
            public QUser User { get; set; }
            public QPerson Person { get; set; }
        }

        private class PersonRowSettings
        {
            public int UniqueIdentifierIndex { get; set; }
            public int FirstNameIndex { get; set; }
            public int LastNameIndex { get; set; }
            public int EmailIndex { get; set; }
            public int AlternateEmailIndex { get; set; }
            public int JobTitleIndex { get; set; }
            public int HonorificIndex { get; set; }

            public int PersonCodeIndex { get; set; }
            public int SocialInsuranceNumberIndex { get; set; }
            public int StartDateIndex { get; set; }
            public int BirthdateIndex { get; set; }
            public int ArchiveStatusIndex { get; set; }
            public int PasswordIndex { get; set; }
            public int PersonLanguageIndex { get; set; }
            public int PersonGenderIndex { get; set; }
            public int EmployerGroupIdentifierIndex { get; set; }
            public int FunctionIndex { get; set; }

            public int TimeZoneIndex { get; set; }

            public PersonAddressSettings HomeAddress { get; } = new PersonAddressSettings();
            public PersonAddressSettings WorkAddress { get; } = new PersonAddressSettings();

            public int TradeworkerNumberIndex { get; set; }
            public int ESLIndex { get; set; }
            public int PhonePreferredIndex { get; set; }
            public int PhoneHomeIndex { get; set; }
            public int PhoneMobileIndex { get; set; }
            public int PhoneOtherIndex { get; set; }
            public int PhoneWorkIndex { get; set; }
            public int EmergencyContactNameIndex { get; set; }
            public int EmergencyContactPhoneIndex { get; set; }
            public int EmergencyContactRelationshipIndex { get; set; }
            public int EmployeeUnionIndex { get; set; }
            public int UserTimeZone { get; set; }
        }

        private class PersonAddressSettings
        {
            public int DescriptionIndex { get; set; }
            public int PhoneIndex { get; set; }

            public int Street1Index { get; set; }
            public int Street2Index { get; set; }
            public int CityIndex { get; set; }
            public int CountryIndex { get; set; }
            public int ProvinceIndex { get; set; }
            public int PostalCodeIndex { get; set; }
        }

        private class Contact
        {
            public QUser User { get; }
            public QPerson Person { get; }

            public Contact(QUser user, QPerson person)
            {
                User = user;
                Person = person;

                Person.User = null;
            }
        }

        [Serializable]
        private class StatusHolder
        {
            public AlertType Alert { get; set; }
            public string Message { get; set; }
        }

        [Serializable]
        private class StatusHolders
        {
            public List<StatusHolder> Statuses { get; } = new List<StatusHolder>();
            public void AddMessage(AlertType status, string message)
            {
                Statuses.Add(new StatusHolder
                {
                    Message = message,
                    Alert = status
                });
            }
        }

        #endregion

        #region Fields

        private static readonly HashSet<char> ValidNameCharacters = new HashSet<char>(new[]
        {
            ' ', '\'', '-', '.', ',', '(', ')',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V',
            'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
            'w', 'x', 'y', 'z'
        });

        private Blowfish _blowfish;

        #endregion

        #region Properties

        private string ReturnUrl => Request["return"];

        private Dictionary<string, List<Tuple<AlertType, string, string>>> ProgressStatus
        {
            get
            {
                return (Dictionary<string, List<Tuple<AlertType, string, string>>>)(Session[ProgressStatusKey]
                    ?? (Session[ProgressStatusKey] = new Dictionary<string, List<Tuple<AlertType, string, string>>>()));
            }
        }

        private static readonly string ProgressStatusKey = typeof(Upload) + nameof(ProgressStatus);

        private string[][] Values
        {
            get => (string[][])ViewState[nameof(Values)];
            set => ViewState[nameof(Values)] = value;
        }

        protected string FirstRecordsTitle { get; set; }

        private List<SerializablePersonImportRow> Duplicates
        {
            get => (List<SerializablePersonImportRow>)ViewState[nameof(Duplicates)];
            set => ViewState[nameof(Duplicates)] = value;
        }

        private int CurrentDuplicateIndex
        {
            get => (int?)ViewState[nameof(CurrentDuplicateIndex)] ?? 1;
            set => ViewState[nameof(CurrentDuplicateIndex)] = value;
        }

        private int DuplicatesSkipedCount
        {
            get => (int?)ViewState[nameof(DuplicatesSkipedCount)] ?? 0;
            set => ViewState[nameof(DuplicatesSkipedCount)] = value;
        }

        private int DuplicatesNewEmailCount
        {
            get => (int?)ViewState[nameof(DuplicatesNewEmailCount)] ?? 0;
            set => ViewState[nameof(DuplicatesNewEmailCount)] = value;
        }

        private int DuplicatesConnectedCount
        {
            get => (int?)ViewState[nameof(DuplicatesConnectedCount)] ?? 0;
            set => ViewState[nameof(DuplicatesConnectedCount)] = value;
        }

        private StatusHolders ScreenStatus
        {
            get => (StatusHolders)ViewState[nameof(ScreenStatus)];
            set => ViewState[nameof(ScreenStatus)] = value;
        }

        #endregion

        #region Inialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FileNextButton.Click += FileNextButton_Click;

            FieldsSaveButton.Click += FieldsSaveButton_Click;

            UploadProgress.Triggers.AddControl(FieldsSaveButton);

            OnePersonDuplicateOption.SelectedIndexChanged += (s, a) => OnOnePersonDuplicateOptionChanged();

            OnePersonDuplicateContinueButton.Click += OnePersonDuplicateContinueButton_Click;
            OnePersonDuplicateCloseButton.Click += (s, a) => OnePersonDuplicateWindow.Visible = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (ScreenStatus is null) ScreenStatus = new StatusHolders();
            Server.ScriptTimeout = 60 * 60 * 5; // 5 hours

            if (IsPostBack)
            {
                var s = ScreenStatus;
                if (!(Duplicates?.Any(x => x.DuplicateProcessChoice == -1) ?? true))
                {
                    FinalizeProcess(GetImportData(false), true);
                }
                return;
            }

            if (!CanCreate)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(Page);

            SetNotification();
            ShowCancelMessage();

            GroupSelector.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
            GroupSelector.Filter.OnlyOperatorCanAddUser = !MembershipPermissionHelper.CanModifyAdminMemberships() ? false : (bool?)null;
            GroupSelector.Value = Guid.TryParse(Request["group"], out Guid result) ? result : (Guid?)null;

            var cancelUrl = ReturnUrl.IfNullOrEmpty(HomeUrl);

            FileCancelButton.NavigateUrl = cancelUrl;
            FieldsCancelButton.NavigateUrl = cancelUrl;
            DuplicatesCancelButton.NavigateUrl = cancelUrl;
            CompleteCancelButton.NavigateUrl = cancelUrl;
        }

        protected override void OnPreRender(EventArgs e)
        {
            ImportFile.ClearMetadata();

            base.OnPreRender(e);
        }

        private void ShowCancelMessage()
        {
            var pContextId = Page.Request.QueryString["progressContext"];
            if (pContextId.IsEmpty() || !ProgressStatus.ContainsKey(pContextId))
                return;

            var status = ProgressStatus[pContextId];

            foreach (var info in status)
                ScreenStatusView.AddMessage(info.Item1, info.Item2);

            ProgressStatus.Remove(pContextId);
        }

        private void SetNotification()
        {
            Create.SetNotification(ScreenStatusView);

            if (IsUpdateAllowed())
                ScreenStatusView.AddMessage(AlertType.Information, $"Use this feature to upload new contacts to the database. Contact <a href=\"mailto:{ServiceLocator.Partition.Email}\">{ServiceLocator.Partition.Email}</a> for assistance with bulk updating existing contact records.");
            else
                ScreenStatusView.AddMessage(AlertType.Information, $"Uploaded contacts will be created as new people.");
        }

        #endregion

        #region Event handlers

        private void OnOnePersonDuplicateOptionChanged()
        {
            var value = OnePersonDuplicateOption.SelectedValue;

            OnePersonDuplicateCancelDescription.Visible = value == "Cancel";
            OnePersonDuplicateConnectDescription.Visible = value == "Connect";
            OnePersonDuplicateCreateDescription.Visible = value == "Create";
            OnePersonDuplicateContinueButton.Visible = value.IsNotEmpty();
        }

        private void OnePersonDuplicateContinueButton_Click(object sender, EventArgs e)
        {
            switch (OnePersonDuplicateOption.SelectedValue)
            {
                case "Skip":
                    DuplicatesSkipedCount++;
                    Duplicates[CurrentDuplicateIndex].DuplicateProcessChoice = 0;
                    break;
                case "Connect":
                    DuplicatesConnectedCount++;
                    ConnectPerson();
                    Duplicates[CurrentDuplicateIndex].DuplicateProcessChoice = 1;
                    break;
                case "Create":
                    DuplicatesNewEmailCount++;
                    Duplicates[CurrentDuplicateIndex].DuplicateProcessChoice = 2;
                    InsertDuplicateResolvedPerson();
                    break;
            }
            ProcessDuplicates();
        }

        private void FileNextButton_Click(object sender, EventArgs e)
        {
            FieldsTab.Visible = false;
            DuplicatesTab.Visible = false;
            CompleteTab.Visible = false;

            if (!Page.IsValid)
                return;

            using (var stream = ImportFile.OpenFile())
            {
                if (!LoadFile(stream))
                    return;
            }

            DuplicatesTab.Visible = false;
            CompleteTab.Visible = false;

            FieldsTab.Visible = true;
            FieldsTab.IsSelected = true;

            ScreenStatusView.AddMessage(
                AlertType.Information,
                "Choose the fields into which you want to upload your contact data. " +
                "Required fields are indicated with an asterisk (<sup class=\"text-danger\"><i class=\"far fa-asterisk fa-xs\"></i></sup>).");
        }

        private void FieldsSaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            DuplicatesTab.Visible = false;
            CompleteTab.Visible = false;

            var data = GetImportData(true);

            if (data == null)
            {
                FirstRecordsRepeater.Visible = false;
                LastRecordsRepeater.Visible = false;
            }
            else if (!HasDuplicates(data) && ImportData(data))
            {
                if (!Page.Response.IsClientConnected)
                {
                    var status = ProgressStatus.ContainsKey(UploadProgress.ContextID)
                        ? ProgressStatus[UploadProgress.ContextID]
                        : null;

                    if (status == null)
                        ProgressStatus.Add(UploadProgress.ContextID, status = new List<Tuple<AlertType, string, string>>());

                    foreach (var message in ScreenStatusView.GetMessages())
                        status.Add(message);

                    status.Add(new Tuple<AlertType, string, string>(AlertType.Error, null, "Cancelled by user."));
                }

                if (Duplicates.Count == 0)
                {
                    FinalizeProcess(data, false);
                }
            }
        }

        private void FinalizeProcess(PersonImport data, bool reloadUsers)
        {
            foreach (var item in ScreenStatus.Statuses)
                ScreenStatusView.AddMessage(item.Alert, item.Message);

            if (reloadUsers)
            {
                var contactFilter = data.Rows.Select(x => x.Email).Distinct().ToList();
                var result = ServiceLocator.PersonSearch
                    .GetPersonsByEmails(contactFilter, Organization.OrganizationIdentifier, x => x.User)
                    .ToDictionary(x => x.User.Email, x => new Contact(x.User, x), StringComparer.OrdinalIgnoreCase);

                var result2 = ServiceLocator.PersonSearch
                    .GetPersonsByAlternateEmails(contactFilter, Organization.OrganizationIdentifier, x => x.User)
                    .ToDictionary(x => x.User.EmailAlternate, x => new Contact(x.User, x), StringComparer.OrdinalIgnoreCase);

                foreach (var item in result2)
                {
                    if (!result.ContainsKey(item.Key))
                        result.Add(item.Key, item.Value);
                }

                var removes = new List<PersonImportRow>();
                data.Rows = data.Rows.Where(x => result.Keys.Contains(x.Email)).ToList();
                foreach (var item in data.Rows)
                {
                    item.User = result[item.Email].User;
                    item.Person = result[item.Email].Person;
                    item.Person.User = null;
                    item.User.Persons = null;

                }
            }
            if (ReturnUrl.HasValue())
            {
                Create.SavedIdentifiers = data.Rows
                    .Where(x => x.User != null)
                    .Select(x => x.User.UserIdentifier)
                    .Distinct()
                    .ToArray();

                var url = Create.SavedIdentifiers.Length > 0
                    ? HttpResponseHelper.BuildUrl(ReturnUrl, "userCreated=1")
                    : ReturnUrl;

                HttpResponseHelper.Redirect(url);
            }
            else
            {
                CompleteTab.Visible = true;
                CompleteTab.IsSelected = true;

                LoadPreviewData(data);
            }
        }

        #endregion

        #region Methods (load file)

        private bool LoadFile(Stream file)
        {
            var encoding = Encoding.GetEncoding(FileEncoding.Value);

            try
            {
                Values = CsvImportHelper.GetValues(file, null, false, encoding);
            }
            catch (ApplicationError ex)
            {
                ScreenStatusView.AddMessage(AlertType.Error, ex.Message);

                return false;
            }

            if (!ValidateFileErrors())
                return false;

            //Person
            BindFieldSelectors(UniqueIdentifierSelector, "Email");
            BindFieldSelectors(FirstNameSelector, "FirstName");
            BindFieldSelectors(LastNameSelector, "LastName");
            BindFieldSelectors(EmailSelector, "Email");
            BindFieldSelectors(JobTitleSelector, "JobTitle");
            BindFieldSelectors(HonorificSelector, null);

            //Settings
            BindFieldSelectors(PersonCodeSelector, "Number");
            BindFieldSelectors(SocialInsuranceNumberSelector, null);
            BindFieldSelectors(StartDateSelector, null);
            BindFieldSelectors(BirthdateSelector, null);
            BindFieldSelectors(ArchiveStatusSelector, null);
            BindFieldSelectors(PasswordSelector, null);

            //Address
            BindFieldSelectors(TimeZoneSelector, null);

            //Home Address
            BindFieldSelectors(HomeAddressDescriptionSelector, null);
            BindFieldSelectors(HomeAddressStreet1Selector, null);
            BindFieldSelectors(HomeAddressStreet2Selector, null);
            BindFieldSelectors(HomeAddressCitySelector, null);
            BindFieldSelectors(HomeAddressCountrySelector, null);
            BindFieldSelectors(HomeAddressProvinceSelector, null);
            BindFieldSelectors(HomeAddressPostalCodeSelector, null);

            //Work Address
            BindFieldSelectors(WorkAddressDescriptionSelector, null);
            BindFieldSelectors(WorkAddressStreet1Selector, null);
            BindFieldSelectors(WorkAddressStreet2Selector, null);
            BindFieldSelectors(WorkAddressCitySelector, null);
            BindFieldSelectors(WorkAddressCountrySelector, null);
            BindFieldSelectors(WorkAddressProvinceSelector, null);
            BindFieldSelectors(WorkAddressPostalCodeSelector, null);

            //Other
            BindFieldSelectors(PhonePreferredSelector, null);
            BindFieldSelectors(PhoneHomeSelector, null);
            BindFieldSelectors(PhoneMobileSelector, null);
            BindFieldSelectors(PhoneOtherSelector, null);
            BindFieldSelectors(PhoneWorkSelector, null);
            BindFieldSelectors(EmergencyContactNameSelector, null);
            BindFieldSelectors(EmergencyContactPhoneSelector, null);
            BindFieldSelectors(EmergencyContactRelationshipSelector, null);

            BindFieldSelectors(EmployerGroupIdentifier, null);
            BindFieldSelectors(Language, null);
            BindFieldSelectors(GenderCombo, null);
            BindFieldSelectors(FunctionType, null);
            BindFieldSelectors(EmailAlternate, null);
            return true;
        }

        private bool ValidateFileErrors()
        {
            if (Values.Length < 2)
            {
                ScreenStatusView.AddMessage(AlertType.Error, "The file has no data to import.");
                return false;
            }

            var errorMessage = new MessageBuilder();
            var columnsCount = Values[0].Length;

            for (var rowIndex = 1; rowIndex < Values.Length; rowIndex++)
            {
                var columns = Values[rowIndex];
                if (columns.Length != columnsCount)
                    errorMessage.AddError("Row {0} has invalid columns count", rowIndex + 1);
            }

            if (errorMessage.IsEmpty)
                return true;

            ScreenStatusView.AddMessage(AlertType.Error, BuildErrorMessage(errorMessage));

            return false;
        }

        private void BindFieldSelectors(ComboBox selector, string defaultField)
        {
            selector.Items.Clear();
            selector.Items.Add(new ComboBoxOption());

            var values = Values[0];
            for (var i = 0; i < values.Length; i++)
            {
                var fieldName = values[i];

                selector.Items.Add(new ComboBoxOption(fieldName, i.ToString())
                {
                    Selected = fieldName.Equals(defaultField, StringComparison.OrdinalIgnoreCase)
                });
            }
        }

        #endregion

        #region Methods (import data)

        private bool HasDuplicates(PersonImport data)
        {
            var duplicates = data.Rows
                .GroupBy(x => x.Email, StringComparer.OrdinalIgnoreCase)
                .Select(g => new { Email = g.Key, Count = g.Count() })
                .Where(x => x.Count > 1)
                .ToArray();

            if (duplicates.Length == 0)
                return false;

            CompleteTab.Visible = false;
            DuplicatesTab.Visible = true;
            DuplicatesTab.IsSelected = true;

            var message = new StringBuilder();
            message.Append("<p>Duplicate login names found:</p><ul>");

            foreach (var info in duplicates.OrderByDescending(x => x.Count))
                message.AppendFormat("<li>{0} <small>({1:n0} contacts)</small></li>", info.Email, info.Count);

            message.Append("</ul>");

            DuplicatesStatus.AddMessage(AlertType.Error, message.ToString());

            return true;
        }

        private bool ImportData(PersonImport data)
        {
            try
            {
                var totalCount = data.Rows.Count;
                var processedCount = 0;
                var progressCount = 0;
                var groupId = GetGroupID();
                var isUpdateAllowed = IsUpdateAllowed();

                UpdateImportProgress();

                var organization = Organization;

                var cache = new List<PersonImportRow>();
                Duplicates = new List<SerializablePersonImportRow>();

                foreach (var row in data.Rows)
                {
                    cache.Add(row);

                    if (cache.Count >= 500)
                    {
                        processedCount += SavePersons(organization, cache, data.Settings, groupId, EnableLoginCredentials.Checked, isUpdateAllowed, OnImportRowUpdated);

                        progressCount = processedCount;

                        UpdateImportProgress();

                        cache.Clear();
                    }

                    if (!Page.Response.IsClientConnected)
                        break;
                }

                if (cache.Count > 0)
                {
                    processedCount += SavePersons(organization, cache, data.Settings, groupId, EnableLoginCredentials.Checked, isUpdateAllowed, OnImportRowUpdated);

                    progressCount = processedCount;

                    UpdateImportProgress();

                    cache.Clear();
                }

                if (processedCount == 0)
                {
                    if (Duplicates.Count == 0)
                    {
                        ScreenStatusView.AddMessage(AlertType.Warning, "No contacts uploaded to the database.");
                    }

                    //Note: the 'LoadDuplicatesView' function should always be called immediately
                    //before the return statement in 'ImportData' function of this page.
                    ProcessDuplicates();
                    return false;
                }

                var message = $"{"contact".ToQuantity(processedCount, "N0")} successfully uploaded.";

                if (groupId.HasValue)
                {
                    var group = ServiceLocator.GroupSearch.GetGroup(groupId.Value);

                    message = $"{"contact".ToQuantity(processedCount, "N0")} successfully uploaded to the {group.GroupType} <a href='/ui/admin/contacts/groups/edit?contact={group.GroupIdentifier}'>{group.GroupName}</a>.";
                }

                ScreenStatus.AddMessage(AlertType.Success, message);

                //Note: the 'LoadDuplicatesView' function should always be called immediately
                //before the return statement in 'ImportData' function of this page.
                ProcessDuplicates();
                return true;

                void OnImportRowUpdated()
                {
                    progressCount++;

                    UpdateImportProgress();
                }

                void UpdateImportProgress()
                {
                    SetProgressStatus(progressCount, totalCount);
                }
            }
            finally
            {
                UploadProgress.RemoveContext();
            }
        }

        private void ProcessDuplicates()
        {
            if (Duplicates.Count > 0)
            {
                OnePersonDuplicateWindow.Visible = true;
                int i = 0;
                foreach (var row in Duplicates)
                {
                    var userEmail = Duplicates[i].Email;
                    CurrentDuplicateIndex = i;
                    i++;
                    if (row.DuplicateProcessChoice != -1) continue;
                    OnePersonDuplicateOption.SelectedIndex = -1;
                    OnOnePersonDuplicateOptionChanged();

                    if (PopulateDuplicatePersonGrid(userEmail))
                        continue;

                    OnePersonDuplicateCounter.Attributes["class"] = "alert alert-info";
                    OnePersonDuplicateCounter.InnerHtml = $"Resolving duplicate user #{i} from {Duplicates.Count}";

                    OnePersonDuplicateMessage.Attributes["class"] = "alert alert-danger";
                    OnePersonDuplicateMessage.InnerHtml = $"There is another existing user account with the same email address.";

                    Duplicates[i - 1].AlternateEmail = UserSearch.CreateUniqueEmailFromDuplicate(userEmail);
                    OnePersonDuplicateDuplicateEmail.Text = Duplicates[i - 1].AlternateEmail;
                    OnePersonDuplicateDuplicateEmailAlternate.Text = userEmail;
                    break;

                }
                FinalizeDuplicateInsertion();
            }
        }

        private bool PopulateDuplicatePersonGrid(string userEmail)
        {
            var anyPerson = PersonSearch.SelectByEmail(null, userEmail)
                ?? throw new ArgumentNullException($"The user {userEmail} is not assigned to any organization.");

            var filter = new PersonFilter
            {
                OrganizationIdentifier = anyPerson.OrganizationIdentifier,
                EmailContains = userEmail
            };

            OnePersonDuplicateWindow.Visible = true;
            OnePersonDuplicateGrid.DisablePaging();
            OnePersonDuplicateGrid.LoadData(filter, new[] { "Name", "Email", "City" }, null);

            if (!OnePersonDuplicateGrid.HasRows)
            {
                OnePersonDuplicateWindow.Visible = false;
                return true;
            }
            return false;
        }

        private void FinalizeDuplicateInsertion()
        {
            if (!Duplicates.Any(x => x.DuplicateProcessChoice == -1))
            {
                OnePersonDuplicateWindow.Visible = false;
                if (DuplicatesSkipedCount > 0)
                {
                    var message = $"{"duplicate contact".ToQuantity(DuplicatesSkipedCount, "N0")} skipped.";


                    ScreenStatus.AddMessage(AlertType.Success, message);
                }

                if (DuplicatesConnectedCount > 0)
                {
                    var message = $"{"duplicate contact".ToQuantity(DuplicatesConnectedCount, "N0")} successfully connected to their existing user account in the system.";

                    ScreenStatus.AddMessage(AlertType.Success, message);
                }

                if (DuplicatesNewEmailCount > 0)
                {
                    var message = $"{"duplicate contact".ToQuantity(DuplicatesNewEmailCount, "N0")} had new user account with new alternate email address created for them.";

                    ScreenStatus.AddMessage(AlertType.Success, message);
                }

                ScriptManager.RegisterStartupScript(this, GetType(),
                    "postback", "javascript:__doPostBack('', '');", true);
            }
        }

        private void ConnectPerson()
        {
            var duplicateUser = Duplicates[CurrentDuplicateIndex];
            var user = ServiceLocator.UserSearch.GetUserByEmail(duplicateUser.Email);
            if (user == null)
            {
                InsertDuplicateResolvedPerson();
                return;
            }

            var person = UserFactory.CreatePerson(Organization.Identifier);

            var row = new PersonImportRow(duplicateUser, new QUser(), person);
            row.Person.UserIdentifier = user.UserIdentifier;
            var rowSettings = SetupSettings();
            SetUserFields(row, rowSettings, EnableLoginCredentials.Checked, null);
            row.User = user;

            InsertUpdateUser(false, row, rowSettings, GetGroupID(), true);
        }

        private void InsertDuplicateResolvedPerson()
        {
            var duplicateUser = Duplicates[CurrentDuplicateIndex];
            if (duplicateUser.AlternateEmail == duplicateUser.Email)
            {
                ScreenStatus.AddMessage(AlertType.Error, $"The user account {duplicateUser.Email} is already registered under another organization account. Please contact your system administrator to register this person for {Organization.CompanyName}.");
                return;
            }

            var newUser = new QUser();
            newUser.TimeZone = Organization.TimeZone.Id;

            var newPerson = UserFactory.CreatePerson(Organization.OrganizationIdentifier);
            var row = new PersonImportRow(duplicateUser, newUser, newPerson);

            (row.AlternateEmail, row.Email) = (row.Email, row.AlternateEmail);
            var rowSettings = SetupSettings();
            SetUserFields(row, rowSettings, EnableLoginCredentials.Checked, null);
            InsertUpdateUser(true, row, rowSettings, GetGroupID(), false);
        }

        private void LoadPreviewData(PersonImport data)
        {
            var updatedRows = data.Rows.Where(x => x.User != null).GroupBy(x => x.User.UserIdentifier).Select(g => g.First()).ToArray();
            if (updatedRows.Length == 0)
                return;

            FirstRecordsRepeater.Visible = true;

            if (updatedRows.Length <= 10)
            {
                FirstRecordsTitle = string.Empty;

                FirstRecordsRepeater.DataSource = updatedRows;
                FirstRecordsRepeater.DataBind();

                LastRecordsRepeater.Visible = false;

                return;
            }

            FirstRecordsTitle = "First 5 contacts:";

            var firstRecords = new List<PersonImportRow>();
            for (var i = 0; i < 5; i++)
                firstRecords.Add(updatedRows[i]);

            var lastRecords = new List<PersonImportRow>();
            for (var i = updatedRows.Length - 5; i < updatedRows.Length; i++)
                lastRecords.Add(updatedRows[i]);

            FirstRecordsRepeater.DataSource = firstRecords;
            FirstRecordsRepeater.DataBind();

            LastRecordsRepeater.Visible = true;

            LastRecordsRepeater.DataSource = lastRecords;
            LastRecordsRepeater.DataBind();
        }

        private Guid? GetGroupID()
        {
            if (GroupSelectorView.IsActive)
            {
                return GroupSelector.Value.HasValue && MembershipPermissionHelper.CanModifyMembership(GroupSelector.Value.Value)
                    ? GroupSelector.Value
                    : (Guid?)null;
            }

            if (GroupText.Text.IsEmpty())
                return null;

            var filter = new QGroupFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                GroupName = GroupText.Text
            };

            var list = ServiceLocator.GroupSearch.GetGroups(filter);
            if (list.Count > 0 && !StringHelper.Equals(list[0].GroupType, "Person"))
                return list[0].GroupIdentifier;

            var id = UniqueIdentifier.Create();

            ServiceLocator.SendCommand(new CreateGroup(id, Organization.Identifier, GroupTypes.List, GroupText.Text));

            return id;
        }

        private PersonImport GetImportData(bool showErrors)
        {
            var result = new PersonImport();

            SetupSettings(result.Settings);

            var columns = CreateColumns(result.Settings);

            var errorMessage = new MessageBuilder();

            for (var rowIndex = 1; rowIndex < Values.Length; rowIndex++)
            {
                var rowColumns = Values[rowIndex];
                var personRow = new PersonImportRow();

                foreach (var column in columns)
                {
                    var value = column.CsvColumnIndex >= 0 ? rowColumns[column.CsvColumnIndex] : null;

                    if (string.IsNullOrEmpty(value) && column.Required)
                    {
                        errorMessage.AddError(
                            "Row {0} is missing the '{1}' ({2}) column value. The {0} is required column",
                            rowIndex + 1, column.Name, column.CsvColumnIndex + 1);
                    }
                    else
                    {
                        var valueFormatError = column.Validate(value);
                        if (!string.IsNullOrEmpty(valueFormatError))
                            errorMessage.AddError(
                                "Row {0} contains incorrect value format in column {1} ({2}): {3}",
                                rowIndex + 1, column.Name, column.CsvColumnIndex + 1, valueFormatError);
                    }

                    if (!errorMessage.IsEmpty)
                        continue;

                    var trimmedValue = StringHelper.TrimAndClean(value);
                    trimmedValue = string.IsNullOrEmpty(trimmedValue) ? null : trimmedValue;

                    column.SetValue(personRow, trimmedValue);
                }

                result.Rows.Add(personRow);
            }

            if (errorMessage.IsEmpty)
                return result;

            if (showErrors) ScreenStatusView.AddMessage(AlertType.Error, BuildErrorMessage(errorMessage));

            return null;
        }

        private PersonRowSettings SetupSettings(PersonRowSettings settings = null)
        {
            if (settings == null)
                settings = new PersonRowSettings();
            // Person
            settings.UniqueIdentifierIndex = UniqueIdentifierSelector.ValueAsInt ?? -1;
            settings.FirstNameIndex = FirstNameSelector.ValueAsInt ?? -1;
            settings.LastNameIndex = LastNameSelector.ValueAsInt ?? -1;
            settings.EmailIndex = EmailSelector.ValueAsInt ?? -1;
            settings.JobTitleIndex = JobTitleSelector.ValueAsInt ?? -1;
            settings.HonorificIndex = HonorificSelector.ValueAsInt ?? -1;

            // Settings
            settings.PersonCodeIndex = PersonCodeSelector.ValueAsInt ?? -1;
            settings.SocialInsuranceNumberIndex = SocialInsuranceNumberSelector.ValueAsInt ?? -1;
            settings.StartDateIndex = StartDateSelector.ValueAsInt ?? -1;
            settings.BirthdateIndex = BirthdateSelector.ValueAsInt ?? -1;
            settings.ArchiveStatusIndex = ArchiveStatusSelector.ValueAsInt ?? -1;
            settings.PasswordIndex = PasswordSelector.ValueAsInt ?? -1;

            // Address
            settings.TimeZoneIndex = TimeZoneSelector.ValueAsInt ?? -1;

            // Home Address
            settings.HomeAddress.DescriptionIndex = HomeAddressDescriptionSelector.ValueAsInt ?? -1;
            settings.HomeAddress.Street1Index = HomeAddressStreet1Selector.ValueAsInt ?? -1;
            settings.HomeAddress.Street2Index = HomeAddressStreet2Selector.ValueAsInt ?? -1;
            settings.HomeAddress.CityIndex = HomeAddressCitySelector.ValueAsInt ?? -1;
            settings.HomeAddress.CountryIndex = HomeAddressCountrySelector.ValueAsInt ?? -1;
            settings.HomeAddress.ProvinceIndex = HomeAddressProvinceSelector.ValueAsInt ?? -1;
            settings.HomeAddress.PostalCodeIndex = HomeAddressPostalCodeSelector.ValueAsInt ?? -1;

            // Work Address
            settings.WorkAddress.DescriptionIndex = WorkAddressDescriptionSelector.ValueAsInt ?? -1;
            settings.WorkAddress.Street1Index = WorkAddressStreet1Selector.ValueAsInt ?? -1;
            settings.WorkAddress.Street2Index = WorkAddressStreet2Selector.ValueAsInt ?? -1;
            settings.WorkAddress.CityIndex = WorkAddressCitySelector.ValueAsInt ?? -1;
            settings.WorkAddress.CountryIndex = WorkAddressCountrySelector.ValueAsInt ?? -1;
            settings.WorkAddress.ProvinceIndex = WorkAddressProvinceSelector.ValueAsInt ?? -1;
            settings.WorkAddress.PostalCodeIndex = WorkAddressPostalCodeSelector.ValueAsInt ?? -1;

            // Other

            settings.PhonePreferredIndex = PhonePreferredSelector.ValueAsInt ?? -1;
            settings.PhoneHomeIndex = PhoneHomeSelector.ValueAsInt ?? -1;
            settings.PhoneMobileIndex = PhoneMobileSelector.ValueAsInt ?? -1;
            settings.PhoneOtherIndex = PhoneOtherSelector.ValueAsInt ?? -1;
            settings.PhoneWorkIndex = PhoneWorkSelector.ValueAsInt ?? -1;
            settings.EmergencyContactNameIndex = EmergencyContactNameSelector.ValueAsInt ?? -1;
            settings.EmergencyContactPhoneIndex = EmergencyContactPhoneSelector.ValueAsInt ?? -1;
            settings.EmergencyContactRelationshipIndex = EmergencyContactRelationshipSelector.ValueAsInt ?? -1;

            settings.EmployerGroupIdentifierIndex = EmployerGroupIdentifier.ValueAsInt ?? -1;
            settings.AlternateEmailIndex = EmailAlternate.ValueAsInt ?? -1;
            settings.PersonGenderIndex = GenderCombo.ValueAsInt ?? -1;
            settings.PersonLanguageIndex = Language.ValueAsInt ?? -1;
            settings.FunctionIndex = FunctionType.ValueAsInt ?? -1;

            return settings;
        }

        private List<CsvColumnInfo> CreateColumns(PersonRowSettings settings)
        {
            var columns = new List<CsvColumnInfo>
            {
                // Person
                new CsvColumnInfo("Unique Identifier", x => x.UniqueIdentifier, settings.UniqueIdentifierIndex, true, null),
                new CsvColumnInfo("First Name", x => x.FirstName, settings.FirstNameIndex, true, ValidateName),
                new CsvColumnInfo("Last Name", x => x.LastName, settings.LastNameIndex, true, ValidateName),
                new CsvColumnInfo("Email", x => x.Email, settings.EmailIndex, true, ValidateEmail),
                new CsvColumnInfo("Job Title", x => x.JobTitle, settings.JobTitleIndex, false, null),
                new CsvColumnInfo("Honorific", x => x.Honorific, settings.HonorificIndex, false, null),
                new CsvColumnInfo("Email Alternate", x => x.AlternateEmail, settings.AlternateEmailIndex, false, null),
                new CsvColumnInfo("Person Gender", x => x.PersonGender, settings.PersonGenderIndex, false, null),
                new CsvColumnInfo("Employed By", x => x.EmployerGroupIdentifier, settings.EmployerGroupIdentifierIndex, false, null),
                new CsvColumnInfo("Function", x => x.Function, settings.FunctionIndex, false, null),

                // Settings
                new CsvColumnInfo("Person Code", x => x.PersonCode, settings.PersonCodeIndex, false, null),
                new CsvColumnInfo("Social Insurance Number", x => x.SocialInsuranceNumber, settings.SocialInsuranceNumberIndex, false, null),
                new CsvColumnInfo("Start Date", x => x.StartDate, settings.StartDateIndex, false, ValidateDate),
                new CsvColumnInfo("Birthdate", x => x.Birthdate, settings.BirthdateIndex, false, ValidateDate),
                new CsvColumnInfo("Archive Status", x => x.ArchiveStatus, settings.ArchiveStatusIndex, false, null),
                new CsvColumnInfo("Password", x => x.Password, settings.PasswordIndex, false, null),

                // Address
                new CsvColumnInfo("Time Zone", x => x.TimeZone, settings.TimeZoneIndex, false, ValidateTimeZone),

                // Home Address
                new CsvColumnInfo("Home Address Description", x => x.HomeAddress.Description, settings.HomeAddress.DescriptionIndex, false, null),
                new CsvColumnInfo("Home Address Street1", x => x.HomeAddress.Street1, settings.HomeAddress.Street1Index, false, null),
                new CsvColumnInfo("Home Address Street2", x => x.HomeAddress.Street2, settings.HomeAddress.Street2Index, false, null),
                new CsvColumnInfo("Home Address City", x => x.HomeAddress.City, settings.HomeAddress.CityIndex, false, null),
                new CsvColumnInfo("Home Address Country", x => x.HomeAddress.Country, settings.HomeAddress.CountryIndex, false, null),
                new CsvColumnInfo("Home Address Province", x => x.HomeAddress.Province, settings.HomeAddress.ProvinceIndex, false, null),
                new CsvColumnInfo("Home Address Postal Code", x => x.HomeAddress.PostalCode, settings.HomeAddress.PostalCodeIndex, false, null),

                // Work Address
                new CsvColumnInfo("Work Address Description", x => x.WorkAddress.Description, settings.WorkAddress.DescriptionIndex, false, null),
                new CsvColumnInfo("Work Address Street1", x => x.WorkAddress.Street1, settings.WorkAddress.Street1Index, false, null),
                new CsvColumnInfo("Work Address Street2", x => x.WorkAddress.Street2, settings.WorkAddress.Street2Index, false, null),
                new CsvColumnInfo("Work Address City", x => x.WorkAddress.City, settings.WorkAddress.CityIndex, false, null),
                new CsvColumnInfo("Work Address Country", x => x.WorkAddress.Country, settings.WorkAddress.CountryIndex, false, null),
                new CsvColumnInfo("Work Address Province", x => x.WorkAddress.Province, settings.WorkAddress.ProvinceIndex, false, null),
                new CsvColumnInfo("Work Address Postal Code", x => x.WorkAddress.PostalCode, settings.WorkAddress.PostalCodeIndex, false, null),

                // Other
                new CsvColumnInfo("Phone Preferred", x => x.PhonePreferred, settings.PhonePreferredIndex, false, null),
                new CsvColumnInfo("Phone Home", x => x.PhoneHome, settings.PhoneHomeIndex, false, null),
                new CsvColumnInfo("Phone Mobile", x => x.PhoneMobile, settings.PhoneMobileIndex, false, null),
                new CsvColumnInfo("Phone Other", x => x.PhoneOther, settings.PhoneOtherIndex, false, null),
                new CsvColumnInfo("Phone Work", x => x.PhoneWork, settings.PhoneWorkIndex, false, null),
                new CsvColumnInfo("Emergency Contact Name", x => x.EmergencyContactName, settings.EmergencyContactNameIndex, false, null),
                new CsvColumnInfo("Emergency Contact Phone", x => x.EmergencyContactPhone, settings.EmergencyContactPhoneIndex, false, null),
                new CsvColumnInfo("Emergency Contact Relationship", x => x.EmergencyContactRelationship, settings.EmergencyContactRelationshipIndex, false, null),
                new CsvColumnInfo("Person Language", x => x.PersonLanguage, settings.PersonLanguageIndex, false, null)
            };

            return columns;
        }

        private static string ValidateName(string value)
        {
            var message = new StringBuilder();

            if (!string.IsNullOrEmpty(value))
            {
                var normalizedValue = value.Normalize(NormalizationForm.FormD);
                for (var i = 0; i < normalizedValue.Length; i++)
                {
                    var ch = normalizedValue[i];
                    if (CharUnicodeInfo.GetUnicodeCategory(ch) == UnicodeCategory.NonSpacingMark)
                        continue;

                    if (!ValidNameCharacters.Contains(ch))
                        message.AppendFormat(", '{0}'", ch);
                }

                if (message.Length > 0)
                    message.Remove(0, 2).Insert(0, "person name contains invalid characters (").Append(")");
            }

            return message.Length == 0 ? null : message.ToString();
        }

        private static string ValidateEmail(string value)
        {
            return !string.IsNullOrEmpty(value) && !EmailAddress.IsValidAddress(value)
                ? string.Format("'{0}' is invalid email address", value)
                : null;
        }

        private static string ValidateDate(string value)
        {
            return !string.IsNullOrEmpty(value) && !DateTime.TryParse(value, out var date)
                ? string.Format("'{0}' is invalid date value", value)
                : null;
        }

        private static string ValidateTimeZone(string value)
        {
            return !string.IsNullOrEmpty(value) && TimeZones.GetInfo(value) == null
                ? $"'{value}' is unsupported time zone"
                : null;
        }


        #endregion

        #region Methods (save)

        private string[] SupportedUniqueIdentifierTypes => new string[] { "Email", "PersonCode" };

        private Dictionary<string, Contact> GetContactsByUniqueKeyType(string uniqueKeyType, string[] contactFilter)
        {
            Dictionary<string, Contact> result;

            if (uniqueKeyType == "PersonCode")
            {
                result = ServiceLocator.PersonSearch
                    .GetPersonsByPersonCodes(contactFilter, Organization.OrganizationIdentifier,
                        x => x.User.Memberships,
                        x => x.HomeAddress,
                        x => x.WorkAddress
                    )
                    .ToDictionary(x => x.PersonCode, x => new Contact(x.User, x), StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                result = ServiceLocator.PersonSearch
                    .GetPersonsByEmails(contactFilter, Organization.OrganizationIdentifier,
                        x => x.User.Memberships,
                        x => x.HomeAddress,
                        x => x.WorkAddress
                    )
                    .ToDictionary(x => x.User.Email, x => new Contact(x.User, x), StringComparer.OrdinalIgnoreCase);
            }

            foreach (var contact in result.Values)
            {
                contact.User.Persons = null;
                contact.Person.User = null;
            }

            return result;
        }

        private int SavePersons(
            OrganizationState organization,
            IList<PersonImportRow> rows,
            PersonRowSettings settings,
            Guid? groupId,
            bool enableLoginCredentials,
            bool isUpdateAllowed,
            Action onRowProcessed
            )
        {
            var updatedCount = 0;
            var allowApprove = enableLoginCredentials;
            var uniqueKeyType = UniqueIdentifierTypeSelector.Value;

            if (!SupportedUniqueIdentifierTypes.Contains(uniqueKeyType))
            {
                ScreenStatusView.AddMessage(
                    AlertType.Error,
                    $"Selected Unique Identifier Type is not supported (supports only: {string.Join(", ", SupportedUniqueIdentifierTypes)})");
                return 0;
            }

            var contactFilter = rows
                .Select(x => x.UniqueIdentifier)
                .Distinct().ToArray();

            var contactsByUniqueKey = GetContactsByUniqueKeyType(uniqueKeyType, contactFilter);

            var approvedContactsByUniqueKey = allowApprove
                ? contactsByUniqueKey.Where(x => x.Value.Person.UserAccessGranted.HasValue).ToDictionary(x => x.Key, x => x.Value)
                : null;

            foreach (var row in rows)
            {
                if (!Page.Response.IsClientConnected)
                    break;

                var isNew = !contactsByUniqueKey.TryGetValue(row.UniqueIdentifier, out var oldContact);

                row.PersonCode = AdjustString(row.PersonCode, 32);

                if (!ValidateUser(isNew, row, settings, oldContact, contactsByUniqueKey, organization, isUpdateAllowed))
                    continue;

                if (!SetUserFields(row, settings, allowApprove, approvedContactsByUniqueKey))
                    continue;

                InsertUpdateUser(isNew, row, settings, groupId, false);

                updatedCount++;

                onRowProcessed();
            }

            if (!Page.Response.IsClientConnected)
                updatedCount = 0;

            return updatedCount;
        }

        private void InsertUpdateUser(bool isNew, PersonImportRow row, PersonRowSettings settings, Guid? groupId, bool newPerson)
        {
            var user = row.User;
            var person = row.Person;

            if (settings.SocialInsuranceNumberIndex >= 0 || settings.PersonCodeIndex >= 0 || settings.EmployeeUnionIndex >= 0)
            {
                if (settings.SocialInsuranceNumberIndex >= 0)
                {
                    var socialInsuranceNumber = AdjustString(row.SocialInsuranceNumber, 32);
                    person.SocialInsuranceNumber = socialInsuranceNumber.HasValue()
                        ? GetBlowfish().EncipherString(socialInsuranceNumber)
                        : null;
                }

                if (settings.PersonCodeIndex >= 0)
                    person.PersonCode = row.PersonCode;

                if (settings.EmployeeUnionIndex >= 0)
                    person.EmployeeUnion = AdjustString(row.EmployeeUnion, 32);
            }

            if (isNew)
            {
                user.MultiFactorAuthentication = Organization.Toolkits.Contacts?.DefaultMFA ?? false;
                UserStore.Insert(user, person);
            }
            else if (newPerson)
            {
                PersonStore.Insert(person);
            }
            else
            {
                UserStore.Update(user, OrganizationSearch.GetPersonFullNamePolicy(Organization.Identifier));
                PersonStore.Update(person);
            }

            if (groupId.HasValue && !user.Memberships.Any(x => x.GroupIdentifier == groupId.Value))
            {
                MembershipHelper.Save(new Membership
                {
                    GroupIdentifier = groupId.Value,
                    UserIdentifier = user.UserIdentifier,
                    Assigned = DateTimeOffset.UtcNow
                });
            }

            if (person.EmployerGroupIdentifier.HasValue
                && !user.Memberships.Any(x => x.GroupIdentifier == person.EmployerGroupIdentifier.Value)
                && MembershipPermissionHelper.CanModifyMembership(person.EmployerGroupIdentifier.Value)
                )
            {
                MembershipHelper.Save(new Membership
                {
                    GroupIdentifier = person.EmployerGroupIdentifier.Value,
                    UserIdentifier = user.UserIdentifier,
                    Assigned = DateTimeOffset.UtcNow,
                    MembershipType = row.Function
                });
            }
        }

        private bool SetUserFields(
            PersonImportRow row,
            PersonRowSettings settings,
            bool allowApprove,
            Dictionary<string, Contact> approvedContactsByUniqueKey)
        {
            if (row == null)
                throw new ArgumentNullException("row");

            var user = row.User ?? throw new ArgumentNullException("row.User");
            var person = row.Person ?? throw new ArgumentNullException("row.Person");

            var uniqueKeyType = UniqueIdentifierTypeSelector.Value;
            var uniqueKey = row.UniqueIdentifier;

            SetAddress(person, ContactAddressType.Home, row.HomeAddress, settings.HomeAddress);
            SetAddress(person, ContactAddressType.Work, row.WorkAddress, settings.WorkAddress);

            user.Email = FixCaseOnEmail(row.Email);
            user.EmailAlternate = FixCaseOnEmail(row.AlternateEmail);
            person.EmailAlternateEnabled = true;
            person.EmailEnabled = true;
            user.FirstName = FixCaseOnName(row.FirstName);
            user.LastName = FixCaseOnName(row.LastName);
            user.FullName = string.IsNullOrEmpty(user.MiddleName) || user.Email != null && user.Email.EndsWith(ServiceLocator.AppSettings.Security.Domain) || !PersonSearch.IsUserAssignedToOrganization(user.UserIdentifier, OrganizationIdentifiers.RCABC)
                ? $"{user.FirstName} {user.LastName}"
                : $"{user.FirstName} {user.MiddleName} {user.LastName}";

            if (settings.JobTitleIndex >= 0)
                person.JobTitle = AdjustString(row.JobTitle, 256);

            if (settings.HonorificIndex >= 0)
                user.Honorific = AdjustString(row.Honorific, 32);

            if (settings.PersonLanguageIndex >= 0)
                person.Language = AdjustString(row.PersonLanguage, 2);

            if (settings.PersonGenderIndex >= 0)
                person.Gender = AdjustString(row.PersonGender, 20);

            if (settings.EmployerGroupIdentifierIndex >= 0)
            {
                if (Guid.TryParse(row.EmployerGroupIdentifier, out var employerGroupIdentifier))
                {
                    person.EmployerGroupIdentifier = employerGroupIdentifier;

                    string function = null;

                    if (settings.FunctionIndex >= 0)
                        function = AdjustString(row.Function, 20);

                    AddMembership(person.UserIdentifier, person.EmployerGroupIdentifier, function);
                }
            }

            if (settings.StartDateIndex >= 0)
                person.MemberStartDate = ParseDate(row.StartDate);

            if (settings.BirthdateIndex >= 0)
                person.Birthdate = ParseDate(row.Birthdate);

            if (settings.ArchiveStatusIndex >= 0)
                if (row.ArchiveStatus == "Archived")
                {
                    user.UtcArchived = DateTime.UtcNow;
                    user.UtcUnarchived = null;
                }
                else
                {
                    user.UtcUnarchived = DateTime.UtcNow;
                    user.UtcArchived = null;
                }

            if (settings.TimeZoneIndex >= 0 && !string.IsNullOrEmpty(row.TimeZone))
                user.TimeZone = TimeZones.GetInfo(row.TimeZone).Id;

            if (settings.TradeworkerNumberIndex >= 0)
                person.TradeworkerNumber = AdjustString(row.TradeworkerNumber, 20);

            if (settings.ESLIndex >= 0)
                person.FirstLanguage = GetFirstLanguage(row.ESL);

            if (settings.PhonePreferredIndex >= 0)
                person.Phone = AdjustString(row.PhonePreferred, 30);

            if (settings.PhoneHomeIndex >= 0)
                person.PhoneHome = AdjustString(row.PhoneHome, 32);

            if (settings.PhoneMobileIndex >= 0)
                user.PhoneMobile = AdjustString(row.PhoneMobile, 32);

            if (settings.PhoneOtherIndex >= 0)
                person.PhoneOther = AdjustString(row.PhoneOther, 32);

            if (settings.PhoneWorkIndex >= 0)
                person.PhoneWork = AdjustString(row.PhoneWork, 32);

            if (settings.EmergencyContactNameIndex >= 0)
                person.EmergencyContactName = AdjustString(row.EmergencyContactName, 100);

            if (settings.EmergencyContactPhoneIndex >= 0)
                person.EmergencyContactPhone = AdjustString(row.EmergencyContactPhone, 32);

            if (settings.EmergencyContactRelationshipIndex >= 0)
                person.EmergencyContactRelationship = AdjustString(row.EmergencyContactRelationship, 50);

            if (allowApprove && person.UserAccessGranted == null)
            {
                if (approvedContactsByUniqueKey != null && approvedContactsByUniqueKey.ContainsKey(uniqueKey) && approvedContactsByUniqueKey[uniqueKey].User?.UserIdentifier != user.UserIdentifier)
                {
                    ScreenStatusView.AddMessage(AlertType.Error, $"There is another user already registered with the {uniqueKeyType} <strong>{uniqueKey}</strong>.");
                    return false;
                }

                person.UserAccessGranted = DateTimeOffset.UtcNow;
                person.UserAccessGrantedBy = User.FullName;
            }

            if (settings.PasswordIndex >= 0 && !string.IsNullOrEmpty(row.Password))
            {
                user.SetPassword(row.Password);
                user.UserPasswordExpired = DateTimeOffset.UtcNow;
            }
            else if (user.IsNullPassword())
                user.SetDefaultPassword();

            return true;
        }

        private static string GetFirstLanguage(string esl)
        {
            return string.Equals(esl, "yes", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(esl, "true", StringComparison.OrdinalIgnoreCase)
                    || esl == "1"
                ? "Not English"
                : "English";
        }

        private void AddMembership(Guid userIdentifier, Guid? employerGroupIdentifier, string function)
        {
            if (employerGroupIdentifier.HasValue
                && MembershipPermissionHelper.CanModifyMembership(employerGroupIdentifier.Value)
                )
            {
                MembershipHelper.Save(new Membership
                {
                    UserIdentifier = userIdentifier,
                    GroupIdentifier = employerGroupIdentifier.Value,
                    Assigned = DateTimeOffset.UtcNow,
                    MembershipType = function,
                });
            }
        }

        private bool ValidateUser(
            bool isNew,
            PersonImportRow row,
            PersonRowSettings settings,
            Contact oldContact,
            Dictionary<string, Contact> contactsByUniqueKey,
            OrganizationState organization,
            bool isUpdateAllowed
            )
        {
            var uniqueKeyType = UniqueIdentifierTypeSelector.Value;

            if (settings.PersonCodeIndex >= 0 && !string.IsNullOrEmpty(row.PersonCode))
            {
                var userId = PersonCriteria.BindFirst(x => (Guid?)x.UserIdentifier, new PersonFilter
                {
                    OrganizationIdentifier = organization.OrganizationIdentifier,
                    CodeExact = row.PersonCode
                });

                if (userId.HasValue && (isNew || oldContact.User.UserIdentifier != userId.Value))
                {
                    ScreenStatusView.AddMessage(AlertType.Error, $"The person code {row.PersonCode} is already used by another user in {organization.CompanyName}. The person code should be unique in the company.");
                    return false;
                }
            }

            if (isNew || !string.Equals(oldContact.User.Email, row.Email, StringComparison.OrdinalIgnoreCase))
            {
                var existing = UserSearch.SelectByEmail(row.Email);
                if (existing != null)
                {
                    if (isNew && !ServiceLocator.PersonSearch.IsPersonExist(existing.UserIdentifier, Organization.OrganizationIdentifier))
                    {
                        Duplicates.Add(new SerializablePersonImportRow(row));
                    }
                    else
                        ScreenStatus.AddMessage(AlertType.Error, $"The email {row.Email} is already used by another account.");

                    return false;
                }
            }

            if (isNew)
            {
                if (uniqueKeyType == "PersonCode")
                {
                    var isPersonExists = PersonCriteria.Exists(new PersonFilter
                    {
                        OrganizationIdentifier = organization.OrganizationIdentifier,
                        EmailExact = row.Email,
                        CodeNotExact = row.UniqueIdentifier
                    });

                    if (isPersonExists)
                    {
                        ScreenStatusView.AddMessage(AlertType.Error, $"There is another user already registered with the Login Name <strong>{row.Email}</strong>.");
                        return false;
                    }
                }

                var newUser = new QUser();
                newUser.TimeZone = organization.TimeZone.Id;

                var newPerson = UserFactory.CreatePerson(organization.OrganizationIdentifier);

                contactsByUniqueKey.Add(row.UniqueIdentifier, new Contact(newUser, newPerson));

                row.User = newUser;
                row.Person = newPerson;

                return true;
            }

            if (!isUpdateAllowed)
            {
                ScreenStatus.AddMessage(AlertType.Error, $"The user account {row.Email} already exists. You are not allowed to update existing accounts by upload.");
                return false;
            }

            if (!ServiceLocator.PersonSearch.IsPersonExist(oldContact.User.UserIdentifier, Organization.OrganizationIdentifier))
            {
                ScreenStatus.AddMessage(AlertType.Error, $"Matches an existing user ({uniqueKeyType}: {row.UniqueIdentifier}) in another organization account. If you want to shared access to the same users then please contact your system administrator to have this configured for you.");
                return false;
            }

            row.User = oldContact.User;
            row.Person = oldContact.Person;

            return true;
        }

        private static DateTime? ParseDate(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            return DateTime.TryParse(text, out DateTime date) ? date : (DateTime?)null;
        }

        private static string AdjustString(string text, int maxLength)
        {
            return !string.IsNullOrEmpty(text) && text.Length > maxLength
                ? text.Substring(0, maxLength)
                : text;
        }

        private static void SetAddress(QPerson person, string addressType, PersonAddress address, PersonAddressSettings settings)
        {
            var addressEntity = person.GetAddress(addressType);

            if (settings.DescriptionIndex >= 0)
                addressEntity.Description = AdjustString(address.Description, 128);

            if (settings.Street1Index >= 0)
                addressEntity.Street1 = AdjustString(address.Street1, 128);

            if (settings.Street2Index >= 0)
                addressEntity.Street2 = AdjustString(address.Street2, 128);

            if (settings.CityIndex >= 0)
                addressEntity.City = AdjustString(address.City, 128);

            if (settings.CountryIndex >= 0)
                addressEntity.Country = AdjustString(address.Country, 32);

            if (settings.ProvinceIndex >= 0)
            {
                addressEntity.Province = Organization.PlatformCustomization.UserRegistration.ConvertProvinceAbbreviation
                    ? ServiceLocator.ProvinceSearch.Unabbreviate(address.Province)
                    : address.Province;
            }

            if (settings.PostalCodeIndex >= 0)
                addressEntity.PostalCode = AdjustString(address.PostalCode, 16);
        }

        private static string FixCaseOnEmail(string email)
        {
            return !string.IsNullOrEmpty(email)
                ? email.ToLower(Cultures.Default)
                : email;
        }

        private static string FixCaseOnName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            var result = new StringBuilder();
            var shouldUpper = true;

            foreach (var c in name)
            {
                var s = c.ToString();

                if (char.IsLetter(c))
                {
                    result.Append(shouldUpper ? s.ToUpper(Cultures.Default) : s.ToLower(Cultures.Default));

                    shouldUpper = false;
                }
                else
                {
                    string[] separators = { " ", "-" };
                    if (StringHelper.ContainsAny(s, separators))
                        shouldUpper = true;

                    result.Append(s);
                }
            }

            return result.ToString();
        }

        #endregion

        #region Helper methods

        private static string BuildErrorMessage(MessageBuilder messages)
        {
            var html = new StringBuilder();

            html.AppendFormat("<p>No contacts have been uploaded because the file contains {0:n0} error(s)", messages.ErrorCount);

            if (messages.ErrorCount > MaxErrorsCount)
                html.AppendFormat(". Here are first {0:n0} errors", MaxErrorsCount);

            html.Append(":</p>");

            messages.WriteErrors(html, MaxErrorsCount);

            html.Append("<p>Please ensure the file you are attempting to upload has been saved with UTF-8 encoding.</p>");

            return html.ToString();
        }

        private void SetProgressStatus(int currentPosition, int positionMax)
        {
            UploadProgress.UpdateContext(context =>
            {
                var progressBar = (ProgressIndicator.ContextData)context.Items["Progress"];
                progressBar.Total = positionMax;
                progressBar.Value = currentPosition;
            });
        }

        private bool IsUpdateAllowed()
            => CurrentSessionState.Identity.IsGranted(ActionName.Admin_Contacts_People_Upload_Update);

        private Blowfish GetBlowfish()
        {
            return _blowfish ?? (_blowfish = new Blowfish(EncryptionKey.Default));
        }

        #endregion
    }
}