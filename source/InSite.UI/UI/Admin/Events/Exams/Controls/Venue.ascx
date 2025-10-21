<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Venue.ascx.cs" Inherits="InSite.Admin.Events.Exams.Controls.Venue" %>

<%@ Register Src="VenueAddress.ascx" TagName="VenueAddress" TagPrefix="uc" %>	

<div class="form-group mb-3">
    <div class="float-end">
        <insite:IconLink Name="pencil" runat="server" id="ExamChangeVenue" style="padding:8px" ToolTip="Change Exam Venue" />
    </div>
    <label id="VenueLabel" runat="server" class="form-label">Venue</label>
    <div>
        <asp:Literal runat="server" ID="VenueName" />
    </div>
</div>

<uc:VenueAddress runat="server" ID="VenueAddress" />
