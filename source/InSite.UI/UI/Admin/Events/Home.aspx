<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="InSite.UI.Admin.Events.Home" %>

<%@ Register Src="./Reports/Controls/RecentClassesList.ascx" TagName="RecentClassesList" TagPrefix="uc" %>
<%@ Register Src="./Reports/Controls/RecentExamsList.ascx" TagName="RecentExamsList" TagPrefix="uc" %>
<%@ Register Src="./Reports/Controls/RecentAppointmentsList.ascx" TagName="RecentAppointmentsList" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <section class="pb-5 mb-md-2" runat="server" id="ClassesPanel">

        <h2 class="h4 mb-3">Classes</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">
                    
                <div class="row row-cols-1 row-cols-lg-4 g-4">
                    <div class="col">
                        <a class="card card-hover card-tile border-0 shadow" href="/ui/admin/events/classes/search">
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="ClassCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-calendar-alt fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Classes</h3>
                            </div>
                        </a>
                    </div>
                    <div class="col">
                        <a class="card card-hover card-tile border-0 shadow" href="/ui/admin/events/registrations/search">
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="RegistrationCount1" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-id-card fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Class Registrations</h3>
                            </div>
                        </a>
                    </div>
                    <div class="col">
                        <a class="card card-hover card-tile border-0 shadow" href="/ui/admin/events/seats/search">
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="SeatCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-money-check-alt fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Seats</h3>
                            </div>
                        </a>
                    </div>
                </div>

                <div class="row mt-4">
                    <div class="col-lg-12">
                        <asp:LinkButton runat="server" class="me-3" ID="DownloadClassesLearningCenterButton"><i class="far fa-download me-1"></i>Download Classes Learning Center</asp:LinkButton>
                    </div>
                </div>

            </div>
        </div>
    
    </section>

    <section class="pb-5 mb-md-2" runat="server" id="ExamsPanel">

        <h2 class="h4 mb-3">Exams</h2>
    
            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <div class="row row-cols-1 row-cols-lg-4 g-4">
                        <asp:Repeater runat="server" ID="ExamCounterRepeater">
		                    <ItemTemplate>
                                <div class="col">
			                        <a class="card card-hover card-tile border-0 shadow" href="<%# GetExamSearchUrl((string)Eval("Name")) %>">
				                        <span class="badge badge-floating badge-pill bg-primary"><%# Eval("Value", "{0:n0}") %></span>
				                        <div class="card-body text-center">
					                        <i class='far fa-calendar-alt fa-3x mb-3'></i>
					                        <h3 class='h5 nav-heading mb-2 text-break'><%# Eval("Name") %></h3>
				                        </div>
			                        </a>
		                        </div>
		                    </ItemTemplate>
	                    </asp:Repeater>

                        <div runat="server" id="ExamCounterZero" class="col">
                            <a class="card card-hover card-tile border-0 shadow" href="/ui/admin/events/exams/search">
                                <span class="badge badge-floating badge-pill bg-primary">0</span>
                                <div class="card-body text-center">
                                    <i class='far fa-id-card fa-3x mb-3'></i>
                                    <h3 class='h5 nav-heading mb-2 text-break'>Exams</h3>
                                </div>
                            </a>
                        </div>

                        <div class="col">
                            <a class="card card-hover card-tile border-0 shadow" href="/ui/admin/events/registrations/exams/search">
                                <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="RegistrationCount2" /></span>
                                <div class="card-body text-center">
                                    <i class='far fa-id-card fa-3x mb-3'></i>
                                    <h3 class='h5 nav-heading mb-2 text-break'>Exam Registrations</h3>
                                </div>
                            </a>
                        </div>

                    </div>

                <div class="row mt-4">
                    <div class="col-lg-12">
                        <a class="me-3" href="/ui/admin/events/exams/create-bulk" runat="server" id="NewExamBulkLink"><i class="far fa-calendar-alt me-1"></i>Bulk Schedule the New Exam Events</a>
                        <a class="me-3" href="/ui/admin/events/reports/exam-event-schedule" runat="server" id="ExamEventScheduleRow"><i class="far fa-calendar-alt me-1"></i>View the Exam Event Schedule</a>
                        <asp:LinkButton runat="server" class="me-3"  ID="DownloadExamSessionsScheduled"><i class="far fa-download me-1"></i>Download Exam Sessions Scheduled</asp:LinkButton>
                        <asp:LinkButton runat="server" class="me-3"  ID="DownloadExamsWrittenByType"><i class="far fa-download me-1"></i>Download Exams Written by Type</asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>

    </section>

    <section class="pb-5 mb-md-2" runat="server" id="AppointmentsPanel">
        <h2 class="h4 mb-3">Appointments</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row row-cols-1 row-cols-md-5 g-4">
                    <div class="col">
                        <a class="card card-hover card-tile border-0 shadow" href="/ui/admin/events/appointments/search">
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="AppointmentCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-calendar-check fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Appointments</h3>
                            </div>
                        </a>
                    </div>
                </div>

            </div>
        </div>

    </section>
 
    <section class="pb-5 mb-md-2" runat="server" ID="HistoryPanel">
        <h2 class="h4 mb-3">Recent Changes</h2>
        <div class="card border-0 shadow-lg">
            <div class="card-header">
                <ul class="nav nav-tabs card-header-tabs" role="tablist">
                    <li class="nav-item"><a class="nav-link active" href="#result1" data-bs-toggle="tab" role="tab" aria-controls="result1" aria-selected="true">Classes</a></li>
                    <li class="nav-item"><a class="nav-link" href="#result2" data-bs-toggle="tab" role="tab" aria-controls="result2" aria-selected="true">Exams</a></li>
                    <li class="nav-item"><a class="nav-link" href="#result3" data-bs-toggle="tab" role="tab" aria-controls="result3" aria-selected="true">Appointments</a></li>
                </ul>
            </div>
            <div class="card-body">
                <div class="tab-content">
                    <div class="tab-pane fade show active" id="result1" role="tabpanel">
                        <uc:RecentClassesList runat="server" ID="RecentClasses" />
                    </div>
                    <div class="tab-pane fade show" id="result2" role="tabpanel">
                        <uc:RecentExamsList runat="server" ID="RecentExams" />
                    </div>
                    <div class="tab-pane fade show" id="result3" role="tabpanel">
                        <uc:RecentAppointmentsList runat="server" ID="RecentAppointmentsList" />
                    </div>
                </div>
            </div>
        </div>
    </section>

    <section class="pb-5 mb-md-2" runat="server" ID="SummaryPanel">
        <h2 class="h4 mb-3">Summaries</h2>
        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <div class="row">
                    <div class="col-md-12">
                        <p>Click here for exam event summary reports:</p>
                        <a href="/ui/admin/reports/events" class="btn btn-info btn-sm"><i class="far fa-file-chart-line me-2"></i> Summaries</a>
                    </div>
                </div>
            </div>
        </div>
    </section>

</asp:Content>
