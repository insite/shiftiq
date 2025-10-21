<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Accounts.Divisions.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-6">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="DivisionName" EmptyMessage="Division Name" />
                    </div>    
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="DivisionCode" EmptyMessage="Division Code" />
                    </div>
                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>
                <div class="col-6">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="CompanyName" EmptyMessage="Organization Name" MaxLength="256" />
                    </div>
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector runat="server" ID="CreatedSince" EmptyMessage="Created &ge;" />
                    </div>
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector runat="server" ID="CreatedBefore" EmptyMessage="Created &lt;" />
                    </div>
                </div>
            </div>                        
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
