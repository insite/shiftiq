<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminDashboardPrototype.ascx.cs" Inherits="InSite.UI.AdminDashboardPrototype" %>

<%@ Register Src="~/UI/Portal/Controls/WelcomeUser.ascx" TagName="WelcomeUser" TagPrefix="uc" %>

<insite:PageHeadContent runat="server">
    <style>
        :root {
            --ar-skillscheck-blue: #68add1;
        }

        .skills-progress {
            --ar-progress-thickness: 0.75rem;
            --ar-progress-bar-bg-primary: var(--ar-skillscheck-blue);
        }

        .notification-grid > div {
            display: grid;
            align-items: center;
            grid-template-columns: 60px 1fr 40px;
            padding-bottom: 1rem;
            margin-bottom: 1rem;
            border-bottom: 1px solid var(--ar-border-color);
        }
        .notification-grid > div:last-child {
            margin-bottom: 0;
        }
        .notification-grid > div > :first-child {
            display: flex;
            align-items: center;
            justify-content: center;
            height: 45px;
            width: 45px;
            font-size: x-large;
            color: #fff;
            border-radius: 8px;
        }
        .notification-grid > div > :nth-child(2) {
            display: flex;
            align-items: center;
            justify-content: space-between;
        }

        .dashboard-card > div {
            flex-grow: 1;
            flex-shrink: 1;
            flex-basis: 0;
        }
        .dashboard-card > div > div {
            padding: 1rem;
        }
        .dashboard-card .chart-status {
            height: 50px;
            text-align: center;
        }
        .dashboard-card .card-description {
            height: 50px;
            margin-bottom: 0.5rem;
        }

        .dashboard-card .action-list {
            align-self: start;
            flex-grow: 1;
            width: 100%;
        }
        .dashboard-card .action-list > div {
            display: grid;
            grid-template-columns: 1fr auto;
            align-items: center;
            margin-bottom: 0.5rem
        }
        .dashboard-card .action-list > div:last-child {
            margin-bottom: 0;
        }
        .dashboard-card .action-list.with-icons > div > span {
            display: grid;
            grid-template-columns: 30px 1fr;
            align-items: center;
        }
        .dashboard-card .action-list.with-icons > div > span i {
            color: var(--ar-skillscheck-blue);
        }

        .text-skillscheck-blue {
            color: var(--ar-skillscheck-blue);
        }
    </style>
</insite:PageHeadContent>
<uc:WelcomeUser runat="server" ID="WelcomeUser" />

<h3 class="mt-4">What's happening this week</h3>

<div class="card border-1 shadow mb-3">
    <div class="card-body notification-grid">
        
        <div>
            <div class="bg-warning">
                <i class="far fa-warning"></i>
            </div>
            <div>
                <div>
                    <a href="#">
                        <i class="far fa-scale-balanced me-1"></i>
                        Flexible Pathways Master Bank
                    </a>
                    <div class="form-text m-0">
                        Fields deleted by Stefan Coetzee-Khan 20 hours ago
                    </div>
                </div>

                <button type="button" class="btn btn-sm btn-primary me-4">
                    <i class="far fa-magnifying-glass me-2"></i>
                    View Assessment
                </button>
            </div>
            <button type="button" class="btn btn-sm btn-outline-primary" title="Delete">
                <i class="far fa-x"></i>
            </button>
        </div>

        <div>
            <div class="bg-warning">
                <i class="far fa-warning"></i>
            </div>
            <div>
                <div>
                    <a href="#">
                        <i class="far fa-chalkboard-teacher me-1"></i>
                        Exam Interface Tutorial
                    </a>
                    <div class="form-text m-0">
                        Course changed by Stefan Coetzee-Khan 2 seconds ago
                    </div>
                </div>

                <button type="button" class="btn btn-sm btn-primary me-4">
                    <i class="far fa-magnifying-glass me-2"></i>
                    View Course
                </button>
            </div>
            <button type="button" class="btn btn-sm btn-outline-primary" title="Delete">
                <i class="far fa-x"></i>
            </button>
        </div>

        <div>
            <div class="bg-info">
                <i class="far fa-hourglass"></i>
            </div>
            <div>
                <div>
                    <a href="#">
                        <i class="far fa-user me-1"></i>
                        Stefan Coetzee-Khan stefan@shiftiq.com
                    </a>
                    <div class="form-text m-0">
                        Person changed by Stefan Coetzee-Khan 19 hours ago
                    </div>
                </div>

                <button type="button" class="btn btn-sm btn-primary me-4">
                    <i class="far fa-magnifying-glass me-2"></i>
                    View Contact
                </button>
            </div>
            <button type="button" class="btn btn-sm btn-outline-primary" title="Delete">
                <i class="far fa-x"></i>
            </button>
        </div>

        <div>
            <div class="bg-success">
                <i class="far fa-grid-2-plus"></i>
            </div>
            <div>
                <div>
                    "Insert App/Toolkit Name" can help you [insert something it will help user do]. Check it out now!
                </div>

                <button type="button" class="btn btn-sm btn-primary me-4">
                    <i class="far fa-magnifying-glass me-2"></i>
                    View App
                </button>
            </div>
            <button type="button" class="btn btn-sm btn-outline-primary" title="Delete">
                <i class="far fa-x"></i>
            </button>
        </div>
    </div>
</div>

