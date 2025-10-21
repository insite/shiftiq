<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PortalDocumentList.ascx.cs" Inherits="InSite.UI.Portal.Contacts.Referral.Controls.PortalDocumentList" %>

<asp:Label runat="server" ID="NoDocuments" Text="No documents" />

<asp:Repeater runat="server" ID="DocumentRepeater">
    <ItemTemplate>
        <div class="mb-1">
            <a href='<%# Eval("DownloadUrl") %>'>
                <i class='far fa-download'></i>
                <%# Eval("DocumentName") %>
            </a>
            <span class="form-text text-body-secondary">
                (<%# Eval("FileSize") %>)
            </span>
        </div>
    </ItemTemplate>
</asp:Repeater>