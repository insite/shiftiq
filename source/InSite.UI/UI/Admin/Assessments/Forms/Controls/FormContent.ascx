<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FormContent.ascx.cs" Inherits="InSite.Admin.Assessments.Forms.Controls.FormContent" %>

<div class="row mb-3">
    <div class="col-lg-6">

        <h3>Content</h3>

        <div class="form-group mb-3">
            <label class="form-label">
                Title
                <insite:IconLink Name="pencil" runat="server" id="C1" ToolTip="Change Content" CssClass="ms-1" />
            </label>
            <div>
                <asp:Literal runat="server" ID="ContentTitle" />
            </div>
            <div class="form-text">
                A user-friendly one-line title for the form.
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Summary
                <insite:IconLink Name="pencil" runat="server" id="C2" ToolTip="Change Content" CssClass="ms-1" />
            </label>
            <div>
                <span style="white-space:pre-wrap;"><asp:Literal runat="server" ID="ContentSummary" /></span>
            </div>
            <div class="form-text">
                The purpose of the form.
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Diagram Book
                <insite:IconLink Name="pencil" runat="server" id="C3" ToolTip="Change Content" CssClass="ms-1" />
            </label>
            <div>
                <asp:Literal runat="server" ID="HasDiagrams" />
            </div>
            <div class="form-text">Does this form have any diagrams associated with the questions it contains?</div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Reference Materials for Online Sessions
                <insite:IconLink Name="pencil" runat="server" id="C4" ToolTip="Change Content" CssClass="ms-1" />
            </label>
            <div>
                <asp:Literal runat="server" ID="HasReferenceMaterials" />
            </div>
            <div class="form-text">Does this form have any reference materials (e.g. formulas, acronyms) included for online exams?</div>
        </div>

    </div>
    <div class="col-lg-6">

        <h3>
            Attempt Notifications
        </h3>

        <div class="form-group mb-3">
            <label class="form-label">
                Started (Administrator)
                <insite:IconLink Name="pencil" runat="server" id="N1" ToolTip="Change Content" CssClass="fs-6 ms-1" />
            </label>
            <div>
                <asp:Literal runat="server" ID="AttemptStartedMessage" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Completed (Administrator)
                <insite:IconLink Name="pencil" runat="server" id="N2" ToolTip="Change Content" CssClass="ms-1" />
            </label>
            <div>
                <asp:Literal runat="server" ID="AttemptCompletedMessage" />
            </div>
        </div>

    </div>
</div>

<div class="row mb-3">
    <div class="col-lg-6 mb-3 mb-lg-0">

        <h3>
            Materials for Distribution
            <insite:IconLink Name="pencil" runat="server" id="M1" ToolTip="Change Content" CssClass="fs-6 ms-1" />
        </h3>

        <div class="form-group mb-3">
            <asp:Literal runat="server" ID="MaterialsForDistribution" />
        </div>

    </div>
    <div class="col-lg-6">

        <h3>
            Materials for Participation (Candidates)
            <insite:IconLink Name="pencil" runat="server" id="M2" ToolTip="Change Content" CssClass="fs-6 ms-1" />
        </h3>

        <div class="form-group mb-3">
            <asp:Literal runat="server" ID="MaterialsForParticipation" />
        </div>

    </div>
</div>

<div class="row">
    <div class="col-lg-6 mb-3 mb-lg-0">

        <h3>
            Instructions for Online
            <insite:IconLink Name="pencil" runat="server" id="I2" ToolTip="Change Content" CssClass="fs-6 ms-1" />
        </h3>

        <div class="form-group mb-3">
            <asp:Literal runat="server" ID="InstructionsForOnline" />
        </div>

    </div>
    <div class="col-lg-6">

        <h3>
            Instructions for Paper
            <insite:IconLink Name="pencil" runat="server" id="I1" ToolTip="Change Content" CssClass="fs-6 ms-1" />
        </h3>

        <div class="form-group mb-3">
            <asp:Literal runat="server" ID="InstructionsForPaper" />
        </div>

    </div>
</div>
