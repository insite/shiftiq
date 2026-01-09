<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ScoreCalculationDetails.ascx.cs" Inherits="InSite.Admin.Assessments.Specifications.Controls.ScoreCalculationDetails" %>

<h3>Scoring Calculation</h3>

<div class="form-group mb-3">
    <label class="form-label">
        Disclosure Type
    </label>
    <div>
        <asp:RadioButtonList runat="server" ID="DisclosureType">
            <asp:ListItem Value="None" Text="No Score or Answers Shown" Selected="True" />
            <asp:ListItem Value="Score" Text="Show Score Only" />
            <asp:ListItem Value="Answers" Text="Show Answers Only" />
            <asp:ListItem Value="Full" Text="Show Score & Correct/Incorrect Answers" />
        </asp:RadioButtonList>
    </div>
    <div class="form-text">
        What information is disclosed to a student/candidate after completing an exam submission?
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Passing Score (%)
        <insite:RequiredValidator runat="server" ControlToValidate="PassingScore" ValidationGroup="Assessment" />
    </label>
    <div>
        <insite:NumericBox runat="server" ID="PassingScore" NumericMode="Integer" MinValue="0" MaxValue="100" ValueAsDecimal="50" />
    </div>
    <div class="form-text">
        What is the minimum score required to pass the exam?
    </div>
</div>