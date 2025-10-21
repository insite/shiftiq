<%@ Page Language="C#" CodeBehind="Assessments.aspx.cs" Inherits="InSite.UI.Portal.Home.Assessments" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="./Controls/DashboardNavigation.ascx" TagName="DashboardNavigation" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">

    <uc:DashboardNavigation runat="server" ID="DashboardNavigation" />

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

<div class="row">
    <div class="col-lg-12">

        <insite:Alert runat="server" ID="StatusAlert" />

        <asp:Repeater runat="server" ID="AttemptRepeater">
            <ItemTemplate>
                <div class="card mb-4">
                    <div class="card-body">
                        <h3 class="card-title" style="width: calc(100% - 75px);">
                            <div runat="server" visible='<%# IsAllowView() %>' class="position-absolute end-0 pe-4">
                                <span runat="server" visible='<%# (bool)Eval("AttemptIsPassing") %>' class='badge bg-success'>Pass</span>
                                <span runat="server" visible='<%# !(bool)Eval("AttemptIsPassing") %>' class='badge bg-danger'>Fail</span>
                                <div runat="server" class="text-body-secondary text-center d-none d-sm-block" visible='<%# IsAllowViewScore() %>'>
                                    <small style="font-size:0.875rem;">
                                        <%# Eval("AttemptScore", "{0:p0}") %>
                                    </small>
                                </div>
                            </div>
                            <%# Eval("Form.FormTitle") %>
                        </h3>
                        <div class="card-text mb-3">
                            <div runat="server" visible='<%# Eval("AttemptStarted") != null %>' class="mb-1"><%= GetDisplayText("Started") %> <%# LocalizeTime(Eval("AttemptStarted")) %></div>
                            <div runat="server" visible='<%# Eval("AttemptGraded") != null %>' class="mb-1"><%= GetDisplayText("Completed") %> <%# LocalizeTime(Eval("AttemptGraded")) %></div>
                            <div class="mb-1"><%= GetDisplayText("Current Status") %>: <%# Eval("AttemptStatus") %></div>
                        </div>
                        <insite:Button runat="server" Text='<%# Translate("Resume") %>' NavigateUrl='<%# GetResumeUrl() %>' Icon="far fa-play" ButtonStyle="Primary" Visible='<%# IsAllowResume() %>' />
                        <insite:Button runat="server" Text='<%# Translate("View") %>' NavigateUrl='<%# GetResultUrl() %>' Icon="far fa-search" ButtonStyle="Primary" Visible='<%# IsAllowView() %>' />
                        <insite:Button runat="server" Text='<%# Translate("Restart") %>' NavigateUrl='<%# GetRestartUrl() %>' Icon="fas fa-redo" ButtonStyle="Primary" Visible='<%# IsAllowRestart() %>' />
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

    </div>
</div>

</asp:Content>