<%@ Page Language="C#" CodeBehind="Catalog.aspx.cs" Inherits="InSite.UI.Portal.Home.Learning.Catalog" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="./Controls/DashboardNavigation.ascx" TagName="DashboardNavigation" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">

    <uc:DashboardNavigation runat="server" ID="DashboardNavigation" />

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <div class="alert alert-info" role="alert">
        <div class="d-flex align-items-center">
            <div>
                SkillsCheck purchases for individuals are coming soon. In the meantime, explore <a target="_blank" href="https://skillscheck.ca">SkillsCheck.ca</a> to learn more.
            </div>
        </div>
    </div>

</asp:Content>