<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssessorDetails.ascx.cs" Inherits="InSite.UI.Admin.Assessments.Attempts.Controls.AssessorDetails" %>

<h3 runat="server" id="ControlTitle"></h3>

<div class="form-group mb-3">
    <label class="form-label">
        User Name
    </label>
    <div>
        <asp:Literal runat="server" ID="UserName" />
        <a runat="server" id="UserLink"></a>
    </div>
</div>
            
<div runat="server" id="UserEmailField" class="form-group mb-3">
    <label class="form-label">
        Email
    </label>
    <div>
        <asp:Literal runat="server" ID="UserEmail" />
    </div>
</div>
