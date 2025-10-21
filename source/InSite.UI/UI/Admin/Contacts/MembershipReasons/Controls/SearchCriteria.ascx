<%@ Control Language="C#" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.UI.Admin.Contacts.MembershipReasons.Controls.SearchCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-6">
                    <div class="mb-2">
                        <insite:FindGroup runat="server" ID="MembershipGroup" EmptyMessage="Membership Group" CurrentOrganizationOnly="true" />
                    </div>

                    <div class="mb-2">
                        <insite:FindPerson runat="server" ID="MembershipUser" EmptyMessage="Membership Person" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="ReasonType" EmptyMessage="Reason Type">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="Referral" Text="Referral" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="ReasonSubtype" EmptyMessage="Reason Subtype" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="PersonOccupation" EmptyMessage="Person Occupation" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="PersonCode" EmptyMessage="Person Code" />
                    </div>

                    <div class="mb-2">
                        <insite:FindPerson runat="server" ID="ReasonCreatedBy" EmptyMessage="Created By" />
                    </div>
                </div>
                <div class="col-6">
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector runat="server" ID="ReasonEffectiveSince" EmptyMessage="Reason Effective Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector runat="server" ID="ReasonEffectiveBefore" EmptyMessage="Reason Effective Before" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector runat="server" ID="ReasonExpirySince" EmptyMessage="Reason Expiry Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector runat="server" ID="ReasonExpiryBefore" EmptyMessage="Reason Expiry Before" />
                    </div>
                </div>
            </div>
            
            <insite:FilterButton runat="server" ID="SearchButton" />
            <insite:ClearButton runat="server" ID="ClearButton" />
        </div>
    </div>
    <div class="col-3">
        <div>
            <h4>Settings</h4>
            <insite:MultiComboBox ID="ShowColumns" runat="server" />
        </div>       
    </div>
    <div class="col-3">       
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>
