<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Contacts.Memberships.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-6">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="GroupName" EmptyMessage="Group Name" />
                    </div>
    
                    <div class="mb-2">
                        <insite:GroupTypeComboBox runat="server" ID="GroupType" EmptyMessage="Group Type" />
                    </div> 
                    
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="GroupLabel" EmptyMessage="Group Tag" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="UserFullName" EmptyMessage="Person Name" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="UserCode" EmptyMessage="Person Code" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="UserEmail" EmptyMessage="Person Email" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>

                </div>
                <div class="col-6">

                    <div class="mb-2">
                        <insite:ItemIdMultiComboBox runat="server" ID="MembershipStatusItemId" EmptyMessage="Membership Status" />
                    </div>

                    <div class="mb-2">
                        <insite:ItemNameMultiComboBox runat="server" ID="MembershipFunction" EmptyMessage="Membership Function">
                            <Settings UseCurrentOrganization="true" UseGlobalOrganizationIfEmpty="true" CollectionName="Contacts/Memberships/Membership/Type" />
                        </insite:ItemNameMultiComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:BooleanComboBox runat="server" ID="MembershipFunctionStatus" TrueText="Has Membership Function" FalseText="Missing Membership Function" EmptyMessage="Membership Function Status" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="EffectiveSince" runat="server" EmptyMessage="Effective Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="EffectiveBefore" runat="server" EmptyMessage="Effective Before" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="ExpirySince" runat="server" EmptyMessage="Expiry Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="ExpiryBefore" runat="server" EmptyMessage="Expiry Before" />
                    </div>

                </div>
            </div> 
        </div>
    </div>
    <div class="col-3">
        <div>
            <h4>Settings</h4>
            <div class="mb-2">
                <insite:MultiComboBox ID="ShowColumns" runat="server" />
            </div>
            
            <div class="mb-2">
                <insite:ComboBox ID="SortColumns" runat="server">
                    <Items>
                        <insite:ComboBoxOption Text="Sort by Group Name, User Full Name" Value="GroupName,UserFullName" />
                        <insite:ComboBoxOption Text="Sort by User Full Name, Group Name" Value="UserFullName,GroupName" />
                    </Items>
                </insite:ComboBox>
            </div>
        </div>       
    </div>
    <div class="col-3">       
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>