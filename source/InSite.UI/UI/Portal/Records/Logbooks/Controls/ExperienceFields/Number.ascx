<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Number.ascx.cs" Inherits="InSite.UI.Portal.Records.Logbooks.Controls.ExperienceFields.Number" %>

<div class="form-group mb-3">
    <label class="form-label">
        <asp:Literal runat="server" ID="FieldTitle" />
        <insite:RequiredValidator runat="server" ID="RequiredValidator" ControlToValidate="NumberValue" Display="None" />
    </label>
    <div>
        <insite:NumericBox runat="server" ID="NumberValue" Width="250px" />
        <insite:CompareValidator runat="server" 
            ID="NumericBoxCompareValidator"
            ControlToValidate="NumberValue" 
            Operator="GreaterThan" 
            ValueToCompare="0" 
            Type="Double" 
            Display="None" 
            ValidationGroup="Journal" 
        />
    </div>
    <div class="form-text">
        <asp:Literal runat="server" ID="HelpText" />
    </div>
</div>
