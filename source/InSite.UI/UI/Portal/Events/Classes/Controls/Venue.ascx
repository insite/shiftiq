<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Venue.ascx.cs" Inherits="InSite.UI.Portal.Events.Classes.Controls.Venue" %>
<%@ Register Src="VenueAddress.ascx" TagName="VenueAddress" TagPrefix="uc" %>	

<div>
    <h5 id="VenueLabel" runat="server" class="mt-2 mb-1">Venue</h5>
    <div>
        <asp:Literal runat="server" ID="VenueName" />
    </div>
</div>

<uc:VenueAddress runat="server" ID="VenueAddress" />
