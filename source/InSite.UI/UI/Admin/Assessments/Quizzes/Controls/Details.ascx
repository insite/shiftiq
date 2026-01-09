<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Details.ascx.cs" Inherits="InSite.UI.Admin.Assessments.Quizzes.Controls.Details" %>

<div class="form-group mb-3">
    <label class="form-label">
        Quiz Type
        <insite:RequiredValidator runat="server" ID="QuizTypeValidator" ControlToValidate="QuizType" FieldName="Quiz Type" ValidationGroup="Quiz" />
    </label>
    <insite:QuizTypeComboBox runat="server" ID="QuizType" AllowBlank="true" />
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Quiz Name
        <insite:RequiredValidator runat="server" ID="QuizNameValidator" ControlToValidate="QuizName" FieldName="Quiz Name" ValidationGroup="Quiz" />
    </label>
    <insite:TextBox runat="server" ID="QuizName" MaxLength="100" />
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Time Limit (seconds)
        <insite:RequiredValidator runat="server" ID="TimeLimitValidator" ControlToValidate="TimeLimit" FieldName="Time Limit" ValidationGroup="Quiz" />
    </label>
    <insite:NumericBox runat="server" ID="TimeLimit" MinValue="0" NumericMode="Integer" Width="150px" />
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Attempt Limit
        <insite:RequiredValidator runat="server" ID="AttemptLimitValidator" ControlToValidate="AttemptLimit" FieldName="Attempt Limit" ValidationGroup="Quiz" />
    </label>
    <insite:NumericBox runat="server" ID="AttemptLimit" MinValue="0" NumericMode="Integer" Width="150px" />
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Maximum Possible Points
    </label>
    <insite:NumericBox runat="server" ID="MaximumPoints" MinValue="0" NumericMode="Float" DecimalPlaces="2" Width="150px" />
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Points Required to Pass
    </label>
    <insite:NumericBox runat="server" ID="PassingPoints" MinValue="0" NumericMode="Float" DecimalPlaces="2" Width="150px" />
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Passing Score (%)
    </label>
    <insite:NumericBox runat="server" ID="PassingScore" MinValue="1" MaxValue="100" NumericMode="Integer" Width="150px" />
</div>

<div runat="server" id="PassingWpmField" class="form-group mb-3">
    <label class="form-label">
        Passing WPM
    </label>
    <insite:NumericBox runat="server" ID="PassingWpm" MinValue="0" NumericMode="Integer" Width="150px" />
</div>

<div runat="server" id="PassingKphField" class="form-group mb-3">
    <label class="form-label">
        Passing KPH
    </label>
    <insite:NumericBox runat="server" ID="PassingKph" MinValue="0" NumericMode="Integer" Width="150px" />
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Passing Accuracy (%)
    </label>
    <insite:NumericBox runat="server" ID="PassingAccuracy" MinValue="0" MaxValue="100" NumericMode="Integer" Width="150px" />
</div>
