<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Contacts.Groups.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-4">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="Name" EmptyMessage="Group Name" />
                    </div>
    
                    <div class="mb-2">
                        <insite:GroupTypeComboBox runat="server" ID="GroupType" EmptyMessage="Group Type" />
                    </div>
                    
                    <div class="mb-2">
                        <insite:GroupLabelComboBox runat="server" ID="GroupLabel" EmptyMessage="Group Tag" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="GroupCode" EmptyMessage="Group Code" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="GroupCategory" EmptyMessage="Group Category" />
                    </div>

                    <div class="mb-2">
                        <insite:CollectionItemComboBox runat="server" ID="GroupStatusId" EmptyMessage="Group Status" />
                    </div>

                    <h4>Workflows</h4>

                    <div class="mb-2">
                        <insite:FindWorkflowForm runat="server" ID="SurveyFormIdentifier" EmptyMessage="Mandatory Form" />
                    </div>

                    <div class="mb-2">
                        <insite:FindProduct runat="server" ID="MembershipProductIdentifier" EmptyMessage="Invoice Product" />
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:ItemNameComboBox runat="server" ID="GroupRegion" EmptyMessage="Group Region" />
                    </div>

                    <div class="mb-2">
                        <insite:GroupAddressTypeMultiComboBox runat="server" ID="AddressType" EmptyMessage="Address Type" />
                    </div>

                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

                    <insite:UpdatePanel runat="server" ID="UpdatePanel">
                        <ContentTemplate>
                            <div class="mb-2">
                                <insite:OrganizationGroupCountryComboBox runat="server" ID="Country" EmptyMessage="Country" />
                            </div>

                            <div class="mb-2">
                                <insite:OrganizationGroupProvinceMultiComboBox runat="server" ID="Province" EmptyMessage="Province" />
                            </div>

                            <div class="mb-2">
                                <insite:OrganizationGroupCityMultiComboBox runat="server" ID="City" EmptyMessage="City" />
                            </div>                            
                        </ContentTemplate>
                    </insite:UpdatePanel>                    
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="UtcCreatedSince" runat="server" EmptyMessage="Created &ge;" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="UtcCreatedBefore" runat="server" EmptyMessage="Created &lt;" />
                    </div>

                    <div class="mb-2">
                        <insite:FindGroup runat="server" ID="ParentGroupIdentifier" EmptyMessage="Group Parent" />
                    </div>

                    <div class="mb-2">
                        <insite:ItemIdMultiComboBox runat="server" ID="MembershipStatusItemId" EmptyMessage="Membership Status" />
                    </div>  
                    
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="GroupExpirySince" runat="server" EmptyMessage="Group Expiry Since" />
                    </div>
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="GroupExpiryBefore" runat="server" EmptyMessage="Group Expiry Before" />
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

        <div class="mb-3">
            <insite:ComboBox ID="SortColumns" runat="server">
                <Items>
                    <insite:ComboBoxOption Text="Sort by Group Name" Value="GroupName,GroupIdentifier" />
                    <insite:ComboBoxOption Text="Sort by Group Size" Value="GroupSize,GroupIdentifier" />
                </Items>
            </insite:ComboBox>
        </div>

        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>