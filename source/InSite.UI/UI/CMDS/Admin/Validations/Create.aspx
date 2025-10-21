<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Custom.CMDS.Admin.Standards.Validations.Forms.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="Detail.ascx" TagName="Detail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <uc:Detail runat="server" ID="Detail" />

    <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Validation" />
    <insite:CancelButton runat="server" NavigateUrl="/ui/cmds/admin/validations/search" />
</asp:Content>
