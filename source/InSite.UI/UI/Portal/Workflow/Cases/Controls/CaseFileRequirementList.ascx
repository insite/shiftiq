<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CaseFileRequirementList.ascx.cs" Inherits="InSite.UI.Portal.Issues.Controls.CaseFileRequirementList" %>

<asp:Repeater runat="server" ID="ListRepeater">
    <HeaderTemplate>
        <table class="table table-striped">
            <thead>
                <tr>
                    <th class="w-50">Document Type</th>
                    <th runat="server" visible="false">Status</th>
                    <th class="w-50">Document</th>
                    <th runat="server" visible="false">Requested</th>
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
                <div runat="server" visible='<%# Eval("DocumentSubtype") != null %>' class="form-text">
                    <%# Eval("DocumentSubtype") %>
                </div>
            </td>
            <td runat="server" visible="false">
                <%# Eval("Status") %>
            </td>
            <td>
                <insite:IconButton runat="server"
                    Visible='<%# Eval("RequestedFromCandidate") %>'
                    Name="upload"
                    Type="Regular"
                    CssClass="text-danger me-1"
                    ToolTip="Upload Document"
                    OnClientClick='<%# Eval("Id", "issueRequestList.showFileUploader(event, \"{0}\")") %>'
                />

                <a runat="server"
                    visible='<%# Eval("RequestedFromCandidate") %>'
                    class="text-danger"
                    href="#"
                    onclick='<%# Eval("Id", "issueRequestList.showFileUploader(event, \"{0}\")") %>'
                >
                    Upload Document
                </a>
            </td>
            <td runat="server" visible="false">
                <%# LocalizeDate(Eval("RequestedTime")) %>
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>

<div class="d-none">
    <insite:FileUploadV2 runat="server" ID="FileUpload"
        AllowedExtensions=".docx,.jpg,.pdf,.png,.txt,.zip"
        FileUploadType="Document"
    />
    <asp:HiddenField runat="server" ID="DocumentTypeId" />
</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">

        (function () {
            const issueRequestList = window.issueRequestList = window.issueRequestList || {};

            issueRequestList.showFileUploader = function (e, id) {
                e.preventDefault();

                document.getElementById('<%= DocumentTypeId.ClientID %>').value = id;

                inSite.common.fileUploadV2.trigger('<%= FileUpload.ClientID %>');
            };
        })();

    </script>
</insite:PageFooterContent>