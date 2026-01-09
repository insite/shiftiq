<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VenueAddressOld.ascx.cs" Inherits="InSite.Admin.Events.Classes.Controls.VenueAddressOld" %>

<div class="form-group mb-3" runat="server" ID="VenueAddressDiv">
    <label runat="server" id="VenueAddressLabel">Physical Location</label>
    <div>
        <asp:Literal runat="server" ID="VenueDetails" />
    </div>
    <div runat="server" id="HelpBlock" class="form-text">
        The physical location within the venue where the event occurs.
    </div>
</div>
