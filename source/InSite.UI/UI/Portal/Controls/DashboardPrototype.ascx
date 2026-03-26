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
        [dir='rtl'] .bar-container > div {
            border-top-left-radius: unset;
            border-bottom-left-radius: unset;
            border-top-right-radius: 6px;
            border-bottom-right-radius: 6px;
        }
        .bar-container > span {
            z-index: 10;
        }
        .bar-container > div > span {
            color: #fff;
            margin-right: 0.25rem;
        }
        [dir='rtl'] .bar-container > div > span {
            margin-right: unset;
            margin-left: 0.25rem;
        }
    </style>
</insite:PageHeadContent>

<uc:WelcomeUser runat="server" ID="WelcomeUser" />

<h3 class="mt-4">
    <%= Translate("What's happening this week") %>
</h3>

<div class="alert alert-info d-flex justify-content-between">
    <div>
        <%= Translate("You started <b>Change Management: Driving Engagement &amp; Performance During Change</b>. Please continue your course to complete it and earn points.") %>
    </div>
    <div class="d-flex align-items-start justify-content-end ms-5">
        <button type="button" class="btn btn-sm btn-primary">
            <%= Translate("Resume") %>
            <i class="far fa-play ms-2"></i>
        </button>
    </div>
</div>

<div class="alert alert-info d-flex justify-content-between mt-3">
    <div>
        <%= Translate("You were enrolled in <b>Develop and Implement HR Policies</b>. Please click Start to begin your course and start earning points.") %>
    </div>
    <div class="d-flex align-items-start justify-content-end ms-5">
        <button type="button" class="btn btn-sm btn-primary">
            <%= Translate("Start") %>
            <i class="far fa-arrow-right ms-2"></i>
        </button>
    </div>
</div>

<div class="d-flex gap-3 mb-3">
    <div class="card">
        <div class="card-body">
            <h3 class="text-center">
                <%= Translate("My Completed Tasks") %>
            </h3>

            <div class="my-4 text-center">
                <%= Translate("Year-at-a-Glance") %>
            </div>

            <div class="d-flex gap-4">
                <div class="d-flex flex-column align-items-center">
                    <div class="mb-2" style="width:120px; height:120px;">
                        <div class="circular-progress skills-progress" style="--ar-progress-value:67; --ar-progress-thickness: 0.75rem;">
                            <span>10/15</span>
                        </div>
                    </div>
                    <%= Translate("Assessments") %>
                    <button type="button" class="btn btn-sm btn-primary mt-2">
                        <i class="far fa-magnifying-glass me-2"></i>
                        <%= Translate("View") %>
                    </button>
                </div>
                <div class="d-flex flex-column align-items-center">
                    <div class="mb-2" style="width:120px; height:120px;">
                        <div class="circular-progress skills-progress" style="--ar-progress-value:30; --ar-progress-thickness: 0.75rem;">
                            <span>3/10</span>
                        </div>
                    </div>
                    <%= Translate("Courses") %>
                    <button type="button" class="btn btn-sm btn-primary mt-2">
                        <i class="far fa-magnifying-glass me-2"></i>
                        <%= Translate("View") %>
                    </button>
                </div>
            </div>
        </div>
    </div>
    <div class="card" style="flex-grow: 1;">
        <div class="card-body">
            <h3 class="text-center">
                <%= Translate("My Points") %>
            </h3>

            <div class="bar-grid">
                <div class="bar-limit bar-limit1"></div>
                <div class="bar-limit bar-limit2"></div>

                <div><%= Translate("Mar") %></div>
                <div class="bar-container">
                    <div class="bg-warning" style="width: 40%;"></div>
                    <span><%= Translate("12 points") %></span>
                </div>

                <div><%= Translate("Feb") %></div>
                <div class="bar-container">
                    <div class="bg-danger" style="width: 30%;"></div>
                    <span><span><%= Translate("9 points") %></span></span>
                </div>

                <div><%= Translate("Jan") %></div>
                <div class="bar-container">
                    <div class="bg-success" style="width: 70%;"></div>
                    <span><%= Translate("21 points") %></span>
                </div>

                <div><%= Translate("Dec") %></div>
                <div class="bar-container">
                    <div class="bg-success" style="width: 99%;">
                        <span><%= Translate("30 points") %></span>
                    </div>
                </div>

                <div><%= Translate("Nov") %></div>
                <div class="bar-container">
                    <div class="bg-success" style="width: 99%;">
                        <span><%= Translate("30 points") %></span>
                    </div>
                </div>

                <div><%= Translate("Oct") %></div>
                <div class="bar-container">
                    <div class="bg-success" style="width: 99%;">
                        <span><%= Translate("30 points") %></span>
                    </div>
                </div>

                <div></div>
                <div class="bar-label-container">
                    <div>
                        <div class="bar-label1">
                            10 points
                        </div>
                    </div>
                    <div>
                        <div class="bar-label2">
                            20 points
                        </div>
                    </div>                    
                    <div>
                        <div class="bar-label3">
                            30 points
                        </div>
                    </div>                    
                </div>
            </div>
        </div>
    </div>
</div>