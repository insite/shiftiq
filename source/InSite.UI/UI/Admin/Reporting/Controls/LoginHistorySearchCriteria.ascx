<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LoginHistorySearchCriteria.ascx.cs" Inherits="InSite.UI.Admin.Reports.Controls.LoginHistorySearchCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-4">
                    <div class="mb-2">
                        <insite:FindOrganization runat="server" ID="OrganizationIdentifier" EmptyMessage="Organization" />
                    </div>
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="UserName" EmptyMessage="User Email" MaxLength="255" />
                    </div>
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="UserLanguage" EmptyMessage="User Language" MaxLength="2" />
                    </div>
                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="SessionStartedSince" runat="server" EmptyMessage="Started Since" />
                    </div>
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="SessionStartedBefore" runat="server" EmptyMessage="Started Before" />
                    </div>
                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="IsValid" EmptyMessage="Status">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Text="Succeeded" Value="True" />
                                <insite:ComboBoxOption Text="Failed" Value="False" />
                            </Items>
                        </insite:ComboBox>
                    </div>
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="UserHostAddress" EmptyMessage="IP Address" MaxLength="100" />
                    </div>
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="UserBrowser" EmptyMessage="Browser" MaxLength="100" />
                    </div>
                    <div class="mb-2">
                        <insite:OrganizationRoleMultiComboBox ID="OrganizationRole" runat="server" EmptyMessage="Organization Role" />
                    </div>
                    
                </div>
            </div> 
        </div>
    </div>
    
    <div class="col-3">
        <div class="mb-4">
            <h4>Settings</h4>
            <insite:MultiComboBox ID="ShowColumns" runat="server" />
        </div>
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>