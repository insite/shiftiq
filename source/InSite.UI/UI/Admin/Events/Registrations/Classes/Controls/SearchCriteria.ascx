<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Events.Registrations.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-4">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="CandidateName" EmptyMessage="Registrant Name" MaxLength="256" />
                    </div>
    
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="CandidateCode" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox ID="CandidateEmailEnabled" runat="server" EmptyMessage="Email Enabled">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="True" Text="Email Enabled" />
                                <insite:ComboBoxOption Value="False" Text="Email Disabled" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:ItemNameComboBox runat="server" ID="ApprovalStatus" EmptyMessage="Approval">
                            <Settings UseCurrentOrganization="true" CollectionName="Registrations/Approval/Status" />
                        </insite:ItemNameComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:ItemNameComboBox runat="server" ID="AttendanceStatus" EmptyMessage="Attendance">
                            <Settings UseCurrentOrganization="true" CollectionName="Registrations/Attendance/Status" />
                        </insite:ItemNameComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="IncludeInT2202" EmptyMessage="T2202">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="True" Text="Include in T2202" />
                                <insite:ComboBoxOption Value="False" Text="Not Include in T2202" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="RegistrationComment" runat="server" EmptyMessage="Comment" MaxLength="256" />
                    </div>
                    
                    <insite:FilterButton runat="server" ID="SearchButton" />
                    <insite:ClearButton runat="server" ID="ClearButton" />
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="RegistrationRequestedSince" runat="server" EmptyMessage="Registration Date Since" ApplyUserTimezone="true" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="RegistrationRequestedBefore" runat="server" EmptyMessage="Registration Date Before" ApplyUserTimezone="true" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="SeatAvailable" EmptyMessage="Seat Availability">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="True" Text="Available for purchase" />
                                <insite:ComboBoxOption Value="False" Text="Hidden" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="RegistrationRequestedByName" MaxLength="256" EmptyMessage="Registered By" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="RegistrationEmployerName" MaxLength="256" EmptyMessage="Employer at Time of Registration" />
                    </div>

                    <div class="mb-2">
                        <insite:ItemNameComboBox runat="server" ID="RegistrationEmployerStatus" EmptyMessage="Employer Group Status" />
                    </div>

                    <div class="mb-2">
                        <insite:ItemNameComboBox runat="server" ID="RegistrationEmployerRegion" EmptyMessage="Employer Region" />
                    </div>
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="PaymentStatus" EmptyMessage="Payment Status">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="Completed" Text="Paid" />
                                <insite:ComboBoxOption Value="Declined" Text="Declined" />
                                <insite:ComboBoxOption Value="N/A" Text="N/A" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="BillingCode" EmptyMessage="Bill To" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="EventTitle" EmptyMessage="Event Title" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="EventScheduledSince" runat="server" EmptyMessage="Event Date Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="EventScheduledBefore" runat="server" EmptyMessage="Event Date Before" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="VenueName" MaxLength="256" EmptyMessage="Venue" />
                    </div>

                    <div class="mb-2">
                        <insite:FindGroup runat="server" ID="DepartmentId" EmptyMessage="Department" />
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
