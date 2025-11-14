<%@ Page Language="C#" CodeBehind="Browse.aspx.cs"
    Inherits="InSite.UI.Admin.Assets.Files.Files.Forms.Browse" 
    MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="FileTreeView" Src="../../Controls/FileTreeView.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <uc:FileTreeView runat="server" ID="TreeView" />

</asp:Content>
