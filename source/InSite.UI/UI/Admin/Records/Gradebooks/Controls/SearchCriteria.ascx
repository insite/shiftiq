<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Records.Gradebooks.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-4">
                    <div class="mb-2">
                        <insite:TextBox ID="GradebookTitle" runat="server" EmptyMessage="Gradebook title" MaxLength="256" />
                    </div>
    
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="GradebookCreatedSince" runat="server" EmptyMessage="Gradebook created since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="GradebookCreatedBefore" runat="server" EmptyMessage="Gradebook created before" />
                    </div>

                    <div class="mb-2">
                        <insite:FindPeriod runat="server" ID="GradebookPeriodIdentiffier" EmptyMessage="Gradebook period" />
                    </div>
                    
                    <insite:FilterButton runat="server" ID="SearchButton" />
                    <insite:ClearButton runat="server" ID="ClearButton" />
                </div>
                <div class="col-4">
                    <div class="mb-2"> 
                        <insite:AchievementComboBox runat="server" ID="AchievementIdentifier" EmptyMessage="Achievement" />
                    </div>

                    <div class="mb-2">
                        <insite:FindStandard runat="server" ID="StandardIdentifier" EmptyMessage="Framework" TextType="Title" IsBoostrap5="true" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="IsLocked" EmptyMessage="Gradebook Status">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="True" Text="Locked" />
                                <insite:ComboBoxOption Value="False" Text="Unlocked" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="CourseName" runat="server" EmptyMessage="Course Name" MaxLength="200" />
                    </div>
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:TextBox ID="EventTitle" runat="server" EmptyMessage="Class Title" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="EventScheduledSince" runat="server" EmptyMessage="Class scheduled since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="EventScheduledBefore" runat="server" EmptyMessage="Class scheduled before" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="Instructor" EmptyMessage="Class instructor" />
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
