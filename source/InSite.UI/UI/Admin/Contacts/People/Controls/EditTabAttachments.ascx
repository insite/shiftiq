<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditTabAttachments.ascx.cs" Inherits="InSite.UI.Admin.Contacts.People.Controls.EditTabAttachments" %>

<%@ Register Src="./DocumentList.ascx" TagName="DocumentList" TagPrefix="uc" %>

<div class="float-end text-end mb-3">
    <insite:Button runat="server" ID="UploadFileButton" ButtonStyle="Default" Text="Add Document" Icon="fas fa-upload" />
</div>

<div class="clearfix" ></div>

<div runat="server" id="NoAttachments" class="card mb-3">
    <div class="card-body">
        No Attachments
    </div>
</div>

<div runat="server" id="DocumentsCard" class="card mb-3">
    <div class="card-body">
        <h3>Documents</h3>

        <uc:DocumentList runat="server" ID="DocumentList" />
    </div>
</div>

<div runat="server" id="ImagesCard" class="card mb-3">
    <div class="card-body">
        <h3>Images</h3>

        <uc:DocumentList runat="server" ID="ImageList" />
    </div>
</div>

<div runat="server" id="ResponsesCard" class="card mb-3">
    <div class="card-body">
        <h3 id="form-attachments">Form Attachments</h3>

        <uc:DocumentList runat="server" ID="ResponseList" />
    </div>
</div>

<div runat="server" id="IssuesCard" class="card mb-3">
    <div class="card-body">
        <h3 id="issue-attachments">Case Attachments</h3>

        <uc:DocumentList runat="server" ID="IssueList" />
    </div>
</div>