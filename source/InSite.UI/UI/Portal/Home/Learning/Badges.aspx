<%@ Page Language="C#" CodeBehind="Badges.aspx.cs" Inherits="InSite.UI.Portal.Home.Learning.Badges" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="./Controls/DashboardNavigation.ascx" TagName="DashboardNavigation" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style>
        .digital-badge-list {
            display: flex;
            flex-direction: column;
            gap: 1rem;
        }
        .digital-badge-list div.card-body {
            display: grid;
            grid-template-columns: 200px 1fr;
            gap: 2rem;
        }
        .digital-badge-list div.card-body > div:first-child {
            display: flex;
            align-items: center;
            justify-content: center;
        }
    </style>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">

    <uc:DashboardNavigation runat="server" ID="DashboardNavigation" />

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="NoBadges" Indicator="Warning">
        No badges
    </insite:Alert>

    <asp:Repeater runat="server" ID="BadgeRepeater">
        <HeaderTemplate>
            <div class="digital-badge-list">
        </HeaderTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
        <ItemTemplate>
            <div class="card">
                <div class="card-body">
                    <div>
                        <img src='<%# Eval("BadgeImageUrl") %>' alt='<%# Eval("AchievementTitle") %>' />
                    </div>

                    <div>
                        <h3 class="card-title">
                            <%# Eval("AchievementTitle") %>
                        </h3>
                        <div class="card-text mb-3">
                            <div class="mb-1">
                                <span class='text-success'><i class='fas fa-badge-check'></i></span>
                                <%# GetDisplayText("Valid") %>
                            </div>
                            <div class="mb-1"><%# GetGrantedHtml() %></div>
                            <div class="mb-1"><%# GetExpiryHtml() %></div>
                        </div>

                        <insite:Button runat="server"
                            CommandName="DownloadBadge"
                            CommandArgument='<%# Eval("CredentialIdentifier") + ";" + Eval("BadgeImageUrl") %>'
                            ButtonStyle="Success"
                            Text="<i class='far fa-award me-2'></i> Download Badge"
                        />

                        <%# GetShareHtml() %>
                    </div>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</asp:Content>