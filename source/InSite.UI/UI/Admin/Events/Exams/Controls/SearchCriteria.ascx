<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Events.Exams.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-4">
                    <div class="mb-2">
                        <insite:ExamTypeComboBox runat="server" ID="ExamType" EmptyMessage="Exam Type" />
                    </div>
    
                    <div class="mb-2">
                        <insite:TextBox ID="EventTitle" runat="server" EmptyMessage="Title" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:NumericBox runat="server" ID="EventNumber" EmptyMessage="Event Number" NumericMode="Integer" DigitGrouping="false" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="EventClassCode" EmptyMessage="Class/Session Code" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:EventFormatComboBox runat="server" ID="EventFormat" EmptyMessage="Format" AllowBlank="true" />
                    </div>

                    
                    <insite:FilterButton runat="server" ID="SearchButton" />
                    <insite:ClearButton runat="server" ID="ClearButton" />
                </div>
                <div class="col-4">
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
                        <insite:DateTimeOffsetSelector ID="EventScheduledSince" runat="server" EmptyMessage="Scheduled &ge;" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="EventScheduledBefore" runat="server" EmptyMessage="Scheduled &lt;" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="VenueOffice" runat="server" EmptyMessage="Invigilating Office" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:MultiComboBox ID="VenueLocationGroup" runat="server" EmptyMessage="Venue" EnableSearch="true" Multiple-ActionsBox="true" />
                    </div>
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:ItemNameComboBox runat="server" ID="EventBillingType" EmptyMessage="Billing Code" />
                    </div>

                    <div class="mb-2">
                        <insite:ItemNameComboBox runat="server" ID="EventRequisitionStatus" EmptyMessage="Request Status" />
                    </div>

                    <div class="mb-2">
                        <insite:ItemNameComboBox runat="server" ID="EventSchedulingStatus" EmptyMessage="Scheduling Status" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="EventMaterialTrackingStatus" EmptyMessage="Material Tracking Status">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="Fully Returned" Text="Fully Returned" />
                                <insite:ComboBoxOption Value="Partially Returned" Text="Partially Returned" />
                                <insite:ComboBoxOption Value="Not Returned" Text="Not Returned" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="ExamFormName" runat="server" EmptyMessage="Exam Form Name" MaxLength="256" />
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