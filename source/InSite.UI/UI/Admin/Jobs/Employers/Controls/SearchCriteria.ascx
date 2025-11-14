<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Jobs.Employers.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-6">

                    <div class="mb-2">
                        <insite:GroupMultiComboBox runat="server" ID="DepartmentGroupIdentifier" EmptyMessage="Department" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="EmployerName" EmptyMessage="Employer Name" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="Email" EmptyMessage="Email" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="Address" EmptyMessage="Address" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="CountryName" EmptyMessage="Country" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="ProvinceName" EmptyMessage="Province" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="CityName" EmptyMessage="City" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox ID="IsApproved" runat="server" EmptyMessage="Contact Status">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="True" Text="Approved" />
                                <insite:ComboBoxOption Value="False" Text="Not Approved" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>
                <div class="col-6">
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="DateRegisteredSince" runat="server" EmptyMessage="Date Registered Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="DateRegisteredBefore" runat="server" EmptyMessage="Date Registered Before" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="EmployerContactName" EmptyMessage="Employer Contact Name" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox ID="EmployerSize" runat="server" EmptyMessage="Employer Size">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="Small (1-99 employees)" Text="Small (1-99 employees)" />
                                <insite:ComboBoxOption Value="Medium (100-499 employees)" Text="Medium (100-499 employees)" />
                                <insite:ComboBoxOption Value="Large (500+ employees)" Text="Large (500+ employees)" />
                                <insite:ComboBoxOption Value="Government" Text="Government" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox ID="Industry" runat="server" EmptyMessage="Industry" AllowBlank="true" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox ID="Sector" runat="server" EmptyMessage="Sector" AllowBlank="true" Visible="false"/>
                    </div>
                </div>
            </div> 
        </div>
    </div>
    <div class="col-3">       
        <div class="mb-2">
            <h4>Settings</h4>
            <insite:MultiComboBox ID="ShowColumns" runat="server" />
        </div>
        <div class="mb-4">
            <insite:ComboBox ID="SortColumns" runat="server">
                <Items>
                    <insite:ComboBoxOption Text="Sort by Date Created" Value="EmployerContactCreated DESC" />
                    <insite:ComboBoxOption Text="Sort by Group Name" Value="GroupName" />
                </Items>
            </insite:ComboBox>
        </div>
    </div>
    <div class="col-3">              
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>
