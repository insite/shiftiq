<%@ Page Language="C#" CodeBehind="Programs.aspx.cs" Inherits="InSite.UI.Portal.Home.Programs" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="./Controls/DashboardNavigation.ascx" TagName="DashboardNavigation" TagPrefix="uc" %>
<%@ Register Src="~/UI/Portal/Learning/Programs/Controls/ProgramSearchControl.ascx" TagPrefix="uc" TagName="ProgramSearchControl" %>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">

    <uc:DashboardNavigation runat="server" ID="DashboardNavigation" />

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <div style="position:relative">
        <insite:Button runat="server" NavigateUrl="/ui/portal/learning/plan" Text="My Training Plan" Icon="far fa-map-location-dot" />

        <div class="mt-4">
            <uc:ProgramSearchControl runat="server" ID="ProgramSearchControl" />
        </div>
    </div>

</asp:Content>
