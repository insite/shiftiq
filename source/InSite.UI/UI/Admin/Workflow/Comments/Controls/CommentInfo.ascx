<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentInfo.ascx.cs" Inherits="InSite.UI.Admin.Issues.Comments.Controls.CommentInfo" %>

<%@ Register TagPrefix="uc" TagName="CommentTextEditor" Src="CommentTextEditor.ascx" %>

<h3>Comment</h3>

<div class="form-group mb-3">
    <label class="form-label">
        Text
        <span class="text-danger">*</span>
    </label>
    <div>
        <uc:CommentTextEditor runat="server" ID="CommentText" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Assigned to
    </label>
    <div>
        <insite:FindPerson runat="server" ID="AssignedTo" />
    </div>
</div>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="CategoryUpdatePanel" />

<insite:UpdatePanel runat="server" ID="CategoryUpdatePanel">
    <ContentTemplate>
        <div class="form-group mb-3">
            <label class="form-label">
                Category
            </label>
            <div>
                <insite:CaseCommentCategoryComboBox runat="server" ID="CommentCategory" />
            </div>
        </div>

        <div class="form-group mb-3" runat="server" id="CommentSubCategoryPanel" visible="false">
            <label class="form-label">
                Subcategory
            </label>
            <div>
                <insite:ItemNameComboBox runat="server" ID="CommentSubCategory" />
            </div>
        </div>
    </ContentTemplate>
</insite:UpdatePanel>

<div class="form-group mb-3">
    <label class="form-label">
        Flag
    </label>
    <div>
        <insite:ColorComboBox runat="server" ID="CommentFlag" />
    </div>
</div>
<div class="form-group mb-3">
    <label class="form-label">
        Tag
    </label>
    <div>
        <insite:ItemNameComboBox runat="server" ID="CommentTag" />
    </div>
</div>