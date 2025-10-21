<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Accounts.Developers.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-6">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="DeveloperName" EmptyMessage="Developer Name" />
                    </div>
    
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector runat="server" ID="TokenIssuedSince" EmptyMessage="Token Issued Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector runat="server" ID="TokenIssuedBefore" EmptyMessage="Token Issued Before" />
                    </div>
                </div>
                <div class="col-6">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="OrganizationCode" EmptyMessage="Organization Code" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector runat="server" ID="TokenExpiredSince" EmptyMessage="Token Expired Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector runat="server" ID="TokenExpiredBefore" EmptyMessage="Token Expired Before" />
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
