<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClassLocationInfo.ascx.cs" Inherits="InSite.UI.Admin.Events.Classes.Controls.ClassLocationInfo" %>

<%@ Register Src="ClassVenueInfo.ascx" TagName="VenueInfo" TagPrefix="uc" %>	

<uc:VenueInfo runat="server" ID="VenueInfo" ShowChangeLink="false" />

<div runat="server" id="VenueRoomField" class="form-group mb-3">
    <label class="form-label">
        Building and Room
    </label>
    <div>
        <asp:Literal runat="server" ID="VenueRoom" />
    </div>
</div>

<div  runat="server" id="ScheduleField" class="form-group mb-3">
    <label class="form-label">
        Scheduled Start/End Date and Time
    </label>
    <div>
        <asp:Literal runat="server" ID="EventScheduledStart" /> - <asp:Literal runat="server" ID="EventScheduledEnd" />
    </div>
</div>
