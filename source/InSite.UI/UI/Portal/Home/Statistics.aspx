<%@ Page Language="C#" CodeBehind="Statistics.aspx.cs" Inherits="InSite.UI.Portal.Home.Statistics" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="./Controls/DashboardNavigation.ascx" TagName="DashboardNavigation" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">

    <uc:DashboardNavigation runat="server" ID="DashboardNavigation" />

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

<div class="row">
    <div class="col-lg-12">

        <div runat="server" id="ShowWhatPanel" class="d-flex align-items-center mb-3">
            <label class="form-label text-nowrap pe-1 me-2 mb-0">Show</label>
            <asp:DropDownList runat="server" ID="ShowWhat" CssClass="form-select form-select-sm">
                <asp:ListItem Selected="True" Text="Primary profile only" Value="Primary" />
                <asp:ListItem Text="All profiles requiring compliance" Value="Mandatory" />
            </asp:DropDownList>
        </div>

        <!-- Stats -->
        <div class="row mx-n2 py-2">
            <div class="col-md-4 col-sm-6 px-2 mb-3">
                <div class="bg-secondary h-100 rounded-3 p-4 text-center">
                    <h3 class="fs-sm fw-medium text-body"><a href="/ui/portal/home/learning-plan">Learning Plan Progress</a></h3>
                    <p runat="server" id="LearningPlanProgressPercent" class="h2 mb-2"></p>
                    <p runat="server" id="LearningPlanProgressPoints" class="fs-ms text-success mb-0"></p>
                </div>
            </div>
            <div class="col-md-4 col-sm-6 px-2 mb-3">
                <div class="bg-secondary h-100 rounded-3 p-4 text-center">
                    <h3 class="fs-sm fw-medium text-body"><a href="/ui/portal/home/achievements">Achievements</a></h3>
                    <p runat="server" id="AchievementsValid" class="h2 mb-2"></p>
                    <p runat="server" id="AchievementsExpired" class="fs-ms text-danger mb-0"></p>
                </div>
            </div>
        </div>

    </div>
</div>

</asp:Content>