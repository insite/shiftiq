<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Admin.Accounts.Senders.Forms.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="Details" Src="../Controls/Details.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Sender" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="SenderSection" Title="Sender" Icon="far fa-mailbox" IconPosition="BeforeText">
            <div class="mt-3">
                <uc:Details runat="server" ID="Details" />
            </div>
        </insite:NavItem>

    </insite:Nav>
        
    <div class="row">
        <div class="col-lg-6">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Sender" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>
