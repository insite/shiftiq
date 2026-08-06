<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Utilities.Tables.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-4">
        <div id="toolbox" class="toolbox-section">

            <h4>Criteria</h4>
            <div class="mb-2">
                <insite:TextBox runat="server" ID="SchemaName" EmptyMessage="Schema Name" MaxLength="256" />
            </div>
            <div class="mb-2">
                <insite:TextBox runat="server" ID="TableName" EmptyMessage="Table Name" MaxLength="256" />
            </div>

            <insite:FilterButton runat="server" ID="SearchButton" />
            <insite:ClearButton runat="server" ID="ClearButton" />
        </div>
    </div>
    <div class="col-4">
        <div>
            <h4>Settings</h4>
            <insite:MultiComboBox ID="ShowColumns" runat="server" />
        </div>       
    </div>
    <div class="col-4">       
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>