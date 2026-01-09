<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TwoDates.ascx.cs" Inherits="InSite.UI.Portal.Records.Logbooks.Controls.ExperienceFields.TwoDates" %>

<div class="form-group mb-3">
    <label class="form-label">
        <asp:Literal runat="server" ID="FieldTitle" />
        <insite:RequiredValidator runat="server" ID="RequiredValidator1" ControlToValidate="DateValue1" Display="None" />
        <insite:RequiredValidator runat="server" ID="RequiredValidator2" ControlToValidate="DateValue2" Display="None" />
    </label>
    <div class="d-flex flex-row">
        <div class="pe-3">
            <insite:DateSelector runat="server" ID="DateValue1" />
        </div>
        <div class="ps-3">
            <insite:DateSelector runat="server" ID="DateValue2" />
            <insite:CompareValidator runat="server"
                ErrorMessage="<strong>End Date</strong> field value must be greater than <strong>Start Date</strong> field value"
                ValidationGroup="Journal" 
                Type="Date" 
                ControlToValidate="DateValue2" 
                ControlToCompare="DateValue1" 
                Operator="GreaterThanEqual" 
                EnableClientScript="false" 
                Display="None" />
        </div>
    </div>
    <div class="form-text">
        <asp:Literal runat="server" ID="HelpText" />
    </div>
</div>