<h3>My Dashboard</h3>

<div class="d-flex gap-3 mb-3 dashboard-card">
    <div class="card border-1 shadow">
        <div class="card-body d-flex flex-column align-items-center">
            <h3 class="text-center">
                Build Assessments
            </h3>

            <div class="card-description">
                Assessments available: <span class="text-skillscheck-blue">Unlimited</span>
            </div>

            <div class="d-flex gap-4 mb-3">
                <div class="d-flex flex-column align-items-center">
                    <div class="mb-2" style="width:120px; height:120px;">
                        <div class="circular-progress skills-progress" style="--ar-progress-value:67;">
                            <span>10/15</span>
                        </div>
                    </div>
                    <div class="chart-status">
                        Unpublished
                    </div>
                </div>
                <div class="d-flex flex-column align-items-center">
                    <div class="mb-2" style="width:120px; height:120px;">
                        <div class="circular-progress skills-progress" style="--ar-progress-value:30;">
                            <span>3/10</span>
                        </div>
                    </div>
                    <div class="chart-status">
                        Active
                    </div>
                </div>
            </div>

            <button type="button" class="btn btn-sm btn-primary">
                <i class="far fa-arrow-right me-2"></i>
                Go to Assessments
            </button>
        </div>
    </div>

    <div class="card border-1 shadow">
        <div class="card-body d-flex flex-column align-items-center">
            <h3 class="text-center">
                Manage Learners
            </h3>

            <div class="card-description">
                Contacts in account: <span class="text-skillscheck-blue">186</span><br />
                Contacts available: <span class="text-skillscheck-blue">Unlimited</span>
            </div>

            <div class="d-flex gap-4 mb-3">
                <div class="d-flex flex-column align-items-center">
                    <div class="mb-2" style="width:120px; height:120px;">
                        <div class="circular-progress skills-progress" style="--ar-progress-value:100;">
                            <span>96</span>
                        </div>
                    </div>
                    <div class="chart-status">
                        Enrolled in Courses (<i>active</i>)
                    </div>
                </div>
                <div class="d-flex flex-column align-items-center">
                    <div class="mb-2" style="width:120px; height:120px;">
                        <div class="circular-progress skills-progress" style="--ar-progress-value:100;">
                            <span>45</span>
                        </div>
                    </div>
                    <div class="chart-status">
                        Assigned Assessments
                    </div>
                </div>
            </div>

            <button type="button" class="btn btn-sm btn-primary">
                <i class="far fa-arrow-right me-2"></i>
                Go to Contacts
            </button>
        </div>
    </div>

    <div class="card border-1 shadow">
        <div class="card-body d-flex flex-column align-items-center">
            <h3 class="text-center">
                Schedule Events
            </h3>

            <div class="action-list">

                <div>
                    <span>Send assessment reminder</span>
                    <button type="button" class="btn btn-sm btn-outline-primary" title="Schedule">
                        <i class="far fa-calendar-days"></i>
                    </button>
                </div>

                <div>
                    <span>Send assessment reminder</span>
                    <button type="button" class="btn btn-sm btn-outline-primary" title="Schedule">
                        <i class="far fa-calendar-days"></i>
                    </button>
                </div>

                <div>
                    <span>Send assessment reminder</span>
                    <button type="button" class="btn btn-sm btn-outline-primary" title="Schedule">
                        <i class="far fa-calendar-days"></i>
                    </button>
                </div>

                <div>
                    <span>Send assessment reminder</span>
                    <button type="button" class="btn btn-sm btn-outline-primary" title="Schedule">
                        <i class="far fa-calendar-days"></i>
                    </button>
                </div>

            </div>

            <button type="button" class="btn btn-sm btn-primary">
                <i class="far fa-arrow-right me-2"></i>
                Go to Events
            </button>
        </div>
    </div>

    <div class="card border-1 shadow">
        <div class="card-body d-flex flex-column align-items-center">
            <h3 class="text-center">
                View Reports
            </h3>

            <div class="action-list with-icons">

                <div>
                    <span>
                        <i class="far fa-clipboard-user"></i>
                        Attendance Report
                    </span>
                    <button type="button" class="btn btn-sm btn-outline-primary" title="View">
                        <i class="far fa-magnifying-glass"></i>
                    </button>
                </div>

                <div>
                    <span>
                        <i class="far fa-graduation-cap"></i>
                        Course Completions
                    </span>
                    <button type="button" class="btn btn-sm btn-outline-primary" title="View">
                        <i class="far fa-magnifying-glass"></i>
                    </button>
                </div>

                <div>
                    <span>
                        <i class="far fa-user-graduate"></i>
                        Grade Reports
                    </span>
                    <button type="button" class="btn btn-sm btn-outline-primary" title="View">
                        <i class="far fa-magnifying-glass"></i>
                    </button>
                </div>

                <div>
                    <span>
                        <i class="far fa-star"></i>
                        CE Performance
                    </span>
                    <a class="btn btn-sm btn-outline-primary" title="View" target="_blank" href="/ui/images/ce-performance.jpg">
                        <i class="far fa-magnifying-glass"></i>
                    </a>
                </div>

            </div>

            <button type="button" class="btn btn-sm btn-primary">
                <i class="far fa-arrow-right me-2"></i>
                Go to Reports
            </button>
        </div>
    </div>
</div>

<h3>My Apps</h3>