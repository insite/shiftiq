<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentTextEditor.ascx.cs" Inherits="InSite.Admin.Contacts.Comments.Controls.CommentTextEditor" %>

<insite:Alert runat="server" ID="CommentTextAlert" />

<insite:MarkdownEditor runat="server" ID="CommentText" UploadControl="CommentUpload" />
<insite:EditorUpload runat="server" ID="CommentUpload" />