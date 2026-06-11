<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PortalDocumentList.ascx.cs" Inherits="InSite.UI.Portal.Contacts.Referral.Controls.PortalDocumentList" %>

<asp:Label runat="server" ID="NoDocuments" Text="No documents" />

<asp:Repeater runat="server" ID="DocumentRepeater">
    <HeaderTemplate>
        <table class="table table-striped">
            <thead>
                <tr>
                    <th>Document Type</th>
                    <th>Document</th>
                    <th>Uploaded</th>
                </tr>
            </thead>
            <tbody>
    </HeaderTemplate>
    <FooterTemplate>
            </tbody>
        </table>
    </FooterTemplate>
    <ItemTemplate>
        <tr>
            <td>
                <%# Eval("DocumentType") %>
            </td>
            <td>
                <a href='<%# Eval("DownloadUrl") %>'>
                    <i class='far fa-download'></i>
                    <%# Eval("DocumentName") %>
                </a>
                <span class="form-text text-body-secondary">
                    (<%# Eval("FileSize") %>)
                </span>
                <span runat="server" visible='<%# Eval("IsAdministrator") %>' class="badge bg-warning ms-2">
                    Administrator
                </span>
                <span runat="server" visible='<%# Eval("IsApplicant") %>' class="badge bg-success ms-2">
                    Applicant
                </span>
            </td>
            <td>
                <%# FormatUploaded() %>
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>