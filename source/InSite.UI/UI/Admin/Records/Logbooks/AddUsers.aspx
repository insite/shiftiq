<%@ Page Language="C#" CodeBehind="AddUsers.aspx.cs" Inherits="InSite.Admin.Records.Logbooks.AddUsers" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="./Controls/AddUsersControl.ascx" TagName="AddUsersControl" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <uc:AddUsersControl runat="server" ID="AddUsersControl" />
</asp:Content>
