<%@ Page Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" CodeBehind="Home.aspx.cs" Inherits="InSite.UI.Admin.Records.Dashboard" %>

<%@ Register Src="./Reports/Controls/RecentGradebookList.ascx" TagName="RecentGradebookList" TagPrefix="uc" %>
<%@ Register Src="./Reports/Controls/RecentCredentialList.ascx" TagName="RecentCredentialList" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Foundations/Controls/HomeCounterRepeater.ascx" TagName="CounterRepeater" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:PageHeadContent runat="server" ID="HideGradebookTabs">
        <style>
            .gradebook-tabs {
                display: none;
            }
        </style>
    </insite:PageHeadContent>

    <insite:PageHeadContent runat="server" ID="HideLogbookTabs">
        <style>
            .logbook-tabs {
                display: none;
            }
        </style>
    </insite:PageHeadContent>

    <section runat="server" id="AchievementsSection" class="pb-5 mb-md-2">

        <h2 class="h4 mb-3">Achievements</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row row-cols-1 row-cols-lg-4 g-4">
                    <uc:CounterRepeater runat="server" ID="AchievementsCounterRepeater" />
                </div>

            </div>
        </div>

    </section>

    <section runat="server" id="GradebooksSection" class="pb-5 mb-md-2">

        <h2 class="h4 mb-3">Gradebooks</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <insite:Nav runat="server" ID="GradebookTabs" CssClass="gradebook-tabs">

                    <insite:NavItem runat="server" ID="GradebookPanelForAdministrators" Title="Adminstrators" Icon="fas fa-cog">
                        <div class="row row-cols-1 row-cols-lg-4 g-4">
                            <uc:CounterRepeater runat="server" ID="GradebookForAdministratorsCounterRepeater" />
                        </div>
                    </insite:NavItem>

                    <insite:NavItem runat="server" ID="GradebookPanelForInstructors" Title="Instructors" Icon="fas fa-graduation-cap">
                        <div class="row row-cols-1 row-cols-lg-4 g-4">
                            <uc:CounterRepeater runat="server" ID="GradebookForInstructorsCounterRepeater" />
                        </div>
                    </insite:NavItem>

                </insite:Nav>

                <div class="row mt-4">
                    <div class="col-lg-12">
                        <a class="me-3" href="#recent-changes"><i class="fas fa-history me-1"></i>Recent Changes</a>
                        <a class="me-3" href="/ui/admin/records/reports/learner-activity" runat="server" id="A4"><i class="fas fa-file-chart-line me-1"></i>Learner Activity Report</a>
                        <a class="me-3" href="/ui/admin/records/reports/engagement-prompt" runat="server" id="A3"><i class="fas fa-paper-plane me-1"></i>Engagement Prompts</a>
                        <a class="me-3" href="/ui/admin/records/reports/learning-mastery" runat="server" id="LearningMastery"><i class="fas fa-ruler-triangle me-1"></i>Learning Mastery</a>
                        <a class="me-3" href="/ui/admin/records/reports/academic-year" runat="server" id="AcademicYear"><i class="fas fa-ruler-triangle me-1"></i>Academic Year</a>
                    </div>
                </div>

            </div>
        </div>

    </section>

    <section runat="server" id="LogbooksSection" class="pb-5 mb-md-2">

        <h2 class="h4 mb-3">Logbooks</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body logbooks">

                <insite:Nav runat="server" ID="LogbookTabs" CssClass="logbook-tabs">

                    <insite:NavItem runat="server" ID="LogbookPanelForAdministrators" Title="Adminstrators" Icon="fas fa-cog">

                        <div class="row row-cols-1 row-cols-md-5 g-4">
                            <uc:CounterRepeater runat="server" ID="LogbookForAdministratorsCounterRepeater" />
                        </div>

                    </insite:NavItem>

                    <insite:NavItem runat="server" ID="LogbookPanelForValidators" Title="Validators" Icon="fas fa-graduation-cap">

                        <div class="row row-cols-1 row-cols-md-5 g-4">
                            <uc:CounterRepeater runat="server" ID="LogbookForValidatorsCounterRepeater" />
                        </div>

                    </insite:NavItem>

                </insite:Nav>

            </div>
        </div>

    </section>

    <section runat="server" id="ProgramsSection" class="pb-5 mb-md-2">

        <h2 class="h4 mb-3">Programs and Periods</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row row-cols-1 row-cols-lg-4 g-4">
                    <uc:CounterRepeater runat="server" ID="ProgramsCounterRepeater" />
                </div>

                <div class="row mt-4">
                    <div class="col-lg-12">
                        <a href="/ui/admin/learning/programs/enrollments/assign"><i class="fas fa-layer-plus me-1"></i>Assign Programs to Learners</a>
                    </div>
                </div>

            </div>
        </div>

    </section>

    <section runat="server" id="RubricsSection" class="pb-5 mb-md-2">

        <h2 class="h4 mb-3">Rubrics</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row row-cols-1 row-cols-lg-4 g-4">
                    <uc:CounterRepeater runat="server" ID="RubricsCounterRepeater" />
                </div>

            </div>
        </div>

    </section>

    <section class="pb-5 mb-md-2" runat="server" id="HistoryPanel">
        <a name="recent-changes" style="scroll-margin-top: 140px;"></a>
        <h2 class="h4 mb-3">Recent Changes</h2>
        <div class="card border-0 shadow-lg">
            <div class="card-header">
                <ul class="nav nav-tabs card-header-tabs" role="tablist">
                    <li class="nav-item"><a class="nav-link active" href="#result1" data-bs-toggle="tab" role="tab" aria-controls="result1" aria-selected="true"><i class="far fa-spell-check me-2"></i>Gradebooks</a></li>
                    <li class="nav-item"><a class="nav-link" href="#result2" data-bs-toggle="tab" role="tab" aria-controls="result2" aria-selected="true"><i class="far fa-award me-2"></i>Achievements</a></li>
                </ul>
            </div>
            <div class="card-body">
                <div class="tab-content">
                    <div class="tab-pane fade show active" id="result1" role="tabpanel">
                        <uc:RecentGradebookList runat="server" ID="RecentGradebooks" />
                    </div>
                    <div class="tab-pane fade show" id="result2" role="tabpanel">
                        <uc:RecentCredentialList runat="server" ID="RecentCredentials" />
                    </div>
                </div>
            </div>
        </div>
    </section>

</asp:Content>
