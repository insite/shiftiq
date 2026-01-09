<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SubjectOutputDetails.ascx.cs" Inherits="InSite.Admin.Assessments.Banks.Controls.SubjectOutputDetails" %>

<div runat="server" id="BankField" class="form-group mb-3">
    <label class="form-label">Bank</label>
    <div><asp:Literal runat="server" ID="BankName" Text="N/A" /></div>
</div>

<div runat="server" id="SubjectContainerField" class="form-group mb-3" visible="false">
    <label class="form-label"><asp:Literal runat="server" ID="SubjectContainerType" /></label>
    <div><asp:Literal runat="server" ID="SubjectContainerName" /></div>
</div>

<div class="form-group mb-3">
    <label class="form-label"><asp:Literal runat="server" ID="SubjectType" /></label>
    <div><asp:Literal runat="server" ID="SubjectName" /></div>
</div>
