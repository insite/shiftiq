<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentDetail.ascx.cs" Inherits="InSite.Admin.Records.Logbooks.Controls.CommentDetail" %>

<div class="row">

    <div class="col-md-6">

        <div runat="server" id="EntryField" class="form-group mb-3" visible="false">
            <label class="form-label">
                Entry
            </label>
            <div>
                <asp:Literal runat="server" ID="EntryName" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Text
                <insite:RequiredValidator runat="server" ControlToValidate="CommentText" ValidationGroup="Comment" />
            </label>
            <div>
                <insite:MarkdownEditor runat="server" ID="CommentText" UploadControl="CommentUpload" />
                <insite:EditorUpload runat="server" ID="CommentUpload" />
            </div>
            <div class="form-text">
                The body/text for the comment.
            </div>
        </div>
                    
        <div class="form-group mb-3">
            <label class="form-label">
                Private
            </label>
            <div>
                <asp:RadioButtonList runat="server" ID="IsPrivate">
                    <asp:ListItem Value="true" Text="Yes" />
                    <asp:ListItem Value="false" Text="No" Selected="true" />
                </asp:RadioButtonList>
            </div>
            <div class="form-text">
                This comment will not be released to the learner if "Yes" is selected.
            </div>
        </div>

    </div>

</div>
