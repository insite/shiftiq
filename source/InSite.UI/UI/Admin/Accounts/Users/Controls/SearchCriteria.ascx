<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Accounts.Users.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-4">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="Name" EmptyMessage="Name" MaxLength="256" />
                    </div>
    
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="Email" EmptyMessage="Email" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox ID="IsEmailValid" runat="server" EmptyMessage="Email Validity">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="True" Text="Valid Email" />
                                <insite:ComboBoxOption Value="False" Text="Invalid Email" />
                            </Items>
                        </insite:ComboBox>
                    </div>
    
                    <div class="mb-2">
                        <insite:ComboBox ID="EmailVerified" runat="server" EmptyMessage="Email Verification">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="True" Text="Email Verified" />
                                <insite:ComboBoxOption Value="False" Text="Email Not Verified" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="IsLicensed" EmptyMessage="License Status">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="True" Text="Agreed to Terms" />
                                <insite:ComboBoxOption Value="False" Text="Not Agreed to Terms" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="IsAccessGranted" EmptyMessage="Access Status">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="True" Text="Granted" />
                                <insite:ComboBoxOption Value="False" Text="Not Granted" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector runat="server" ID="UserAccessGrantedSince" EmptyMessage="Access Granted Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector runat="server" ID="UserAccessGrantedBefore" EmptyMessage="Access Granted Before" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="UserSessionStatus" EmptyMessage="Sign In Status">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="Signed In" Text="Signed In" />
                                <insite:ComboBoxOption Value="Never Signed In" Text="Never Signed In" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector runat="server" ID="LastAuthenticatedSince" EmptyMessage="Last Authenticated Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector runat="server" ID="LastAuthenticatedBefore" EmptyMessage="Last Authenticated Before" />
                    </div>

                </div>
                <div class="col-4">

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="CompanyName" EmptyMessage="Organization" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="OrganizationStatus" EmptyMessage="Organization Status">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="No Organization" Text="No Organization" />
                                <insite:ComboBoxOption Value="Single Organization" Text="Single Organization" />
                                <insite:ComboBoxOption Value="Multiple Organizations" Text="Multiple Organizations" />
                            </Items>
                        </insite:ComboBox>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="col-3">       
        <div class="pb-3">
            <h4>Settings</h4>
            <insite:MultiComboBox ID="ShowColumns" runat="server" />
        </div>
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>
