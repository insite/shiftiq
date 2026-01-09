<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Custom.CMDS.Admin.Standards.ValidationChanges.Forms.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="Detail.ascx" TagName="Detail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <uc:Detail runat="server" ID="Detail" />

    <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Competency" />
    <insite:CancelButton runat="server" ID="CancelButton" />
</asp:Content>
