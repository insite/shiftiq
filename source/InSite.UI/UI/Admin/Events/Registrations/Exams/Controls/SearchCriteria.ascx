<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Events.Registrations.Exams.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-4">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="CandidateName" EmptyMessage="User Name" MaxLength="256" />
                    </div>
    
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="CandidateCode" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:ItemNameComboBox runat="server" ID="ApprovalStatus" EmptyMessage="Approval">
                            <Settings UseCurrentOrganization="true" CollectionName="Registrations/Approval/Status" />
                        </insite:ItemNameComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:ItemNameComboBox runat="server" ID="AttendanceStatus" EmptyMessage="Attendance" AllowNullSearch="true">
                            <Settings UseCurrentOrganization="true" CollectionName="Registrations/Attendance/Status" />
                        </insite:ItemNameComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>
                <div class="col-4">
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
                        <insite:MultiComboBox ID="VenueLocationGroup" runat="server" EmptyMessage="Venue" DropDown-Size="10" EnableSearch="true" Multiple-ActionsBox="true" />
                    </div>
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="RegistrationRequestedSince" runat="server" EmptyMessage="Registration Date Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="RegistrationRequestedBefore" runat="server" EmptyMessage="Registration Date Before" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="RegistrationComment" runat="server" EmptyMessage="Comment" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="CandidateType" runat="server" EmptyMessage="Candidate Type" MaxLength="256" />
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
