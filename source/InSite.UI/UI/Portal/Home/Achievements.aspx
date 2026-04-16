<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Achievements.aspx.cs" Inherits="InSite.UI.Portal.Home.Achievements" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="./Controls/DashboardNavigation.ascx" TagName="DashboardNavigation" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">

    <uc:DashboardNavigation runat="server" ID="DashboardNavigation" />

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <asp:Repeater runat="server" ID="AchievementRepeater">
        <ItemTemplate>
            <div class="card mb-4">
                <div class="card-body d-flex">

                    <div runat="server" visible='<%# Eval("BadgeImageUrl") != null %>' style="width:200px;" class="d-flex align-items-center justify-content-center me-4">
                        <img src='<%# Eval("BadgeImageUrl") %>' alt='<%# Eval("AchievementTitle") %>' style="max-height:200px;" />
                    </div>

                    <div>
                        <h3 class="card-title">
                            <span class="d-block fs-sm text-body-secondary"><%# Eval("AchievementLabel") %></span>
                            <%# Eval("AchievementTitle") %>
                        </h3>

                        <div class="card-text mb-3">
                            <div class="mb-1"><%# GetStatusHtml() %></div>
                            <div class="mb-1"><%# GetGrantedHtml() %></div>
                            <div class="mb-1"><%# GetExpiryHtml() %></div>
                        </div>

                        <%# GetDownloadHtml() %>

                        <insite:Button runat="server"
                            CommandName="DownloadBadge"
                            ButtonStyle="Success"
                            Visible='<%# IsBadgeConfigured() %>'
                            Text="<i class='far fa-award me-2'></i> Download Badge" />

                        <%# GetShareHtml() %>
                    </div>

                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>

</asp:Content>