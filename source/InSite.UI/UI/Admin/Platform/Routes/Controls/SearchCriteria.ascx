<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Utilities.Actions.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">

            <h4>Criteria</h4>

            <div class="row">

                <div class="col-6">

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="ActionUrl" EmptyMessage="Action URL" />
                    </div>
    
                    <div class="mb-2">
                        <insite:RouteTypeComboBox runat="server" ID="ActionType" EmptyMessage="Action Type" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="ActionName" EmptyMessage="Action Name" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="ActionList" EmptyMessage="Action List" />
                    </div>
    
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="ControllerPath" EmptyMessage="Controller Path" />
                    </div>

                </div>

                <div class="col-6">

                    <div class="mb-2">
                        <insite:BooleanComboBox runat="server" ID="IsLinkedToHelp" EmptyMessage="Help Status" TrueText="Linked" FalseText="Not Linked" />
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