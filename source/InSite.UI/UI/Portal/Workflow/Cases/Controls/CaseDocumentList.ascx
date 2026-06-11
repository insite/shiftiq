<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CaseDocumentList.ascx.cs" Inherits="InSite.UI.Portal.Issues.Controls.CaseDocumentList" %>

<asp:Repeater runat="server" ID="ListRepeater">
    <HeaderTemplate>
        <table class="table table-striped">
            <thead>
                <tr>
                    <th class="w-50"><insite:Literal runat="server" Text="Document Type" /></th>
                    <th runat="server" visible="false"><insite:Literal runat="server" Text="Status" /></th>
                    <th class="w-50"><insite:Literal runat="server" Text="Document" /></th>
                    <th runat="server" visible="false"><insite:Literal runat="server" Text="Date" /></th>
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
                <div runat="server" visible='<%# false && Eval("DocumentSubtype") != null %>' class="form-text">
                    <%# Eval("DocumentSubtype") %>
                </div>
            </td>
            <td runat="server" visible="false">
                <%# Eval("Status") %>
            </td>
            <td>
                <a runat="server" visible='<%# Eval("AllowLearnerToView") %>' href='<%# Eval("DownloadUrl") %>'>
                    <i class='far fa-download me-1'></i>
                    <%# Eval("FileName") %>
                </a>
                <span runat="server" visible='<%# !(bool)Eval("AllowLearnerToView") %>'>
                    <%# Eval("FileName") %>
                </span>
                <span class="form-text text-body-secondary">
                    <%# Eval("FileSize", "({0})") %>
                </span>
                <span runat="server" visible='<%# Eval("IsAdministrator") %>' class="badge bg-success ms-2">
                    <insite:Literal runat="server" Text="Administrator" />
                </span>
                <span runat="server" visible='<%# Eval("IsApplicant") %>' class="badge bg-warning ms-2">
                    <insite:Literal runat="server" Text="Applicant" />
                </span>
            </td>
            <td runat="server" visible="false">
                <div>
                    <insite:Literal runat="server" Text="Uploaded" />: <%# LocalizeDate(Eval("Uploaded")) %>
                </div>
                <div runat="server" visible='<%# Eval("Reviewed") != null %>'>
                    <insite:Literal runat="server" Text="Reviewed" />: <%# LocalizeDate(Eval("Reviewed")) %>
                </div>

                <div runat="server" visible='<%# Eval("Approved") != null %>'>
                    <insite:Literal runat="server" Text="Approved" />: <%# LocalizeDate(Eval("Approved")) %>
                </div>
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>

<div runat="server" id="NewAttachmentPanel" class="w-50 mt-3">
    <insite:FileUploadV2 runat="server" ID="AttachmentInput" AllowedExtensions=".docx,.jpg,.pdf,.png,.txt,.zip" FileUploadType="Document" />
</div>
