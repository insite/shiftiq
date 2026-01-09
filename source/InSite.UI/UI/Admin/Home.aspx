<%@ Page Language="C#" CodeBehind="Home.aspx.cs" Inherits="InSite.UI.Admin.Home" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/HomeAdmin.ascx" TagName="HomeAdmin" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <uc:HomeAdmin runat="server" />
</asp:Content>
