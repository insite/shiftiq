<%@ Page Language="C#" CodeBehind="Register.aspx.cs" Inherits="InSite.UI.Portal.Events.Classes.Register" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="Controls/PaymentDetail.ascx" TagName="PaymentDetail" TagPrefix="uc" %>
<%@ Register Src="Controls/CardDetailConfirm.ascx" TagName="CardDetailConfirm" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style>
        .payment-overlay {
            width: 100%;
            height: 100%;
            position: absolute;
            top: 0;
            left: 0;
            background: rgba( 205, 205, 205, 0.4 )  url('/images/animations/loader.gif') 50% 50% no-repeat;
            z-index: 9000;
        }
    </style>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <div id="PaymentOverlay"></div>

    <insite:Alert runat="server" ID="StatusAlert" />
    
    <insite:ValidationSummary runat="server" />

    <asp:CustomValidator runat="server" ID="AvailabilityValidator" />

    <asp:Repeater runat="server" ID="TopProgressBar">
        <HeaderTemplate><ul class='dot-indicator'></HeaderTemplate>
        <FooterTemplate></ul></FooterTemplate>
        <ItemTemplate>
            <li data-num='<%# Eval("Number") %>' class='<%# (bool)Eval("IsCompleted") ? "step-complete" : "" %>'>
                <%# Eval("Title") %>
            </li>
        </ItemTemplate>
    </asp:Repeater>

    <insite:Accordion runat="server" IsFlush="false">
        <insite:AccordionPanel runat="server" ID="PersonDetailsPanel" Icon="far fa-user" Title="Account">

            <div class="row settings">

                <div class="col-md-4">

                    <div runat="server" id="PersonalGroup" class="p-4 shadow rounded-3">

                        <h5 class="mb-2"><insite:Literal runat="server" Text="Personal" /></h5>
                        
                        <div runat="server" id="FirstNameField" class="form-group mb-3">
                            <label class="form-label" for="<%# FirstName.ClientID %>">
                                <insite:Literal runat="server" Text="First Name" />
                                <insite:RequiredValidator runat="server" ID="FirstNameRequiredValidator" ControlToValidate="FirstName" Display="None" />
                            </label>
                            <div>
                                <insite:TextBox ID="FirstName" runat="server" MaxLength="32" Width="100%" />
                            </div>
                        </div>

                        <div runat="server" id="MiddleNameField" class="form-group mb-3">
                            <label class="form-label" for="<%# MiddleName.ClientID %>">
                                <insite:Literal runat="server" Text="Middle Name" />
                                <insite:RequiredValidator runat="server" ID="MiddleNameRequiredValidator" ControlToValidate="MiddleName" Display="None" />
                            </label>
                            <div>
                                <insite:TextBox ID="MiddleName" runat="server" MaxLength="32" Width="100%" />
                            </div>
                        </div>

                        <div runat="server" id="LastNameField" class="form-group mb-3">
                            <label class="form-label" for="<%# LastName.ClientID %>">
                                <insite:Literal runat="server" Text="Last Name" />
                                <insite:RequiredValidator runat="server" ID="LastNameRequiredValidator" ControlToValidate="LastName" Display="None" />
                            </label>
                            <div>
                                <insite:TextBox ID="LastName" runat="server" MaxLength="32" Width="100%" />
                            </div>
                        </div>

                        <div runat="server" id="EmailField" class="form-group mb-3">
                            <label class="form-label" for="<%# Email.ClientID %>">
                                <insite:Literal runat="server" Text="Email" />
                                <insite:RequiredValidator runat="server" ID="EmailRequiredValidator" ControlToValidate="Email" Display="None" />
                                <insite:CustomValidator runat="server" ID="EmailUniqueValidator"
                                    ControlToValidate="Email"
                                    ErrorMessage="This email address is already in use"
                                    Display="None"
                                />
                            </label>
                            <div>
                                <insite:TextBox ID="Email" runat="server" MaxLength="128" TextMode="Email" />
                            </div>
                        </div>

                        <div runat="server" id="BirthdateField" class="form-group mb-3">
                            <label class="form-label" for="<%# Birthdate.ClientID %>">
                                <insite:Literal runat="server" Text="Birthdate" />
                                <insite:RequiredValidator runat="server" ID="BirthdateRequiredValidator" ControlToValidate="Birthdate" Display="None" />
                            </label>
                            <div>
                                <insite:DateSelector ID="Birthdate" runat="server" />
                            </div>
                        </div>

                        <div runat="server" id="PersonCodeField" class="form-group mb-3">
                            <label class="form-label" for="<%# PersonCode.ClientID %>">
                                <insite:ContentTextLiteral runat="server" ContentLabel="Person Code"/>
                                <insite:RequiredValidator runat="server" FieldName="Person Code" ID="PersonCodeRequiredValidator" ControlToValidate="PersonCode" Display="None" />
                                <insite:CustomValidator runat="server" ID="PersonCodeUniqueValidator" Display="None"/>
                            </label>
                            <div>
                                <insite:TextBox ID="PersonCode" runat="server" Width="100%" MaxLength="20" />
                            </div>
                            <div class="form-text">
                                <insite:Literal runat="server" ID="PersonCodeHelpText" />
                            </div>
                        </div>

                        <div runat="server" id="PersonCodeIdAvailabilityField" class="form-group mb-3">
                            <div>
                                <insite:CheckBox ID="PersonCodeIdAvailability" runat="server" Text="Learner ID not available" />
                            </div>
                        </div>

                        <div runat="server" id="WorkBasedHoursToDateField" class="form-group mb-3">
                            <label class="form-label" for="<%# WorkBasedHoursToDate.ClientID %>">
                                <insite:Literal runat="server" Text="Number of Work Based Hours to date" />
                                <insite:RequiredValidator runat="server" ID="WorkBasedHoursToDateRequiredValidator" ControlToValidate="WorkBasedHoursToDate" Display="None" />
                            </label>
                            <div>
                                <insite:NumericBox ID="WorkBasedHoursToDate" runat="server" NumericMode="Integer" />
                            </div>
                        </div>

                        <div runat="server" id="FirstLanguageField" class="form-group mb-3">
                            <div>
                                <insite:CheckBox ID="FirstLanguage" runat="server" Text="English Language Learner" />
                            </div>
                        </div>

                    </div>

                </div>
                        
                <div class="col-md-5">
                                
                    <div runat="server" id="AddressGroup" class="p-4 shadow rounded-3">
                        
                        <h5 class="mb-2"><insite:Literal runat="server" Text="Address" /></h5>

                        <div class="form-group mb-3" runat="server" ID="Street1Field">
                            <label for="<%# Street1.ClientID %>" class="form-label">
                                <insite:Literal runat="server" Text="Address 1" />
                                <insite:RequiredValidator runat="server" ID="Street1RequiredValidator" ControlToValidate="Street1" Display="None" />
                            </label>
                            <div>
                                <insite:TextBox ID="Street1" runat="server" MaxLength="128" />
                            </div>
                        </div>

                        <div class="form-group mb-3" runat="server" ID="CityField">
                            <label for="<%# City.ClientID %>" class="form-label">
                                <insite:Literal runat="server" Text="City" />
                                <insite:RequiredValidator runat="server" ID="CityRequiredValidator" ControlToValidate="City" Display="None" />
                            </label>
                            <div>
                                <insite:TextBox ID="City" runat="server" MaxLength="128" />
                            </div>
                        </div>

                        <div class="form-group mb-3" runat="server" id="CountryField">
                            <label for="<%# Country.ClientID %>" class="form-label" runat="server" ID="CountryFieldLabel">
                                <insite:Literal runat="server" Text="Country" />
                                <insite:RequiredValidator runat="server" ID="CountryRequiredValidator" ControlToValidate="Country" />
                            </label>
                            <div>
                                <insite:CountryComboBox ID="Country" runat="server" DropDown-Size="15" />
                            </div>
                        </div>

                        <div class="form-group mb-3" runat="server" ID="ProvinceField">
                            <label for="<%# Province.ClientID %>" class="form-label" runat="server" ID="ProvinceFieldLabel">
                                <insite:Literal runat="server" Text="Province" />
                                <insite:RequiredValidator runat="server" ID="ProvinceRequiredValidator" ControlToValidate="Province" Display="None" />
                            </label>
                            <div>
                                <insite:ProvinceComboBox ID="Province" runat="server" UseCodeAsValue="true" OnlyCanadaAndUnitedStates="true" />
                            </div>
                        </div>

                        <div class="form-group mb-3" runat="server" ID="PostalCodeField">
                            <label for="<%# PostalCode.ClientID %>" class="form-label" runat="server" ID="PostalCodeFieldLabel">
                                <insite:Literal runat="server" Text="Postal Code" />
                                <insite:RequiredValidator runat="server" ID="PostalCodeRequiredValidator" ControlToValidate="PostalCode" Display="None" />
                            </label>
                            <div>
                                <insite:TextBox ID="PostalCode" runat="server" MaxLength="16" />
                            </div>
                        </div>

                    </div>

                    <div runat="server" id="EmergencyContactGroup" class="p-4 shadow rounded-3 mt-4">
                            
                        <h5 class="mb-2"><insite:Literal runat="server" Text="Emergency Contact" /></h5>
    
                        <div runat="server" id="EmergencyContactNameField" class="form-group mb-3">
                            <label class="form-label" for="<%# EmergencyContactName.ClientID %>">
                                <insite:Literal runat="server" Text="Name" />
                                <insite:RequiredValidator runat="server" ID="EmergencyContactNameRequiredValidator" ControlToValidate="EmergencyContactName" Display="None" />
                            </label>
                            <div>
                                <insite:TextBox ID="EmergencyContactName" runat="server" MaxLength="100" />
                            </div>
                        </div>

                        <div runat="server" id="EmergencyContactPhoneField" class="form-group mb-3">
                            <label class="form-label" for="<%# EmergencyContactPhone.ClientID %>">
                                <insite:Literal runat="server" Text="Phone" />
                                <insite:RequiredValidator runat="server" ID="EmergencyContactPhoneRequiredValidator" ControlToValidate="EmergencyContactPhone" Display="None" />
                            </label>
                            <div>
                                <insite:TextBox ID="EmergencyContactPhone" runat="server" MaxLength="32" />
                            </div>
                        </div>

                        <div runat="server" id="EmergencyContactRelationshipField" class="form-group mb-3">
                            <label class="form-label" for="<%# EmergencyContactRelationship.ClientID %>">
                                <insite:Literal runat="server" Text="Relationship" />
                                <insite:RequiredValidator runat="server" ID="EmergencyContactRelationshipRequiredValidator" ControlToValidate="EmergencyContactRelationship" Display="None" />
                            </label>
                            <div>
                                <insite:TextBox ID="EmergencyContactRelationship" runat="server" MaxLength="50" />
                            </div>
                        </div>

                    </div>

                </div>
                        
                <div class="col-md-3">

                    <div runat="server" id="PhoneNumbersGroup" class="p-4 shadow rounded-3">
                        
                        <h5 class="mb-2"><insite:Literal runat="server" Text="Phone Numbers" /></h5>
    
                        <div runat="server" id="PhoneField" class="form-group mb-3">
                            <label class="form-label" for="<%# Phone.ClientID %>">
                                <insite:Literal runat="server" Text="Preferred" />
                                <insite:RequiredValidator runat="server" ID="PhoneRequiredValidator" ControlToValidate="Phone" Display="None" />
                            </label>
                            <div>
                                <insite:TextBox ID="Phone" runat="server" MaxLength="32" Width="100%" />
                            </div>
                            <small class="form-text">
                                <insite:Literal runat="server" Text="Please call this number first" />
                            </small>
                        </div>

                        <div runat="server" id="PhoneHomeField" class="form-group mb-3">
                            <label class="form-label" for="<%# PhoneHome.ClientID %>">
                                <insite:Literal runat="server" Text="Home" />
                                <insite:RequiredValidator runat="server" ID="PhoneHomeRequiredValidator" ControlToValidate="PhoneHome" Display="None" />
                            </label>
                            <div>
                                <insite:TextBox ID="PhoneHome" runat="server" MaxLength="32" Width="100%" />
                            </div>
                        </div>

                        <div runat="server" id="PhoneWorkField" class="form-group mb-3">
                            <label class="form-label" for="<%# PhoneWork.ClientID %>">
                                <insite:Literal runat="server" Text="Work" />
                                <insite:RequiredValidator runat="server" ID="PhoneWorkRequiredValidator" ControlToValidate="PhoneWork" Display="None" />
                            </label>
                            <div>
                                <insite:TextBox ID="PhoneWork" runat="server" MaxLength="32" Width="100%" />
                            </div>
                        </div>

                        <div runat="server" id="PhoneMobileField" class="form-group mb-3">
                            <label class="form-label" for="<%# PhoneMobile.ClientID %>">
                                <insite:Literal runat="server" Text="Mobile" />
                                <insite:RequiredValidator runat="server" ID="PhoneMobileRequiredValidator" ControlToValidate="PhoneMobile" Display="None" />
                            </label>
                            <div>
                                <insite:TextBox ID="PhoneMobile" runat="server" MaxLength="32" Width="100%" />
                            </div>
                        </div>

                        <div runat="server" id="PhoneOtherField" class="form-group mb-3">
                            <label class="form-label" for="<%# PhoneOther.ClientID %>">
                                <insite:Literal runat="server" Text="Other" />
                                <insite:RequiredValidator runat="server" ID="PhoneOtherRequiredValidator" ControlToValidate="PhoneOther" Display="None" />
                            </label>
                            <div>
                                <insite:TextBox ID="PhoneOther" runat="server" MaxLength="32" Width="100%" />
                            </div>
                        </div>

                    </div>

                </div>

            </div>

            <div class="pt-3">
                <insite:NextButton runat="server" ID="NextButton1" />
                <insite:CancelButton runat="server" ID="CancelButton1" ConfirmText="Are you sure you want to close page without saving any information?" />
            </div>

        </insite:AccordionPanel>
        <insite:AccordionPanel runat="server" ID="EmployerPanel" Icon="far fa-user-tie" Title="Employer" Visible="false">

            <div class="row">

                <div class="col-md-4">
                    <div class="p-4 shadow rounded-3 h-100">

                        <h5 class="mb-2">
                            <insite:Literal runat="server" Text="Company Name" />
                        </h5>

                        <div runat="server" id="CompanyOptions">
                            <div class="mb-3">
                                <div class="mb-1">
                                    <insite:RadioButton runat="server" ID="CompanyTypeExisting" Text="Click to search for company:" Checked="true" GroupName="CompanyType" />
                                    <div runat="server" id="ExistingCompanyPanel" class="ms-3 my-2">
                                        <insite:FindGroup ID="EmployerGroupIdentifier" runat="server" ShowCities="true" />
                                    </div>
                                </div>
                                <div class="fs-sm mt-2 mb-1">If company isn't found in list above:</div>
                                <insite:RadioButton runat="server" ID="CompanyTypeNew" Text="Add a new company" GroupName="CompanyType" />
                            </div>
                        </div>
                        <div runat="server" id="CompanyNameDisplay">
                            <span class="d-block"><asp:Literal ID="CompanyName" runat="server" /></span>
                        </div>

                    </div>
                </div>

                <div runat="server" id="AddressAndPhoneColumn" class="col-md-4">
                    <div class="p-4 shadow rounded-3 h-100">

                        <h5 class="mb-2" runat="server" id="EmployerGroupName"></h5>

                        <ul class="list-unstyled mb-0">
                            <li id="AddressArea" runat="server" class="d-flex py-1">
                                <i class="far fa-map-marker-alt fs-lg align-self-center text-primary"></i>
                                <div class="ps-3">
                                    <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Mailing Address" /></span>
                                    <asp:HyperLink ID="EmployerAddress" runat="server" Target="_blank" />
                                </div>
                            </li>
                            <li runat="server" id="EmployerPhoneArea" class="d-flex py-1">
                                <i class="far fa-phone fs-lg align-self-center text-primary"></i>
                                <div>
                                    <div runat="server" id="EmployerPhoneField" class="ps-3">
                                        <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Phone" /></span>
                                        <asp:Literal ID="EmployerPhone" runat="server" />
                                    </div>
                                    <div runat="server" id="EmployerFaxField" class="ps-3">
                                        <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Fax" /></span>
                                        <asp:Literal ID="EmployerFax" runat="server" />
                                    </div>
                                </div>
                            </li>
                        </ul>

                    </div>
                </div>

                <div runat="server" id="EmployerContactColumn" class="col-md-4">
                    <div class="p-4 shadow rounded-3 h-100">

                        <h5 class="mb-2">
                            <insite:Literal runat="server" Text="Company Contact" />
                        </h5>

                        <div class="d-flex pt-2 pb-3">
                            <i class="far fa-user fs-lg align-self-center text-primary"></i>
                            <div class="ps-3"><span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Name" /></span><span class="d-block"><asp:Literal ID="EmployerContactName" runat="server" /></span></div>
                        </div>

                        <div runat="server" id="EmployerContactPhoneNumberField" class="d-flex pt-2 pb-3">
                            <i class="far fa-phone fs-lg align-self-center text-primary"></i>
                            <div class="ps-3"><span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Phone" /></span><span class="d-block"><asp:Literal ID="EmployerContactPhoneNumber" runat="server" /></span></div>
                        </div>

                        <div class="d-flex pt-2 pb-3"><i class="far fa-envelope fs-lg align-self-center text-primary"></i>
                            <div class="ps-3"><span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Email" /></span><span class="d-block"><asp:Literal ID="EmployerContactEmail" runat="server" /></span></div>
                        </div>

                    </div>
                </div>

                <div runat="server" id="NewEmployerColumn" class="col-md-4">
                    <div class="p-4 shadow rounded-3 h-100">

                        <h5 class="mb-2">
                            <insite:Literal runat="server" Text="New Company" />
                        </h5>
                        <div class="form-group mb-3">
                            <label class="form-label" for="<%# NewCompanyName.ClientID %>">
                                <insite:Literal runat="server" Text="Name" />
                                <insite:RequiredValidator runat="server" ControlToValidate="NewCompanyName" Display="None" />
                            </label>
                            <insite:TextBox ID="NewCompanyName" runat="server" MaxLength="90" Width="100%" />
                        </div>
                        <div class="form-group mb-3">
                            <label class="form-label" for="<%# NewCompanyPhone.ClientID %>">
                                <insite:Literal runat="server" Text="Phone" />
                                <insite:RequiredValidator runat="server" ControlToValidate="NewCompanyPhone" Display="None" />
                            </label>
                            <insite:TextBox ID="NewCompanyPhone" runat="server" MaxLength="32" Width="100%" />
                        </div>

                        <div class="form-group mb-3">
                            <label for="<%# NewCompanyStreet1.ClientID %>" class="form-label">
                                <insite:Literal runat="server" Text="Address 1" />
                                <insite:RequiredValidator runat="server" ControlToValidate="NewCompanyStreet1" Display="None" />
                            </label>
                            <div>
                                <insite:TextBox ID="NewCompanyStreet1" runat="server" MaxLength="128" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label for="<%# NewCompanyCity.ClientID %>" class="form-label">
                                <insite:Literal runat="server" Text="City" />
                                <insite:RequiredValidator runat="server" ControlToValidate="NewCompanyCity" Display="None" />
                            </label>
                            <div>
                                <insite:TextBox ID="NewCompanyCity" runat="server" MaxLength="128" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label for="<%# NewCompanyProvinceSelector.ClientID %>" class="form-label">
                                <insite:Literal runat="server" Text="Province" />
                                <insite:RequiredValidator runat="server" ControlToValidate="NewCompanyProvinceSelector" Display="None" />
                            </label>
                            <div>
                                <insite:ProvinceComboBox ID="NewCompanyProvinceSelector" runat="server" UseCodeAsValue="true" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label for="<%# NewCompanyPostalCode.ClientID %>" class="form-label">
                                <insite:Literal runat="server" Text="Postal Code" />
                                <insite:RequiredValidator runat="server" ControlToValidate="NewCompanyPostalCode" Display="None" />
                            </label>
                            <div>
                                <insite:TextBox ID="NewCompanyPostalCode" runat="server" MaxLength="16" />
                            </div>
                        </div>

                    </div>
                </div>

                <div runat="server" id="NewEmployerContactColumn" class="col-md-4">
                    <div class="p-4 shadow rounded-3 h-100">

                        <h5 class="mb-2">
                            <insite:Literal runat="server" Text="New Company Contact" />
                        </h5>

                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="NewCompanyContactUpdatePanel" />

                        <insite:UpdatePanel runat="server" ID="NewCompanyContactUpdatePanel">
                            <ContentTemplate>
                                <div class="form-group mb-3">
                                    <insite:RadioButtonList runat="server" ID="NewCompanyContactType" RepeatDirection="Vertical" RepeatLayout="Table">
                                        <asp:ListItem Value="Existing" Text="I'm the primary contact" />
                                        <asp:ListItem Value="New" Text="Someone else is the primary contact" Selected="True" />
                                    </insite:RadioButtonList>
                                </div>

                                <insite:Container runat="server" id="NewCompanyContactFields">
                                    <div class="form-group mb-3">
                                        <label class="form-label" for="<%# NewCompanyContactFirstName.ClientID %>">
                                            <insite:Literal runat="server" Text="First Name" />
                                            <insite:RequiredValidator runat="server" ControlToValidate="NewCompanyContactFirstName" Display="None" />
                                        </label>
                                        <insite:TextBox ID="NewCompanyContactFirstName" runat="server" MaxLength="32" Width="100%" />
                                    </div>
                                    <div class="form-group mb-3">
                                        <label class="form-label" for="<%# NewCompanyContactLastName.ClientID %>">
                                            <insite:Literal runat="server" Text="Last Name" />
                                            <insite:RequiredValidator runat="server" ControlToValidate="NewCompanyContactLastName" Display="None" />
                                        </label>
                                        <insite:TextBox ID="NewCompanyContactLastName" runat="server" MaxLength="32" Width="100%" />
                                    </div>
                                    <div class="form-group mb-3">
                                        <label class="form-label" for="<%# NewCompanyContactPhone.ClientID %>">
                                            <insite:Literal runat="server" Text="Phone" />
                                            <insite:RequiredValidator runat="server" ControlToValidate="NewCompanyContactPhone" Display="None" />
                                        </label>
                                        <insite:TextBox ID="NewCompanyContactPhone" runat="server" MaxLength="32" Width="100%" />
                                    </div>
                                    <div class="form-group mb-3">
                                        <label class="form-label" for="<%# NewCompanyContactEmail.ClientID %>">
                                            <insite:Literal runat="server" Text="Email" />
                                            <insite:RequiredValidator runat="server" ControlToValidate="NewCompanyContactEmail" Display="None" />
                                        </label>
                                        <insite:TextBox ID="NewCompanyContactEmail" runat="server" MaxLength="32" Width="100%" />
                                    </div>
                                </insite:Container>
                            </ContentTemplate>
                        </insite:UpdatePanel>

                    </div>
                </div>

            </div>

            <div class="pt-3">
                <insite:Button runat="server" ID="BackButton1" Text="Back" ButtonStyle="Default" Icon="fas fa-arrow-alt-left" CausesValidation="false" />
                <insite:NextButton runat="server" ID="NextButton2" />
                <insite:CancelButton runat="server" ID="CancelButton2" ConfirmText="Are you sure you want to close page without saving any information?" />
            </div>

        </insite:AccordionPanel>
        <insite:AccordionPanel runat="server" ID="SeatPanel" Icon="far fa-money-check-alt" Title="Seat Selection" Visible="false">

            <div class="row">

                <div class="col-md-4">
                    <div class="p-4 shadow rounded-3">
                        <h5 class="mb-2">
                            <insite:Literal runat="server" ID="MultipleSeatTitle" Text="Select your registration below" />
                            <insite:Literal runat="server" ID="OneSeatTitle" Text="Your Registration" />
                        </h5>

                        <div class="mb-3">
                            <asp:Repeater runat="server" ID="SeatRepeater">
                                <ItemTemplate>
                                    <input type="radio"
                                        value='<%# Eval("SeatIdentifier") %>'
                                        id='<%# Eval("SeatIdentifier") %>'
                                        name="Seats"
                                        onclick="register.onSeatChanged();"
                                        <%# Eval("SeatIdentifier").ToString() == SelectedSeat.Value ? "checked" : "" %>
                                    />
                                    <label for='<%# Eval("SeatIdentifier") %>'><b><%# Eval("SeatTitle") %></b></label>
                                    <p><%# GetDescription(Container.DataItem) %></p>
                                </ItemTemplate>
                            </asp:Repeater>

                            <asp:CustomValidator runat="server" ID="SeatRequiredValidator" />

                            <asp:HiddenField runat="server" ID="SelectedSeat" />
                            <asp:Button runat="server" ID="ChangeSeatButton" style="display:none;" CausesValidation="false" />
                        </div>
                    </div>

                    <div runat="server" id="PriceArea" visible="false" class="p-4 shadow rounded-3 mt-4">
                        <h5 class="mb-2"><insite:Literal runat="server" Text="Price options" /></h5>

                        <div class="mb-3">
                            <b><insite:Literal ID="FreePrice" runat="server" Text="Free" /></b>
                            <b><asp:Literal runat="server" ID="SinglePrice" Visible="false" /></b>

                            <b><insite:RadioButtonList runat="server" ID="MultiplePrice" Visible="false" /></b>
                        </div>
                    </div>
                </div>

                <div class="col-md-8" runat="server" id="Agreement" visible="false">
                    <div class="p-4 shadow rounded-3">

                        <h5 class="mb-2"><i class="far fa-file-contract me-2"></i><insite:Literal runat="server" Text="Agreement" /></h5>

                        <ul class="list-unstyled mb-0">
                            <li class="d-flex py-1">
                                
                                <div class="ps-3">
                                    <asp:Literal runat="server" ID="AgreementText" />
                                    <div class="pt-3 option-item">
                                        <insite:CheckBox runat="server" ID="Agreed" Text="I Agree" Visible="false" />
                                        <insite:Button runat="server" ID="IAgreeButton" Text="I Agree" ButtonStyle="Success" Icon="fas fa-check" CausesValidation="true" />
                                    </div>
                                </div>
                            </li>
                            <li runat="server" id="BillingCustomerField" class="d-flex py-1">
                                <i class="far fa-money-check-edit-alt fs-lg align-self-center text-primary border-top"></i>
                                <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Billing To" />
                                    <insite:RequiredValidator runat="server" ControlToValidate="BillingCustomer" Display="None" />
                                </span>
                                <div class="ps-3">
                                    <insite:ComboBox runat="server" ID="BillingCustomer" Width="350px" />
                                </div>
                            </li>
                        </ul>

                    </div>
                </div>

            </div>

            <div class="pt-3">
                <insite:Button runat="server" ID="BackButton2" Text="Back" ButtonStyle="Default" Icon="fas fa-arrow-alt-left" CausesValidation="false" />
                <insite:NextButton runat="server" ID="NextButton3" Enabled="false" />
                <insite:CancelButton runat="server" ID="CancelButton3" ConfirmText="Are you sure you want to close page without saving any information?" />
            </div>

        </insite:AccordionPanel>
        <insite:AccordionPanel runat="server" ID="PaymentSection" Icon="far fa-credit-card-front" Title="Payment" Visible="false">
            <insite:Alert runat="server" ID="PaymentNotAvailable" />
            <insite:Alert runat="server" ID="PaymentAlert" />

            <div>
                <uc:PaymentDetail runat="server" ID="PaymentDetail" />

                <asp:CustomValidator runat="server" ID="SeatRequiredValidator2" ValidationGroup="Payment" />
            </div>

            <div class="pt-3">
                <insite:Button runat="server" ID="BackButton3" Text="Back" ButtonStyle="Default" Icon="fas fa-arrow-alt-left" CausesValidation="false" />
                <insite:NextButton runat="server" ID="NextButton4" ValidationGroup="Payment" />
                <insite:CancelButton runat="server" ID="CancelButton4" ConfirmText="Are you sure you want to close page without saving any information?" />
            </div>

        </insite:AccordionPanel>
        <insite:AccordionPanel runat="server" ID="ConfirmSection" Icon="far fa-file-check" Title="Confirm Details & Payment" Visible="false">

            <div class="row">

                <div class="col-md-12">
                    <div class="p-4 shadow rounded-3">
                        <h5 class="mb-2" runat="server"><asp:Literal runat="server" ID="ConfirmClassName" /></h5>
                        <ul class="list-unstyled mb-0">
                            <li class="d-flex py-1">
                                <i class="far fa-calendar-alt fs-lg align-self-center text-primary"></i>
                                <div class="ps-3">
                                    <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="When" /></span>
                                    <asp:Literal runat="server" ID="ConfirmClassDates" />
                                </div>
                            </li>
                            <li class="d-flex py-1 border-top">
                                <i class="far fa-location fs-lg align-self-center text-primary"></i>
                                <div class="ps-3">
                                    <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Where" /></span>
                                    <asp:Literal runat="server" ID="ConfirmClassVenue" />
                                </div>
                            </li>
                            <li class="d-flex py-1 border-top">
                                <i class="far fa-map-marker-alt fs-lg align-self-center text-primary"></i>
                                <div class="ps-3">
                                    <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Address" /></span>
                                    <asp:HyperLink runat="server" ID="ConfirmClassVenueAddress" Target="_blank" />
                                </div>
                            </li>
                        </ul>
                    </div>
                </div>

                <div runat="server" id="ConfirmClassRefundPanel" class="row">
                    <div class="col-md-12">
                        <div class="p-4 shadow rounded-3" style="margin-top:15px;">
                            <h5 class="mb-2"><insite:Literal runat="server" Text="Cancellation &amp; Refund Policy" /></h5>

                            <div class="mb-3">
                                <div><asp:Literal runat="server" ID="ConfirmClassRefundContent" /></div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>

            <div class="row mt-4">
                <div class="col-md-4">
                    <div class="p-4 shadow rounded-3">
                        <h5 class="mb-2" runat="server"><asp:Literal runat="server" ID="ConfirmParticipantName" /></h5>
                        <ul class="list-unstyled mb-0">
                            <li class="d-flex py-1">
                                <i class="far fa-envelope fs-lg align-self-center text-primary"></i>
                                <div class="ps-3">
                                    <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Email" /></span>
                                    <asp:Literal runat="server" ID="ConfirmParticipantEmail" />
                                </div>
                            </li>
                            <li id="ConfirmParticipantAddressDiv" runat="server"  class="d-flex py-1 border-top">
                                <i class="far fa-map-marker-alt fs-lg align-self-baseline mt-1 text-primary"></i>
                                <div class="ps-3">
                                    <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Address" /></span>
                                    <asp:HyperLink runat="server" ID="ConfirmParticipantAddress" Target="_blank" />
                                </div>
                            </li>
                        </ul>
                    </div>

                    <div class="p-4 shadow rounded-3" style="margin-top:15px;">
                        <h5 class="mb-2" runat="server"><asp:Literal runat="server" ID="ConfirmEmployerName" /></h5>
                        <ul class="list-unstyled mb-0">
                            <li id="ComfirmEmployerEmailDiv" runat="server" class="d-flex py-1">
                                <i class="far fa-envelope fs-lg align-self-center text-primary"></i>
                                <div class="ps-3">
                                    <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Email" /></span>
                                    <asp:Literal ID="ConfirmEmployerEmail" runat="server" />
                                </div>
                            </li>
                            <li id="ConfirmEmployerAddressDiv" runat="server"  class="d-flex py-1">
                                <i class="far fa-map-marker-alt fs-lg align-self-center text-primary"></i>
                                <div class="ps-3">
                                    <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Mailing Address" /></span>
                                    <asp:HyperLink ID="ConfirmEmployerAddress" runat="server" Target="_blank" />
                                </div>
                            </li>
                            <li runat="server" id="ComfirmEmployerPhonesDiv" class="d-flex py-1">
                                <i class="far fa-phone fs-lg align-self-center text-primary"></i>
                                <div>
                                    <div runat="server" id="ConfirmEmployerPhoneDiv" class="ps-3">
                                        <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Phone" /></span>
                                        <asp:Literal ID="ConfirmEmployerPhone" runat="server" />
                                    </div>
                                    <div runat="server" id="ConfirmEmployerFaxDiv" class="ps-3">
                                        <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Fax" /></span>
                                        <asp:Literal ID="ConfirmEmployerFax" runat="server" />
                                    </div>
                                </div>
                            </li>
                        </ul>
                    </div>

                    <div runat="server" id="CreditCardPanel" class="p-4 shadow rounded-3"  style="margin-top:15px;">
                        <h5 class="mb-2"><insite:Literal runat="server" Text="Credit Card" /></h5>
    
                        <uc:CardDetailConfirm runat="server" ID="CardDetailConfirm" />
                    </div>

                    <div runat="server" id="BillingCodePanel" class="p-4 shadow rounded-3"  style="margin-top:15px;">
                        <h5 class="mb-2"><insite:Literal runat="server" Text="Bill To" /></h5>
    
                        <div runat="server" id="BillingCodeConfirmation" class="white-space:pre-wrap;"></div>
                    </div>

                </div>

                <div class="col-md-8">
                    <div class="p-4 shadow rounded-3">

                        <h5 class="mb-2" runat="server"><asp:Literal runat="server" ID="ConfirmSeatName" /></h5>
                        <ul class="list-unstyled mb-0">
                            <li class="d-flex py-1">
                                <i class="far fa-dollar-sign fs-lg align-self-center text-primary"></i>
                                <div class="ps-3">
                                    <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Payment Amount ($)" /></span>
                                    <asp:Literal runat="server" ID="PaymentAmountLiteral" />
                                </div>
                            </li>
                            <li class="d-flex py-1">
                                <i class="far fa-file-contract fs-lg align-self-center text-primary border-top"></i>
                                <div class="ps-3">
                                    <asp:Literal runat="server" ID="ConfirmSeatAgreement" />
                                </div>
                            </li>
                            <li runat="server" id="BillingCustomerDiv" class="d-flex py-1 border-top">
                                <i class="far fa-money-check-edit-alt fs-lg align-self-center text-primary"></i>
                                <div class="ps-3">
                                    <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Tuition fee is being paid by" /></span>
                                    <asp:Literal runat="server" ID="ConfirmSeatPaidBy" />
                                </div>
                            </li>
                        </ul>

                    </div>
                </div>
                
            </div>

            <div class="pt-3">
                <insite:Button runat="server" ID="BackButton4" Text="Back" ButtonStyle="Default" Icon="fas fa-arrow-alt-left" CausesValidation="false" />
                <insite:Button runat="server" ID="SaveButton" Text="Confirm Registration" ButtonStyle="Success" Icon="fas fa-cloud-upload" />
                <insite:CancelButton runat="server" ID="CancelButton5" ConfirmText="Are you sure you want to close page without saving any information?" />
            </div>

        </insite:AccordionPanel>
    </insite:Accordion>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            (function () {
                const instance = window.register = Object.freeze({
                    onSeatChanged: onSeatChanged
                });

                function onSeatChanged() {
                    const value = $('input[name="Seats"]:checked').val();
                    $('#<%= SelectedSeat.ClientID %>').val(value);
                    __doPostBack('<%= ChangeSeatButton.UniqueID %>', '');
                }

                let saveIntervalHandler = null;

                const $saveButton = $('#<%= SaveButton.ClientID %>').on('click', function (e) {
                    if (this.disabled) {
                        e.preventDefault();
                        return;
                    }

                    Page_IsValid = null;
                    lockScreen(true);

                    saveIntervalHandler = setInterval(onSaveValidationInterval, 500);
                });

                function onSaveValidationInterval() {
                    if (Page_IsValid === null)
                        return;

                    clearInterval(saveIntervalHandler);

                    if (Page_IsValid === true)
                        return;

                    lockScreen(false);
                    
                    $('html,body').animate({ scrollTop: 0 }, 500);
                }

                function lockScreen(show) {
                    const $overlay = $('#PaymentOverlay');
                    const overlayClass = 'payment-overlay';
                    if (show !== false) {
                        $overlay.addClass(overlayClass);
                        $saveButton.prop('disabled', true);
                    } else {
                        $overlay.removeClass(overlayClass);
                        $saveButton.prop('disabled', false);
                    }
                }
            })();
        </script>
    </insite:PageFooterContent>

</asp:Content>