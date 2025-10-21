<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.UI.Admin.Database.Entities.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-3">
        <div id="toolbox" class="toolbox-section">

            <h4>Component</h4>

            <div class="mb-2">
                <insite:TextBox runat="server" ID="ComponentType" EmptyMessage="Component Type" MaxLength="40" />
            </div>
            <div class="mb-2">
                <insite:TextBox runat="server" ID="ComponentName" EmptyMessage="Component Name" MaxLength="40" />
            </div>
            <div class="mb-2">
                <insite:TextBox runat="server" ID="ComponentPart" EmptyMessage="Component Part" MaxLength="40" />
            </div>
            <div class="mb-2">
                <insite:TextBox runat="server" ID="EntityName" EmptyMessage="Entity Name" MaxLength="40" />
            </div>

            <insite:FilterButton runat="server" ID="SearchButton" />
            <insite:ClearButton runat="server" ID="ClearButton" />
        </div>
    </div>
    <div class="col-3">
        <div>
            <h4>Storage</h4>
            <div class="mb-2">
                <insite:TextBox runat="server" ID="StorageStructure" EmptyMessage="Storage Structure" MaxLength="40" />
            </div>
            <div class="mb-2">
                <insite:TextBox runat="server" ID="StorageSchema" EmptyMessage="Storage Schema" MaxLength="40" />
            </div>
            <div class="mb-2">
                <insite:TextBox runat="server" ID="StorageTable" EmptyMessage="Storage Table" MaxLength="40" />
            </div>
            <div class="mb-2">
                <insite:TextBox runat="server" ID="StorageKey" EmptyMessage="Storage Key" MaxLength="40" />
            </div>
        </div>       
    </div>
    <div class="col-3">
        <div>
            <h4>Other</h4>
            <div class="mb-2">
                <insite:TextBox runat="server" ID="Keyword" EmptyMessage="Keyword" MaxLength="40" />
            </div>
            <div class="mb-2">
                <insite:TextBox runat="server" ID="CollectionSlug" EmptyMessage="Collection" MaxLength="40" />
            </div>
        </div>
    </div>
    <div class="col-3">       
        <div>
            <h4>Settings</h4>
            <insite:MultiComboBox ID="ShowColumns" runat="server" />

            <h4 class="mt-4">Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>