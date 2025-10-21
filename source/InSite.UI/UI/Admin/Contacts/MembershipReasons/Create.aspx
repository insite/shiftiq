<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.UI.Admin.Contacts.MembershipReasons.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master"  %>

<%@ Register TagPrefix="uc" TagName="ReasonDetail" Src="./Controls/ReasonDetail.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Reason" />

    <div class="row mb-3">
        <div class="col-lg-6">
            <h4 class="mb-3">Reason</h4>
            <div class="card border-0 shadow-lg">
                <div class="card-body">
                    <uc:ReasonDetail runat="server" ID="ReasonDetail" />
                </div>
            </div>
        </div>
    </div>
        
    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Reason" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>
</asp:Content>
