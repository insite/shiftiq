<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.UI.Admin.Records.Validators.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-6">
                    <div class="mb-2">
                        <insite:TextBox ID="JournalSetupName" runat="server" EmptyMessage="Logbook Name" MaxLength="256" />
                    </div>
    
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="JournalSetupCreatedSince" runat="server" EmptyMessage="Created &ge;" />
                    </div>
                    
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="JournalSetupCreatedBefore" runat="server" EmptyMessage="Created &lt;" />
                    </div>

                    <div class="mb-2">
                        <insite:FindAchievement runat="server" ID="AchievementIdentifier" EmptyMessage="Achievement" />
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>
                <div class="col-6">
                    <div class="mb-2">
                        <insite:TextBox ID="EventTitle" runat="server" EmptyMessage="Event Title" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="EventScheduledSince" runat="server" EmptyMessage="Scheduled &ge;" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="EventScheduledBefore" runat="server" EmptyMessage="Scheduled &lt;" />
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
    </div>
    <div class="col-3">
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>
