<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CaseFileRequirementList.ascx.cs" Inherits="InSite.UI.Portal.Issues.Controls.CaseFileRequirementList" %>

<asp:Repeater runat="server" ID="ListRepeater">
    <ItemTemplate>
        <div class="mt-3 text-danger">
            <insite:IconButton runat="server"
                Name="upload"
                Type="Regular"
                CssClass="me-1 text-danger fs-4"
                ToolTip="Upload Document"
                OnClientClick='<%# Eval("Id", "issueRequestList.showFileUploader(event, \"{0}\")") %>'
            />

            <a href="#" 
               class="upload-request-text text-danger text-decoration-none"
               onclick='<%# Eval("Id", "issueRequestList.showFileUploader(event, \"{0}\")") %>'>
                Request to upload <b><%# Eval("DocumentName") %></b> document
            </a>

        </div>
    </ItemTemplate>
</asp:Repeater>

<div class="d-none">
    <insite:FileUploadV2 runat="server" ID="FileUpload"
        AllowedExtensions=".docx,.jpg,.pdf,.png,.txt,.zip"
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