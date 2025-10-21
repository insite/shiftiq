<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CaseDocumentList.ascx.cs" Inherits="InSite.UI.Portal.Issues.Controls.CaseDocumentList" %>

<asp:Repeater runat="server" ID="ListRepeater">
    <ItemTemplate>
        <div class="d-block my-2">
            <a href='<%# Eval("DownloadUrl") %>'>
                <i class='far fa-download me-1'></i>
                <%# Eval("FileName") %>
            </a>
            <span class="form-text text-body-secondary">
                <%# Eval("FileSize", "({0})") %>
            </span>

            <span runat="server" visible='<%# Eval("DocumentType") != null %>' class="badge bg-info ms-2">
                <%# Eval("DocumentType") %>
            </span>
        </div>

    </ItemTemplate>
</asp:Repeater>

<div class="w-50 mt-3">
    <insite:FileUploadV2 runat="server" ID="AttachmentInput" LabelText="New Attachment" AllowedExtensions=".docx,.jpg,.pdf,.png,.txt,.zip" />
</div>