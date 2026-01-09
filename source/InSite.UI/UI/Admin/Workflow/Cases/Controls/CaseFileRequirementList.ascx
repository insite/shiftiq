<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CaseFileRequirementList.ascx.cs" Inherits="InSite.UI.Admin.Issues.Outlines.Controls.CaseFileRequirementList" %>

<asp:Repeater runat="server" ID="ListRepeater">
    <HeaderTemplate>
        <table class="table table-striped">
            <thead>
                <tr>
                    <th></th>
                    <th>Document Type</th>
                    <th>Requested</th>
                    <th>Requested From</th>
                    <th>Description</th>
                </tr>
            </thead>
            <tbody>
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <td style="width:100px;">
                <insite:IconButton runat="server"
                    ID="UploadButton"
                    Name="upload"
                    Type="Regular"
                    ToolTip="Upload Document"
                    CommandArgument='<%# Eval("DocumentType") %>'
                    OnClientClick='<%# string.Format("issueRequestList.showFileUploader(event, {0})", Container.ItemIndex) %>'
                />

                <insite:IconLink runat="server"
                    Name="pencil"
                    Type="Regular"
                    ToolTip="Edit request"
                    NavigateUrl='<%# Eval("ModifyUrl") %>'
                />

                <insite:IconButton runat="server"
                    Name="trash-alt"
                    ToolTip="Delete Request"
                    CommandName="Delete"
                    CommandArgument='<%# Eval("DocumentType") %>'
                    ConfirmText="Are you sure you want to delete this request?"
                />
            </td>
            <td>
                <%# Eval("DocumentType") %>
                <div class="form-text">
                    <%# Eval("DocumentSubType") %>
                </div>
            </td>
            <td>
                <%# Eval("RequestedTime") %>
                <div class="form-text">
                    <%# Eval("RequestedUserName") %>
                </div>
            </td>
            <td>
                <%# Eval("RequestedFrom") %>
            </td>
            <td>
                <%# Eval("Description") %>
            </td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
            </tbody>
        </table>
    </FooterTemplate>
</asp:Repeater>

<div class="d-none">
    <insite:FileUploadV2 runat="server" ID="FileUpload"
        AllowedExtensions=".docx,.jpg,.pdf,.png,.txt,.zip"
        FileUploadType="Document"
    />
    <asp:HiddenField runat="server" ID="DocumentTypeIndex" />
</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">

        (function () {
            const issueRequestList = window.issueRequestList = window.issueRequestList || {};

            issueRequestList.showFileUploader = function (e, documentTypeIndex) {
                e.preventDefault();

                document.getElementById('<%= DocumentTypeIndex.ClientID %>').value = documentTypeIndex;

                inSite.common.fileUploadV2.trigger('<%= FileUpload.ClientID %>');
            };
        })();

    </script>
</insite:PageFooterContent>