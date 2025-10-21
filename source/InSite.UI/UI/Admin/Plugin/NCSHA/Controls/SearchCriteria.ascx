<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Custom.NCSHA.History.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-6">
                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="EventGroupSelector" EmptyMessage="Event Type" />
                    </div>
                    <div class="mb-2">
                        <insite:MultiComboBox runat="server" ID="EventNameSelector" EmptyMessage="Event Name" />
                    </div>
                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="UserSelector" EmptyMessage="User" />
                    </div>
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="UserEmail" EmptyMessage="User Email" />
                    </div>
                </div>
                <div class="col-6">
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="RecordTimeSince" runat="server" EmptyMessage="Event Time &ge;" />
                    </div>
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="RecordTimeBefore" runat="server" EmptyMessage="Event Time &lt;" />
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