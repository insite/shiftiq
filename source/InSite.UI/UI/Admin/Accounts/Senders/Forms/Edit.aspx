<%@ Page Language="C#" CodeBehind="Edit.aspx.cs" Inherits="InSite.Admin.Accounts.Senders.Forms.Edit" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/Details.ascx" TagName="Details" TagPrefix="uc" %>
<%@ Register Src="../Controls/SenderOrganizationList.ascx" TagName="SenderOrganizationList" TagPrefix="uc" %>
<%@ Register Src="../Controls/MessageGrid.ascx" TagName="MessageGrid" TagPrefix="uc" %>

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

        <insite:NavItem runat="server" ID="OrganizationsSection" Title="Organizations" Icon="far fa-city" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3 mb-3">
                <div class="card-body">
                    <h4 class="card-title mb-3">Organizations</h4>

                    <uc:SenderOrganizationList runat="server" ID="OrganizationList" />
                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="MessagesSection" Title="Messages" Icon="far fa-paper-plane" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3 mb-3">
                <div class="card-body">
                    <h4 class="card-title mb-3">Messages</h4>

                    <uc:MessageGrid runat="server" ID="MessageGrid" />
                </div>
            </div>
        </insite:NavItem>

    </insite:Nav>
        
    <div class="row">
        <div class="col-lg-6">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Sender" />
            <insite:DeleteButton runat="server" ID="DeleteButton" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>
</asp:Content>

