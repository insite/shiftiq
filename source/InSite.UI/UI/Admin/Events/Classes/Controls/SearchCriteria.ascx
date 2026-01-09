<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Events.Classes.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-4">
                    <div class="mb-2">
                        <insite:AchievementComboBox runat="server" ID="AchievementIdentifier" EmptyMessage="Achievement" />
                    </div>
    
                    <div class="mb-2">
                        <insite:TextBox ID="EventTitle" runat="server" EmptyMessage="Title" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="Venue" runat="server" EmptyMessage="Venue" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="CommentKeyword" runat="server" EmptyMessage="Comment" MaxLength="256" />
                    </div>                    

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:MultiComboBox runat="server" ID="EventStatus" EmptyMessage="Event Status" Multiple-ActionsBox="true" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="IsRegistrationLocked" EmptyMessage="Registrations">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="Locked" Text="Locked" />
                                <insite:ComboBoxOption Value="Unlocked" Text="Unlocked" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:FindGroup runat="server" ID="GroupIdentifier" EmptyMessage="Group Permissions" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="Availability" EmptyMessage="Availability" />
                    </div>

                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="EventScheduledSince" runat="server" EmptyMessage="Scheduled Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="EventScheduledBefore" runat="server" EmptyMessage="Scheduled Before" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="Instructor" EmptyMessage="Instructor" />
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
        <div class="mb-3">
            <insite:ComboBox ID="SortColumns" runat="server">
                <Items>
                    <insite:ComboBoxOption Text="Sort by Class Scheduled Date" Value="EventScheduledStart desc,EventTitle" />
                    <insite:ComboBoxOption Text="Sort by Class Title" Value="EventTitle,EventScheduledStart desc" />
                    <insite:ComboBoxOption Text="Sort by Venue" Value="VenueLocation.GroupName,EventScheduledStart desc,EventTitle" />
                </Items>
            </insite:ComboBox>
        </div>
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>