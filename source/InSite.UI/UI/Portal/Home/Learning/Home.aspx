<%@ Page Language="C#" CodeBehind="Home.aspx.cs" Inherits="InSite.UI.Portal.Home.Learning.Home" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="~/UI/Admin/Foundations/Controls/AnnouncementToast.ascx" TagName="AnnouncementToast" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Foundations/Controls/MaintenanceToast.ascx" TagName="MaintenanceToast" TagPrefix="uc" %>

<%@ Register Src="./Controls/DashboardNavigation.ascx" TagName="DashboardNavigation" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style>
        .product-info {
            flex: 4 0 0;
        }
        .product-info img {
            margin-bottom: 0.25rem;
        }
        .attempt-info {
            flex: 5 0 0;
        }

        .alert-skillscheck .btn-close {
          background: transparent url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 16 16'%3e%3cpath d='M8 1.2A6.74 6.74 0 0 0 1.2 8 6.74 6.74 0 0 0 8 14.8 6.74 6.74 0 0 0 14.8 8 6.74 6.74 0 0 0 8 1.2zM0 8c0-4.4 3.6-8 8-8s8 3.6 8 8-3.6 8-8 8-8-3.6-8-8zm10.6-2.6a.61.61 0 0 1 0 .8L8.8 8l1.9 1.9a.61.61 0 0 1 0 .8.61.61 0 0 1-.8 0L8 8.8l-1.9 1.9a.61.61 0 0 1-.8 0 .61.61 0 0 1 0-.8L7.2 8 5.4 6.1a.61.61 0 0 1 0-.8.61.61 0 0 1 .8 0l1.9 1.9L10 5.3c.1-.1.4-.1.6.1z' fill-rule='evenodd' fill='%23ffffff'/%3e%3c/svg%3e") center/1.375em auto no-repeat;
        }
    </style>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">

    <uc:DashboardNavigation runat="server" ID="DashboardNavigation" />

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:UserLicenseCheck runat="server" />
    <insite:UserPasswordCheck runat="server" />
    <uc:AnnouncementToast runat="server" ID="AnnouncementToast" />
    <uc:MaintenanceToast runat="server" ID="MaintenanceToast" ShowOnEachRequest="true" />

    <div runat="server" id="NoCoursesPanel" class="alert alert-info" role="alert">
        <div class="d-flex align-items-center">
            Your SkillsCheck is on its way, please keep an eye out for an email confirming once it has been assigned.
        </div>
    </div>

    <div runat="server" id="DashboardPanel" visible="false">

        <div runat="server" id="BannerPanel" class="alert alert-skillscheck d-flex bg-primary text-white" role="alert">
            <div class="d-flex align-items-center">
                <div class="fs-1">
                    <i class="fas fa-party-horn"></i>
                </div>
                <div class="px-3 fs-1 text-nowrap">
                    New SkillsCheck assigned!
                </div>
                <div>
                    You’ve been invited to complete a SkillsCheck. Get started by clicking the Start button in your dashboard below. 
                </div>
            </div>
            <button
                runat="server"
                id="CloseButton"
                type="button"
                class="btn-close"
                data-bs-dismiss="alert"
                aria-label="Close"
            >
            </button>
        </div>

        <h1>Hi <asp:Literal runat="server" ID="UserFirstName" />!</h1>

        <div class="row mb-4">
            <div class="col-5">
                <h6 class="text-info">Welcome to SkillsCheck – Helping you align your skills with in-demand training and employment opportunities</h6>
                <h6>Here you'll find your assigned SkillsChecks, or you can browse our catalog to explore other opportunities</h6>
            </div>
            <div class="col-7 ps-0">
                <div runat="server" id="AttemptCard" class="card">
                    <div class="card-body p-3">
                        <div class="d-flex gap-2">
                            <div class="product-info">
                                <img runat="server" id="ProductImage" src="/" alt="" >
                                <asp:Literal runat="server" ID="ProductName" />
                            </div>
                            <div class="attempt-info">
                                <div runat="server" id="AttemptCardHeader" class="mb-2"></div>
                                <div runat="server" id="AttemptCardBody" class="mb-2">
                        
                                </div>
                                <div class="mt-3 text-end">
                                    <insite:Button runat="server" ID="AttemptCardStartButton" Text="Start" ButtonStyle="Success" />
                                    <insite:Button runat="server" ID="AttemptCardContinueButton" Text="Continue" ButtonStyle="Success" />
                                    <insite:Button runat="server" ID="AttemptCardAddToCartButton" Text="Add to Cart" ButtonStyle="Success"
                                        OnClientClick="alert('Not Implemented'); return false;" />
                                    <insite:Button runat="server" ID="AttemptCardViewCatalogButton" Text="View Catalog" ButtonStyle="Success" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <asp:Repeater runat="server" ID="AssessmentRepeater">
            <HeaderTemplate>
                <table class="table">
                    <thead>
                        <tr>
                            <th>SkillsCheck</th>
                            <th>Date Assigned</th>
                            <th>Date Completed</th>
                            <th>Action</th>
                            <th>Score</th>
                            <th>Report</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <FooterTemplate>
                    </tbody>
                </table>
            </FooterTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("ProductName") %></td>
                    <td><%# LocalizeDate(Eval("DistributionAssigned")) %></td>
                    <td><%# LocalizeDate(Eval("AttemptSubmitted")) %></td>
                    <td>
                        <insite:Button runat="server" Text="Start" Size="ExtraSmall" ButtonStyle="Default"
                            NavigateUrl='<%# GetGridAttemptStartUrl() %>'
                            Visible='<%# Eval("AttemptStarted") == null && Eval("AttemptImported") == null %>' />
                        <insite:Button runat="server" Text="Continue" Size="ExtraSmall" ButtonStyle="Default"
                            NavigateUrl='<%# GetGridAttemptStartUrl() %>'
                            Visible='<%# Eval("AttemptStarted") != null && Eval("AttemptSubmitted") == null %>' />
                        <insite:Button runat="server" Text="Share" Size="ExtraSmall" ButtonStyle="Default"
                            OnClientClick="alert('Not Implemented'); return false;"
                            Style="display: none"
                            Visible='<%# Eval("AttemptSubmitted") != null %>' />
                    </td>
                    <td><%# GetGridScoreHtml() %></td>
                    <td>
                        <insite:Container runat="server" Visible='<%# Eval("AttemptGraded") != null %>'>
                            <insite:IconButton runat="server"
                                Name="file-lines"
                                CommandName="Download"
                                CommandArgument='<%# Eval("AttemptIdentifier") + ";" + Eval("ManagerUserIdentifier") %>'
                            />
                        </insite:Container>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>

    </div>

    <insite:PageFooterContent runat="server">
        <script>
            (() => {
                document.getElementById("<%= CloseButton.ClientID %>").addEventListener("click", () => {
                    fetch(window.location.href, {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/x-www-form-urlencoded"
                        },
                        body: "isHideBannerRequest=true"
                    });
                });
            })();
        </script>
    </insite:PageFooterContent>

</asp:Content>