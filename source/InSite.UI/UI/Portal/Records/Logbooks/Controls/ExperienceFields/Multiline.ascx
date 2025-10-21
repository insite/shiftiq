<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Multiline.ascx.cs" Inherits="InSite.UI.Portal.Records.Logbooks.Controls.ExperienceFields.Multiline" %>

<div class="form-group mb-3">
    <label class="form-label">
        <asp:Literal runat="server" ID="FieldTitle" />
        <insite:RequiredValidator runat="server" ID="RequiredValidator" ControlToValidate="MultilineValue" Display="None" />
    </label>
    <div>
        <insite:TextBox runat="server" ID="MultilineValue" Width="100%" Rows="5" TextMode="MultiLine" />
    </div>
    <div class="form-text">
        <asp:Literal runat="server" ID="HelpText" />
    </div>
</div>
