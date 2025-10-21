<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentDetail.ascx.cs" Inherits="InSite.UI.Portal.Records.Logbooks.Controls.CommentDetail" %>

<div class="row">

    <div class="col-md-6">

        <div runat="server" id="EntryField" class="form-group mb-3" visible="false">
            <label class="form-label">
                <insite:Literal runat="server" Text="Entry" />
            </label>
            <div>
                <asp:Literal runat="server" ID="EntryName" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                <insite:Literal runat="server" Text="Text" />
                <insite:RequiredValidator runat="server" ControlToValidate="CommentText" Display="None" ValidationGroup="Comment" />
            </label>
            <div>
                <insite:MarkdownEditor runat="server" ID="CommentText" UploadControl="CommentUpload" />
                <insite:EditorUpload runat="server" ID="CommentUpload" />
            </div>
            <div class="form-text">
                The body/text for the comment.
            </div>
        </div>

    </div>

</div>