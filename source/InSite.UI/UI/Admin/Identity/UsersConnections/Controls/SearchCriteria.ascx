<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.UI.Admin.Identity.UsersConnections.Controls.SearchCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-4">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="FromUserName" EmptyMessage="From User Name" MaxLength="255" />
                    </div>
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="ToUserName" EmptyMessage="To User Name" MaxLength="255" />
                    </div>
                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:CheckBox runat="server" ID="IsManager" Text="Manager" />
                    </div>
                    <div class="mb-2">
                        <insite:CheckBox runat="server" ID="IsSupervisor" Text="Supervisor" />
                    </div>
                    <div class="mb-2">
                        <insite:CheckBox runat="server" ID="IsValidator" Text="Validator" />
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