<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttachmentsSection.ascx.cs" Inherits="InSite.Admin.Assessments.Outlines.Controls.AttachmentsSection" %>

<%@ Register TagPrefix="uc" TagName="AttachmentsTabsNav" Src="~/UI/Admin/Assessments/Attachments/Controls/AttachmentsTabsNav.ascx" %>

<div class="row" style="padding-bottom:18px;">
    <div class="col-lg-6">
        <insite:TextBox runat="server" ID="FilterAttachmentsTextBox" EmptyMessage="Filter Attachments" />
    </div>
    <div class="col-lg-6 text-end">
        <insite:Button runat="server" id="AddAttachmentButton" ButtonStyle="Default" ToolTip="Add Attachment" Text="Add Attachment" Icon="fas fa-plus-circle" />
        <insite:Button runat="server" id="ScanImagesLink" ButtonStyle="Default" ToolTip="Scan Images" Text="Scan Images" Icon="fas fa-images" />
    </div>
</div>

<uc:AttachmentsTabsNav runat="server" ID="AttachmentsNav" KeywordInput="FilterAttachmentsTextBox" />
