<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FieldInputSubject.ascx.cs" Inherits="InSite.Admin.Messages.Messages.Controls.FieldInputSubject" %>

<div class="form-group mb-3">
    <label class="form-label">
        External Subject
        <insite:RequiredValidator runat="server" ID="SubjectValidator" ControlToValidate="SubjectInput" FieldName="External Subject" />
    </label>
    <insite:TextBox runat="server" TranslationControl="SubjectInput" MaxLength="256" />
    <div class="mt-1">
        <insite:EditorTranslation runat="server" ID="SubjectInput" />
    </div>
    <div class="form-text">
        This is the subject line on the message sent to recipients.
    </div>
</div>
