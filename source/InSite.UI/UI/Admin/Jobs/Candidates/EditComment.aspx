<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditComment.aspx.cs" Inherits="InSite.UI.Admin.Jobs.Candidates.EditComment" MasterPageFile="~/UI/Layout/Admin/AdminModal.master" %>

<%@ Register Src="Controls/CommentDetail.ascx" TagName="CommentDetail" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">
    <uc:CommentDetail runat="server" ID="Detail" />

    <div class="row">
        <div class="col-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="ContactComment" />
            <insite:CloseButton runat="server" ID="CloseButton" OnClientClick="modalManager.closeModal(); return false;" />
        </div>
    </div>
</asp:Content>