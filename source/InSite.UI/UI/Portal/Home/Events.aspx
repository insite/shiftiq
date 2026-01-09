<%@ Page Language="C#" CodeBehind="Events.aspx.cs" Inherits="InSite.UI.Portal.Home.Events" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="./Controls/DashboardNavigation.ascx" TagName="DashboardNavigation" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">

    <uc:DashboardNavigation runat="server" ID="DashboardNavigation" />

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

<insite:Alert runat="server" ID="StatusAlert" />

<!-- Content-->
<div class="row mb-4">
    <div class="col-lg-12">
        <insite:EventTypeMultiComboBox runat="server" ID="EventTypeMultiComboBox" Multiple-ActionsBox="true" EmptyMessage="Event Type" />
    </div>
</div>

<asp:Panel runat="server" ID="MyAppointments" Visible="false">

    <!-- Title -->
    <div class="d-sm-flex align-items-center justify-content-between pb-4 text-center text-sm-start">
        <h2 class="h3 mb-2 text-nowrap">My Appointment Registrations</h2>
        <div runat="server" id="AppointmentCount" class="mb-2 fs-sm text-body-secondary"></div>
    </div>

    <insite:Alert runat="server" ID="MyAppointmentsStatus" />

    <!-- Content-->
    <div class="row">
        <div class="col-lg-12">
        
            <asp:Repeater runat="server" ID="AppointmentRepeater">
                <ItemTemplate>
                
                    <div class="card mb-4">
                      <div class="card-body">
                        <h3 class="card-title">
                            <%# Eval("AchievementTitle") %>
                            <span class="fs-sm text-body-secondary">
                                <%# Eval("EventTitle") %>
                            </span>
                        </h3>
                        <div class="card-text mb-3">
                            <div class="mb-1"><%# Eval("Summary") %></div>
                            <div class="mb-1">
                                <b><%# Eval("EventScheduledText") %></b>
                            </div>
                        </div>
                        <div class="float-end">
                            <span runat="server" visible='<%# Eval("IsScheduled") %>' class="badge bg-success"><insite:Literal runat="server" Text="Scheduled" /></span>
                            <span runat="server" visible='<%# Eval("IsRegistered") %>' class="badge bg-success"><insite:Literal runat="server" Text="Registered" /></span>
                            <span runat="server" visible='<%# Eval("IsWaitlisted") %>' class="badge bg-info"><insite:Literal runat="server" Text="In Waiting List" /></span>
                            <span runat="server" visible='<%# Eval("IsInvited") %>' class="badge bg-info"><insite:Literal runat="server" Text="Invitation Sent" /></span>
                            <span runat="server" visible='<%# Eval("IsMoved") %>' class="badge bg-danger"><insite:Literal runat="server" Text="Moved" /></span>
                            <span runat="server" visible='<%# Eval("IsCancelled") %>' class="badge bg-danger"><insite:Literal runat="server" Text="Cancelled" /></span>
                        </div>
                        <a href="#" class="disabled btn btn-sm btn-secondary"><i class="fas fa-search me-1"></i>View</a>
                      </div>
                    </div>
                
                </ItemTemplate>
            </asp:Repeater>

        </div>
    </div>

</asp:Panel>

<asp:Panel runat="server" ID="MyClasses" Visible="false">

    <!-- Title -->
    <div class="d-sm-flex align-items-center justify-content-between pb-4 text-center text-sm-start">
        <h2 class="h3 mb-2 text-nowrap">My Class Registrations</h2>
        <div runat="server" id="ClassCount" class="mb-2 fs-sm text-body-secondary"></div>
    </div>

    <insite:Alert runat="server" ID="MyClassesStatus" />

    <!-- Content-->
    <div class="row">
        <div class="col-lg-12">
        
            <asp:Repeater runat="server" ID="ClassRepeater">
                <ItemTemplate>
                
                    <div class="card mb-4">
                      <div class="card-body">
                        <h3 class="card-title">
                            <%# Eval("AchievementTitle") %>
                            <span class="fs-sm text-body-secondary">
                                <%# Eval("EventTitle") %>
                            </span>
                        </h3>
                        <div class="card-text mb-3">
                            <div class="mb-1"><%# Eval("Summary") %></div>
                            <div class="mb-1">
                                <b><%# Eval("EventScheduledText") %></b>
                            </div>
                        </div>
                        <div class="float-end">
                            <span runat="server" visible='<%# Eval("IsRegistered") %>' class="badge bg-success"><insite:Literal runat="server" Text="Registered" /></span>
                            <span runat="server" visible='<%# Eval("IsWaitlisted") %>' class="badge bg-info"><insite:Literal runat="server" Text="In Waiting List" /></span>
                            <span runat="server" visible='<%# Eval("IsInvited") %>' class="badge bg-info"><insite:Literal runat="server" Text="Invitation Sent" /></span>
                            <span runat="server" visible='<%# Eval("IsMoved") %>' class="badge bg-danger"><insite:Literal runat="server" Text="Moved" /></span>
                            <span runat="server" visible='<%# Eval("IsCancelled") %>' class="badge bg-danger"><insite:Literal runat="server" Text="Cancelled" /></span>
                        </div>
                        <insite:Button runat="server" Icon="far fa-search" Text="View" ButtonStyle="Info"
                            NavigateUrl='<%# Eval("EventIdentifier", "/ui/portal/events/classes/outline?event={0}") %>'
                        />
                      </div>
                    </div>
                
                </ItemTemplate>
            </asp:Repeater>

        </div>
    </div>

</asp:Panel>

<asp:Panel runat="server" ID="MyExams" Visible="false">

    <!-- Title -->
    <div class="d-sm-flex align-items-center justify-content-between pb-4 text-center text-sm-start">
        <h2 class="h3 mb-2 text-nowrap">My Exam Registrations</h2>
        <div runat="server" id="ExamCount" class="mb-2 fs-sm text-body-secondary"></div>
    </div>

    <insite:Alert runat="server" ID="MyExamsStatus" />

    <!-- Content-->
    <div class="row">
        <div class="col-lg-12">
        
            <asp:Repeater runat="server" ID="ExamRepeater">
                <ItemTemplate>
                
                    <div class="card mb-4">
                      <div class="card-body">
                        <h3 class="card-title">
                            <%# Eval("EventTitle") %>
                        </h3>
                        <div class="card-text mb-3">
                            <div class="mb-1"><%= GetDisplayText("Scheduled Start Date and Time") %>: <%# Eval("EventScheduledText") %></div>
                            <div class="mb-1"><%= GetDisplayText("Exam Type") %>: <%# Eval("ExamType") %></div>
                            <div class="mb-1"><%= GetDisplayText("Exam Format") %>: <%# Eval("ExamFormat") %></div>
                            <div class="mb-1"><%= GetDisplayText("Event Number") %>: <%# Eval("EventNumber") %></div>
                        </div>
                      </div>
                    </div>
                
                </ItemTemplate>
            </asp:Repeater>

        </div>
    </div>
</asp:Panel>

</asp:Content>