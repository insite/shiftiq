<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" 
    Inherits="InSite.UI.Admin.Assessments.Dashboard"
    MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="Reports/Controls/RecentList.ascx" TagPrefix="uc" TagName="RecentList" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <section class="pb-4 mb-md-2">
      
        <h2 class="h4 mb-3">Banks</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row row-cols-1 row-cols-lg-4 g-4">

                    <div runat="server" id="BankCounter" class="col">
                        <a runat="server" id="BankLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="BankCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-balance-scale fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Banks</h3>
                            </div>
                        </a>
                    </div>

                    <div runat="server" id="SpecCounter" class="col">
                        <a runat="server" id="SpecLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="SpecCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-clipboard-list fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Specifications</h3>
                            </div>
                        </a>
                    </div>

                    <div runat="server" id="FormsCounter" class="col">
                        <a runat="server" id="FormsLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="FormsCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-window fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Forms</h3>
                            </div>
                        </a>
                    </div>

                    <div runat="server" id="QuestionsCounter" class="col">
                        <a runat="server" id="QuestionsLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="QuestionsCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-question fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Questions</h3>
                            </div>
                        </a>
                    </div>

                    <div runat="server" id="BankCommentsCounter" class="col">
                        <a runat="server" id="BankCommentsLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="BankCommentsCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-comments fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Comments</h3>
                            </div>
                        </a>
                    </div>

                </div>

            </div>
        </div>

    </section>

    <section runat="server" id="QuizPanel" class="pb-4 mb-md-2">

        <h2 class="h4 mb-3">Single-Question Quizzes</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row row-cols-1 row-cols-lg-4 g-4">

                    <div runat="server" id="QuizTypingSpeedCounter" class="col">
                        <a runat="server" id="QuizTypingSpeedLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="QuizTypingSpeedCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-square-question fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Typing Speed</h3>
                            </div>
                        </a>
                    </div>

                    <div runat="server" id="QuizTypingAccuracyCounter" class="col">
                        <a runat="server" id="QuizTypingAccuracyLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="QuizTypingAccuracyCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-square-question fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Data Entry</h3>
                            </div>
                        </a>
                    </div>

                    <div runat="server" id="QuizAttemptCounter" class="col">
                        <a runat="server" id="QuizAttemptLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="QuizAttemptCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-square-question fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Attempts</h3>
                            </div>
                        </a>
                    </div>

                </div>

            </div>
        </div>
    </section>

    <section runat="server" id="AttemptSection" class="pb-5 mb-md-2">
        <h2 class="h4 mb-3">Attempts</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <div class="row row-cols-1 row-cols-lg-4 g-4">

                    <div runat="server" id="AttemptsCounter" class="col">
                        <a runat="server" id="AttemptsLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="AttemptsCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-tasks fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Attempts</h3>
                            </div>
                        </a>
                    </div>

                    <div runat="server" id="AttemptCommentsCounter" class="col">
                        <a runat="server" id="AttemptCommentsLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="AttemptCommentsCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-comments fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Comments</h3>
                            </div>
                        </a>
                    </div>

                </div>

                <div class="mt-4">
                    <a class="me-3" href="/ui/admin/assessments/attempts/ad-hoc"><i class="fas fa-chart-bar me-1"></i>Ad-Hoc Attempt Report</a>
                    <%-- <a class="me-3" href="/ui/admin/assessments/attempts/advanced-analytics" runat="server" id="A1"><i class="fas fa-chart-network me-1"></i>Advanced Analytics</a> --%>
                    <%-- <a class="me-3" href="/admin/assessments/reports/attempt-comments" runat="server" id="SearchAttemptsRow"><i class="fas fa-comments me-1"></i>Attempt Comments</a> --%>
                    <a class="me-3" href="/ui/admin/assessments/attempts/upload" runat="server" id="UploadAttemptsRow"><i class="fas fa-upload me-1"></i>Upload Attempts</a>
                    <a class="me-3" href="/ui/admin/assessments/attempts/performance-report" runat="server" id="PerformanceReportLink">
                        <i class="fas fa-balance-scale me-1"></i>
                        Performance Report
                    </a>
                    <a class="me-3" href="/ui/admin/assessments/attempts/taker-report">
                        <i class="fas fa-balance-scale me-1"></i>
                        High Stakes Test Taker Report
                    </a>
                </div>

            </div>
        </div>
    </section>

    <section runat="server" id="PublicationSection" class="pb-5 mb-md-2">
        <h2 class="h4 mb-3">Standalone Assessments</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <div class="row row-cols-1 row-cols-lg-4 g-4">

                    <div runat="server" id="PublicationAssessmentsCounter" class="col">
                        <a runat="server" id="PublicationAssessmentsLink" class="card card-hover card-tile border-0 shadow">
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="PublicationAssessmentsCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-balance-scale fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Assessments</h3>
                            </div>
                        </a>
                    </div>

                </div>

            </div>
        </div>
    </section>

    <section runat="server" id="HistorySection" class="pb-4 mb-md-2">
        <h2 class="h4 mb-3">Recent Changes</h2>
        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <uc:RecentList runat="server" ID="RecentChanges" />
            </div>
        </div>
    </section>

</asp:Content>
