<%@ Page Language="C#" CodeBehind="AddToWaitingList.aspx.cs" Inherits="InSite.UI.Portal.Events.Classes.AddToWaitingList" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:ValidationSummary runat="server" />

    <div class="row" runat="server" ID="PersonDetailsPanel" >
        <div class="col-md-6 mb-3 mb-md-0">
            <div class="card border-0 shadow h-100">
                <div class="card-header">
                    <h3 class="m-0 text-primary">
                        <i class="far fa-user me-1"></i>
                        <insite:Literal runat="server" Text="Personal" />
                    </h3>
                </div>
                <div class="card-body">

                    <div runat="server" id="FirstNameField" class="form-group mb-3">
                        <label class="form-label" for="<%# FirstName.ClientID %>">
                            <insite:Literal runat="server" Text="First Name" />
                            <insite:RequiredValidator runat="server" FieldName="First Name" ID="FirstNameRequiredValidator" ControlToValidate="FirstName" Display="None" />
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
                            <insite:RequiredValidator runat="server" ID="LastNameRequiredValidator" FieldName="Last Name" ControlToValidate="LastName" Display="None" />
                        </label>
                        <div>
                            <insite:TextBox ID="LastName" runat="server" MaxLength="32" Width="100%" />
                        </div>
                    </div>

                    <div runat="server" id="EmailField" class="form-group mb-3">
                        <label class="form-label" for="<%# Email.ClientID %>">
                            <insite:Literal runat="server" Text="Email" />
                            <insite:RequiredValidator runat="server" ID="EmailRequiredValidator"  FieldName="Email" ControlToValidate="Email" Display="None" />
                            <insite:CustomValidator runat="server" ID="EmailUniqueValidator"
                                ControlToValidate="Email"
                                Display="None"
                                ErrorMessage="There is another existing user account with the same Email address" />
                        </label>
                        <div>
                            <div>
                                <insite:TextBox ID="Email" runat="server" MaxLength="128" />
                            </div>
                        </div>
                    </div>

                    <div runat="server" id="BirthdateField" class="form-group mb-3">
                        <label class="form-label" for="<%# Birthdate.ClientID %>">
                            <insite:Literal runat="server" Text="Birthdate" />
                            <insite:RequiredValidator runat="server" ID="BirthdateRequiredValidator"  FieldName="Birthdate" ControlToValidate="Birthdate" Display="None" />
                        </label>
                        <div>
                            <insite:DateSelector ID="Birthdate" runat="server" />
                        </div>
                    </div>

                    <div runat="server" id="PersonCodeField" class="form-group mb-3">
                        <label class="form-label" for="<%# PersonCode.ClientID %>">
                            <insite:ContentTextLiteral runat="server" ContentLabel="Person Code"/>
                            <insite:RequiredValidator runat="server" ID="PersonCodeRequiredValidator"  FieldName="Person Code" ControlToValidate="PersonCode" Display="None" />
                            <insite:CustomValidator runat="server" ID="PersonCodeUniqueValidator"
                                ControlToValidate="PersonCode"
                                Display="None"
                                ErrorMessage="Learner ID entered is assigned to another user" />
                        </label>
                        <div>
                            <insite:TextBox ID="PersonCode" runat="server" Width="100%" MaxLength="20" />
                        </div>
                        <div class="form-text">
                            <insite:Literal runat="server" ID="PersonCodeHelpText" />
                        </div>
                    </div>

                    <div runat="server" id="EmployerField" class="form-group mb-3">
                        <label class="form-label">
                            <insite:Literal runat="server" Text="Employer" />
                            <insite:RequiredValidator runat="server" ID="EmployerGroupValidator" FieldName="Employer" ControlToValidate="EmployerGroupIdentifier" Display="None" />
                        </label>
                        <div class="mb-3">
                            <div class="mb-1">
                                <insite:RadioButton runat="server" ID="EmployerTypeExisting" Text="Click to search for company:" Checked="true" GroupName="EmployerType" />
                                <div runat="server" id="ExistingEmployerPanel" class="ms-3 my-2">
                                    <insite:FindGroup ID="EmployerGroupIdentifier" runat="server" ShowCities="true" />
                                </div>
                            </div>
                            <div class="fs-sm mt-2 mb-1">If company isn't found in list above:</div>
                            <insite:RadioButton runat="server" ID="EmployerTypeNew" Text="Add a new company" GroupName="EmployerType" />
                        </div>
                    </div>

                </div>
            </div>
        </div>

        <div runat="server" id="ExistingEmployerColumn" class="col-md-6">
            <div class="card border-0 shadow">
                <div class="card-header">
                    <h3 class="m-0 text-primary">
                        <i class="far fa-user-tie me-1"></i>
                        <asp:Literal runat="server" ID="ExistingEmployerName" />
                    </h3>
                </div>

                <div class="card-body">

                    <insite:Container runat="server" ID="ExistingEmployerAddress">
                        <ul class="list-unstyled mb-3">
                            <li id="ExistingEmployerAddressItem" runat="server">
                                <i class="far fa-map-marker-alt fs-lg mt-2 mb-0 text-primary"></i>
                                <div class="ps-3">
                                    <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Mailing Address" /></span>
                                    <asp:HyperLink ID="ExistingEmployerAddressLink" runat="server" Target="_blank" />
                                </div>
                            </li>
                            <li runat="server" id="ExistingEmployerPhoneItem" class="d-flex pt-2">
                                <i class="far fa-phone fs-lg mt-2 mb-0 text-primary"></i>
                                <div>
                                    <div runat="server" id="ExistingEmployerPhoneField" class="ps-3">
                                        <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Phone" /></span>
                                        <asp:Literal ID="ExistingEmployerPhone" runat="server" />
                                    </div>
                                    <div runat="server" id="ExistingEmployerFaxField" class="ps-3">
                                        <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Fax" /></span>
                                        <asp:Literal ID="ExistingEmployerFax" runat="server" />
                                    </div>
                                </div>
                            </li>
                        </ul>
                    </insite:Container>

                    <insite:Container runat="server" ID="ExistingEmployerContact">
                        <h6 class="mb-3">
                            <insite:Literal runat="server" Text="Company Contact" />
                        </h6>

                        <div class="d-flex pt-2 pb-3"><i class="far fa-user fs-lg mt-2 mb-0 text-primary"></i>
                            <div class="ps-3"><span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Name" /></span><span class="d-block"><asp:Literal ID="ExistingEmployerContactName" runat="server" /></span></div>
                        </div>

                        <div runat="server" id="ExistingEmployerContactPhoneField" class="d-flex pt-2 pb-3"><i class="far fa-phone fs-lg mt-2 mb-0 text-primary"></i>
                            <div class="ps-3"><span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Phone" /></span><span class="d-block"><asp:Literal ID="ExistingEmployerContactPhone" runat="server" /></span></div>
                        </div>

                        <div class="d-flex pt-2 pb-3"><i class="far fa-envelope fs-lg mt-2 mb-0 text-primary"></i>
                            <div class="ps-3"><span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Email" /></span><span class="d-block"><asp:Literal ID="ExistingEmployerContactEmail" runat="server" /></span></div>
                        </div>
                    </insite:Container>

                </div>
            </div>
        </div>

        <div runat="server" id="NewEmployerColumn" class="col-md-6">
            <div class="card border-0 shadow">
                <div class="card-header">
                    <h3 class="m-0 text-primary">
                        <i class="far fa-user-tie me-1"></i>
                        <insite:Literal runat="server" Text="New Company" />
                    </h3>
                </div>

                <div class="card-body">
                    <div class="form-group mb-3">
                        <label class="form-label" for="<%# NewEmployerName.ClientID %>">
                            <insite:Literal runat="server" Text="Name" />
                            <insite:RequiredValidator runat="server" FieldName="Name" ControlToValidate="NewEmployerName" Display="None" />
                        </label>
                        <insite:TextBox ID="NewEmployerName" runat="server" MaxLength="90" Width="100%" />
                    </div>
                    <div class="form-group mb-3">
                        <label class="form-label" for="<%# NewEmployerPhone.ClientID %>">
                            <insite:Literal runat="server" Text="Phone" />
                            <insite:RequiredValidator runat="server" FieldName="Phone" ControlToValidate="NewEmployerPhone" Display="None" />
                        </label>
                        <insite:TextBox ID="NewEmployerPhone" runat="server" MaxLength="32" Width="100%" />
                    </div>

                    <div class="form-group mb-3">
                        <label for="<%# NewEmployerStreet1.ClientID %>" class="form-label">
                            <insite:Literal runat="server" Text="Address 1" />
                            <insite:RequiredValidator runat="server" FieldName="Address 1" ControlToValidate="NewEmployerStreet1" Display="None" />
                        </label>
                        <div>
                            <insite:TextBox ID="NewEmployerStreet1" runat="server" MaxLength="128" />
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label for="<%# NewEmployerCity.ClientID %>" class="form-label">
                            <insite:Literal runat="server" Text="City" />
                            <insite:RequiredValidator runat="server" FieldName="City" ControlToValidate="NewEmployerCity" Display="None" />
                        </label>
                        <div>
                            <insite:TextBox ID="NewEmployerCity" runat="server" MaxLength="128" />
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label for="<%# NewEmployerProvinceSelector.ClientID %>" class="form-label">
                            <insite:Literal runat="server" Text="Province" />
                            <insite:RequiredValidator runat="server" FieldName="Province" ControlToValidate="NewEmployerProvinceSelector" Display="None" />
                        </label>
                        <div>
                            <insite:ProvinceComboBox ID="NewEmployerProvinceSelector" runat="server" UseCodeAsValue="true" />
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label for="<%# NewEmployerPostalCode.ClientID %>" class="form-label">
                            <insite:Literal runat="server" Text="Postal Code" />
                            <insite:RequiredValidator runat="server" FieldName="Postal Code" ControlToValidate="NewEmployerPostalCode" Display="None" />
                        </label>
                        <div>
                            <insite:TextBox ID="NewEmployerPostalCode" runat="server" MaxLength="16" />
                        </div>
                    </div>

                    <h6 class="my-3">
                        <insite:Literal runat="server" Text="Company Contact" />
                    </h6>

                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="NewEmployerContactUpdatePanel" />

                    <insite:UpdatePanel runat="server" ID="NewEmployerContactUpdatePanel">
                        <ContentTemplate>
                            <div class="form-group mb-3">
                                <insite:RadioButtonList runat="server" ID="NewEmployerContactType" RepeatDirection="Vertical" RepeatLayout="Table">
                                    <asp:ListItem Value="Existing" Text="I'm the primary contact" />
                                    <asp:ListItem Value="New" Text="Someone else is the primary contact" Selected="True" />
                                </insite:RadioButtonList>
                            </div>

                            <insite:Container runat="server" id="NewEmployerContactFields">
                                <div class="form-group mb-3">
                                    <label class="form-label" for="<%# NewEmployerContactFirstName.ClientID %>">
                                        <insite:Literal runat="server" Text="First Name" />
                                        <insite:RequiredValidator runat="server" FieldName="First Name" ControlToValidate="NewEmployerContactFirstName" Display="None" />
                                    </label>
                                    <insite:TextBox ID="NewEmployerContactFirstName" runat="server" MaxLength="32" Width="100%" />
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label" for="<%# NewEmployerContactLastName.ClientID %>">
                                        <insite:Literal runat="server" Text="Last Name" />
                                        <insite:RequiredValidator runat="server" FieldName="Last Name" ControlToValidate="NewEmployerContactLastName" Display="None" />
                                    </label>
                                    <insite:TextBox ID="NewEmployerContactLastName" runat="server" MaxLength="32" Width="100%" />
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label" for="<%# NewEmployerContactPhone.ClientID %>">
                                        <insite:Literal runat="server" Text="Phone" />
                                        <insite:RequiredValidator runat="server" FieldName="Phone" ControlToValidate="NewEmployerContactPhone" Display="None" />
                                    </label>
                                    <insite:TextBox ID="NewEmployerContactPhone" runat="server" MaxLength="32" Width="100%" />
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label" for="<%# NewEmployerContactEmail.ClientID %>">
                                        <insite:Literal runat="server" Text="Email" />
                                        <insite:RequiredValidator runat="server" FieldName="Email" ControlToValidate="NewEmployerContactEmail" Display="None" />
                                    </label>
                                    <insite:TextBox ID="NewEmployerContactEmail" runat="server" MaxLength="32" Width="100%" />
                                </div>
                            </insite:Container>
                        </ContentTemplate>
                    </insite:UpdatePanel>

                </div>
            </div>
        </div>
    </div>

    <div class="mt-3">
        <insite:Button runat="server" ID="SaveButton" Text="Add to Waiting List" ButtonStyle="Success" Icon="fas fa-user-plus" />
        <insite:CancelButton runat="server" ID="CancelButton" ConfirmText="Are you sure to cancel?" />
    </div>

</asp:Content>
