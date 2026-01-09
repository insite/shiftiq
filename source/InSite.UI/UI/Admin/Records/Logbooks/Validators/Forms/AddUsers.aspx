<%@ Page Language="C#" CodeBehind="AddUsers.aspx.cs" Inherits="InSite.UI.Admin.Records.Validators.Forms.AddUsers" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../../Controls/AddUsersControl.ascx" TagName="AddUsersControl" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <uc:AddUsersControl runat="server" ID="AddUsersControl" />
</asp:Content>
