<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExamEventScheduleCriteria.ascx.cs" Inherits="InSite.Admin.Events.Reports.Controls.ExamEventScheduleCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-md-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-lg-6">
                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="CustomDateRange" EmptyMessage="Custom Date Range">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="Next 30 Days" Text="Next 30 Days" />
                                <insite:ComboBoxOption Value="Next 60 Days" Text="Next 60 Days" />
                                <insite:ComboBoxOption Value="Next 90 Days" Text="Next 90 Days" />
                            </Items>
                        </insite:ComboBox>
                    </div>
    
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="ScheduledSince" runat="server" EmptyMessage="Scheduled &ge;" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="ScheduledBefore" runat="server" EmptyMessage="Scheduled &lt;" />
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>
            </div> 
        </div>
    </div>
    <div class="col-md-3">
        <div class="mb-2">
            <h4>Settings</h4>
            <insite:MultiComboBox ID="ShowColumns" runat="server" />
        </div>
    </div>
    <div class="col-md-3">
        <div class="mb-2">
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>
