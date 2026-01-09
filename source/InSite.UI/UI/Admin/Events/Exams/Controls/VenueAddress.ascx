<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VenueAddress.ascx.cs" Inherits="InSite.Admin.Events.Exams.Controls.VenueAddress" %>

<div class="form-group mb-3" runat="server" ID="VenueAddressDiv">
    <label runat="server" id="VenueAddressLabel" class="form-label">Physical Location</label>
    <div>
        <asp:Literal runat="server" ID="VenueDetails" />
    </div>
    <div runat="server" id="HelpBlock" class="form-text">
        The physical location within the venue where the event occurs.
    </div>
</div>
