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
                If you’re interested in purchasing a SkillsCheck, please visit www.skillscheck.ca for more information.
            </div>
        </div>
    </div>

</asp:Content>