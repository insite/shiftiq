<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttachmentsSection.ascx.cs" Inherits="InSite.Admin.Issues.Outlines.Controls.AttachmentsSection" %>

<%@ Register TagPrefix="uc" TagName="CaseDocumentList" Src="CaseDocumentList.ascx" %>
<%@ Register TagPrefix="uc" TagName="CaseFileRequirementList" Src="CaseFileRequirementList.ascx" %>

<div class="mt-4 mb-4">
    <insite:Button runat="server" ID="AddAttachmentButton" ButtonStyle="Default" Text="Attach Document" Icon="fas fa-upload" />
    <insite:Button runat="server" ID="AddRequestButton" ButtonStyle="Default" Text="Request Document" Icon="fas fa-upload" />
</div>

<div runat="server" id="RequestsSection" class="card border-0 shadow-lg mb-3">
    <div class="card-body">
        <h3>Requests</h3>
        <uc:CaseFileRequirementList runat="server" ID="CaseFileRequirementList" />
    </div>
</div>

<div class="card border-0 shadow-lg">
    <div class="card-body">
        <h3>Documents</h3>
        <uc:CaseDocumentList runat="server" ID="CaseDocumentList" />
    </div>
</div>