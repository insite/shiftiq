<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EventSummary.ascx.cs" Inherits="InSite.Admin.Events.Reports.Controls.EventSummary" %>

<%@ Register TagPrefix="uc" TagName="StatisticsAnalyticsTable" Src="StatisticsAnalyticsTable.ascx"  %>
<%@ Register TagPrefix="uc" TagName="StatisticsRegistrationsPivotTable" Src="StatisticsRegistrationsPivotTable.ascx"  %>
<%@ Register TagPrefix="uc" TagName="StatisticsAccommodationsPivotTable" Src="StatisticsAccommodationsPivotTable.ascx"  %>

<div class="card border-0 shadow-lg mb-3">
    <div class="card-body">
        <h3>Criteria</h3>

        <div class="row">
            <div class="col-lg-3">
                <div class="mb-2">
                    <insite:ComboBox runat="server" ID="DateType">
                        <Items>
                            <insite:ComboBoxOption Value="Event Scheduled" Text="Event Scheduled" />
                            <insite:ComboBoxOption Value="Attempt Completed" Text="Attempt Completed" />
                        </Items>
                    </insite:ComboBox>
                </div>
                <div class="mb-2">
                    <insite:AttendanceStatusComboBox runat="server" ID="AttendanceStatus" EmptyMessage="Attendance Status" />
                </div>
                <div class="mb-2">
                    <insite:EventFormatComboBox runat="server" ID="EventFormat" EmptyMessage="Event Format" />
                </div>
            </div>
            <div class="col-lg-3">
                <div class="mb-2">
                    <insite:ComboBox runat="server" ID="DateRangeSelector" AllowBlank="false" />
                </div>
                <div runat="server" id="DateRangeCustom">
                    <div class="mb-2">
                        <insite:DateSelector runat="server" ID="DateRangeSince" EmptyMessage="&ge;" />
                    </div>
                    <div class="mb-2">
                        <insite:DateSelector runat="server" ID="DateRangeBefore" EmptyMessage="&lt;" />
                    </div>                    
                </div>
            </div>
        </div>

        <insite:FilterButton runat="server" ID="ApplyFilter" Text="Filter" />
    </div>
</div>

<div class="card border-0 shadow-lg">
    <div class="card-body">
        <insite:Nav runat="server" ID="NavigationTabs">

            <insite:NavItem runat="server" ID="TabCounts" Title="<i class='far fa-signal'></i> Counts">
                <uc:StatisticsAnalyticsTable runat="server" ID="AnalyticsTable" />
            </insite:NavItem>

            <insite:NavItem runat="server" ID="TabRegistrations" Title="<i class='far fa-id-card'></i> Registrations">
                <uc:StatisticsRegistrationsPivotTable runat="server" ID="RegistrationsTable" />
            </insite:NavItem>

            <insite:NavItem runat="server" ID="TabAccommodations" Title="<i class='far fa-assistive-listening-systems'></i> Accommodations">
                <uc:StatisticsAccommodationsPivotTable runat="server" ID="AccommodationsTable" />
            </insite:NavItem>

        </insite:Nav>
    </div>
</div>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        .pivottable-insite table.pvtUi .pvtAxisContainer.pvtUnused,
        .pivottable-insite table.pvtUi .pvtAxisContainer.pvtRows {
            width: 150px;
        }
    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            Sys.Application.add_load(function () {
                $('#<%= DateRangeSelector.ClientID %>')
                    .off('change', onDateRangeSelected)
                    .on('change', onDateRangeSelected);

                onDateRangeSelected();
            });

            function onDateRangeSelected() {
                var $combo = $('#<%= DateRangeSelector.ClientID %>');
                var $panel = $('#<%= DateRangeCustom.ClientID %>');

                if ($combo.selectpicker('val') == 'Custom')
                    $panel.removeClass('d-none');
                else
                    $panel.addClass('d-none');
            }
        })();
    </script>
</insite:PageFooterContent>
