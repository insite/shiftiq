<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.UI.Desktops.Custom.SkilledTradesBC.Distributions.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            
            <h4>Criteria</h4>

            <div class="row">

                <div class="col-6">

                    <div class="mb-2">
                        <insite:ExamTypeComboBox runat="server" ID="ExamType" EmptyMessage="Exam Event Type" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="EventClassCode" EmptyMessage="Class/Session Code" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="EventScheduledSince" runat="server" EmptyMessage="Event Scheduled &ge;" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="EventScheduledBefore" runat="server" EmptyMessage="Event Scheduled &lt;" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="DistributionExpectedSince" runat="server" EmptyMessage="Distribution Expected &ge;" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="DistributionExpectedBefore" runat="server" EmptyMessage="Distribution Expected &lt;" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="UndistributedExamsInclusion" EmptyMessage="Undistributed Exams">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="Only" Text="Undistributed Exams Only" />
                                <insite:ComboBoxOption Value="Exclude" Text="Exclude Undistributed Exams" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                </div>

                <div class="col-6">

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="EventFormat" EmptyMessage ="Format">
                            <Items>
                                <insite:ComboBoxOption Text="Paper" Value="Paper" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="EventTitle" runat="server" EmptyMessage="Title" MaxLength="256" />
                    </div>
                    <div class="mb-2">
                        <insite:TextBox ID="VenueOffice" runat="server" EmptyMessage="Invigilating Office" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="Venue" runat="server" EmptyMessage="Venue" MaxLength="256" />
                    </div>
    
                </div>

                <div class="col-6">

                    <div class="mb-2">
                        <insite:NumericBox runat="server" ID="EventNumber" EmptyMessage="Event Number" NumericMode="Integer" DigitGrouping="false" />
                    </div>

                    <div class="mb-2">
                        <insite:ItemNameComboBox runat="server" ID="EventBillingType" EmptyMessage="Billing Code" />
                    </div>

                    <div class="mb-2">
                        <insite:ItemNameComboBox runat="server" ID="EventSchedulingStatus" EmptyMessage="Scheduling Status" DropDownWidth="250px" />
                    </div>

                    <div class="mb-2">
                        <insite:ItemNameComboBox runat="server" ID="EventPublicationStatus" EmptyMessage="Publication Status" DropDownWidth="250px" />
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
        <div>
            <h4 class="mt-4">Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>