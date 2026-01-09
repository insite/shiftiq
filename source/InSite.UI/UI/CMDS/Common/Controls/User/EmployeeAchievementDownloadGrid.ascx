<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmployeeAchievementDownloadGrid.ascx.cs" Inherits="InSite.Custom.CMDS.User.Progressions.Controls.EmployeeAchievementDownloadGrid" %>
<%@ Import Namespace="InSite.Cmds.Infrastructure" %>

<asp:Repeater runat="server" ID="Repeater">
    <ItemTemplate>

        <div class="mb-2">
            <div runat="server" id="Commands" class="float-start me-3" visible="<%# AllowEdit %>">
                <cmds:IconButton runat="server" IsFontIcon="true" CssClass="trash-alt" ToolTip="Delete File" ConfirmText="Are you sure you want to delete this file?" CommandArgument='<%# Eval("UploadIdentifier") %>' CommandName="DeleteFile" />
            </div>

            <a target="_blank" runat="server" href='<%# CmdsUploadProvider.GetFileRelativeUrl((Guid)Eval("ContainerIdentifier"), (string)Eval("Name")) %>'><%# Eval("Title") %></a>
            <span class="form-text"><%# ((int)Eval("ContentSize") / 1024).ToString("n0") + " KB" %></span>

            <div runat="server" visible='<%# ValueConverter.IsNotNull(Eval("Description")) %>'>
                <%# Eval("Description") %>
            </div>
        </div>

    </ItemTemplate>
</asp:Repeater>

<asp:Button ID="RefreshDownloadsButton" runat="server" style="display:none;" />

<div class="mt-3">
    <insite:Button ID="UploadFileButton" runat="server" ButtonStyle="Primary"  Text="Upload File" Icon="fas fa-upload" />
    <asp:Label runat="server" ID="UploadFileError" CssClass="alert alert-danger" EnableViewState="false" Visible="false" />
</div>

<insite:Modal runat="server" ID="FileUploadWindow" Title="Upload File" Width="600px">
    <ContentTemplate>
        
        <div class="row">
            <div class="col-md-6">
                <div class="form-group mb-3">
                    <label class="form-label">
                        File
                    </label>
                    <div>
                        <asp:FileUpload ID="File" runat="server" />
                    </div>
                </div>
                <div class="form-group mb-3">
                    <label class="form-label">
                        Title
                    </label>
                    <div>
                        <insite:TextBox ID="Title" runat="server" MaxLength="256" />
                    </div>
                </div>
                <div class="form-group mb-3">
                    <label class="form-label">
                        Description
                    </label>
                    <div>
                        <insite:TextBox ID="Description" runat="server" TextMode="MultiLine" />
                    </div>
                </div>

                <insite:SaveButton runat="server" ID="AttachFileButton" />
                <insite:CloseButton runat="server" ButtonStyle="Default" OnClientClick="closeFileUploader(); return false;" />
            </div>
        </div>

    </ContentTemplate>
</insite:Modal>

<script type="text/javascript">

    function showFileUploader() {
        modalManager.show($('#<%= FileUploadWindow.ClientID %>'));
    }

    function closeFileUploader() {
        modalManager.close($('#<%= FileUploadWindow.ClientID %>'));
    }

</script>
