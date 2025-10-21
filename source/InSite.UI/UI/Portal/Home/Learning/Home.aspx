<%@ Page Language="C#" CodeBehind="Home.aspx.cs" Inherits="InSite.UI.Portal.Home.Learning.Home" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="~/UI/Admin/Foundations/Controls/AnnouncementToast.ascx" TagName="AnnouncementToast" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Foundations/Controls/MaintenanceToast.ascx" TagName="MaintenanceToast" TagPrefix="uc" %>

<%@ Register Src="./Controls/DashboardNavigation.ascx" TagName="DashboardNavigation" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">

    <uc:DashboardNavigation runat="server" ID="DashboardNavigation" />

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:UserLicenseCheck runat="server" />
    <insite:UserPasswordCheck runat="server" />
    <uc:AnnouncementToast runat="server" ID="AnnouncementToast" />
    <uc:MaintenanceToast runat="server" ID="MaintenanceToast" ShowOnEachRequest="true" />

    <div class="alert alert-info" role="alert">
        <div class="d-flex align-items-center">
            <div class="fs-1">
                <i class="fas fa-fw fa-party-horn"></i>
            </div>
            <div class="px-3 fs-1 text-nowrap">
                New SkillsCheck assigned!
            </div>
            <div>
                You’ve been invited to complete a SkillsCheck. Get started by clicking the Start button in your dashboard below. 
            </div>
        </div>
    </div>

    <h1>Hi <asp:Literal runat="server" ID="UserFirstName" />!</h1>

    <div class="row">
        <div class="col-6">
            <h6 class="text-success">Welcome to SkillsCheck – Helping you align your skills with in-demand training and employment opportunities</h6>
            <h6>Here you'll find your assigned SkillsChecks, or you can browse our catalog to explore other opportunities</h6>
        </div>
        <div class="col-6">
            <div runat="server" id="AttemptCard" class="card" visible="false">
                <div class="card-body">
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
                    <insite:Button runat="server" Text="Start" Size="ExtraSmall" ButtonStyle="Success"
                        NavigateUrl='<%# GetGridAttemptStartUrl() %>'
                        Visible='<%# Eval("AttemptStarted") == null && Eval("AttemptImported") == null %>' />
                    <insite:Button runat="server" Text="Continue" Size="ExtraSmall" ButtonStyle="Success"
                        NavigateUrl='<%# GetGridAttemptStartUrl() %>'
                        Visible='<%# Eval("AttemptStarted") != null && Eval("AttemptSubmitted") == null %>' />
                    <insite:Button runat="server" Text="Share" Size="ExtraSmall" ButtonStyle="Primary"
                        OnClientClick="alert('Not Implemented'); return false;"
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

</asp:Content>