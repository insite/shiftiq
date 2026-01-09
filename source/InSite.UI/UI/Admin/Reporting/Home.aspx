<%@ Page Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" CodeBehind="Home.aspx.cs" Inherits="InSite.UI.Admin.Reporting.Home" %>

<%@ Register Src="~/UI/Admin/Reporting/Controls/DashboardUsers.ascx" TagName="OnlineUsersGrid" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    
</asp:Content>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">
  
    <section runat="server" id="FrequentlyUsedReports" class="pb-5 mb-md-2">

        <h2 class="h4 mb-3">Frequently used reports</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row row-cols-1 row-cols-lg-4 g-4">

                    <div class="col">
                        <a class="card card-hover card-tile border-0 shadow" href='/ui/admin/reports/dashboards'>
                            <div class="card-body text-center">
                                <i class='far fa-tachometer-alt fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Dashboards</h3>
                            </div>
                        </a>
                    </div>

                    <div runat="server" id="QueryCounter" class="col">
                        <a runat="server" id="QueryLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="QueryCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-database fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break' runat="server" id="QueriesLabel">Queries</h3>
                            </div>
                        </a>
                    </div>

                    <div runat="server" id="AuthenticationsCounter" class="col">
                        <a runat="server" id="AuthenticationsLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary">
                                <asp:Literal runat="server" ID="AuthenticationsCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-sign-in-alt fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Login History</h3>
                            </div>
                        </a>
                    </div>

                    <div runat="server" id="ImpersonationsCounter" class="col">
                        <a runat="server" id="ImpersonationsLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary">
                                <asp:Literal runat="server" ID="ImpersonationsCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-user-secret fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Impersonations</h3>
                            </div>
                        </a>
                    </div>

                </div>

               <div class="row mt-4">
                    <div class="col-lg-12">
                        <a runat="server" id="DatabaseQueryRow" href="/ui/admin/reports/queries/sql"><i class="far fa-database me-1 ms-2"></i>Run a dynamic SQL query</a>
                        <a href="/ui/admin/reports/build"><i class="far fa-file-alt me-1"></i>Build a custom report</a>
                    </div>
                </div>

            </div>
        </div>
    </section>

    <section runat="server" id="CurrentUserSessions" class="pb-5 mb-md-2">
        <h2 class="h4 mb-3">Current user sessions</h2>
        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <uc:OnlineUsersGrid runat="server" ID="CurrentSessionGrid" />
            </div>
        </div>
    </section>

    <section class="pb-5 mb-md-2">

        <h2 class="h4 mb-3">Management and administration reports</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">               
                
                <div class="row row-cols-1 row-cols-md-3 g-4">

                    <div class="col">
                        <div class="card card-hover card-tile shadow">
                            <div class="card-body text-center">
                                <i class='far fa-ruler-triangle fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Competency Reporting</h3>
                            </div>
                            <ul class="list-group list-group-flush">
                                <li class="list-group-item"><a href="/ui/cmds/admin/reports/compliance-summary"><%= InSite.Cmds.Admin.Reports.Forms.ComplianceSummaryReport.GetReportTitle(false) %></a></li>
                                <li class="list-group-item"><a href="/ui/cmds/admin/reports/competency-status-history">Competency Status History Chart</a></li>
                                <li class="list-group-item"><a href="/ui/cmds/admin/reports/competency-status-per-user">Competency Statuses per Person</a></li>
                                <li class="list-group-item"><a href="/ui/cmds/admin/reports/competencies-per-department">Competency Listing per Department</a></li>
                                <li class="list-group-item"><a href="/ui/cmds/admin/reports/department-profile-details">Department Profile Details</a></li>
                                <li class="list-group-item"><a href="/ui/cmds/admin/reports/department-profile-summary">Department Profile Summary</a></li>
                                <li class="list-group-item"><a href="/ui/cmds/admin/reports/group-competency-summary">Group Competency Summaries</a></li>
                                <li class="list-group-item"><a href="/ui/cmds/admin/reports/user-competency-summary">Worker Competency Summary</a></li>
                                <li class="list-group-item"><a href="/ui/cmds/admin/reports/individual-profile-assignment">Individual Profile Assignment</a></li>
                                <li class="list-group-item"><a href="/ui/cmds/admin/reports/notification-subscribers">Notification Subscribers</a></li>
                            </ul>
                        </div>
                    </div>

                    <div class="col">
                        <div class="card card-hover card-tile shadow">
                            <div class="card-body text-center">
                                <i class='far fa-award fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Learner Activity Reporting</h3>
                            </div>
                            <ul class="list-group list-group-flush">
                                <li class="list-group-item"><a href="/ui/cmds/admin/reports/college-certificates">College Certificates</a></li>
                                <li class="list-group-item"><a href="/ui/cmds/admin/reports/user-training-details">Worker Training Details</a></li>
                                <li class="list-group-item"><a href="/ui/cmds/admin/reports/training-expiry-dates">Training Expiry Dates</a></li>
                                <li class="list-group-item"><a href="/ui/cmds/admin/reports/training-completions">Training Completions</a></li>
                                <li class="list-group-item"><a href="/ui/cmds/admin/reports/training-history-per-user">Training History by Worker</a></li>
                                <li class="list-group-item"><a href="/ui/cmds/admin/reports/training-requirements-per-competency">Training Requirements per Competency</a></li>
                                <li class="list-group-item"><a href="/ui/cmds/admin/reports/training-requirements-per-user">Training Requirements per Worker</a></li>
                                <li class="list-group-item"><a href="/ui/cmds/admin/reports/user-training-summary">Training Summary</a></li>
                            </ul>
                        </div>
                    </div>

                    <div runat="server" id="SystemAdministrationPanel" class="col">
                        <div class="card card-hover card-tile shadow">
                            <div class="card-body text-center">
                                <i class='far fa-cogs fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Administration Reporting</h3>
                            </div>
                            <ul class="list-group list-group-flush">
                                <li class="list-group-item"><a href="/ui/cmds/admin/reports/active-users">All Active Users</a></li>
                                <li class="list-group-item"><a href="/ui/cmds/admin/reports/competency-count-per-category">Competencies per Category</a></li>
                                <li class="list-group-item"><a href="/ui/cmds/admin/reports/expired-credentials">Expired/Expiring Achievements</a></li>
                                <li class="list-group-item"><a href="/ui/admin/contact/persons/anomalies">Missing and Invalid Emails</a></li>
                                <li class="list-group-item"><a href="/ui/cmds/admin/reports/multiorganization-users">Multi-Organization Users</a></li>
                                <li class="list-group-item"><a href="/ui/cmds/admin/reports/organization-summary">Organization Summary</a></li>
                                <li class="list-group-item"><a href="/ui/cmds/admin/reports/snapshots">Snapshots</a></li>
                                <li class="list-group-item"><a href="/ui/cmds/admin/reports/unprioritized-competencies">Unprioritized Competencies</a></li>
                            </ul>
                        </div>
                    </div>

                </div>

            </div>
        </div>

    </section>

</asp:Content>
