<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.Admin.Courses.Outlines.Modules.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">
    
    <asp:Panel runat="server" ID="DeletePanel">
        
        <div class="alert alert-danger" role="alert" runat="server" ID="ConfirmText">
             <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
            Are you sure you want to delete this module?
        </div>

        <div class="mb-3">
            <insite:DeleteButton runat="server" ID="DeleteButton" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>

    </asp:Panel>
        
    <asp:Panel runat="server" ID="ErrorPanel" Visible="false">
        <div class="alert alert-danger mb-3" role="alert">
             <i class="fas fa-stop-circle"></i>
            <asp:Literal runat="server" ID="ErrorText" />
        </div>

        <div class="mb-3">
            <insite:CloseButton runat="server" ID="CloseButton" />
        </div>
    </asp:Panel>

</div>
</asp:Content>
