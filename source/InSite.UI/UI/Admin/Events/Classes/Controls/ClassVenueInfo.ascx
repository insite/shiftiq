<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClassVenueInfo.ascx.cs" Inherits="InSite.UI.Admin.Events.Classes.Controls.ClassVenueInfo" %>

<%@ Register Src="ClassVenueAddressInfo.ascx" TagName="Address" TagPrefix="uc" %>	

<div class="form-group mb-3">
    <div class="float-end">
        <insite:IconLink runat="server" id="ChangeLink" style="padding:8px" ToolTip="Change Class Venue" Name="pencil" />
    </div>
    <label runat="server" id="FieldLabel" class="form-label">
        Venue
    </label>
    <div runat="server" id="VenueName">
    </div>
</div>

<uc:Address runat="server" ID="Address" />
