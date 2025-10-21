<%@ Control AutoEventWireup="true" CodeBehind="AddressEditor.ascx.cs" Inherits="InSite.Admin.Contacts.Addresses.Controls.AddressEditor" Language="C#" %>

<div class="row">
    <div class="col-lg-7 mb-3 mb-lg-0">
        <div class="card">
            <div class="card-body">

                <h3 runat="server" id="AddressHeading"></h3>

                <div runat="server" id="DescriptionField" class="form-group mb-3">
                    <label class="form-label">
                        <insite:Literal runat="server" Text="Description" />
                        <insite:RequiredValidator runat="server" ID="DescriptionRequired" Visible="false" ControlToValidate="Description" FieldName="Description" Display="Dynamic" />
                    </label>
                    <insite:TextBox ID="Description" runat="server" MaxLength="128" />
                    <div class="form-text">
                    </div>
                </div>

                <div runat="server" id="Street1Field" class="form-group mb-3">
                    <label class="form-label">
                        <insite:Literal runat="server" Text="Address 1" />
                        <insite:RequiredValidator runat="server" ID="Street1Required" Visible="false" ControlToValidate="Street1" FieldName="Address 1" Display="Dynamic" />
                    </label>
                    <insite:TextBox ID="Street1" runat="server" MaxLength="128" />
                    <div class="form-text">
                    </div>
                </div>

                <div runat="server" id="Street2Field" class="form-group mb-3">
                    <label class="form-label">
                        <insite:Literal runat="server" Text="Address 2" />
                        <insite:RequiredValidator runat="server" ID="Street2Required" Visible="false" ControlToValidate="Street2" FieldName="Address 2" Display="Dynamic" />
                    </label>
                    <insite:TextBox ID="Street2" runat="server" MaxLength="128" />
                    <div class="form-text">
                    </div>
                </div>

                <div runat="server" id="CityField" class="form-group mb-3">
                    <label class="form-label">
                        <insite:Literal runat="server" Text="City" />
                        <insite:RequiredValidator runat="server" ID="CityRequired" Visible="false" ControlToValidate="City" FieldName="City" Display="Dynamic" />
                    </label>
                    <insite:TextBox ID="City" runat="server" MaxLength="128" />
                    <div class="form-text">
                    </div>
                </div>

                <div runat="server" id="ProvinceField" class="form-group mb-3">
                    <label runat="server" id="ProvinceFieldLabel" class="form-label">
                        State/Province
                    </label>
                    <insite:TextBox ID="Province" runat="server" MaxLength="128" />
                    <div class="form-text">
                    </div>
                </div>

                <div runat="server" id="PostalCodeField" class="form-group mb-3">
                    <label runat="server" id="PostalCodeFieldLabel" class="form-label">
                        <insite:Literal runat="server" Text="Postal Code" />
                        <insite:RequiredValidator runat="server" ID="PostalCodeRequired" Visible="false" ControlToValidate="PostalCode" FieldName="Postal Code" Display="Dynamic" />
                    </label>
                    <insite:TextBox ID="PostalCode" runat="server" MaxLength="16" />
                    <div class="form-text">
                    </div>
                </div>

                <div runat="server" id="CountryField" class="form-group mb-3">
                    <label class="form-label">
                        <insite:Literal runat="server" Text="Country" />
                        <insite:CustomValidator runat="server" ID="CountryValidator" ControlToValidate="CountrySelector" ErrorMessage="Country is required for the address" />
                    </label>
                    <insite:FindCountry ID="CountrySelector" runat="server" CanadaFirst="true" />
                    <div class="form-text">
                    </div>
                </div>

                <div runat="server" id="MapField" class="form-group mb-3">
                    <asp:HyperLink
                        Visible="false"
                        Width="100%"
                        CssClass="btn btn-outline-secondary"
                        runat="server"
                        NavigateUrl="https://www.google.com/maps"
                        ID="MapURL"
                        Target="_new">
                        <i class="fas fa-map"></i>&nbsp;&nbsp;View Map
                    </asp:HyperLink>
                </div>

            </div>
        </div>
    </div>
    <div class="col-lg-5">

        <insite:Container runat="server" ID="EmployerAddressPanel" Visible="false">
            <div class="card">
                <div class="card-body">

                    <h3><insite:Literal runat="server" ID="EmployerAddressTitle" Text="Employer Address" /></h3>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            <asp:Literal runat="server" ID="EmployerName" />
                        </label>
                        <div>
                            <asp:HyperLink runat="server" NavigateUrl="https://www.google.com/maps" ID="EmployerAddress" Target="_new" />
                        </div>
                        <div class="form-text">
                        </div>
                    </div>
                    <div class="form-group mb-3">
                        <div><insite:Button runat="server" ID="CopyEmployerAddress" Icon="fal fa-copy" Text="Copy address"  ButtonStyle="Success" /></div>
                    </div>

                </div>
            </div>
        </insite:Container>

        <insite:Container runat="server" ID="HomeAddressPanel" Visible="false">
            <div class="card">
                <div class="card-body">

                    <h3><insite:Literal runat="server" Text="Home Address" /></h3>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            <insite:Literal runat="server" Text="Address" />
                        </label>
                        <div>
                            <asp:HyperLink runat="server" NavigateUrl="https://www.google.com/maps" ID="HomeAddress" Target="_new" />
                        </div>
                        <div class="form-text">
                        </div>
                    </div>
                    <div class="form-group mb-3">
                        <div><insite:Button runat="server" ID="CopyHomeAddress" Icon="fal fa-copy" Text="Copy address"  ButtonStyle="Success" /></div>
                    </div>

                </div>
            </div>
        </insite:Container>

        <insite:Container runat="server" ID="EmployeeAddressPanel" Visible="false">
            <div class="card">
                <div class="card-body">

                    <h3><insite:Literal runat="server" Text="Employee Address" /></h3>

                    <asp:ListView runat="server" ID="SharedAddressUsers">
                        <LayoutTemplate>
                            <ul>
                                <asp:PlaceHolder runat="server" ID="itemPlaceholder" />
                            </ul>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <li>
                                <%# Eval("FullName") %>
                            </li>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <p>No shared this address users.</p>
                        </EmptyDataTemplate>
                    </asp:ListView>

                </div>
            </div>
        </insite:Container>

    </div>
</div>

