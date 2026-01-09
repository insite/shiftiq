<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Standards.Standards.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-4">
                    <div class="mb-2">
                        <insite:StandardTypeComboBox ID="StandardType" runat="server" EmptyMessage="Standard Type" AllowBlank="true" />
                    </div>

                    <div class="mb-2"> 
                        <insite:TextBox ID="ParentTitle" runat="server" EmptyMessage="Parent Name" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="StandardTier" runat="server" EmptyMessage="Tier" MaxLength="30" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="StandardLabel" runat="server" EmptyMessage="Tag" MaxLength="30" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="Code" runat="server" EmptyMessage="Code" MaxLength="256" />
                    </div>
                    
                    <insite:FilterButton runat="server" ID="SearchButton" />
                    <insite:ClearButton runat="server" ID="ClearButton" />
                </div>
                <div class="col-4">
                    <div class="mb-2"> 
                        <insite:TextBox ID="Title" runat="server" EmptyMessage="Title" MaxLength="256" />
                    </div>

                    <div class="mb-2"> 
                        <insite:TextBox ID="ContentName" runat="server" EmptyMessage="Internal Name" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="Keyword" runat="server" EmptyMessage="Keyword" MaxLength="30" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="Number" runat="server" EmptyMessage="Number" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:FindDepartment runat="server" ID="DepartmentIdentifier" EmptyMessage="Department" />
                    </div>
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:MultiComboBox runat="server" ID="TagSelector" EmptyMessage="Flags" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="Scope" EmptyMessage="Scope" AllowBlank="true" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="UtcModifiedSince" runat="server" EmptyMessage="Last Modified Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="UtcModifiedBefore" runat="server" EmptyMessage="Last Modified Before" />
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
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>