<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CaseInfo.ascx.cs" Inherits="InSite.Admin.Issues.Issues.Controls.CaseInfo" %>

<h3 runat="server" id="IssueNumber"></h3>

<div runat="server" id="TypeDiv" class="form-group mb-3">
    <label class="form-label">Case Type</label>
    <div>
        <asp:Literal runat="server" ID="IssueType" />
    </div>
</div>

<div runat="server" id="TitleDiv" class="form-group mb-3">
    <label class="form-label">Case Summary</label>
    <div>
        <a runat="server" id="IssueLink">
            <asp:Literal runat="server" ID="IssueTitle" />
        </a>
    </div>
</div>

<div runat="server" id="DescriptionDiv" class="form-group mb-3">
    <label class="form-label">Case Description</label>
    <div>
        <asp:Literal runat="server" ID="IssueDescription" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">Current Status</label>
    <div>
        <asp:Literal runat="server" ID="IssueStatus" />
    </div>
</div>