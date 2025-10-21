<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmployerDetail.ascx.cs" Inherits="InSite.UI.Portal.Events.Classes.Controls.EmployerDetail" %>

<div class="row">
    <div class="col-md-4">

        <div class="card border-0 shadow h-100">
            <div class="card-body">

                <h5 class="card-title">
                    <insite:Literal runat="server" Text="Company Name" />
                </h5>

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
        </div>

    </div>
    <div runat="server" id="AddressAndPhoneColumn" class="col-md-4">
        <div class="card border-0 shadow h-100">
            <div class="card-body">

                <h5 class="card-title" runat="server" id="EmployerGroupName"></h5>

                <ul class="list-unstyled mb-0">
                    <li id="AddressArea" runat="server">
                        <i class="far fa-map-marker-alt fs-lg mt-2 mb-0 text-primary"></i>
                        <div class="ps-3">
                            <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Mailing Address" /></span>
                            <asp:Literal ID="EmployerAddress" runat="server" />
                        </div>
                    </li>
                    <li runat="server" id="EmployerPhoneArea" class="d-flex pt-2">
                        <i class="far fa-phone fs-lg mt-2 mb-0 text-primary"></i>
                        <div>
                            <div runat="server" id="EmployerPhoneField" class="ps-3">
                                <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Phone" /></span>
                                <asp:Literal ID="EmployerPhone" runat="server" />
                            </div>
                        </div>
                        </li>
                </ul>


            </div>
        </div>
    </div>

    <div runat="server" id="EmployerContactColumn" class="col-md-4">
        <div class="card border-0 shadow h-100">
            <div class="card-body">

                <h5 class="card-title">
                    <insite:Literal runat="server" Text="Company Contact" />
                </h5>

                <div class="d-flex pt-2 pb-3"><i class="far fa-user fs-lg mt-2 mb-0 text-primary"></i>
                    <div class="ps-3"><span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Name" /></span><span class="d-block"><asp:Literal ID="EmployerContactName" runat="server" /></span></div>
                </div>

                <div runat="server" id="EmployerContactPhoneNumberField" class="d-flex pt-2 pb-3"><i class="far fa-phone fs-lg mt-2 mb-0 text-primary"></i>
                    <div class="ps-3"><span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Phone" /></span><span class="d-block"><asp:Literal ID="EmployerContactPhoneNumber" runat="server" /></span></div>
                </div>

                <div class="d-flex pt-2 pb-3"><i class="far fa-envelope fs-lg mt-2 mb-0 text-primary"></i>
                    <div class="ps-3"><span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Email" /></span><span class="d-block"><asp:Literal ID="EmployerContactEmail" runat="server" /></span></div>
                </div>

            </div>
        </div>
    </div>

    <div runat="server" id="NewEmployerColumn" class="col-md-4">
        <div class="card border-0 shadow h-100">
            <div class="card-body">
                <h5 class="card-title">
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
                        <insite:ProvinceComboBox ID="NewCompanyProvinceSelector" runat="server" />
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
    </div>

    <div runat="server" id="NewEmployerContactColumn" class="col-md-4">
        <div class="card border-0 shadow h-100">
            <div class="card-body">
                <h5 class="card-title">
                    <insite:Literal runat="server" Text="New Company Contact" />
                </h5>

                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="NewCompanyContactUpdatePanel" />

                <insite:UpdatePanel runat="server" ID="NewCompanyContactUpdatePanel">
                    <ContentTemplate>
                        <div class="form-group mb-3">
                            <insite:RadioButtonList runat="server" ID="NewCompanyContactType" RepeatDirection="Vertical" RepeatLayout="Flow">
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

</div>
