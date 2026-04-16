<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VenueAddress.ascx.cs" Inherits="InSite.UI.Portal.Events.Classes.Controls.VenueAddress" %>

<div runat="server" ID="VenueAddressDiv">
    <h5 class="mt-2 mb-1"><insite:Literal id="VenueAddressLabel" runat="server" Text="Physical Location" /></h5>
    <div>
        <asp:Literal runat="server" ID="VenueDetails" />
    </div>
</div>
