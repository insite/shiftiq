<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PerformanceReportCriteria.ascx.cs" Inherits="InSite.UI.Admin.Assessments.Attempts.Controls.PerformanceReportCriteria" %>

<h3>Criteria</h3>

<div class="form-group mb-3">
    <label class="form-label">
        Exam Candidate
        <insite:RequiredValidator runat="server" ControlToValidate="ExamCandidateID" FieldName="Exam Candidate" ValidationGroup="Report" />
    </label>
    <div>
        <insite:FindPerson runat="server" ID="ExamCandidateID" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Exam Form
    </label>
    <div>
        <insite:FindBankForm runat="server" ID="FormIdentifier" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Completed &ge;
    </label>
    <div>
        <insite:DateTimeOffsetSelector ID="AttemptGradedSince" runat="server" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Completed &lt;
    </label>
    <div>
        <insite:DateTimeOffsetSelector ID="AttemptGradedBefore" runat="server" />
    </div>
</div>
