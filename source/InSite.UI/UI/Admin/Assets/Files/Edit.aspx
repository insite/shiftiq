<%@ Page CodeBehind="Edit.aspx.cs" Inherits="InSite.UI.Admin.Assets.Files.Edit" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="UploadDetail" Src="./Controls/UploadDetail.ascx" %>
<%@ Register TagPrefix="uc" TagName="FileHistoryList" Src="./Controls/FileHistoryList.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="File" />
    <insite:ValidationSummary runat="server" ValidationGroup="File" />

    <insite:Nav runat="server" CssClass="mb-3">
        <insite:NavItem runat="server" Title="File" Icon="far fa-file" IconPosition="BeforeText">
            <uc:UploadDetail runat="server" ID="Detail" />
        </insite:NavItem>
        <insite:NavItem runat="server" Title="History" Icon="far fa-history" IconPosition="BeforeText">
            <uc:FileHistoryList runat="server" ID="HistoryList" />
        </insite:NavItem>
    </insite:Nav>

    <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="File" DisableAfterClick="true" />
    <insite:CancelButton runat="server" ID="CancelButton" />

</asp:Content>