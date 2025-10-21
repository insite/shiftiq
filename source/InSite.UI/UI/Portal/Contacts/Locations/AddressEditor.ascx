<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddressEditor.ascx.cs" Inherits="InSite.UI.Individual.Controls.AddressEditor" %>

<div class="row">
    <div class="col-md-6">
        <div class="card mb-3">
            <div class="card-body">
                <h3 runat="server" id="AddressHeading"></h3>

                <div class="form-group mb-3" runat="server" ID="DescriptionField">
                    <label for="<%# Description.ClientID %>" class="form-label">
                        <insite:Literal runat="server" Text="Description" />
                    </label>
                    <div>
                        <insite:TextBox ID="Description" runat="server" MaxLength="128" />
                    </div>
                </div>

                <div class="form-group mb-3" runat="server" ID="Street1Field">
                    <label for="<%# Street1.ClientID %>" class="form-label">
                        <insite:Literal runat="server" Text="Address 1" />
                    </label>
                    <div>
                        <insite:TextBox ID="Street1" runat="server" MaxLength="128" />
                    </div>
                </div>

                <div class="form-group mb-3" runat="server" ID="Street2Field">
                    <label for="<%# Street2.ClientID %>" class="form-label">
                        <insite:Literal runat="server" Text="Address 2" />
                    </label>
                    <div>
                        <insite:TextBox ID="Street2" runat="server" MaxLength="128" />
                    </div>
                </div>

                <div class="form-group mb-3" runat="server" ID="CityField">
                    <label for="<%# City.ClientID %>" class="form-label">
                        <insite:Literal runat="server" Text="City" />
                    </label>
                    <div>
                        <insite:TextBox ID="City" runat="server" MaxLength="128" />
                    </div>
                </div>

                <div class="form-group mb-3" runat="server" ID="ProvinceField">
                    <label for="<%# Province.ClientID %>" class="form-label" runat="server" ID="ProvinceFieldLabel">
                        <insite:Literal runat="server" Text="State/Province" />
                    </label>
                    <div>
                        <insite:TextBox ID="Province" runat="server" MaxLength="128" />
                    </div>
                </div>

                <div class="form-group mb-3" runat="server" ID="PostalCodeField">
                    <label for="<%# PostalCode.ClientID %>" class="form-label" runat="server" ID="PostalCodeFieldLabel">
                        <insite:Literal runat="server" Text="Postal Code" />
                    </label>
                    <div>
                        <insite:TextBox ID="PostalCode" runat="server" MaxLength="16" />
                    </div>
                </div>

                <div class="form-group mb-3" runat="server" ID="CountryField">
                    <label for="<%# Country.ClientID %>" class="form-label">
                        <insite:Literal runat="server" Text="Country" />
                    </label>
                    <div>
                        <insite:FindCountry ID="Country" runat="server" EmptyMessage="Country" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="col-md-4">
        
        <div runat="server" id="EmployerAddressPanel" visible="false">
            <div class="card mb-3">
            <div class="card-body">
                <h3 runat="server" id="EmployerAddressTitle"><insite:Literal runat="server" Text="Employer Address" /></h3>
                <div class="form-group mb-3">
                    <label for="<%# EmployerAddress.ClientID %>" class="form-label">
                        <asp:Literal runat="server" ID="EmployerName" />
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="EmployerAddress" />
                    </div>
                </div>
                <div class="form-group mb-3">
                    <div><insite:Button runat="server" ID="CopyEmployerAddress" Icon="fal fa-copy" Text="Copy address"  ButtonStyle="Success" /></div>
                </div>
            </div>
            </div>
        </div>
        <div runat="server" id="HomeAddressPanel" visible="false">
            <div class="card">
            <div class="card-body">
                <h3><insite:Literal runat="server" Text="Home Address" /></h3>
                <div class="form-group mb-3">
                    <label for="<%# HomeAddress.ClientID %>" class="form-label">
                        <insite:Literal runat="server" Text="Address" />
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="HomeAddress" />
                    </div>
                </div>
                <div class="form-group mb-3">
                    <div><insite:Button runat="server" ID="CopyHomeAddress" Icon="fal fa-copy" Text="Copy address"  ButtonStyle="Success" /></div>
                </div>
            </div>
            </div>
        </div>
        <div runat="server" id="EmployeeAddressPanel" visible="false">
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
                    <p><insite:Literal runat="server" Text="No shared this address users." /></p>
                </EmptyDataTemplate>
            </asp:ListView>
        </div>
    </div>

</div>