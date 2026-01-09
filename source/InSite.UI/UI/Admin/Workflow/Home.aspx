<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="InSite.UI.Admin.Issues.Dashboard" %>

<%@ Register Src="./Cases/Controls/RecentList.ascx" TagName="RecentCaseChangeList" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Workflow/Forms/Controls/RecentList.ascx" TagName="RecentFormChangeList" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <section class="pb-4 mb-md-2">

        <h2 class="h4 mb-3">Cases</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row row-cols-1 row-cols-lg-4 g-4">      
                        
                    <div class="col">
                        <a class="card card-hover card-tile border-0 shadow" href="/ui/admin/workflow/cases/search">
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="IssueCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-briefcase fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Cases</h3>
                            </div>
                        </a>
                    </div>

                </div>

                <div class="row mt-4">
                    <div class="col-lg-12">
                        <a href="/ui/admin/workflow/cases/reports" class="me-3"><i class="fas fa-file-chart-line me-1"></i>Summary Reports</a>
                        <a runat="server" id="CaseStatusesLink" href="/client/admin/workflows/case-statuses/search"><i class="fas fa-briefcase me-1"></i>Case Statuses</a>
                    </div>
                </div>

            </div>                
        </div>

    </section>

    <section class="pb-4 mb-md-2">

        <h2 class="h4 mb-3">Forms</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <div runat="server" id="Div1">

                    <div class="row row-cols-1 row-cols-lg-4 g-4">

                        <div class="col">
                            <a class="card card-hover card-tile border-0 shadow" href="/ui/admin/workflow/forms/search">
                                <span class="badge badge-floating badge-pill bg-primary">
                                    <asp:Literal runat="server" ID="SurveyFormCount" /></span>
                                <div class="card-body text-center">
                                    <i class='far fa-check-square fa-3x mb-3'></i>
                                    <h3 class='h5 nav-heading mb-2 text-break'>Forms</h3>
                                </div>
                            </a>
                        </div>

                        <div class="col">
                            <a class="card card-hover card-tile border-0 shadow" href="/ui/admin/workflow/forms/submissions/search">
                                <span class="badge badge-floating badge-pill bg-primary">
                                    <asp:Literal runat="server" ID="ResponseSessionCount" /></span>
                                <div class="card-body text-center">
                                    <i class='far fa-poll-people fa-3x mb-3'></i>
                                    <h3 class='h5 nav-heading mb-2 text-break'>Submissions</h3>
                                </div>
                            </a>
                        </div>

                    </div>

                </div>
            </div>
        </div>

    </section>

    <section class="pb-5 mb-md-2" runat="server" id="HistoryPanel">
        <h2 class="h4 mb-3">Recent Changes</h2>
        <div class="card border-0 shadow-lg">
            <div class="card-header">
                <ul class="nav nav-tabs card-header-tabs" role="tablist">
                    <li class="nav-item"><a class="nav-link active" href="#cases" data-bs-toggle="tab" role="tab" aria-controls="result1" aria-selected="true">Cases</a></li>
                    <li class="nav-item"><a class="nav-link" href="#forms" data-bs-toggle="tab" role="tab" aria-controls="result2" aria-selected="true">Forms</a></li>
                </ul>
            </div>
            <div class="card-body tab-content">
                <div class="tab-pane fade show active" id="cases" role="tabpanel">
                    <uc:RecentCaseChangeList runat="server" ID="RecentCaseChanges" />
                </div>
                <div class="tab-pane fade" id="forms" role="tabpanel">
                    <uc:RecentFormChangeList runat="server" ID="RecentFormChanges" />
                </div>
            </div>
        </div>
    </section>

</asp:Content>
