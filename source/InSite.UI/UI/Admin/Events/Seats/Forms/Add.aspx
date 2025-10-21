<%@ Page Language="C#" CodeBehind="Add.aspx.cs" Inherits="InSite.Admin.Events.Seats.Forms.Add" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/Detail.ascx" TagName="Detail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Seat" />

    <uc:Detail runat="server" ID="Detail" />

    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Seat" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>