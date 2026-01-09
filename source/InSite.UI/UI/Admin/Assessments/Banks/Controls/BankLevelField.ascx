<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BankLevelField.ascx.cs" Inherits="InSite.Admin.Assessments.Banks.Controls.BankLevelField" %>

<div class="form-group mb-3">
    <label class="form-label">
        Level Type
    </label>
    <div>
        <insite:TextBox runat="server" ID="TypeInput" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Level Number
    </label>
    <div>
        <insite:NumericBox runat="server" ID="NumberInput" NumericMode="Integer" MinValue="0" Width="100%" />
    </div>
</div>
