<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Markdown.ascx.cs" Inherits="InSite.UI.Admin.Records.Logbooks.Controls.ExperienceFields.Markdown" %>

<div class="form-group mb-3">
    <label class="form-label">
        <asp:Literal runat="server" ID="FieldTitle" />
        <insite:RequiredValidator runat="server" ID="RequiredValidator" ControlToValidate="MarkdownValue" ErrorMessage="This is a required value" Display="None" />
    </label>
    <div>
        <insite:MarkdownEditor runat="server" ID="MarkdownValue" UploadControl="MarkdownUpload" />
        <insite:EditorUpload runat="server" ID="MarkdownUpload" />
    </div>
    <div class="form-text">
        <asp:Literal runat="server" ID="HelpText" />
    </div>
</div>
