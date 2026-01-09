<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentInfo.ascx.cs" Inherits="InSite.UI.Portal.Learning.Controls.CommentInfo" %>

<%@ Register TagPrefix="uc" TagName="CommentTextEditor" Src="CommentTextEditor.ascx" %>

<h3 class="d-none mt-5">Reply</h3>

<div class="form-group mb-3">
    <div>
        <uc:CommentTextEditor runat="server" ID="CommentText" />
    </div>
</div>