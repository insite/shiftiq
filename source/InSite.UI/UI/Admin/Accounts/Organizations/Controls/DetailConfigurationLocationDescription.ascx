<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailConfigurationLocationDescription.ascx.cs" Inherits="InSite.UI.Admin.Accounts.Organizations.Controls.DetailConfigurationLocationDescription" %>

<div class="form-group mb-3">
    <label class="form-label">Location Type</label>
    <insite:ComboBox runat="server" ID="LocationType">
        <Items>
            <insite:ComboBoxOption Value="None" Text="None" />
            <insite:ComboBoxOption Value="Home" Text="Home" />
            <insite:ComboBoxOption Value="Work" Text="Work" />
            <insite:ComboBoxOption Value="Shipping" Text="Shipping" />
            <insite:ComboBoxOption Value="Billing" Text="Billing" />
        </Items>
    </insite:ComboBox>
    <div class="form-text">What type of address location is this?</div>
</div>

<div class="form-group mb-3">
    <label class="form-label">Location Description</label>
    <insite:TextBox runat="server" ID="LocationDescription" MaxLength="32" />
    <div class="form-text">A brief description or comment, if applicable, related to this location.</div>
</div>
        
<h3>Telephone and Email</h3>

<div class="form-group mb-3">
    <label class="form-label">Phone</label>
    <insite:TextBox runat="server" ID="LocationPhone" MaxLength="32" />
    <div class="form-text">Primary telephone number.</div>
</div>
                            
<div class="form-group mb-3">
    <label class="form-label">Toll Free</label>
    <insite:TextBox runat="server" ID="LocationTollFree" MaxLength="32" />
    <div class="form-text">1-800 toll-free telephone number.</div>
</div>

<div class="form-group mb-3">
    <label class="form-label">Office Email</label>
    <insite:TextBox runat="server" ID="LocationEmail" MaxLength="32" />
    <div class="form-text">Default email address.</div>
</div>