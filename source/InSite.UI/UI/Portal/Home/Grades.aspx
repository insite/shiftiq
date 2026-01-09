<%@ Page Language="C#" CodeBehind="Grades.aspx.cs" Inherits="InSite.UI.Portal.Home.Grades" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="./Controls/DashboardNavigation.ascx" TagName="DashboardNavigation" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">

    <uc:DashboardNavigation runat="server" ID="DashboardNavigation" />

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

<div class="row">
    <div class="col-lg-12">
        
        <insite:Alert runat="server" ID="StatusAlert" />

        <div class="mb-3">
            <insite:GradebookComboBox runat="server" ID="GradebookComboBox" AllowBlank="false" />
        </div>
        <asp:Literal runat="server" ID="GradeItemsHtml" />
        
    </div>
</div>

</asp:Content>