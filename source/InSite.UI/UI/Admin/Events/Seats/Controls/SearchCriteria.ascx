<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Events.Seats.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-6">
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="EventScheduledSince" runat="server" EmptyMessage="Event Date Since" />
                    </div>
    
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="EventScheduledBefore" runat="server" EmptyMessage="Event Date Before" />
                    </div>

                    <div class="mb-2">
                        <insite:AchievementComboBox runat="server" ID="AchievementIdentifier" EmptyMessage="Event Achievement" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="EventTitle" EmptyMessage="Event Title" MaxLength="256" />
                    </div>

                    <insite:FilterButton runat="server" ID="SearchButton" />
                    <insite:ClearButton runat="server" ID="ClearButton" />
                </div>
                <div class="col-6">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="SeatTitle" EmptyMessage="Seat Name" MaxLength="200" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox ID="IsAvailable" runat="server" EmptyMessage="Seat Availability">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="True" Text="Available for purchase" />
                                <insite:ComboBoxOption Value="False" Text="Hide this ticket to prevent purchase" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox ID="IsTaxable" runat="server" EmptyMessage="Seat Taxes">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="True" Text="Yes" />
                                <insite:ComboBoxOption Value="False" Text="No" />
                            </Items>
                        </insite:ComboBox>
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

