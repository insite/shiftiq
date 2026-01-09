<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DashboardPrototype.ascx.cs" Inherits="InSite.UI.Portal.Controls.DashboardPrototype" %>

<%@ Register Src="~/UI/Portal/Controls/WelcomeUser.ascx" TagName="WelcomeUser" TagPrefix="uc" %>

<insite:PageHeadContent runat="server">
    <style>
        .bar-grid {
            display: grid;
            align-items: center;
            grid-template-columns: 40px 1fr;
            position: relative;
            row-gap: 1rem;
            font-size: smaller;
            margin-left: 15px;
        }

        .bar-limit {
            position: absolute;
            top: 0;
            width: 3px;
            height: 200px;
            background-color: #fff;
        }
        .bar-limit.bar-limit1 {
            left: 171px;
        }
        .bar-limit.bar-limit2 {
            left: 302px;
        }

        .bar-label-container {
            width: 393px;
            display: flex;
            z-index: 10;
        }
        .bar-label-container > div {
            flex-grow: 1;
            flex-shrink: 1;
            flex-basis: 0;
        }
        .bar-label-container > div > div {
            width: 120px;
            text-align: center;
            font-weight: bold;
            font-size: smaller;
        }
        .bar-label-container > div > div.bar-label1 {
            margin-left: calc(100% - 60px);
        }
        .bar-label-container > div > div.bar-label2 {
            margin-left: calc(100% - 60px);
        }
        .bar-label-container > div > div.bar-label3 {
            margin-left: calc(100% - 60px);
        }

        .bar-container {
            width: 393px;
            height: 20px;
            background-color: #ECF0F4;
            display: flex;
            gap: 0.25rem;
            align-items: center;
            border-radius: 6px;
        }
        .bar-container > div {
            display: flex;
            justify-content: end;
            height: 20px;
            border-top-left-radius: 6px;
            border-bottom-left-radius: 6px;
        }
        .bar-container > span {
            z-index: 10;
        }
        .bar-container > div > span {
            color: #fff;
            margin-right: 0.25rem;
        }
    </style>
</insite:PageHeadContent>

<uc:WelcomeUser runat="server" ID="WelcomeUser" />

<h3 class="mt-4">What's happening this week</h3>

<div class="alert alert-info d-flex">
    <div>
        You started the Pain Management and Opioids course on September 13, 2025.
        Please continue your course before it expires to avoid losing access.
    </div>
    <div class="d-flex align-items-start justify-content-end ms-5">
        <button type="button" class="btn btn-sm btn-primary">
            Resume
            <i class="far fa-play ms-2"></i>
        </button>
    </div>
</div>

<div class="alert alert-info d-flex mt-3">
    <div>
        You were enrolled in the Sickle Cell Disease course.
        Please click the Start button to begin your course.
    </div>
    <div class="d-flex align-items-start justify-content-end ms-5">
        <button type="button" class="btn btn-sm btn-primary">
            Start
            <i class="far fa-arrow-right ms-2"></i>
        </button>
    </div>
</div>

<div class="d-flex gap-3 mb-3">
    <div class="card">
        <div class="card-body">
            <h3 class="text-center">
                My Completed Tasks
            </h3>

            <div class="my-4 text-center">
                Year-at-a-Glance
            </div>

            <div class="d-flex gap-4">
                <div class="d-flex flex-column align-items-center">
                    <div class="mb-2" style="width:120px; height:120px;">
                        <div class="circular-progress skills-progress" style="--ar-progress-value:67; --ar-progress-thickness: 0.75rem;">
                            <span>10/15</span>
                        </div>
                    </div>
                    Assessments
                    <button type="button" class="btn btn-sm btn-primary mt-2">
                        <i class="far fa-magnifying-glass me-2"></i>
                        View
                    </button>
                </div>
                <div class="d-flex flex-column align-items-center">
                    <div class="mb-2" style="width:120px; height:120px;">
                        <div class="circular-progress skills-progress" style="--ar-progress-value:30; --ar-progress-thickness: 0.75rem;">
                            <span>3/10</span>
                        </div>
                    </div>
                    Courses
                    <button type="button" class="btn btn-sm btn-primary mt-2">
                        <i class="far fa-magnifying-glass me-2"></i>
                        View
                    </button>
                </div>
            </div>
        </div>
    </div>
    <div class="card" style="flex-grow: 1;">
        <div class="card-body">
            <h3 class="text-center">
                My CE Credits
            </h3>

            <div class="bar-grid">
                <div class="bar-limit bar-limit1"></div>
                <div class="bar-limit bar-limit2"></div>

                <div>2025</div>
                <div class="bar-container">
                    <div class="bg-warning" style="width: 40%;"></div>
                    <span>12 completed</span>
                </div>

                <div>2024</div>
                <div class="bar-container">
                    <div class="bg-danger" style="width: 30%;"></div>
                    <span>9 completed</span>
                </div>

                <div>2023</div>
                <div class="bar-container">
                    <div class="bg-success" style="width: 70%;"></div>
                    <span>21 completed</span>
                </div>

                <div>2022</div>
                <div class="bar-container">
                    <div class="bg-success" style="width: 99%;">
                        <span>30 completed</span>
                    </div>
                </div>

                <div>2021</div>
                <div class="bar-container">
                    <div class="bg-success" style="width: 99%;">
                        <span>30 completed</span>
                    </div>
                </div>

                <div></div>
                <div class="bar-label-container">
                    <div>
                        <div class="bar-label1">
                            1 CEU/10 CEC
                        </div>
                    </div>
                    <div>
                        <div class="bar-label2">
                            2 CEU/20 CEC
                        </div>
                    </div>                    
                    <div>
                        <div class="bar-label3">
                            3 CEU/30 CEC
                        </div>
                    </div>                    
                </div>
            </div>
        </div>
    </div>
</div>