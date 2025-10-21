<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SetInfo.ascx.cs" Inherits="InSite.Admin.Assessments.Sets.Controls.SetInfo" %>

<%@ Register TagPrefix="assessments" Assembly="InSite.UI" Namespace="InSite.Admin.Assessments.Web.UI" %>

<div class="form-group mb-3">
    <label class="form-label">Set Number</label>
    <div>
        <asp:Literal runat="server" ID="SetNumber" />
    </div>
    <div class="form-text">
        This set contains <asp:Literal runat="server" ID="SetQuestionCount" /> of the <asp:Literal runat="server" ID="BankQuestionCount" /> questions in the bank.
    </div>
</div>

<div runat="server" id="NameDiv" class="form-group mb-3">
    <label class="form-label">Set Name</label>
    <div>
        <asp:Literal runat="server" ID="Name" />
    </div>
</div>

<div runat="server" id="StandardDiv" class="form-group mb-3">
    <label class="form-label">Standard</label>
    <div>
        <assessments:AssetTitleDisplay runat="server" ID="Standard" />
    </div>
</div>

<div runat="server" id="RandomizationDiv" class="form-group mb-3">
    <label class="form-label">Randomization</label>
    <div>
        <asp:Literal runat="server" ID="Randomization" />
        <asp:Literal runat="server" ID="RandomizationDescription" />
    </div>
</div>

<div id="CutScoreDiv" runat="server" class="form-group mb-3">
    <label class="form-label">Cut Score</label>
    <div>
        <asp:Literal runat="server" ID="CutScore" />
    </div>
</div>

<div id="PointsDiv" runat="server" class="form-group mb-3">
    <label class="form-label">Points</label>
    <div>
        <asp:Literal runat="server" ID="Points" />
    </div>
</div>

