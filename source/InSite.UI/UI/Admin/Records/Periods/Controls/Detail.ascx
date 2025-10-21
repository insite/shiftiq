<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detail.ascx.cs" Inherits="InSite.Admin.Records.Periods.Controls.Detail" %>

<div class="form-group mb-3">
    <label class="form-label">
        Period Name
        <insite:RequiredValidator runat="server" ControlToValidate="PeriodName" ValidationGroup="Period" />
    </label>
    <div>
        <insite:TextBox runat="server" ID="PeriodName" MaxLength="20" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Period Start
        <insite:RequiredValidator runat="server" ControlToValidate="PeriodStart" ValidationGroup="Period" />
    </label>
    <div>
        <insite:DateSelector runat="server" ID="PeriodStart" Width="200" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Period End
        <insite:RequiredValidator runat="server" ControlToValidate="PeriodEnd" ValidationGroup="Period" />
        <insite:CustomValidator runat="server" ID="EndGreaterValidator" ControlToValidate="PeriodEnd" ValidationGroup="Period" ErrorMessage="End date must be greater than Start" Display="None" />
    </label>
    <div>
        <insite:DateSelector runat="server" ID="PeriodEnd" Width="200" />
    </div>
</div>
